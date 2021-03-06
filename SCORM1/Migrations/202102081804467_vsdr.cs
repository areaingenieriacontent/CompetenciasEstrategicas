namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vsdr : DbMigration
    {
        public override void Up()
        {
            
            CreateTable(
                "dbo.VsdrSession",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        case_content = c.String(),
                        start_date = c.DateTime(nullable: false),
                        end_date = c.DateTime(nullable: false),
                        resource_url = c.String(),
                        available = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);

            CreateTable(
                "dbo.VsdrEnrollment",
                c => new
                    {
                        user_id = c.String(nullable: false, maxLength: 128),
                        vsdr_id = c.Int(nullable: false),
                        vsdr_enro_init_date = c.DateTime(nullable: false),
                        vsdr_enro_finish_date = c.DateTime(nullable: false),
                        qualification = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.user_id, t.vsdr_id })
                .ForeignKey("dbo.Users", t => t.user_id, cascadeDelete: true)
                .ForeignKey("dbo.VsdrSession", t => t.vsdr_id, cascadeDelete: true)
                .Index(t => t.user_id)
                .Index(t => t.vsdr_id);
            
            CreateTable(
                "dbo.VsdrTeacherComment",
                c => new
                    {
                        user_id = c.String(maxLength: 128),
                        vsdr_id = c.Int(nullable: false),
                        comment_id = c.Int(nullable: false, identity: true),
                        teacher_id = c.String(),
                        content = c.String(),
                        commentDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.comment_id)
                .ForeignKey("dbo.VsdrEnrollment", t => new { t.user_id, t.vsdr_id })
                .Index(t => new { t.user_id, t.vsdr_id });
            
            CreateTable(
                "dbo.VsdrUserFile",
                c => new
                    {
                        user_id = c.String(maxLength: 128),
                        vsdr_id = c.Int(nullable: false),
                        vsdr_file_id = c.Int(nullable: false, identity: true),
                        register_name = c.String(),
                        file_description = c.String(),
                        file_name = c.String(),
                        file_extention = c.String(),
                        registered_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.vsdr_file_id)
                .ForeignKey("dbo.VsdrEnrollment", t => new { t.user_id, t.vsdr_id })
                .Index(t => new { t.user_id, t.vsdr_id });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VsdrUserFile", new[] { "user_id", "vsdr_id" }, "dbo.VsdrEnrollment");
            DropForeignKey("dbo.VsdrTeacherComment", new[] { "user_id", "vsdr_id" }, "dbo.VsdrEnrollment");
            DropForeignKey("dbo.VsdrEnrollment", "vsdr_id", "dbo.VsdrSession");
            DropForeignKey("dbo.VsdrEnrollment", "user_id", "dbo.Users");
            DropIndex("dbo.VsdrUserFile", new[] { "user_id", "vsdr_id" });
            DropIndex("dbo.VsdrTeacherComment", new[] { "user_id", "vsdr_id" });
            DropIndex("dbo.VsdrEnrollment", new[] { "vsdr_id" });
            DropIndex("dbo.VsdrEnrollment", new[] { "user_id" });
            DropTable("dbo.VsdrUserFile");
            DropTable("dbo.VsdrTeacherComment");
            DropTable("dbo.VsdrEnrollment");
            DropTable("dbo.VsdrSession");
        }
    }
}
