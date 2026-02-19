using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedbackTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProceduresAndViews : Migration
    {
        /// <inheritdoc />
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Views
            migrationBuilder.Sql(@"
                CREATE VIEW vw_FeedbackView AS
                SELECT f.*, u1.Name AS FromUserName, u2.Name AS ToUserName
                FROM TFeedbacks f
                LEFT JOIN TUsers u1 ON f.FromUserId = u1.Id
                LEFT JOIN TUsers u2 ON f.ToUserId = u2.Id;
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW vw_RecognitionView AS
                SELECT r.*, u1.Name AS FromUserName, u2.Name AS ToUserName
                FROM TRecognitions r
                LEFT JOIN TUsers u1 ON r.FromUserId = u1.Id
                LEFT JOIN TUsers u2 ON r.ToUserId = u2.Id;
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW vw_ReviewView AS
                SELECT r.*, f.Description AS FeedbackDescription, u.Name AS ReviewerName
                FROM TReviews r
                JOIN TFeedbacks f ON r.FeedbackId = f.Id
                JOIN TUsers u ON r.ReviewerId = u.Id;
            ");

            // Stored Procedures
            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_AddRecognition
                    @FromUserId INT,
                    @ToUserId INT,
                    @BadgeType NVARCHAR(MAX),
                    @Points INT,
                    @Comments NVARCHAR(MAX),
                    @Date DATETIME
                AS
                BEGIN
                    INSERT INTO TRecognitions (FromUserId, ToUserId, BadgeType, Points, Comments, Date)
                    VALUES (@FromUserId, @ToUserId, @BadgeType, @Points, @Comments, @Date);
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_GenerateReport
                AS
                BEGIN
                    SELECT 
                        (SELECT COUNT(*) FROM TFeedbacks) AS TotalFeedback,
                        (SELECT COUNT(*) FROM TRecognitions) AS TotalRecognitions,
                        (SELECT COUNT(*) FROM TUsers WHERE IsActive = 1) AS ActiveUsers;
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_Notification_Create
                    @Message NVARCHAR(MAX),
                    @UserId INT,
                    @Date DATETIME
                AS
                BEGIN
                    INSERT INTO TNotifications (Message, UserId, IsRead, Date)
                    VALUES (@Message, @UserId, 0, @Date);
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_Notification_GetAll
                AS
                BEGIN
                    SELECT * FROM TNotifications ORDER BY Date DESC;
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_Notification_GetByUserId
                    @UserId INT
                AS
                BEGIN
                    SELECT * FROM TNotifications WHERE UserId = @UserId ORDER BY Date DESC;
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_Review_Insert
                    @FeedbackId INT,
                    @ReviewerId INT,
                    @Comments NVARCHAR(MAX),
                    @Date DATETIME
                AS
                BEGIN
                    INSERT INTO TReviews (FeedbackId, ReviewerId, Comments, Date)
                    VALUES (@FeedbackId, @ReviewerId, @Comments, @Date);
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_Review_Update
                    @Id INT,
                    @Comments NVARCHAR(MAX)
                AS
                BEGIN
                    UPDATE TReviews SET Comments = @Comments WHERE Id = @Id;
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_Review_GetAll
                AS
                BEGIN
                    SELECT * FROM TReviews ORDER BY Date DESC;
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_Review_GetByUserId
                    @UserId INT
                AS
                BEGIN
                    SELECT r.* FROM TReviews r
                    JOIN TFeedbacks f ON r.FeedbackId = f.Id
                    WHERE f.ToUserId = @UserId
                    ORDER BY r.Date DESC;
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_GetRecognition
                    @UserId INT
                AS
                BEGIN
                    SELECT * FROM TRecognitions WHERE ToUserId = @UserId ORDER BY Date DESC;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_FeedbackView;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_RecognitionView;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_ReviewView;");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_AddRecognition;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_GenerateReport;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_Notification_Create;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_Notification_GetAll;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_Notification_GetByUserId;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_Review_Insert;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_Review_Update;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_Review_GetAll;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_Review_GetByUserId;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_GetRecognition;");
        }
    }
}
