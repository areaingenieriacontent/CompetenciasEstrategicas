namespace SCORM1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Cate_Id = c.Int(nullable: false, identity: true),
                        Cate_Name = c.String(),
                        Cate_Desc = c.String(),
                        ToCo_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Cate_Id)
                .ForeignKey("dbo.TopicsCourse", t => t.ToCo_Id, cascadeDelete: true)
                .Index(t => t.ToCo_Id);
            
            CreateTable(
                "dbo.CategoryQuestionBank",
                c => new
                    {
                        Cate_Id = c.Int(nullable: false),
                        Modu_Id = c.Int(nullable: false),
                        EvaluatedQuestionQuantity = c.Int(nullable: false),
                        AprovedCategoryPercentage = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.Cate_Id, t.Modu_Id })
                .ForeignKey("dbo.Category", t => t.Cate_Id, cascadeDelete: true)
                .ForeignKey("dbo.ProtectedFailureTest", t => t.Modu_Id, cascadeDelete: true)
                .Index(t => t.Cate_Id)
                .Index(t => t.Modu_Id);
            
            CreateTable(
                "dbo.ProtectedFailureTest",
                c => new
                    {
                        Modu_Id = c.Int(nullable: false),
                        PF_Name = c.String(),
                        PF_Description = c.String(),
                        GeneralAproveScore = c.Single(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        testAvailable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Modu_Id)
                .ForeignKey("dbo.Module", t => t.Modu_Id)
                .Index(t => t.Modu_Id);
            
            CreateTable(
                "dbo.FlashQuestion",
                c => new
                    {
                        FlashQuestionId = c.Int(nullable: false, identity: true),
                        FlashTestId = c.Int(nullable: false),
                        Enunciado = c.String(),
                    })
                .PrimaryKey(t => t.FlashQuestionId)
                .ForeignKey("dbo.FlashTest", t => t.FlashTestId, cascadeDelete: true)
                .Index(t => t.FlashTestId);
            
            CreateTable(
                "dbo.FlashQuestionAnswer",
                c => new
                    {
                        FlashQuestionAnswerId = c.Int(nullable: false, identity: true),
                        FlashQuestionId = c.Int(nullable: false),
                        Content = c.String(),
                        CorrectAnswer = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FlashQuestionAnswerId)
                .ForeignKey("dbo.FlashQuestion", t => t.FlashQuestionId, cascadeDelete: true)
                .Index(t => t.FlashQuestionId);
            
            CreateTable(
                "dbo.FlashTest",
                c => new
                    {
                        FlashTestId = c.Int(nullable: false, identity: true),
                        ToCo_Id = c.Int(nullable: false),
                        FlashTestName = c.String(),
                        AprovedPercentage = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.FlashTestId)
                .ForeignKey("dbo.TopicsCourse", t => t.ToCo_Id, cascadeDelete: true)
                .Index(t => t.ToCo_Id);
            
            CreateTable(
                "dbo.ProtectedFailureAnswer",
                c => new
                    {
                        answerId = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(nullable: false),
                        AnswerContent = c.String(),
                        isCorrectQuestion = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.answerId)
                .ForeignKey("dbo.ProtectedFailureMultiChoice", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.ProtectedFailureMultiChoice",
                c => new
                    {
                        Category_Id = c.Int(nullable: false),
                        Modu_Id = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.QuestionId)
                .ForeignKey("dbo.CategoryQuestionBank", t => new { t.Category_Id, t.Modu_Id }, cascadeDelete: true)
                .Index(t => new { t.Category_Id, t.Modu_Id });
            
            CreateTable(
                "dbo.ProtectedFailureMultiChoiceAnswer",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        AnswerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.AnswerId })
                .ForeignKey("dbo.ProtectedFailureAnswer", t => t.AnswerId, cascadeDelete: true)
                .Index(t => t.AnswerId);
            
            CreateTable(
                "dbo.ProtectedFailureResults",
                c => new
                    {
                        Enro_id = c.Int(nullable: false),
                        Cate_Id = c.Int(nullable: false),
                        correctAnswersQuantity = c.Int(nullable: false),
                        Score = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.Enro_id, t.Cate_Id })
                .ForeignKey("dbo.Category", t => t.Cate_Id, cascadeDelete: true)
                .ForeignKey("dbo.Enrollment", t => t.Enro_id, cascadeDelete: true)
                .Index(t => t.Enro_id)
                .Index(t => t.Cate_Id);
            
            CreateTable(
                "dbo.UserModuleAdvance",
                c => new
                    {
                        Enro_id = c.Int(nullable: false),
                        ToCo_id = c.Int(nullable: false),
                        Completed = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Enro_id, t.ToCo_id })
                .ForeignKey("dbo.Enrollment", t => t.Enro_id, cascadeDelete: true)
                .ForeignKey("dbo.TopicsCourse", t => t.ToCo_id, cascadeDelete: true)
                .Index(t => t.Enro_id)
                .Index(t => t.ToCo_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserModuleAdvance", "ToCo_id", "dbo.TopicsCourse");
            DropForeignKey("dbo.UserModuleAdvance", "Enro_id", "dbo.Enrollment");
            DropForeignKey("dbo.ProtectedFailureResults", "Enro_id", "dbo.Enrollment");
            DropForeignKey("dbo.ProtectedFailureResults", "Cate_Id", "dbo.Category");
            DropForeignKey("dbo.ProtectedFailureMultiChoiceAnswer", "AnswerId", "dbo.ProtectedFailureAnswer");
            DropForeignKey("dbo.ProtectedFailureAnswer", "QuestionId", "dbo.ProtectedFailureMultiChoice");
            DropForeignKey("dbo.ProtectedFailureMultiChoice", new[] { "Category_Id", "Modu_Id" }, "dbo.CategoryQuestionBank");
            DropForeignKey("dbo.FlashQuestion", "FlashTestId", "dbo.FlashTest");
            DropForeignKey("dbo.FlashTest", "ToCo_Id", "dbo.TopicsCourse");
            DropForeignKey("dbo.FlashQuestionAnswer", "FlashQuestionId", "dbo.FlashQuestion");
            DropForeignKey("dbo.CategoryQuestionBank", "Modu_Id", "dbo.ProtectedFailureTest");
            DropForeignKey("dbo.ProtectedFailureTest", "Modu_Id", "dbo.Module");
            DropForeignKey("dbo.CategoryQuestionBank", "Cate_Id", "dbo.Category");
            DropForeignKey("dbo.Category", "ToCo_Id", "dbo.TopicsCourse");
            DropIndex("dbo.UserModuleAdvance", new[] { "ToCo_id" });
            DropIndex("dbo.UserModuleAdvance", new[] { "Enro_id" });
            DropIndex("dbo.ProtectedFailureResults", new[] { "Cate_Id" });
            DropIndex("dbo.ProtectedFailureResults", new[] { "Enro_id" });
            DropIndex("dbo.ProtectedFailureMultiChoiceAnswer", new[] { "AnswerId" });
            DropIndex("dbo.ProtectedFailureMultiChoice", new[] { "Category_Id", "Modu_Id" });
            DropIndex("dbo.ProtectedFailureAnswer", new[] { "QuestionId" });
            DropIndex("dbo.FlashTest", new[] { "ToCo_Id" });
            DropIndex("dbo.FlashQuestionAnswer", new[] { "FlashQuestionId" });
            DropIndex("dbo.FlashQuestion", new[] { "FlashTestId" });
            DropIndex("dbo.ProtectedFailureTest", new[] { "Modu_Id" });
            DropIndex("dbo.CategoryQuestionBank", new[] { "Modu_Id" });
            DropIndex("dbo.CategoryQuestionBank", new[] { "Cate_Id" });
            DropIndex("dbo.Category", new[] { "ToCo_Id" });
            DropTable("dbo.UserModuleAdvance");
            DropTable("dbo.ProtectedFailureResults");
            DropTable("dbo.ProtectedFailureMultiChoiceAnswer");
            DropTable("dbo.ProtectedFailureMultiChoice");
            DropTable("dbo.ProtectedFailureAnswer");
            DropTable("dbo.FlashTest");
            DropTable("dbo.FlashQuestionAnswer");
            DropTable("dbo.FlashQuestion");
            DropTable("dbo.ProtectedFailureTest");
            DropTable("dbo.CategoryQuestionBank");
            DropTable("dbo.Category");
        }
    }
}
