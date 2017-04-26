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
                        IdFacebookConversation = c.String(nullable: false, maxLength: 128),
                        Firtsname = c.String(),
                        IsSend = c.Boolean(nullable: false),
                        ToId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdFacebookConversation)
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
