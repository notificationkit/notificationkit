namespace NotificationKit.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScheduleLogs", "Message", c => c.String());
            AddColumn("dbo.SendLogs", "Message", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SendLogs", "Message");
            DropColumn("dbo.ScheduleLogs", "Message");
        }
    }
}
