namespace AdaWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IndicatePassage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IndicatePassages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdFacebookConversation = c.String(),
                        Firtsname = c.String(),
                        IsSend = c.Boolean(nullable: false),
                        ToId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.ToId, cascadeDelete: true)
                .Index(t => t.ToId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IndicatePassages", "ToId", "dbo.People");
            DropIndex("dbo.IndicatePassages", new[] { "ToId" });
            DropTable("dbo.IndicatePassages");
        }
    }
}
