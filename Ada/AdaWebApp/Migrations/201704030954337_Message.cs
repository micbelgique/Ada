namespace AdaWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Message : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        From = c.String(),
                        Contenu = c.String(),
                        IsRead = c.Boolean(nullable: false),
                        Send = c.DateTime(nullable: false),
                        Read = c.DateTime(),
                        ToId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.ToId, cascadeDelete: true)
                .Index(t => t.ToId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "ToId", "dbo.People");
            DropIndex("dbo.Messages", new[] { "ToId" });
            DropTable("dbo.Messages");
        }
    }
}
