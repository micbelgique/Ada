namespace AdaWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProfilePictureModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProfilePictures", "Moustache", c => c.Double(nullable: false));
            AddColumn("dbo.ProfilePictures", "Beard", c => c.Double(nullable: false));
            AddColumn("dbo.ProfilePictures", "Sideburns", c => c.Double(nullable: false));
            AddColumn("dbo.ProfilePictures", "Glasses", c => c.String());
            AddColumn("dbo.WorkList", "Moustache", c => c.Double(nullable: false));
            AddColumn("dbo.WorkList", "Beard", c => c.Double(nullable: false));
            AddColumn("dbo.WorkList", "Sideburns", c => c.Double(nullable: false));
            AddColumn("dbo.WorkList", "Glasses", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkList", "Glasses");
            DropColumn("dbo.WorkList", "Sideburns");
            DropColumn("dbo.WorkList", "Beard");
            DropColumn("dbo.WorkList", "Moustache");
            DropColumn("dbo.ProfilePictures", "Glasses");
            DropColumn("dbo.ProfilePictures", "Sideburns");
            DropColumn("dbo.ProfilePictures", "Beard");
            DropColumn("dbo.ProfilePictures", "Moustache");
        }
    }
}
