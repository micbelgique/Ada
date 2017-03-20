using AdaSDK;
using AdaWebApp.Helpers;
using AdaWebApp.Models.Entities;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Linq;
using static System.Web.Hosting.HostingEnvironment;

namespace AdaWebApp.Models.DAL.Repositories
{
    public class ProfilePictureRepository : BaseRepository<ProfilePicture>
    {
        public ProfilePictureRepository(ApplicationDbContext context) : base(context) { }

        public ProfilePicture CreateProfilePicture(Guid personApiId, string imagePath, Face face, double confidence)
        {
            // generates a new path for picture and create a picture with a face box at this location
            string newPath = $"{Global.PersonsDatabaseDirectory}{personApiId}/{Guid.NewGuid()}.jpg";
            ImageHelpers.CreateImageWithRectangleFace(MapPath(imagePath), MapPath(newPath), face.FaceRectangle);

            // Adds a new profile picture for this person 
            ProfilePicture newPicture =  new ProfilePicture
            {
                Confidence = confidence,
                Gender = GenderValuesHelper.Parse(face.FaceAttributes.Gender),
                FaceRectangle = face.FaceRectangle,
                Uri = newPath,
                Glasses = Convert.ToString(face.FaceAttributes.Glasses),
                Moustache = face.FaceAttributes.FacialHair.Moustache,
                Beard = face.FaceAttributes.FacialHair.Beard,
                Sideburns = face.FaceAttributes.FacialHair.Sideburns
            };

            newPicture.Age = face.FaceAttributes.Age;

            return newPicture;
        }

        public ProfilePicture GetOlderProfilePictureInOxford(int personId)
        {
            return Table.Include("Visit").FirstOrDefault
                (pp => pp.FaceApiId != default(Guid) && pp.Visit.PersonId == personId);
        }
    }
}
