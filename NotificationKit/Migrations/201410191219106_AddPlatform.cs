namespace NotificationKit.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlatform : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScheduleLogs", "Platform", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScheduleLogs", "Platform");
        }
    }
}
