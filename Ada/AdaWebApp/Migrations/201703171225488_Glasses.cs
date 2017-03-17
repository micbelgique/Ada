namespace AdaWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Glasses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkList", "Glasses", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkList", "Glasses");
        }
    }
}
