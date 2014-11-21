namespace NotificationKit.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeToDateTimeOffset : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ScheduleLogs", "ScheduledOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.ScheduleLogs", "CancelledOn", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.ScheduleLogs", "CreatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.SendLogs", "CreatedOn", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SendLogs", "CreatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ScheduleLogs", "CreatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ScheduleLogs", "CancelledOn", c => c.DateTime());
            AlterColumn("dbo.ScheduleLogs", "ScheduledOn", c => c.DateTime(nullable: false));
        }
    }
}
