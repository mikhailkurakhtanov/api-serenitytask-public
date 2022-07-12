using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerenityTask.API.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AchievementTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Goal = table.Column<int>(type: "int", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlantTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxLeaves = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeZoneTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeZoneId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeZoneIdIANA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZoneTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    IsUserOnline = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsGoogleCalendarConnected = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmationTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfirmationTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoogleCalendarAccessRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsAccessGranted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoogleCalendarAccessRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoogleCalendarAccessRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoogleCredentials",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiresAt = table.Column<long>(type: "bigint", nullable: false),
                    ExpiresIn = table.Column<long>(type: "bigint", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CalendarId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoogleCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoogleCredentials_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HubConnections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HubConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Browser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HubConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HubConnections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CurrentExperience = table.Column<double>(type: "float", nullable: false),
                    MaxExperience = table.Column<int>(type: "int", nullable: false),
                    TotalDeadLeaves = table.Column<int>(type: "int", nullable: false),
                    IsDead = table.Column<bool>(type: "bit", nullable: false),
                    IsGrowthFinished = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantTypeId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plants_PlantTypes_PlantTypeId",
                        column: x => x.PlantTypeId,
                        principalTable: "PlantTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Plants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SessionRequests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<long>(type: "bigint", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionRequests_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessionRequests_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<long>(type: "bigint", nullable: false),
                    IsHardModeActived = table.Column<bool>(type: "bit", nullable: false),
                    SessionMembersJSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoogleCalendarEventId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SettingsNotifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Result = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingsNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettingsNotifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemNotifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemNotifications_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    TrackedTime = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    ParentTaskId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Tasks_ParentTaskId",
                        column: x => x.ParentTaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserConnectors",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FriendId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConnectors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConnectors_Users_FriendId",
                        column: x => x.FriendId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserConnectors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interests = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Languages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscordTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LookingFor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TelegramUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeZoneId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDetails_TimeZoneTypes_TimeZoneId",
                        column: x => x.TimeZoneId,
                        principalTable: "TimeZoneTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserDetails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserNotifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotifications_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserNotifications_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimerSettings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SessionUser",
                columns: table => new
                {
                    ParticipantsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionUser", x => new { x.ParticipantsId, x.SessionsId });
                    table.ForeignKey(
                        name: "FK_SessionUser_Sessions_SessionsId",
                        column: x => x.SessionsId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessionUser_Users_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<double>(type: "float", nullable: false),
                    TaskId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Files_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlantHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedExperience = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskDetailsJSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionDetailsJSON = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantId = table.Column<long>(type: "bigint", nullable: false),
                    ExperienceObjectType = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantHistory_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlantHistory_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskHistory_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    UserDetailsId = table.Column<long>(type: "bigint", nullable: false),
                    TypeId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Achievements_AchievementTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "AchievementTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Achievements_UserDetails_UserDetailsId",
                        column: x => x.UserDetailsId,
                        principalTable: "UserDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AchievementTypes",
                columns: new[] { "Id", "Description", "Goal", "Icon", "Name", "Rate" },
                values: new object[,]
                {
                    { 1L, "Grow your first plant without loosing level", 1, "https://storage.serenitytask.com/system/media/pictures/achievements/gardener/tier_1.png", "Gardener", 1 },
                    { 2L, "Grow your third plant without loosing level", 3, "https://storage.serenitytask.com/system/media/pictures/achievements/gardener/tier_2.png", "Gardener", 2 },
                    { 3L, "Grow your fifth plant without lossing level", 5, "https://storage.serenitytask.com/system/media/pictures/achievements/gardener/tier_3.png", "Gardener", 3 },
                    { 4L, "Complete 10 tasks before deadline", 10, "https://storage.serenitytask.com/system/media/pictures/achievements/serenity_soul/tier_1.png", "Serenity Soul", 1 },
                    { 5L, "Complete 25 tasks before deadline", 25, "https://storage.serenitytask.com/system/media/pictures/achievements/serenity_soul/tier_2.png", "Serenity Soul", 2 },
                    { 6L, "Complete 50 tasks before deadline", 50, "https://storage.serenitytask.com/system/media/pictures/achievements/serenity_soul/tier_3.png", "Serenity Soul", 3 },
                    { 7L, "Start growing your first plant", 1, "https://storage.serenitytask.com/system/media/pictures/achievements/ordinary/first_step.png", "First Step", 0 }
                });

            migrationBuilder.InsertData(
                table: "PlantTypes",
                columns: new[] { "Id", "MaxLeaves", "Name" },
                values: new object[,]
                {
                    { 1L, 5, "Sorana" },
                    { 2L, 17, "Selnera" }
                });

            migrationBuilder.InsertData(
                table: "Quotes",
                columns: new[] { "Id", "AuthorName", "Context" },
                values: new object[,]
                {
                    { 1L, "Harriet Beecher Stowe", "“Never give up, for that is just the place and time that the tide will turn.”" },
                    { 2L, "Elbert Hubbard", "“There is no failure except in no longer trying.”" },
                    { 3L, "James A. Michener", "“Character consists of what you do on the third and fourth tries.”" },
                    { 4L, "Chuck Yeager", "“You do what you can for as long as you can, and when you finally can’t, you do the next best thing. You back up but you don’t give up.”" },
                    { 5L, "Roy T. Bennett", "“Do not fear failure but rather fear not trying.”" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                table: "TimeZoneTypes",
                columns: new[] { "Id", "DisplayName", "TimeZoneId", "TimeZoneIdIANA" },
                values: new object[,]
                {
                    { 1L, "(UTC-12:00) International Date Line West", "Dateline Standard Time", "Etc/GMT+12" },
                    { 2L, "(UTC-11:00) Coordinated Universal Time-11", "UTC-11", "Etc/GMT+11" },
                    { 3L, "(UTC-10:00) Aleutian Islands", "Aleutian Standard Time", "America/Adak" },
                    { 4L, "(UTC-10:00) Hawaii", "Hawaiian Standard Time", "Pacific/Honolulu" },
                    { 5L, "(UTC-09:30) Marquesas Islands", "Marquesas Standard Time", "Pacific/Marquesas" },
                    { 6L, "(UTC-09:00) Alaska", "Alaskan Standard Time", "America/Anchorage" },
                    { 7L, "(UTC-09:00) Coordinated Universal Time-09", "UTC-09", "Etc/GMT+9" },
                    { 8L, "(UTC-08:00) Baja California", "Pacific Standard Time (Mexico)", "America/Tijuana" },
                    { 9L, "(UTC-08:00) Coordinated Universal Time-08", "UTC-08", "Etc/GMT+8" },
                    { 10L, "(UTC-08:00) Pacific Time (US & Canada)", "Pacific Standard Time", "America/Los_Angeles" },
                    { 11L, "(UTC-07:00) Arizona", "US Mountain Standard Time", "America/Phoenix" },
                    { 12L, "(UTC-07:00) Chihuahua, La Paz, Mazatlan", "Mountain Standard Time (Mexico)", "America/Chihuahua" },
                    { 13L, "(UTC-07:00) Mountain Time (US & Canada)", "Mountain Standard Time", "America/Denver" },
                    { 14L, "(UTC-07:00) Yukon", "Yukon Standard Time", "America/Whitehorse" },
                    { 15L, "(UTC-06:00) Central America", "Central America Standard Time", "America/Guatemala" },
                    { 16L, "(UTC-06:00) Central Time (US & Canada)", "Central Standard Time", "America/Chicago" },
                    { 17L, "(UTC-06:00) Easter Island", "Easter Island Standard Time", "Pacific/Easter" },
                    { 18L, "(UTC-06:00) Guadalajara, Mexico City, Monterrey", "Central Standard Time (Mexico)", "America/Mexico_City" },
                    { 19L, "(UTC-06:00) Saskatchewan", "Canada Central Standard Time", "America/Regina" },
                    { 20L, "(UTC-05:00) Bogota, Lima, Quito, Rio Branco", "SA Pacific Standard Time", "America/Bogota" },
                    { 21L, "(UTC-05:00) Chetumal", "Eastern Standard Time (Mexico)", "America/Cancun" },
                    { 22L, "(UTC-05:00) Eastern Time (US & Canada)", "Eastern Standard Time", "America/New_York" },
                    { 23L, "(UTC-05:00) Haiti", "Haiti Standard Time", "America/Port-au-Prince" },
                    { 24L, "(UTC-05:00) Havana", "Cuba Standard Time", "America/Havana" },
                    { 25L, "(UTC-05:00) Indiana (East)", "US Eastern Standard Time", "America/Indiana/Indianapolis" },
                    { 26L, "(UTC-05:00) Turks and Caicos", "Turks And Caicos Standard Time", "America/Grand_Turk" }
                });

            migrationBuilder.InsertData(
                table: "TimeZoneTypes",
                columns: new[] { "Id", "DisplayName", "TimeZoneId", "TimeZoneIdIANA" },
                values: new object[,]
                {
                    { 27L, "(UTC-04:00) Asuncion", "Paraguay Standard Time", "America/Asuncion" },
                    { 28L, "(UTC-04:00) Atlantic Time (Canada)", "Atlantic Standard Time", "America/Halifax" },
                    { 29L, "(UTC-04:00) Caracas", "Venezuela Standard Time", "America/Caracas" },
                    { 30L, "(UTC-04:00) Cuiaba", "Central Brazilian Standard Time", "America/Cuiaba" },
                    { 31L, "(UTC-04:00) Georgetown, La Paz, Manaus, San Juan", "SA Western Standard Time", "America/La_Paz" },
                    { 32L, "(UTC-04:00) Santiago", "Pacific SA Standard Time", "America/Santiago" },
                    { 33L, "(UTC-03:30) Newfoundland", "Newfoundland Standard Time", "America/St_Johns" },
                    { 34L, "(UTC-03:00) Araguaina", "Tocantins Standard Time", "America/Araguaina" },
                    { 35L, "(UTC-03:00) Brasilia", "E. South America Standard Time", "America/Sao_Paulo" },
                    { 36L, "(UTC-03:00) Cayenne, Fortaleza", "SA Eastern Standard Time", "America/Cayenne" },
                    { 37L, "(UTC-03:00) City of Buenos Aires", "Argentina Standard Time", "America/Argentina/Buenos_Aires" },
                    { 38L, "(UTC-03:00) Greenland", "Greenland Standard Time", "America/Nuuk" },
                    { 39L, "(UTC-03:00) Montevideo", "Montevideo Standard Time", "America/Montevideo" },
                    { 40L, "(UTC-03:00) Punta Arenas", "Magallanes Standard Time", "America/Punta_Arenas" },
                    { 41L, "(UTC-03:00) Saint Pierre and Miquelon", "Saint Pierre Standard Time", "America/Miquelon" },
                    { 42L, "(UTC-03:00) Salvador", "Bahia Standard Time", "America/Bahia" },
                    { 43L, "(UTC-02:00) Coordinated Universal Time-02", "UTC-02", "Etc/GMT+2" },
                    { 44L, "(UTC-02:00) Mid-Atlantic - Old", "Mid-Atlantic Standard Time", "Etc/GMT+2" },
                    { 45L, "(UTC-01:00) Azores", "Azores Standard Time", "Atlantic/Azores" },
                    { 46L, "(UTC-01:00) Cabo Verde Is.", "Cape Verde Standard Time", "Atlantic/Cape_Verde" },
                    { 47L, "(UTC) Coordinated Universal Time", "UTC", "Etc/UTC" },
                    { 48L, "(UTC+00:00) Dublin, Edinburgh, Lisbon, London", "GMT Standard Time", "Europe/London" },
                    { 49L, "(UTC+00:00) Monrovia, Reykjavik", "Greenwich Standard Time", "Atlantic/Reykjavik" },
                    { 50L, "(UTC+00:00) Sao Tome", "Sao Tome Standard Time", "Africa/Sao_Tome" },
                    { 51L, "(UTC+01:00) Casablanca", "Morocco Standard Time", "Africa/Casablanca" },
                    { 52L, "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna", "W. Europe Standard Time", "Europe/Berlin" },
                    { 53L, "(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague", "Central Europe Standard Time", "Europe/Budapest" },
                    { 54L, "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris", "Romance Standard Time", "Europe/Paris" },
                    { 55L, "(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb", "Central European Standard Time", "Europe/Warsaw" },
                    { 56L, "(UTC+01:00) West Central Africa", "W. Central Africa Standard Time", "Africa/Lagos" },
                    { 57L, "(UTC+02:00) Amman", "Jordan Standard Time", "Asia/Amman" },
                    { 58L, "(UTC+02:00) Athens, Bucharest", "GTB Standard Time", "Europe/Bucharest" },
                    { 59L, "(UTC+02:00) Beirut", "Middle East Standard Time", "Asia/Beirut" },
                    { 60L, "(UTC+02:00) Cairo", "Egypt Standard Time", "Africa/Cairo" },
                    { 61L, "(UTC+02:00) Chisinau", "E. Europe Standard Time", "Europe/Chisinau" },
                    { 62L, "(UTC+02:00) Damascus", "Syria Standard Time", "Asia/Damascus" },
                    { 63L, "(UTC+02:00) Gaza, Hebron", "West Bank Standard Time", "Asia/Hebron" },
                    { 64L, "(UTC+02:00) Harare, Pretoria", "South Africa Standard Time", "Africa/Johannesburg" },
                    { 65L, "(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius", "FLE Standard Time", "Europe/Kiev" },
                    { 66L, "(UTC+02:00) Jerusalem", "Israel Standard Time", "Asia/Jerusalem" },
                    { 67L, "(UTC+02:00) Juba", "South Sudan Standard Time", "Africa/Juba" },
                    { 68L, "(UTC+02:00) Kaliningrad", "Kaliningrad Standard Time", "Europe/Kaliningrad" }
                });

            migrationBuilder.InsertData(
                table: "TimeZoneTypes",
                columns: new[] { "Id", "DisplayName", "TimeZoneId", "TimeZoneIdIANA" },
                values: new object[,]
                {
                    { 69L, "(UTC+02:00) Khartoum", "Sudan Standard Time", "Africa/Khartoum" },
                    { 70L, "(UTC+02:00) Tripoli", "Libya Standard Time", "Africa/Tripoli" },
                    { 71L, "(UTC+02:00) Windhoek", "Namibia Standard Time", "Africa/Windhoek" },
                    { 72L, "(UTC+03:00) Baghdad", "Arabic Standard Time", "Asia/Baghdad" },
                    { 73L, "(UTC+03:00) Istanbul", "Turkey Standard Time", "Europe/Istanbul" },
                    { 74L, "(UTC+03:00) Kuwait, Riyadh", "Arab Standard Time", "Asia/Riyadh" },
                    { 75L, "(UTC+03:00) Minsk", "Belarus Standard Time", "Europe/Minsk" },
                    { 76L, "(UTC+03:00) Moscow, St. Petersburg", "Russian Standard Time", "Europe/Moscow" },
                    { 77L, "(UTC+03:00) Nairobi", "E. Africa Standard Time", "Africa/Nairobi" },
                    { 78L, "(UTC+03:00) Volgograd", "Volgograd Standard Time", "Europe/Volgograd" },
                    { 79L, "(UTC+03:30) Tehran", "Iran Standard Time", "Asia/Tehran" },
                    { 80L, "(UTC+04:00) Abu Dhabi, Muscat", "Arabian Standard Time", "Asia/Dubai" },
                    { 81L, "(UTC+04:00) Astrakhan, Ulyanovsk", "Astrakhan Standard Time", "Europe/Astrakhan" },
                    { 82L, "(UTC+04:00) Baku", "Azerbaijan Standard Time", "Asia/Baku" },
                    { 83L, "(UTC+04:00) Izhevsk, Samara", "Russia Time Zone 3", "Europe/Samara" },
                    { 84L, "(UTC+04:00) Port Louis", "Mauritius Standard Time", "Indian/Mauritius" },
                    { 85L, "(UTC+04:00) Saratov", "Saratov Standard Time", "Europe/Saratov" },
                    { 86L, "(UTC+04:00) Tbilisi", "Georgian Standard Time", "Asia/Tbilisi" },
                    { 87L, "(UTC+04:00) Yerevan", "Caucasus Standard Time", "Asia/Yerevan" },
                    { 88L, "(UTC+04:30) Kabul", "Afghanistan Standard Time", "Asia/Kabul" },
                    { 89L, "(UTC+05:00) Ashgabat, Tashkent", "West Asia Standard Time", "Asia/Tashkent" },
                    { 90L, "(UTC+05:00) Ekaterinburg", "Ekaterinburg Standard Time", "Asia/Yekaterinburg" },
                    { 91L, "(UTC+05:00) Islamabad, Karachi", "Pakistan Standard Time", "Asia/Karachi" },
                    { 92L, "(UTC+05:00) Qyzylorda", "Qyzylorda Standard Time", "Asia/Qyzylorda" },
                    { 93L, "(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi", "India Standard Time", "Asia/Kolkata" },
                    { 94L, "(UTC+05:30) Sri Jayawardenepura", "Sri Lanka Standard Time", "Asia/Colombo" },
                    { 95L, "(UTC+05:45) Kathmandu", "Nepal Standard Time", "Asia/Kathmandu" },
                    { 96L, "(UTC+06:00) Astana", "Central Asia Standard Time", "Asia/Almaty" },
                    { 97L, "(UTC+06:00) Dhaka", "Bangladesh Standard Time", "Asia/Dhaka" },
                    { 98L, "(UTC+06:00) Omsk", "Omsk Standard Time", "Asia/Omsk" },
                    { 99L, "(UTC+06:30) Yangon (Rangoon)", "Myanmar Standard Time", "Asia/Yangon" },
                    { 100L, "(UTC+07:00) Bangkok, Hanoi, Jakarta", "SE Asia Standard Time", "Asia/Bangkok" },
                    { 101L, "(UTC+07:00) Barnaul, Gorno-Altaysk", "Altai Standard Time", "Asia/Barnaul" },
                    { 102L, "(UTC+07:00) Hovd", "W. Mongolia Standard Time", "Asia/Hovd" },
                    { 103L, "(UTC+07:00) Krasnoyarsk", "North Asia Standard Time", "Asia/Krasnoyarsk" },
                    { 104L, "(UTC+07:00) Novosibirsk", "N. Central Asia Standard Time", "Asia/Novosibirsk" },
                    { 105L, "(UTC+07:00) Tomsk", "Tomsk Standard Time", "Asia/Tomsk" },
                    { 106L, "(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi", "China Standard Time", "Asia/Shanghai" },
                    { 107L, "(UTC+08:00) Irkutsk", "North Asia East Standard Time", "Asia/Irkutsk" },
                    { 108L, "(UTC+08:00) Kuala Lumpur, Singapore", "Singapore Standard Time", "Asia/Singapore" },
                    { 109L, "(UTC+08:00) Perth", "W. Australia Standard Time", "Australia/Perth" },
                    { 110L, "(UTC+08:00) Taipei", "Taipei Standard Time", "Asia/Taipei" }
                });

            migrationBuilder.InsertData(
                table: "TimeZoneTypes",
                columns: new[] { "Id", "DisplayName", "TimeZoneId", "TimeZoneIdIANA" },
                values: new object[,]
                {
                    { 111L, "(UTC+08:00) Ulaanbaatar", "Ulaanbaatar Standard Time", "Asia/Ulaanbaatar" },
                    { 112L, "(UTC+08:45) Eucla", "Aus Central W. Standard Time", "Australia/Eucla" },
                    { 113L, "(UTC+09:00) Chita", "Transbaikal Standard Time", "Asia/Chita" },
                    { 114L, "(UTC+09:00) Osaka, Sapporo, Tokyo", "Tokyo Standard Time", "Asia/Tokyo" },
                    { 115L, "(UTC+09:00) Pyongyang", "North Korea Standard Time", "Asia/Pyongyang" },
                    { 116L, "(UTC+09:00) Seoul", "Korea Standard Time", "Asia/Seoul" },
                    { 117L, "(UTC+09:00) Yakutsk", "Yakutsk Standard Time", "Asia/Yakutsk" },
                    { 118L, "(UTC+09:30) Adelaide", "Cen. Australia Standard Time", "Australia/Adelaide" },
                    { 119L, "(UTC+09:30) Darwin", "AUS Central Standard Time", "Australia/Darwin" },
                    { 120L, "(UTC+10:00) Brisbane", "E. Australia Standard Time", "Australia/Brisbane" },
                    { 121L, "(UTC+10:00) Canberra, Melbourne, Sydney", "AUS Eastern Standard Time", "Australia/Sydney" },
                    { 122L, "(UTC+10:00) Guam, Port Moresby", "West Pacific Standard Time", "Pacific/Port_Moresby" },
                    { 123L, "(UTC+10:00) Hobart", "Tasmania Standard Time", "Australia/Hobart" },
                    { 124L, "(UTC+10:00) Vladivostok", "Vladivostok Standard Time", "Asia/Vladivostok" },
                    { 125L, "(UTC+10:30) Lord Howe Island", "Lord Howe Standard Time", "Australia/Lord_Howe" },
                    { 126L, "(UTC+11:00) Bougainville Island", "Bougainville Standard Time", "Pacific/Bougainville" },
                    { 127L, "(UTC+11:00) Chokurdakh", "Russia Time Zone 10", "Asia/Srednekolymsk" },
                    { 128L, "(UTC+11:00) Magadan", "Magadan Standard Time", "Asia/Magadan" },
                    { 129L, "(UTC+11:00) Norfolk Island", "Norfolk Standard Time", "Pacific/Norfolk" },
                    { 130L, "(UTC+11:00) Sakhalin", "Sakhalin Standard Time", "Asia/Sakhalin" },
                    { 131L, "(UTC+11:00) Solomon Is., New Caledonia", "Central Pacific Standard Time", "Pacific/Guadalcanal" },
                    { 132L, "(UTC+12:00) Anadyr, Petropavlovsk-Kamchatsky", "Russia Time Zone 11", "Asia/Kamchatka" },
                    { 133L, "(UTC+12:00) Auckland, Wellington", "New Zealand Standard Time", "Pacific/Auckland" },
                    { 134L, "(UTC+12:00) Coordinated Universal Time+12", "UTC+12", "Etc/GMT-12" },
                    { 135L, "(UTC+12:00) Fiji", "Fiji Standard Time", "Pacific/Fiji" },
                    { 136L, "(UTC+12:00) Petropavlovsk-Kamchatsky - Old", "Kamchatka Standard Time", "Asia/Kamchatka" },
                    { 137L, "(UTC+12:45) Chatham Islands", "Chatham Islands Standard Time", "Pacific/Chatham" },
                    { 138L, "(UTC+13:00) Coordinated Universal Time+13", "UTC+13", "Etc/GMT-13" },
                    { 139L, "(UTC+13:00) Nuku'alofa", "Tonga Standard Time", "Pacific/Tongatapu" },
                    { 140L, "(UTC+13:00) Samoa", "Samoa Standard Time", "Pacific/Apia" },
                    { 141L, "(UTC+14:00) Kiritimati Island", "Line Islands Standard Time", "Pacific/Kiritimati" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsEmailConfirmed", "IsGoogleCalendarConnected", "IsUserOnline", "Name", "PasswordHash", "RoleId", "Username" },
                values: new object[] { new Guid("6073e6de-ed9d-4fbd-b382-a41262ce2ae4"), "mikhail@csfullstack.com", true, false, false, "Mikhail", "4OcqV3nepadpciZId7IpzwgzBmsmttHcf3uiyAFRXuo=", 1, "mikhail@csfullstack.com" });

            migrationBuilder.InsertData(
                table: "UserDetails",
                columns: new[] { "Id", "Age", "Avatar", "DiscordTag", "Interests", "Languages", "LookingFor", "TelegramUsername", "TimeZoneId", "UserId" },
                values: new object[] { 1L, null, "assets/img/authorized/workspace/user.png", null, null, null, null, null, null, new Guid("6073e6de-ed9d-4fbd-b382-a41262ce2ae4") });

            migrationBuilder.InsertData(
                table: "UserSettings",
                columns: new[] { "Id", "TimerSettings", "UserId" },
                values: new object[] { 1L, "{ \"mode\": 0, \"type\": 0 }", new Guid("6073e6de-ed9d-4fbd-b382-a41262ce2ae4") });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_TypeId",
                table: "Achievements",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_UserDetailsId",
                table: "Achievements",
                column: "UserDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationTokens_UserId",
                table: "ConfirmationTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_TaskId",
                table: "Files",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_UserId",
                table: "Files",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GoogleCalendarAccessRequests_UserId",
                table: "GoogleCalendarAccessRequests",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoogleCredentials_UserId",
                table: "GoogleCredentials",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HubConnections_UserId",
                table: "HubConnections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantHistory_PlantId",
                table: "PlantHistory",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantHistory_TaskId",
                table: "PlantHistory",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_PlantTypeId",
                table: "Plants",
                column: "PlantTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_UserId",
                table: "Plants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionRequests_ReceiverId",
                table: "SessionRequests",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionRequests_SenderId",
                table: "SessionRequests",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_OwnerId",
                table: "Sessions",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionUser_SessionsId",
                table: "SessionUser",
                column: "SessionsId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingsNotifications_UserId",
                table: "SettingsNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemNotifications_ReceiverId",
                table: "SystemNotifications",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistory_TaskId",
                table: "TaskHistory",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ParentTaskId",
                table: "Tasks",
                column: "ParentTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserConnectors_FriendId",
                table: "UserConnectors",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_UserConnectors_UserId",
                table: "UserConnectors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_TimeZoneId",
                table: "UserDetails",
                column: "TimeZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_UserId",
                table: "UserDetails",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_ReceiverId",
                table: "UserNotifications",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_SenderId",
                table: "UserNotifications",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "ConfirmationTokens");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "GoogleCalendarAccessRequests");

            migrationBuilder.DropTable(
                name: "GoogleCredentials");

            migrationBuilder.DropTable(
                name: "HubConnections");

            migrationBuilder.DropTable(
                name: "PlantHistory");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "SessionRequests");

            migrationBuilder.DropTable(
                name: "SessionUser");

            migrationBuilder.DropTable(
                name: "SettingsNotifications");

            migrationBuilder.DropTable(
                name: "SystemNotifications");

            migrationBuilder.DropTable(
                name: "TaskHistory");

            migrationBuilder.DropTable(
                name: "UserConnectors");

            migrationBuilder.DropTable(
                name: "UserNotifications");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "AchievementTypes");

            migrationBuilder.DropTable(
                name: "UserDetails");

            migrationBuilder.DropTable(
                name: "Plants");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "TimeZoneTypes");

            migrationBuilder.DropTable(
                name: "PlantTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
