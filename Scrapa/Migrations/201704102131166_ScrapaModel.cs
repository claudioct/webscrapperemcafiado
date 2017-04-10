namespace Scrapa.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScrapaModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Estantes",
                c => new
                    {
                        IdEstante = c.Int(nullable: false, identity: true),
                        Nome = c.String(maxLength: 255),
                        Link = c.String(),
                    })
                .PrimaryKey(t => t.IdEstante)
                .Index(t => t.Nome, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Estantes", new[] { "Nome" });
            DropTable("dbo.Estantes");
        }
    }
}
