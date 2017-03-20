namespace AdaWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PictureOK : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.WorkList", "Moustache");
            DropColumn("dbo.WorkList", "Beard");
            DropColumn("dbo.WorkList", "Sideburns");
            DropColumn("dbo.WorkList", "Glasses");
        }
        
        //public override void Down()
        //{
        //    AddColumn("dbo.WorkList", "Glasses", c => c.Int(nullable: false));
        //    AddColumn("dbo.WorkList", "Sideburns", c => c.Double(nullable: false));
        //    AddColumn("dbo.WorkList", "Beard", c => c.Double(nullable: false));
        //    AddColumn("dbo.WorkList", "Moustache", c => c.Double(nullable: false));
        //}
    }
}
