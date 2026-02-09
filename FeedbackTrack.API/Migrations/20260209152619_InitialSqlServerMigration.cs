using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FeedbackTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServerMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDepartments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRoles", x => x.Id);
                });



            migrationBuilder.CreateTable(
                name: "TUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TUsers_TDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "TDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TUsers_TRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "TRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<int>(type: "int", nullable: true),
                    ToUserId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAnonymous = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TFeedbacks_TUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "TUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TFeedbacks_TUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "TUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TRecognitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<int>(type: "int", nullable: false),
                    ToUserId = table.Column<int>(type: "int", nullable: false),
                    BadgeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRecognitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TRecognitions_TUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "TUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TRecognitions_TUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "TUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedbackId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TReviews_TFeedbacks_FeedbackId",
                        column: x => x.FeedbackId,
                        principalTable: "TFeedbacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TDepartments",
                columns: new[] { "Id", "DepartmentName" },
                values: new object[,]
                {
                    { 1, "IT" },
                    { 2, "HR" },
                    { 3, "Sales" }
                });

            migrationBuilder.InsertData(
                table: "TRoles",
                columns: new[] { "Id", "RoleName" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Manager" },
                    { 3, "Employee" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TFeedbacks_FromUserId",
                table: "TFeedbacks",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TFeedbacks_ToUserId",
                table: "TFeedbacks",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TRecognitions_FromUserId",
                table: "TRecognitions",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TRecognitions_ToUserId",
                table: "TRecognitions",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TReviews_FeedbackId",
                table: "TReviews",
                column: "FeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_TUsers_DepartmentId",
                table: "TUsers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TUsers_RoleId",
                table: "TUsers",
                column: "RoleId");

            // Custom View and Stored Procedure (Moved to end to ensure tables exist)
            migrationBuilder.Sql(@"
                CREATE VIEW vw_UserProfileView AS
                SELECT u.Id AS UserId, u.Name AS FullName, u.Email, r.RoleName, d.DepartmentName
                FROM TUsers u
                LEFT JOIN TRoles r ON u.RoleId = r.Id
                LEFT JOIN TDepartments d ON u.DepartmentId = d.Id
            ");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TNotifications");

            migrationBuilder.DropTable(
                name: "TRecognitions");

            migrationBuilder.DropTable(
                name: "TReviews");

            migrationBuilder.Sql("DROP PROCEDURE sp_GetUserFeedbackStats");
            migrationBuilder.Sql("DROP VIEW vw_UserProfileView");

            migrationBuilder.DropTable(
                name: "TFeedbacks");

            migrationBuilder.DropTable(
                name: "TUsers");

            migrationBuilder.DropTable(
                name: "TDepartments");

            migrationBuilder.DropTable(
                name: "TRoles");
        }
    }
}
