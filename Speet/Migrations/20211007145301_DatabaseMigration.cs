using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Speet.Migrations
{
    public partial class DatabaseMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IconUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Description);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    GoogleId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.GoogleId);
                });

            migrationBuilder.CreateTable(
                name: "SportGroup",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupName = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    MeetupDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByGoogleId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SportGroup_User_CreatedByGoogleId",
                        column: x => x.CreatedByGoogleId,
                        principalTable: "User",
                        principalColumn: "GoogleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assigned",
                columns: table => new
                {
                    AssignedGroupsId = table.Column<long>(type: "INTEGER", nullable: false),
                    TagsDescription = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assigned", x => new { x.AssignedGroupsId, x.TagsDescription });
                    table.ForeignKey(
                        name: "FK_Assigned_SportGroup_AssignedGroupsId",
                        column: x => x.AssignedGroupsId,
                        principalTable: "SportGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assigned_Tag_TagsDescription",
                        column: x => x.TagsDescription,
                        principalTable: "Tag",
                        principalColumn: "Description",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Joins",
                columns: table => new
                {
                    JoinedGroupsId = table.Column<long>(type: "INTEGER", nullable: false),
                    ParticipantsGoogleId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Joins", x => new { x.JoinedGroupsId, x.ParticipantsGoogleId });
                    table.ForeignKey(
                        name: "FK_Joins_SportGroup_JoinedGroupsId",
                        column: x => x.JoinedGroupsId,
                        principalTable: "SportGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Joins_User_ParticipantsGoogleId",
                        column: x => x.ParticipantsGoogleId,
                        principalTable: "User",
                        principalColumn: "GoogleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assigned_TagsDescription",
                table: "Assigned",
                column: "TagsDescription");

            migrationBuilder.CreateIndex(
                name: "IX_Joins_ParticipantsGoogleId",
                table: "Joins",
                column: "ParticipantsGoogleId");

            migrationBuilder.CreateIndex(
                name: "IX_SportGroup_CreatedByGoogleId",
                table: "SportGroup",
                column: "CreatedByGoogleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assigned");

            migrationBuilder.DropTable(
                name: "Joins");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "SportGroup");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
