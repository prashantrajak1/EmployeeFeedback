using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedbackTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class FixRecognitionSPJoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER PROCEDURE usp_GetRecognition
                    @UserId INT
                AS
                BEGIN
                    SELECT * FROM vw_RecognitionView WHERE ToUserId = @UserId ORDER BY Date DESC;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER PROCEDURE usp_GetRecognition
                    @UserId INT
                AS
                BEGIN
                    SELECT * FROM TRecognitions WHERE ToUserId = @UserId ORDER BY Date DESC;
                END;
            ");
        }
    }
}
