using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedbackTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class FixUspGetRecognition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_GetRecognition
                    @UserId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    BEGIN TRY
                        SELECT * FROM vw_RecognitionView WHERE ToUserId = @UserId ORDER BY Date DESC;
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
    }
}
