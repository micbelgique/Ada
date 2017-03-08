using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Common.Logging;
using AdaSDK;
using AdaWebApp.Helpers;
using AdaWebApp.Models.DAL;
using AdaWebApp.Models.Entities;
using AdaWebApp.Models.Entities.EntitiesExtensions;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Person = AdaWebApp.Models.Entities.Person;
using static System.Web.Hosting.HostingEnvironment;
using Rectangle = Microsoft.ProjectOxford.Common.Rectangle;

namespace AdaWebApp.Models.Services.PersonService
{
    /// <summary>
    /// This service allows to make face and emotion recognition and then, 
    /// based on the result of recognitions, manage persons in oxford api database 
    /// and local database. 
    /// </summary>
    public class PersonService
    {
        private readonly IFaceServiceClient _faceServiceClient;
        private readonly EmotionServiceClient _emotionServiceClient;
        private readonly UnitOfWork _unit;

        public PersonService(UnitOfWork unit)
        {
            _faceServiceClient = new FaceServiceClient(Global.OxfordFaceApiKey);
            _emotionServiceClient = new EmotionServiceClient(Global.OxfordEmotionApiKey);
            _unit = unit;
            
            Directory.CreateDirectory(MapPath(Global.TemporaryUploadsFolder));
        }

        /// <summary>
        /// Asynchronously saves the temporary picture and return its path
        /// </summary>
        public async Task<string> SaveTemporaryFileAsync(HttpPostedFile file)
        {
            return await Task.Run(() =>
            {
                string filePath = $"{Global.TemporaryUploadsFolder}{Guid.NewGuid()}.jpg";
                file.SaveAs(MapPath(filePath));
                return filePath;
            });
        }

        /// <summary>
        /// Asynchronously detect multiple faces with gender and age from a stream of picture
        /// </summary>
        public async Task<Face[]> DetectFacesFromPictureAsync(string imagePath)
        {
            // Makes face detection
            Face[] faces = await _faceServiceClient.DetectAsync(UrlHelpers.Content(Global.Host, imagePath),
                returnFaceAttributes: new[] { FaceAttributeType.Age, FaceAttributeType.Gender });

            // If there aren't at least one face in picture
            if (!faces.Any())
            {
                throw new FaceAPIException("NotFaceDetected", "No face detected in picture", HttpStatusCode.BadRequest);
            }

            return faces;
        }

        /// <summary>
        /// Asynchronously recognize persons from oxford and database and return 
        /// recognition results
        /// </summary>
        public async Task<List<RecognitionItem>> RecognizePersonsAsync(Face[] faces, string imagePath)
        {
            List<RecognitionItem> results = new List<RecognitionItem>();

            // Selects max 10 faces to make identification ordered by the size of rectangle of faces
            var selectedFaces = faces.Take(10).ToList();

            // Makes identification from microsoft oxford
            IList<IdentifyResult> identifyResults = new List<IdentifyResult>();
            IEnumerable<Person> recognizedPersons = new List<Person>();

            try
            {
                identifyResults = (await _faceServiceClient.IdentifyAsync(Global.OxfordPersonGroupId, selectedFaces.Select(f => f.FaceId).ToArray())).ToList();
                // Extracts guids of found candidates
                IList<Guid> candidatesId = identifyResults.Where(i => i.Candidates.Any()).Select(i => i.Candidates[0].PersonId).ToList();
                // Gets candidates' information in database
                recognizedPersons = _unit.PersonRepository.GetPersonsByCandidateIds(candidatesId);
            }
            catch (FaceAPIException e)
            {
                var persons = await _faceServiceClient.GetPersonsAsync(Global.OxfordPersonGroupId);
                LogManager.GetLogger(GetType()).Info("Error during recognition", e);
                if (persons.Any()) return results;
            }

            int imageCounter = 0;

            foreach (var face in selectedFaces)
            {
                Person person = null;

                // Gets identification result which match with current face
                IdentifyResult identifyResult = identifyResults.FirstOrDefault(i => i.FaceId == face.FaceId);
                double confidence = 1;

                if (identifyResult != null && identifyResult.Candidates.Any())
                {
                    person = recognizedPersons.FirstOrDefault(p => p.PersonApiId == identifyResult.Candidates[0].PersonId);
                    confidence = identifyResult.Candidates[0].Confidence;
                }

                imageCounter++;

                results.Add(new RecognitionItem
                {
                    Face = face,
                    Person = person,
                    ImageUrl = imagePath,
                    ImageCounter = imageCounter,
                    Confidence = confidence,
                    DateOfRecognition = DateTime.UtcNow
                });
            }

            return results;
        }

        /// <summary>
        /// Asynchronously recognize emotions from a picture
        /// </summary>
        public async Task<EmotionScores> RecognizeEmotions(string imageUrl, FaceRectangle faceRectangle)
        {
            Emotion emotion = (await _emotionServiceClient.RecognizeAsync(
                UrlHelpers.Content(Global.Host, imageUrl),
                new[]
                {
                    new Rectangle
                    {
                        Height = faceRectangle.Height,
                        Left = faceRectangle.Left,
                        Top = faceRectangle.Top,
                        Width = faceRectangle.Width
                    }
                })).FirstOrDefault();

            return emotion.ToEmotionScores();
        }

        /// <summary>
        /// Asynchronously creates a new person in api and local database.
        /// Creates also the directory to store pictures of this person
        /// <see cref="PersonCreationFallBack(Guid?)" />
        /// </summary>
        public async Task<Person> CreatePerson(Face face)
        {
            CreatePersonResult apiCreationResult = null; 

            try
            {
                // Creates person on oxford api
                apiCreationResult = await _faceServiceClient.CreatePersonAsync(Global.OxfordPersonGroupId, $"{Guid.NewGuid()}");
                return _unit.PersonRepository.CreatePerson(apiCreationResult.PersonId, face); 
            }
            catch(FaceAPIException e)
            {
                LogManager.GetLogger(GetType()).Info("Error occured during creation of person on oxford api", e);
                return null; 
            }
            catch(Exception e)
            {
                // FallBack: Remove person on oxford to avoid ambiguous person in api database
                await PersonCreationFallBack(apiCreationResult?.PersonId); 

                LogManager.GetLogger(GetType()).Info("General error occured during creation of person. See exception for more details", e);
                return null; 
            }
            
        }

        /// <summary>
        /// Asynchronously removes a person from api database. 
        /// this method is call when an error occured during creation of person or 
        /// during the creation of its first picture
        /// </summary>
        public async Task PersonCreationFallBack(Guid? personApiId)
        {
            if (personApiId == null) return; 

            try
            {
                await _faceServiceClient.DeletePersonAsync(Global.OxfordPersonGroupId, personApiId.Value);
                Directory.Delete(MapPath($"{Global.PersonsDatabaseDirectory}/{personApiId}"), true);
            }
            catch (Exception e)
            {
                LogManager.GetLogger(GetType()).Info("Error occured during fallback of creation of a new person", e);
            }
        }

        /// <summary>
        /// Asynchronously add face in oxford for a person and link it with a profile picture 
        /// <see cref="PersonCreationFallBack(Guid?)"/>
        /// </summary>
        public async Task<bool> AddFaceToPersonInOxford(Person person, ProfilePicture picture, Face face, string temporaryImage)
        {
            Guid faceApiId = default(Guid); 

            try
            {
                // if the person has reached the limit of 64 faces in oxford
                if(person.HasReachedMaxLimitOfFaces()) await RemoveFaceFromOxford(person);

                AddPersistedFaceResult result =  await _faceServiceClient.AddPersonFaceAsync(Global.OxfordPersonGroupId,
                    person.PersonApiId, UrlHelpers.Content(Global.Host, temporaryImage), targetFace:face.FaceRectangle);

                faceApiId = result.PersistedFaceId;

                // Link profile picture with face on oxford api
                picture.FaceApiId = faceApiId;

                return true; 
            }
            catch (FaceAPIException e)
            {
                LogManager.GetLogger(GetType()).Info("Error occured during adding od face to person", e);

                // If is a new person we rollback its creation to avoid person without face
                if (person.Id == 0)
                {
                    await PersonCreationFallBack(person.PersonApiId);
                    File.Delete(MapPath(temporaryImage));
                    return false; 
                }

                return true; 
            }
        }

        /// <summary>
        /// Asynchronously remove the older face of a picture from oxford 
        /// and update linked entity
        /// </summary>
        public async Task RemoveFaceFromOxford(Person person)
        {
            ProfilePicture pictureToDelete = _unit.ProfilePicturesRepository.GetOlderProfilePictureInOxford(person.Id);

            if (pictureToDelete == null) return;

            try
            {
                await _faceServiceClient.DeletePersonFaceAsync
                    (Global.OxfordPersonGroupId, person.PersonApiId, pictureToDelete.FaceApiId);
            }
            catch (FaceAPIException e)
            {
                LogManager.GetLogger(GetType()).Info("Error occured during suppression of a face from api", e);
            }

            pictureToDelete.FaceApiId = default(Guid);
        }

        /// <summary>
        /// Asynchronously train a persons group 
        /// </summary>
        public async Task<bool> TrainPersonGroupAsync()
        {
            return await Task.Run(async () =>
            {
                Status status;

                try
                {

                    await _faceServiceClient.TrainPersonGroupAsync(Global.OxfordPersonGroupId);

                    while (true)
                    {
                        await Task.Delay(1000);
                        var trainningStatus = await _faceServiceClient.GetPersonGroupTrainingStatusAsync(Global.OxfordPersonGroupId);

                        status = trainningStatus.Status;

                        if (status != Status.Running)
                        {
                            break;
                        }
                    }
                }
                catch (FaceAPIException e)
                {
                    status = Status.Failed;
                    LogManager.GetLogger(GetType()).Info("Error during training of group", e);
                }

                return status == Status.Succeeded;
            });
        }

        #region Queue methods

        /// <summary>
        /// Asynchronously put a recognition item into queue
        /// </summary>
        public async Task Enqueue(IList<RecognitionItem> recognitionItems)
        {
            // Write work items
            foreach (var recognitionItem in recognitionItems)
            {
                // Gets recognized person
                var person = recognitionItem.Person;

                if (person != null)
                {
                    // add or update visit for person 
                    _unit.VisitsRepository.AddOrUpdateVisit(person, recognitionItem.DateOfRecognition);
                    // Create a new profile picture 
                    ProfilePicture newPicture = _unit.ProfilePicturesRepository.CreateProfilePicture
                        (person.PersonApiId, recognitionItem.ImageUrl, recognitionItem.Face, recognitionItem.Confidence);

                    person.Visits.Last().ProfilePictures.Add(newPicture);

                    // Update person's statistics 
                    person.UpdateAge(recognitionItem.Face.FaceAttributes.Age);
                    person.UpdateGender(GenderValuesHelper.Parse(recognitionItem.Face.FaceAttributes.Gender));

                    _unit.PersonRepository.Update(person);

                    // Updates recognition item 
                    recognitionItem.ProfilePictureId = newPicture.Id;
                }

                _unit.RecognitionItemsRepository.Insert(recognitionItem);
                await _unit.SaveAsync();
            }
        }

        /// <summary>
        ///  Asynchronously process a queue item
        /// </summary>
        public async Task<Person> ProcessRecognitionItem(RecognitionItem item)
        {
            Task<EmotionScores> emotionTask = RecognizeEmotions(item.ImageUrl, item.Face.FaceRectangle);

            #region Process of person
            Person person = item.Person;

            // if the person isn't recognized
            if (item.Person == null)
            {
                // Makes recognition to be sure the person isn't recognized
                var recognitionResults = await RecognizePersonsAsync(new[] { item.Face }, item.ImageUrl);

                if (recognitionResults.Any()) // be sure there is results
                {
                    person = recognitionResults.First().Person;

                    // If after that person is still not recognized, we create it
                    // and add a new visit
                    if (person == null)
                    {
                        person = await CreatePerson(item.Face);
                        _unit.VisitsRepository.AddOrUpdateVisit(person, item.DateOfRecognition);
                    }
                }
            }
            #endregion

            if (person != null)
            {
                #region Process of profilePictures
                ProfilePicture picture = item.ProfilePicture;

                // Creates a profile picture for new person
                if (item.ProfilePicture == null)
                {
                    picture = _unit.ProfilePicturesRepository
                        .CreateProfilePicture(person.PersonApiId, item.ImageUrl, item.Face, item.Confidence);

                    person.Visits.Last().ProfilePictures.Add(picture);
                }

                // Adds picture to oxford, update information and clean temporary folder
                // If an erorr occured during addind face for a new person, stops process and rollback
                if(!await AddFaceToPersonInOxford(person, picture, item.Face, item.ImageUrl)) return null;
                picture.EmotionScores = await emotionTask;

                // Clean temporary image file
                if (item.ImageCounter == 0) File.Delete(MapPath(item.ImageUrl));
                #endregion

                // trains the person group
                await TrainPersonGroupAsync();

                // Creates person is it's new
                if (person.Id == 0) _unit.PersonRepository.Insert(person);
               
                await _unit.SaveAsync();
            }

            return person;
        }

        /// <summary>
        /// Asynchronously process a queue item from id of recognition item
        /// </summary>
        public async Task<Person> ProcessRecognitionItem(int itemId)
        {
            var recognitionItem = await _unit.RecognitionItemsRepository.GetByIdAsync(itemId);

            if (recognitionItem != null)
            {
                var result = await ProcessRecognitionItem(recognitionItem);
                _unit.RecognitionItemsRepository.Remove(recognitionItem);
                await _unit.SaveAsync();
                return result;
            }

            return null;
        }


        /// <summary>
        /// Asynchronously process queue items
        /// </summary>
        public async Task ProcessQueue()
        {
            RecognitionItem item;

            while ((item = _unit.RecognitionItemsRepository.Pop()) != null)
            {
                await ProcessRecognitionItem(item);
                _unit.RecognitionItemsRepository.Remove(item);
                await _unit.SaveAsync();
            }
        }

        /// <summary>
        /// Asynchronously clean the queue
        /// </summary>
        public async Task CleanQueue()
        {
            _unit.RecognitionItemsRepository.RemoveAll();
            Directory.Delete(MapPath(Global.TemporaryUploadsFolder), true);
            await _unit.SaveAsync();
        }

        #endregion
    }
}