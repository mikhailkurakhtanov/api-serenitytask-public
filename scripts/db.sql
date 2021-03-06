USE [u1356333_SerenityTask_Develop]
GO
/****** Object:  Table [u1356333_mikhail].[__EFMigrationsHistory]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[ChatMessages]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[ChatMessages](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[SendingDate] [datetime2](7) NOT NULL,
	[Type] [int] NOT NULL,
	[IsRead] [bit] NOT NULL,
	[SenderId] [uniqueidentifier] NOT NULL,
	[ReceiverId] [uniqueidentifier] NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ChatMessages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[ConfirmationTokens]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[ConfirmationTokens](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Token] [nvarchar](max) NOT NULL,
	[Type] [int] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ExpirationDate] [datetime2](7) NOT NULL,
	[ActivationDate] [datetime2](7) NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[IsUsed] [bit] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ConfirmationTokens] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[Files]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[Files](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Size] [float] NOT NULL,
	[TaskId] [bigint] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Extension] [nvarchar](max) NULL,
	[UploadDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[GoogleCalendarAccessRequests]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[GoogleCalendarAccessRequests](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[IsAccessGranted] [bit] NOT NULL,
 CONSTRAINT [PK_GoogleCalendarAccessRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[GoogleCredentials]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[GoogleCredentials](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AccessToken] [nvarchar](max) NULL,
	[TokenType] [nvarchar](max) NULL,
	[ExpiresIn] [bigint] NULL,
	[RefreshToken] [nvarchar](max) NULL,
	[Scope] [nvarchar](max) NULL,
	[IssuedUtc] [datetime2](7) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[CalendarId] [nvarchar](max) NULL,
 CONSTRAINT [PK_GoogleCredentials] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[HubConnections]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[HubConnections](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[HubConnectionId] [nvarchar](max) NOT NULL,
	[Browser] [nvarchar](max) NULL,
	[BrowserVersion] [nvarchar](max) NULL,
	[OS] [nvarchar](max) NULL,
	[OSVersion] [nvarchar](max) NULL,
	[UserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_HubConnections] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[PlantHistory]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[PlantHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ActionDate] [datetime2](7) NOT NULL,
	[ExperienceStatus] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[PlantId] [bigint] NOT NULL,
	[TaskId] [bigint] NOT NULL,
 CONSTRAINT [PK_PlantHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[Plants]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[Plants](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[CreationDate] [datetime2](7) NULL,
	[Age] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[CurrentExperience] [float] NOT NULL,
	[MaxExperience] [int] NOT NULL,
	[TotalDeadLeaves] [int] NOT NULL,
	[IsDead] [bit] NOT NULL,
	[IsGrowthFinished] [bit] NOT NULL,
	[PlantTypeId] [bigint] NULL,
	[UserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Plants] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[PlantTypes]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[PlantTypes](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[MaxLeaves] [int] NOT NULL,
 CONSTRAINT [PK_PlantTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[Quotes]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[Quotes](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AuthorName] [nvarchar](max) NOT NULL,
	[Context] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Quotes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[Roles]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[Roles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[Sessions]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[Sessions](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[IsHardModeActived] [bit] NOT NULL,
	[OwnerId] [uniqueidentifier] NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[Duration] [int] NOT NULL,
 CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[SessionUser]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[SessionUser](
	[ParticipantsId] [uniqueidentifier] NOT NULL,
	[SessionsId] [bigint] NOT NULL,
 CONSTRAINT [PK_SessionUser] PRIMARY KEY CLUSTERED 
(
	[ParticipantsId] ASC,
	[SessionsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[SettingsNotifications]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[SettingsNotifications](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Result] [bit] NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Type] [int] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_SettingsNotifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[SystemNotifications]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[SystemNotifications](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[Type] [int] NOT NULL,
	[IsRead] [bit] NOT NULL,
	[ReceiverId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_SystemNotifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[TaskHistory]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[TaskHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Action] [nvarchar](max) NULL,
	[TaskId] [bigint] NOT NULL,
 CONSTRAINT [PK_TaskHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[Tasks]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[Tasks](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[Date] [datetime2](7) NULL,
	[ReminderDate] [datetime2](7) NULL,
	[Deadline] [datetime2](7) NULL,
	[Priority] [int] NOT NULL,
	[IsCompleted] [bit] NOT NULL,
	[ParentTaskId] [bigint] NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[TrackedTime] [int] NOT NULL,
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[UserConnectors]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[UserConnectors](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[FriendId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UserConnectors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[UserDetails]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[UserDetails](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Avatar] [nvarchar](max) NULL,
	[Age] [nvarchar](max) NULL,
	[TimeZone] [nvarchar](max) NULL,
	[Interests] [nvarchar](max) NULL,
	[DiscordTag] [nvarchar](max) NULL,
	[TelegramUsername] [nvarchar](max) NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Languages] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[UserNotifications]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[UserNotifications](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[Type] [int] NOT NULL,
	[IsRead] [bit] NOT NULL,
	[SenderId] [uniqueidentifier] NOT NULL,
	[ReceiverId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UserNotifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[Users]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Username] [nvarchar](max) NOT NULL,
	[PasswordHash] [nvarchar](max) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[IsEmailConfirmed] [bit] NOT NULL,
	[RoleId] [int] NOT NULL,
	[IsUserOnline] [bit] NOT NULL,
	[GoogleOAuthToken] [nvarchar](max) NULL,
	[IsGoogleCalendarConnected] [bit] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [u1356333_mikhail].[UserSettings]    Script Date: 5/23/2022 6:29:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [u1356333_mikhail].[UserSettings](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TimerSettings] [nvarchar](max) NULL,
	[UserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_UserSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [u1356333_mikhail].[ChatMessages] ADD  DEFAULT (N'') FOR [Message]
GO
ALTER TABLE [u1356333_mikhail].[GoogleCredentials] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [IssuedUtc]
GO
ALTER TABLE [u1356333_mikhail].[Sessions] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [StartDate]
GO
ALTER TABLE [u1356333_mikhail].[Sessions] ADD  DEFAULT ((0)) FOR [Duration]
GO
ALTER TABLE [u1356333_mikhail].[TaskHistory] ADD  DEFAULT (CONVERT([bigint],(0))) FOR [TaskId]
GO
ALTER TABLE [u1356333_mikhail].[Tasks] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [UserId]
GO
ALTER TABLE [u1356333_mikhail].[Tasks] ADD  DEFAULT ((0)) FOR [TrackedTime]
GO
ALTER TABLE [u1356333_mikhail].[UserDetails] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [UserId]
GO
ALTER TABLE [u1356333_mikhail].[Users] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsUserOnline]
GO
ALTER TABLE [u1356333_mikhail].[Users] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsGoogleCalendarConnected]
GO
ALTER TABLE [u1356333_mikhail].[ChatMessages]  WITH CHECK ADD  CONSTRAINT [FK_ChatMessages_Users_ReceiverId] FOREIGN KEY([ReceiverId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[ChatMessages] CHECK CONSTRAINT [FK_ChatMessages_Users_ReceiverId]
GO
ALTER TABLE [u1356333_mikhail].[ChatMessages]  WITH CHECK ADD  CONSTRAINT [FK_ChatMessages_Users_SenderId] FOREIGN KEY([SenderId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[ChatMessages] CHECK CONSTRAINT [FK_ChatMessages_Users_SenderId]
GO
ALTER TABLE [u1356333_mikhail].[ConfirmationTokens]  WITH CHECK ADD  CONSTRAINT [FK_ConfirmationTokens_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[ConfirmationTokens] CHECK CONSTRAINT [FK_ConfirmationTokens_Users_UserId]
GO
ALTER TABLE [u1356333_mikhail].[Files]  WITH CHECK ADD  CONSTRAINT [FK_Files_Tasks_TaskId] FOREIGN KEY([TaskId])
REFERENCES [u1356333_mikhail].[Tasks] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[Files] CHECK CONSTRAINT [FK_Files_Tasks_TaskId]
GO
ALTER TABLE [u1356333_mikhail].[Files]  WITH CHECK ADD  CONSTRAINT [FK_Files_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[Files] CHECK CONSTRAINT [FK_Files_Users_UserId]
GO
ALTER TABLE [u1356333_mikhail].[GoogleCalendarAccessRequests]  WITH CHECK ADD  CONSTRAINT [FK_GoogleCalendarAccessRequests_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[GoogleCalendarAccessRequests] CHECK CONSTRAINT [FK_GoogleCalendarAccessRequests_Users_UserId]
GO
ALTER TABLE [u1356333_mikhail].[GoogleCredentials]  WITH CHECK ADD  CONSTRAINT [FK_GoogleCredentials_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[GoogleCredentials] CHECK CONSTRAINT [FK_GoogleCredentials_Users_UserId]
GO
ALTER TABLE [u1356333_mikhail].[HubConnections]  WITH CHECK ADD  CONSTRAINT [FK_HubConnections_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[HubConnections] CHECK CONSTRAINT [FK_HubConnections_Users_UserId]
GO
ALTER TABLE [u1356333_mikhail].[PlantHistory]  WITH CHECK ADD  CONSTRAINT [FK_PlantHistory_Plants_PlantId] FOREIGN KEY([PlantId])
REFERENCES [u1356333_mikhail].[Plants] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[PlantHistory] CHECK CONSTRAINT [FK_PlantHistory_Plants_PlantId]
GO
ALTER TABLE [u1356333_mikhail].[PlantHistory]  WITH CHECK ADD  CONSTRAINT [FK_PlantHistory_Tasks_TaskId] FOREIGN KEY([TaskId])
REFERENCES [u1356333_mikhail].[Tasks] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[PlantHistory] CHECK CONSTRAINT [FK_PlantHistory_Tasks_TaskId]
GO
ALTER TABLE [u1356333_mikhail].[Plants]  WITH CHECK ADD  CONSTRAINT [FK_Plants_PlantTypes_PlantTypeId] FOREIGN KEY([PlantTypeId])
REFERENCES [u1356333_mikhail].[PlantTypes] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[Plants] CHECK CONSTRAINT [FK_Plants_PlantTypes_PlantTypeId]
GO
ALTER TABLE [u1356333_mikhail].[Plants]  WITH CHECK ADD  CONSTRAINT [FK_Plants_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[Plants] CHECK CONSTRAINT [FK_Plants_Users_UserId]
GO
ALTER TABLE [u1356333_mikhail].[Sessions]  WITH CHECK ADD  CONSTRAINT [FK_Sessions_Users_OwnerId] FOREIGN KEY([OwnerId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[Sessions] CHECK CONSTRAINT [FK_Sessions_Users_OwnerId]
GO
ALTER TABLE [u1356333_mikhail].[SessionUser]  WITH CHECK ADD  CONSTRAINT [FK_SessionUser_Sessions_SessionsId] FOREIGN KEY([SessionsId])
REFERENCES [u1356333_mikhail].[Sessions] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[SessionUser] CHECK CONSTRAINT [FK_SessionUser_Sessions_SessionsId]
GO
ALTER TABLE [u1356333_mikhail].[SessionUser]  WITH CHECK ADD  CONSTRAINT [FK_SessionUser_Users_ParticipantsId] FOREIGN KEY([ParticipantsId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[SessionUser] CHECK CONSTRAINT [FK_SessionUser_Users_ParticipantsId]
GO
ALTER TABLE [u1356333_mikhail].[SettingsNotifications]  WITH CHECK ADD  CONSTRAINT [FK_SettingsNotifications_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[SettingsNotifications] CHECK CONSTRAINT [FK_SettingsNotifications_Users_UserId]
GO
ALTER TABLE [u1356333_mikhail].[SystemNotifications]  WITH CHECK ADD  CONSTRAINT [FK_SystemNotifications_Users_ReceiverId] FOREIGN KEY([ReceiverId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[SystemNotifications] CHECK CONSTRAINT [FK_SystemNotifications_Users_ReceiverId]
GO
ALTER TABLE [u1356333_mikhail].[TaskHistory]  WITH CHECK ADD  CONSTRAINT [FK_TaskHistory_Tasks_TaskId] FOREIGN KEY([TaskId])
REFERENCES [u1356333_mikhail].[Tasks] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[TaskHistory] CHECK CONSTRAINT [FK_TaskHistory_Tasks_TaskId]
GO
ALTER TABLE [u1356333_mikhail].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_Tasks_ParentTaskId] FOREIGN KEY([ParentTaskId])
REFERENCES [u1356333_mikhail].[Tasks] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[Tasks] CHECK CONSTRAINT [FK_Tasks_Tasks_ParentTaskId]
GO
ALTER TABLE [u1356333_mikhail].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[Tasks] CHECK CONSTRAINT [FK_Tasks_Users_UserId]
GO
ALTER TABLE [u1356333_mikhail].[UserConnectors]  WITH CHECK ADD  CONSTRAINT [FK_UserConnectors_Users_FriendId] FOREIGN KEY([FriendId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[UserConnectors] CHECK CONSTRAINT [FK_UserConnectors_Users_FriendId]
GO
ALTER TABLE [u1356333_mikhail].[UserConnectors]  WITH CHECK ADD  CONSTRAINT [FK_UserConnectors_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[UserConnectors] CHECK CONSTRAINT [FK_UserConnectors_Users_UserId]
GO
ALTER TABLE [u1356333_mikhail].[UserDetails]  WITH CHECK ADD  CONSTRAINT [FK_UserDetails_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[UserDetails] CHECK CONSTRAINT [FK_UserDetails_Users_UserId]
GO
ALTER TABLE [u1356333_mikhail].[UserNotifications]  WITH CHECK ADD  CONSTRAINT [FK_UserNotifications_Users_ReceiverId] FOREIGN KEY([ReceiverId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[UserNotifications] CHECK CONSTRAINT [FK_UserNotifications_Users_ReceiverId]
GO
ALTER TABLE [u1356333_mikhail].[UserNotifications]  WITH CHECK ADD  CONSTRAINT [FK_UserNotifications_Users_SenderId] FOREIGN KEY([SenderId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[UserNotifications] CHECK CONSTRAINT [FK_UserNotifications_Users_SenderId]
GO
ALTER TABLE [u1356333_mikhail].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [u1356333_mikhail].[Roles] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[Users] CHECK CONSTRAINT [FK_Users_Roles_RoleId]
GO
ALTER TABLE [u1356333_mikhail].[UserSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserSettings_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [u1356333_mikhail].[Users] ([Id])
GO
ALTER TABLE [u1356333_mikhail].[UserSettings] CHECK CONSTRAINT [FK_UserSettings_Users_UserId]
GO
