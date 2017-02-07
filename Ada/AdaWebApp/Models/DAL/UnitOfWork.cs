using System;
using System.Threading.Tasks;
using AdaWebApp.Models.DAL.Repositories;
using AdaWebApp.Models.Entities;

namespace AdaWebApp.Models.DAL
{
    public class UnitOfWork
    {
        private bool _disposed;
        private readonly ApplicationDbContext _context;

        // Repository for Visitor entity
        private readonly Lazy<PersonRepository> _personRepository;
        public PersonRepository PersonRepository => _personRepository.Value;

        // Repository for Staff Member entity
        private readonly Lazy<BaseRepository<StaffMember>> _staffMemberRepository;
        public BaseRepository<StaffMember> StaffMembersRepository => _staffMemberRepository.Value;

        // Repository for ProfilePicture entity
        private readonly Lazy<ProfilePictureRepository> _profilePictureRepository;
        public ProfilePictureRepository ProfilePicturesRepository => _profilePictureRepository.Value;

        // Repository for Visit entity
        private readonly Lazy<VisitRepository> _visitsRepository;
        public VisitRepository VisitsRepository => _visitsRepository.Value;

        // Repository for Visit entity
        private readonly Lazy<BaseRepository<Unavailability>> _unavRepository;
        public BaseRepository<Unavailability> UnavailabilitieRepository => _unavRepository.Value;

        // Repository for EmotionScores
        private readonly Lazy<BaseRepository<EmotionScores>> _emotionScores;
        public BaseRepository<EmotionScores> EmotionScoresRepository => _emotionScores.Value;

        private readonly Lazy<RecognitionItemRepository> _recognitionItemsRepository;
        public RecognitionItemRepository RecognitionItemsRepository => _recognitionItemsRepository.Value;

        private readonly Lazy<StatRepository> _statRepository;
        public StatRepository StatRepository => _statRepository.Value;

        public UnitOfWork() : this(ApplicationDbContext.Create()) { }

        public UnitOfWork(ApplicationDbContext context){
            _context = context;
            _personRepository = new Lazy<PersonRepository>(() => new PersonRepository(context));
            _profilePictureRepository = new Lazy<ProfilePictureRepository>(() => new ProfilePictureRepository(context));
            _visitsRepository = new Lazy<VisitRepository>(() => new VisitRepository(context));
            _staffMemberRepository = new Lazy<BaseRepository<StaffMember>>(() => new BaseRepository<StaffMember>(context));
            _unavRepository = new Lazy<BaseRepository<Unavailability>>(() => new BaseRepository<Unavailability>(context));
            _emotionScores = new Lazy<BaseRepository<EmotionScores>>(() =>new BaseRepository<EmotionScores>(context));
            _recognitionItemsRepository = new Lazy<RecognitionItemRepository>(() => new RecognitionItemRepository(context));
            _statRepository = new Lazy<StatRepository>(() => new StatRepository(context));
        }

        public async Task SaveAsync(){
            await _context.SaveChangesAsync();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
