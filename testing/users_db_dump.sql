USE [users_db]
GO
/****** Object:  Table [dbo].[users]    Script Date: 23.12.2020 22:26:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[email] [nvarchar](256) NOT NULL,
	[nickname] [nvarchar](256) NULL,
	[password_hash] [binary](32) NULL,
	[favorite_number] [int] NULL,
 CONSTRAINT [PK_users_id] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[users] ON 
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (1, N'rmoorrud0@geocities.jp', N'zbramwich0', 0x053B3889EE60EAE788A6AFEA0478EE07F64790788A022F86D869198091ACCBF6, 14)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (2, N'smanklow1@free.fr', N'welphinstone1', 0x021C4650AB1D65D0B147E35828F55C6A8DEDF2E63FCF61EBD97F9854A6B77C10, 66)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (3, N'tlavallie2@spotify.com', N'bthewles2', 0x7AF08D1A3DD1979B1C0D9600A7BD5DE2BCF0F316BC3F7E63E73ED2E4337D3F9A, 31)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (4, N'hlydiate3@newyorker.com', N'rmiskimmon3', 0x221BB47FA4AECAEE2987FF99ECC8C0BD409E01CCD41CA5790624FC475CCD97F8, 25)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (5, N'hgreensitt4@mtv.com', N'mbruce4', 0xBA4A1BEF697F1157933B4B7A6331666E048F3FEF2E66E85476E13B8AEA8FBEE0, 73)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (6, N'wnowell5@google.ca', N'tpilmoor5', 0x42241CEA69188D3BAF34E186BA7554F0EA70C1E2F2CF8B65D34C036D7A649825, 50)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (7, N'jcornes6@so-net.ne.jp', N'zlimming6', 0x2BE535A2C0B0F32C81819BE6CDB9F6ED9AF6D858C30F2782CA57BEA664CDE258, 76)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (8, N'bcodman7@networksolutions.com', N'cbummfrey7', 0x45517EE86DE817F6CCE93C76EA38E70662C64E425F74D44220C9FE7E5904D680, 93)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (9, N'mlogsdale8@mail.ru', N'hkeen8', 0x28C43362264CF8913F3E608F55B7777466AFAC55B8B4211D5C336A55F45EB994, 76)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (10, N'ksquelch9@dailymotion.com', N'kgain9', 0x51F693FBA5B2E40A8BFC9BB3837E5035BA72123420AC92B175AE7547FBB7542C, 29)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (11, N'wsprostona@networkadvertising.org', N'sgrunwalda', 0x83830812CB0398EC038C2EFFA18DFE40C30281FBF59EE5B353EE5F5B65A2F46B, 15)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (12, N'pstirlingb@google.com.au', N'gmelsonb', 0x93D24CE7F324C6E759A97B0ADB8135FCA3CF4F7C73E320552C6374775EFB6F8C, 56)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (13, N'jringec@japanpost.jp', N'eeversleyc', 0x4A63D10BDF9FE11B671A7E7034A8751B81D8828564A1678D25637E932E4A2E0C, 31)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (14, N'ahoulstond@privacy.gov.au', N'lridesd', 0x6C492DB31576894CDADF65684F842BAA431C3D7BCF6A2586396964F085FD78D8, 83)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (15, N'jsmurfitte@ycombinator.com', N'ryankove', 0x48CC2D4A0954891B39462F842D084393363D21369B1435542716D48DD69BEAE4, 43)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (16, N'fmonahanf@vkontakte.ru', N'dilemf', 0xBF6769A9EC3C062140943E1428FD2CD3BD8EA483E2E2D9A5EBD760D7ACECF649, 64)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (17, N'sdeglanvilleg@odnoklassniki.ru', N'swinslettg', 0xDEADBEEF48FE2F78046B39F68ECE3CCAAA971E19B56CB9EDF4FA6A212AF9B751, 95)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (18, N'rfetherstoneh@themeforest.net', N'dfrierh', 0x1546855E4D3EB7A3EB71B4F6AD6062D4DAEBFDD69F8EA1E44D9B722CF3B0567E, 27)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (19, N'deckhardi@digg.com', N'fcomstyi', 0xCB3B37C99063AF443B58DA1B894924D716B37CC882A83222A7BE80C9F4EF11A9, 57)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (20, N'bapedailej@xing.com', N'dmcmichanj', 0xAC3D5BCDF7D3959903A7E120ECA7D998517334967413FE87730E3A0AC94A99DB, 64)
GO
INSERT [dbo].[users] ([id], [email], [nickname], [password_hash], [favorite_number]) VALUES (21, N'apileg@php.net', N'apileg', NULL, 42)
GO
SET IDENTITY_INSERT [dbo].[users] OFF
GO
/****** Object:  StoredProcedure [dbo].[delete_user]    Script Date: 23.12.2020 22:26:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[delete_user]
@user_id int
AS
SET NOCOUNT ON
DELETE FROM users
WHERE id = @user_id;
GO
/****** Object:  StoredProcedure [dbo].[insert_user]    Script Date: 23.12.2020 22:26:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[insert_user]
@email nvarchar(256),
@nickname nvarchar(256),
@favorite_number int,
@inserted_user_id int OUTPUT
AS
SET NOCOUNT ON
BEGIN

	DECLARE @inserted_id TABLE(id int);

	INSERT INTO users (email, nickname, favorite_number)
	OUTPUT inserted.id INTO @inserted_id
	VALUES (@email, @nickname, @favorite_number);

	SELECT @inserted_user_id = id FROM @inserted_id;

END
GO
/****** Object:  StoredProcedure [dbo].[select_users_without_password_hashes]    Script Date: 23.12.2020 22:26:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[select_users_without_password_hashes]
AS
SELECT id, email, nickname, favorite_number
FROM users;
GO
/****** Object:  StoredProcedure [dbo].[update_user]    Script Date: 23.12.2020 22:26:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[update_user]
@user_id int,
@email nvarchar(256),
@nickname nvarchar(256),
@favorite_number int
AS
SET NOCOUNT ON
UPDATE 
	users 
SET
	email = @email, 
    nickname = @nickname,
	favorite_number = @favorite_number
WHERE 
	id = @user_id;
GO
