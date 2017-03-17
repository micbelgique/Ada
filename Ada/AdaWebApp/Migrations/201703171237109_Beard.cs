namespace AdaWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Beard : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkList", "Moustache", c => c.Double(nullable: false));
            AddColumn("dbo.WorkList", "Beard", c => c.Double(nullable: false));
            AddColumn("dbo.WorkList", "Sideburns", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkList", "Sideburns");
            DropColumn("dbo.WorkList", "Beard");
            DropColumn("dbo.WorkList", "Moustache");
        }
    }
}
