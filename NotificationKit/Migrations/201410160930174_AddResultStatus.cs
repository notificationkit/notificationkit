namespace NotificationKit.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResultStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SendLogs", "Success", c => c.Long(nullable: false));
            AddColumn("dbo.SendLogs", "Failure", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SendLogs", "Failure");
            DropColumn("dbo.SendLogs", "Success");
        }
    }
}
