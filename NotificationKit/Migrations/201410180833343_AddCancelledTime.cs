namespace NotificationKit.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCancelledTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScheduleLogs", "CancelledOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScheduleLogs", "CancelledOn");
        }
    }
}
