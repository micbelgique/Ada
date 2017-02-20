namespace AdaWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmotionScores",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Anger = c.Single(nullable: false),
                        Contempt = c.Single(nullable: false),
                        Disgust = c.Single(nullable: false),
                        Fear = c.Single(nullable: false),
                        Happiness = c.Single(nullable: false),
                        Neutral = c.Single(nullable: false),
                        Sadness = c.Single(nullable: false),
                        Surprise = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProfilePictures", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.ProfilePictures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FaceApiId = c.Guid(nullable: false),
                        Uri = c.String(),
                        Age = c.Double(nullable: false),
                        Gender = c.Int(nullable: false),
                        Confidence = c.Double(nullable: false),
                        FaceWidth = c.Int(nullable: false),
                        FaceHeight = c.Int(nullable: false),
                        FaceLeft = c.Int(nullable: false),
                        FaceTop = c.Int(nullable: false),
                        VisitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Visits", t => t.VisitId, cascadeDelete: true)
                .Index(t => t.FaceApiId)
                .Index(t => t.VisitId);
            
            CreateTable(
                "dbo.Visits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Reason = c.String(),
                        NbPasses = c.Int(nullable: false),
                        PersonId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.PersonId, cascadeDelete: true)
                .Index(t => t.PersonId);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PersonApiId = c.Guid(nullable: false),
                        LastName = c.String(),
                        FirstName = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        Gender = c.Int(nullable: false),
                        MaleCounter = c.Int(nullable: false),
                        FemaleCounter = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.PersonApiId);
            
            CreateTable(
                "dbo.WorkList",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FaceId = c.Guid(nullable: false),
                        FaceWidth = c.Int(nullable: false),
                        FaceHeight = c.Int(nullable: false),
                        FaceLeft = c.Int(nullable: false),
                        FaceTop = c.Int(nullable: false),
                        Age = c.Double(nullable: false),
                        Gender = c.String(),
                        ImageUrl = c.String(),
                        ImageCounter = c.Int(nullable: false),
                        Confidence = c.Double(nullable: false),
                        DateOfRecognition = c.DateTime(nullable: false),
                        PersonId = c.Int(),
                        ProfilePictureId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.PersonId)
                .ForeignKey("dbo.ProfilePictures", t => t.ProfilePictureId)
                .Index(t => t.PersonId)
                .Index(t => t.ProfilePictureId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.StaffMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false),
                        FirstName = c.String(nullable: false),
                        PhoneNumber = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Unavailabilities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StarTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        StaffMemberId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StaffMembers", t => t.StaffMemberId, cascadeDelete: true)
                .Index(t => t.StaffMemberId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Unavailabilities", "StaffMemberId", "dbo.StaffMembers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.WorkList", "ProfilePictureId", "dbo.ProfilePictures");
            DropForeignKey("dbo.WorkList", "PersonId", "dbo.People");
            DropForeignKey("dbo.ProfilePictures", "VisitId", "dbo.Visits");
            DropForeignKey("dbo.Visits", "PersonId", "dbo.People");
            DropForeignKey("dbo.EmotionScores", "Id", "dbo.ProfilePictures");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Unavailabilities", new[] { "StaffMemberId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.WorkList", new[] { "ProfilePictureId" });
            DropIndex("dbo.WorkList", new[] { "PersonId" });
            DropIndex("dbo.People", new[] { "PersonApiId" });
            DropIndex("dbo.Visits", new[] { "PersonId" });
            DropIndex("dbo.ProfilePictures", new[] { "VisitId" });
            DropIndex("dbo.ProfilePictures", new[] { "FaceApiId" });
            DropIndex("dbo.EmotionScores", new[] { "Id" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Unavailabilities");
            DropTable("dbo.StaffMembers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.WorkList");
            DropTable("dbo.People");
            DropTable("dbo.Visits");
            DropTable("dbo.ProfilePictures");
            DropTable("dbo.EmotionScores");
        }
    }
}

