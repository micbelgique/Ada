using System.Data.Entity;
using AdaWebApp.Models.Entities;
using AdaWebApp.Models.Services.PersonService;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.ProjectOxford.Face.Contract;
using Person = AdaWebApp.Models.Entities.Person;

namespace AdaWebApp.Models.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Person> Visitors { get; set; }
        public DbSet<ProfilePicture> ProfilePictures { get; set; }
        public DbSet<EmotionScores> EmotionScores { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Unavailability> Unavailabilities { get; set; }
        public DbSet<StaffMember> StaffMembers { get; set; }
        public DbSet<RecognitionItem> QueueItems { get; set; }
        public DbSet<UserIndentified> UserIndentifieds { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<IndicatePassage> IndicatePassage { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {}

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapping relationship between ProfilePicture and EmotionScores
            modelBuilder.Entity<ProfilePicture>().HasOptional(pp => pp.EmotionScores).WithRequired(e => e.ProfilePicture); 

            // Mapping face complex type
            modelBuilder.ComplexType<Face>();
            modelBuilder.ComplexType<Face>().Property(f => f.FaceId).HasColumnName("FaceId");
            modelBuilder.ComplexType<Face>().Ignore(f => f.FaceLandmarks);

            // Mapping face attributes from face complex type
            modelBuilder.ComplexType<FaceAttributes>().Property(fa => fa.Age).HasColumnName("Age");
            modelBuilder.ComplexType<FaceAttributes>().Property(fa => fa.Gender).HasColumnName("Gender");
            modelBuilder.ComplexType<FacialHair>().Property(fa => fa.Moustache).HasColumnName("Moustache");
            modelBuilder.ComplexType<FacialHair>().Property(fa => fa.Beard).HasColumnName("Beard");
            modelBuilder.ComplexType<FacialHair>().Property(fa => fa.Sideburns).HasColumnName("Sideburns");
            modelBuilder.ComplexType<FaceAttributes>().Property(fa => fa.Glasses).HasColumnName("Glasses");

            // Ignoring properties
            //modelBuilder.ComplexType<FaceAttributes>().Ignore(fa => fa.FacialHair);
            //modelBuilder.ComplexType<FaceAttributes>().Ignore(fa => fa.Glasses);
            modelBuilder.ComplexType<FaceAttributes>().Ignore(fa => fa.HeadPose);
            modelBuilder.ComplexType<FaceAttributes>().Ignore(fa => fa.Smile);

            // Mapping face rectangle from face complex type
            modelBuilder.ComplexType<FaceRectangle>().Property(fr => fr.Left).HasColumnName("FaceLeft");
            modelBuilder.ComplexType<FaceRectangle>().Property(fr => fr.Top).HasColumnName("FaceTop");
            modelBuilder.ComplexType<FaceRectangle>().Property(fr => fr.Width).HasColumnName("FaceWidth");
            modelBuilder.ComplexType<FaceRectangle>().Property(fr => fr.Height).HasColumnName("FaceHeight");
        }
       
    }
}