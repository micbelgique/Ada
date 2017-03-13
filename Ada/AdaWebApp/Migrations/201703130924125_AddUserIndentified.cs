namespace AdaWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserIndentified : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserIndentifieds",
                c => new
                    {
                        IdUserIndentified = c.Int(nullable: false, identity: true),
                        Firtsname = c.String(),
                        LastName = c.String(),
                        IdFacebook = c.String(),
                        authorization = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdUserIndentified);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserIndentifieds");
        }
    }
}
