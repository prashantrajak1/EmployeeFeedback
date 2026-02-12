using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedbackTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop previous stored procedure
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetUserFeedbackStats') DROP PROCEDURE sp_GetUserFeedbackStats");

            // Create usp_User_GetById
            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_User_GetById
                    @UserId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        BEGIN TRANSACTION
                            SELECT Id, Name, Email, IsActive, RoleId, DepartmentId 
                            FROM TUsers 
                            WHERE Id = @UserId;
                        COMMIT TRANSACTION
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END
            ");

            // Create usp_User_GetByEmail
            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_User_GetByEmail
                    @Email NVARCHAR(256)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        BEGIN TRANSACTION
                            SELECT Id, Name, Email, IsActive, RoleId, DepartmentId 
                            FROM TUsers 
                            WHERE Email = @Email;
                        COMMIT TRANSACTION
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END
            ");

            // Create usp_User_Insert
            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_User_Insert
                    @Name NVARCHAR(MAX),
                    @Email NVARCHAR(MAX),
                    @Password NVARCHAR(MAX),
                    @RoleId INT,
                    @DepartmentId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        BEGIN TRANSACTION
                            INSERT INTO TUsers (Name, Email, Password, IsActive, RoleId, DepartmentId)
                            VALUES (@Name, @Email, @Password, 1, @RoleId, @DepartmentId);
                            
                            SELECT SCOPE_IDENTITY() AS NewUserId;
                        COMMIT TRANSACTION
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END
            ");

            // Create usp_User_Update
            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_User_Update
                    @UserId INT,
                    @Name NVARCHAR(MAX),
                    @Email NVARCHAR(MAX),
                    @RoleId INT,
                    @DepartmentId INT,
                    @IsActive BIT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        BEGIN TRANSACTION
                            UPDATE TUsers
                            SET Name = @Name,
                                Email = @Email,
                                RoleId = @RoleId,
                                DepartmentId = @DepartmentId,
                                IsActive = @IsActive
                            WHERE Id = @UserId;
                        COMMIT TRANSACTION
                    END TRY
                    BEGIN CATCH
                        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
                        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                        RAISERROR(@ErrorMessage, 16, 1);
                    END CATCH
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_User_GetById");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_User_GetByEmail");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_User_Insert");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_User_Update");

            // Recreate sp_GetUserFeedbackStats (simplified rollback)
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetUserFeedbackStats
                @UserId INT
                AS
                BEGIN
                    SELECT 
                        @UserId AS UserId,
                        (SELECT COUNT(*) FROM TFeedbacks WHERE ToUserId = @UserId) AS FeedbackReceived,
                        (SELECT COUNT(*) FROM TFeedbacks WHERE FromUserId = @UserId) AS FeedbackGiven
                END
            ");
        }
    }
}
