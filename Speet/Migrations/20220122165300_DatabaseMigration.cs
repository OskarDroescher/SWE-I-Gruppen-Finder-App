using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Speet.Migrations
{
    public partial class DatabaseMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityTag",
                columns: table => new
                {
                    ActivityCategory = table.Column<string>(type: "TEXT", nullable: false),
                    IconUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTag", x => x.ActivityCategory);
                });

            migrationBuilder.CreateTable(
                name: "GenderRestrictionTag",
                columns: table => new
                {
                    GenderRestriction = table.Column<string>(type: "TEXT", nullable: false),
                    IconUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenderRestrictionTag", x => x.GenderRestriction);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    GoogleId = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Birthday = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", nullable: false),
                    PictureUrl = table.Column<string>(type: "TEXT", nullable: false)
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
                    MaxParticipants = table.Column<int>(type: "INTEGER", nullable: false),
                    MeetupRecurrence = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedByGoogleId = table.Column<string>(type: "TEXT", nullable: false),
                    GenderRestrictionTagGenderRestriction = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SportGroup_GenderRestrictionTag_GenderRestrictionTagGenderRestriction",
                        column: x => x.GenderRestrictionTagGenderRestriction,
                        principalTable: "GenderRestrictionTag",
                        principalColumn: "GenderRestriction",
                        onDelete: ReferentialAction.Restrict);
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
                    ActivityTagsActivityCategory = table.Column<string>(type: "TEXT", nullable: false),
                    AssignedGroupsId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assigned", x => new { x.ActivityTagsActivityCategory, x.AssignedGroupsId });
                    table.ForeignKey(
                        name: "FK_Assigned_ActivityTag_ActivityTagsActivityCategory",
                        column: x => x.ActivityTagsActivityCategory,
                        principalTable: "ActivityTag",
                        principalColumn: "ActivityCategory",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assigned_SportGroup_AssignedGroupsId",
                        column: x => x.AssignedGroupsId,
                        principalTable: "SportGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Joins",
                columns: table => new
                {
                    JoinedGroupsId = table.Column<long>(type: "INTEGER", nullable: false),
                    ParticipantsGoogleId = table.Column<string>(type: "TEXT", nullable: false)
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
                name: "IX_Assigned_AssignedGroupsId",
                table: "Assigned",
                column: "AssignedGroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_Joins_ParticipantsGoogleId",
                table: "Joins",
                column: "ParticipantsGoogleId");

            migrationBuilder.CreateIndex(
                name: "IX_SportGroup_CreatedByGoogleId",
                table: "SportGroup",
                column: "CreatedByGoogleId");

            migrationBuilder.CreateIndex(
                name: "IX_SportGroup_GenderRestrictionTagGenderRestriction",
                table: "SportGroup",
                column: "GenderRestrictionTagGenderRestriction");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assigned");

            migrationBuilder.DropTable(
                name: "Joins");

            migrationBuilder.DropTable(
                name: "ActivityTag");

            migrationBuilder.DropTable(
                name: "SportGroup");

            migrationBuilder.DropTable(
                name: "GenderRestrictionTag");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
