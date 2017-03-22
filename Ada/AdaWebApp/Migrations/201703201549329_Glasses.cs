namespace AdaWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Glasses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProfilePictures", "Moustache", c => c.Double(nullable: false));
            AddColumn("dbo.ProfilePictures", "Beard", c => c.Double(nullable: false));
            AddColumn("dbo.ProfilePictures", "Sideburns", c => c.Double(nullable: false));
            AddColumn("dbo.ProfilePictures", "Glasses", c => c.Int(nullable: false));
            AddColumn("dbo.WorkList", "Glasses", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkList", "Glasses");
            DropColumn("dbo.ProfilePictures", "Glasses");
            DropColumn("dbo.ProfilePictures", "Sideburns");
            DropColumn("dbo.ProfilePictures", "Beard");
            DropColumn("dbo.ProfilePictures", "Moustache");
        }
    }
}
