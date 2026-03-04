using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedbackTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddExceptionHandlingToStoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update usp_Notification_MarkAsRead
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_Notification_MarkAsRead
                    @NotificationId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        UPDATE TNotifications SET IsRead = 1 WHERE Id = @NotificationId;
                    END TRY
                    BEGIN CATCH
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END;
            ");

            // Update usp_AddRecognition
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_AddRecognition
                    @FromUserId INT,
                    @ToUserId INT,
                    @BadgeType NVARCHAR(MAX),
                    @Points INT,
                    @Comments NVARCHAR(MAX),
                    @Date DATETIME
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        INSERT INTO TRecognitions (FromUserId, ToUserId, BadgeType, Points, Comments, Date)
                        VALUES (@FromUserId, @ToUserId, @BadgeType, @Points, @Comments, @Date);
                    END TRY
                    BEGIN CATCH
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END;
            ");

            // Update usp_GenerateReport
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_GenerateReport
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        SELECT 
                            (SELECT COUNT(*) FROM TFeedbacks) AS TotalFeedback,
                            (SELECT COUNT(*) FROM TRecognitions) AS TotalRecognitions,
                            (SELECT COUNT(*) FROM TUsers WHERE IsActive = 1) AS ActiveUsers;
                    END TRY
                    BEGIN CATCH
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END;
            ");

            // Update usp_Notification_Create
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_Notification_Create
                    @Message NVARCHAR(MAX),
                    @UserId INT,
                    @Date DATETIME
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        INSERT INTO TNotifications (Message, UserId, IsRead, Date)
                        VALUES (@Message, @UserId, 0, @Date);
                    END TRY
                    BEGIN CATCH
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END;
            ");

            // Update usp_Notification_GetAll
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_Notification_GetAll
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        SELECT * FROM TNotifications ORDER BY Date DESC;
                    END TRY
                    BEGIN CATCH
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END;
            ");

            // Update usp_Notification_GetByUserId
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_Notification_GetByUserId
                    @UserId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        SELECT * FROM TNotifications WHERE UserId = @UserId ORDER BY Date DESC;
                    END TRY
                    BEGIN CATCH
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END;
            ");

            // Update usp_Review_Insert
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_Review_Insert
                    @FeedbackId INT,
                    @ReviewerId INT,
                    @Comments NVARCHAR(MAX),
                    @Date DATETIME
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        INSERT INTO TReviews (FeedbackId, ReviewerId, Comments, Date)
                        VALUES (@FeedbackId, @ReviewerId, @Comments, @Date);
                    END TRY
                    BEGIN CATCH
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END;
            ");

            // Update usp_Review_Update
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_Review_Update
                    @Id INT,
                    @Comments NVARCHAR(MAX)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        UPDATE TReviews SET Comments = @Comments WHERE Id = @Id;
                    END TRY
                    BEGIN CATCH
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END;
            ");

            // Update usp_Review_GetAll
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_Review_GetAll
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        SELECT * FROM TReviews ORDER BY Date DESC;
                    END TRY
                    BEGIN CATCH
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END;
            ");

            // Update usp_Review_GetByUserId
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_Review_GetByUserId
                    @UserId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        SELECT r.* FROM TReviews r
                        JOIN TFeedbacks f ON r.FeedbackId = f.Id
                        WHERE f.ToUserId = @UserId
                        ORDER BY r.Date DESC;
                    END TRY
                    BEGIN CATCH
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END;
            ");

            // Update usp_GetRecognition
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_GetRecognition
                    @UserId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        SELECT * FROM TRecognitions WHERE ToUserId = @UserId ORDER BY Date DESC;
                    END TRY
                    BEGIN CATCH
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert to versions without TRY/CATCH if necessary, 
            // but usually we can just leave them as CREATE OR ALTER with TRY/CATCH 
            // since exception handling is almost always desired.
            // For a strict rollback, we'd redefine them without TRY/CATCH.
        }
    }
}
