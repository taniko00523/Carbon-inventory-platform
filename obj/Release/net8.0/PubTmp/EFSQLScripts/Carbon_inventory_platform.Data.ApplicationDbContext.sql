IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [Discriminator] nvarchar(max) NOT NULL,
        [UserLimitData] datetime2 NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [defaultDevices] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Material] nvarchar(max) NOT NULL,
        [Scope] nvarchar(max) NOT NULL,
        [EmissionPattern] nvarchar(max) NOT NULL,
        [Type] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_defaultDevices] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [deviceDatas] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(20) NOT NULL,
        [Scope] nvarchar(10) NOT NULL,
        [EmissionPattern] nvarchar(10) NOT NULL,
        [Material] nvarchar(max) NOT NULL,
        [Data_Correction] int NOT NULL,
        [Device_Correction] int NOT NULL,
        [unit] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_deviceDatas] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [GWPs] (
        [Name] nvarchar(450) NOT NULL,
        [Num] decimal(18,10) NOT NULL,
        [GWP_Year] int NOT NULL,
        CONSTRAINT [PK_GWPs] PRIMARY KEY ([Name])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [Materials] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(20) NOT NULL,
        [Scope] nvarchar(10) NOT NULL,
        [EmissionPattern] nvarchar(10) NOT NULL,
        [CO2CEF] decimal(18,10) NOT NULL,
        [CO2ULL] decimal(18,10) NOT NULL,
        [CO2UUL] decimal(18,10) NOT NULL,
        [CH4CEF] decimal(18,10) NOT NULL,
        [CH4ULL] decimal(18,10) NOT NULL,
        [CH4UUL] decimal(18,10) NOT NULL,
        [N2OCEF] decimal(18,10) NOT NULL,
        [N2OULL] decimal(18,10) NOT NULL,
        [N2OUUL] decimal(18,10) NOT NULL,
        [HFCSCEF] decimal(18,10) NOT NULL,
        [HFCSULL] decimal(18,10) NOT NULL,
        [HFCSUUL] decimal(18,10) NOT NULL,
        [PFCSCEF] decimal(18,10) NOT NULL,
        [PFCSULL] decimal(18,10) NOT NULL,
        [PFCSUUL] decimal(18,10) NOT NULL,
        [SF6CEF] decimal(18,10) NOT NULL,
        [SF6ULL] decimal(18,10) NOT NULL,
        [NF3UUL] decimal(18,10) NOT NULL,
        [NF3CEF] decimal(18,10) NOT NULL,
        [NF3ULL] decimal(18,10) NOT NULL,
        [SF6UUL] decimal(18,10) NOT NULL,
        [CEF_Correction] int NOT NULL,
        [DataUUL] decimal(18,10) NOT NULL,
        [DataULL] decimal(18,10) NOT NULL,
        [Year] int NOT NULL,
        [Unit] nvarchar(10) NOT NULL,
        CONSTRAINT [PK_Materials] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [Companies] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [EasyName] nvarchar(10) NULL,
        [EnglishName] nvarchar(100) NOT NULL,
        [EasyEnglishName] nvarchar(100) NULL,
        [ContactName] nvarchar(20) NOT NULL,
        [Email] nvarchar(50) NOT NULL,
        [Phone] nvarchar(20) NOT NULL,
        [CompanyInformation] nvarchar(max) NOT NULL,
        [AddressInformation] nvarchar(max) NOT NULL,
        [ReportingInformation] nvarchar(max) NOT NULL,
        [GHGInformation] nvarchar(max) NOT NULL,
        [Scope1Information] nvarchar(max) NOT NULL,
        [Scope2Information] nvarchar(max) NOT NULL,
        [isDeleted] tinyint NOT NULL,
        [CreateTime] datetime2 NOT NULL,
        [ModifiedTime] datetime2 NULL,
        [DeleteTime] datetime2 NULL,
        CONSTRAINT [PK_Companies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Companies_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [Areas] (
        [Id] uniqueidentifier NOT NULL,
        [CompanyId] uniqueidentifier NOT NULL,
        [Name] nvarchar(20) NOT NULL,
        [PostalCode] int NOT NULL,
        [UniqueCode] int NOT NULL,
        [FactorCode] int NOT NULL,
        [City] nvarchar(10) NOT NULL,
        [District] nvarchar(10) NOT NULL,
        [Address] nvarchar(50) NOT NULL,
        [FullAddress] nvarchar(100) NOT NULL,
        [Year] int NOT NULL,
        [BaseYear] bit NOT NULL,
        [Type] nvarchar(10) NOT NULL,
        [OrganizationImagePath] nvarchar(max) NULL,
        [MapImagePath] nvarchar(max) NULL,
        [ShopDrawingsPath] nvarchar(max) NULL,
        [isDeleted] tinyint NOT NULL,
        [CreateTime] datetime2 NOT NULL,
        [ModifiedTime] datetime2 NULL,
        [DeleteTime] datetime2 NULL,
        [Scope1_CO2] decimal(18,4) NOT NULL,
        [Scope1_CH4] decimal(18,4) NOT NULL,
        [Scope1_N2O] decimal(18,4) NOT NULL,
        [Scope1_HFCS] decimal(18,4) NOT NULL,
        [Scope1_PFCS] decimal(18,4) NOT NULL,
        [Scope1_SF6] decimal(18,4) NOT NULL,
        [Scope1_NF3] decimal(18,4) NOT NULL,
        [Scope2_CO2] decimal(18,4) NOT NULL,
        [Scope2_CH4] decimal(18,4) NOT NULL,
        [Scope2_N2O] decimal(18,4) NOT NULL,
        [Scope2_HFCS] decimal(18,4) NOT NULL,
        [Scope2_PFCS] decimal(18,4) NOT NULL,
        [Scope2_SF6] decimal(18,4) NOT NULL,
        [Scope2_NF3] decimal(18,4) NOT NULL,
        [CO2] decimal(18,4) NOT NULL,
        [CH4] decimal(18,4) NOT NULL,
        [N2O] decimal(18,4) NOT NULL,
        [HFCS] decimal(18,4) NOT NULL,
        [PFCS] decimal(18,4) NOT NULL,
        [SF6] decimal(18,4) NOT NULL,
        [NF3] decimal(18,4) NOT NULL,
        [percentage1_CO2] decimal(18,2) NOT NULL,
        [percentage1_CH4] decimal(18,2) NOT NULL,
        [percentage1_N2O] decimal(18,2) NOT NULL,
        [percentage1_HFCS] decimal(18,2) NOT NULL,
        [percentage1_PFCS] decimal(18,2) NOT NULL,
        [percentage1_SF6] decimal(18,2) NOT NULL,
        [percentage1_NF3] decimal(18,2) NOT NULL,
        [percentage2_CO2] decimal(18,2) NOT NULL,
        [percentage2_CH4] decimal(18,2) NOT NULL,
        [percentage2_N2O] decimal(18,2) NOT NULL,
        [percentage2_HFCS] decimal(18,2) NOT NULL,
        [percentage2_PFCS] decimal(18,2) NOT NULL,
        [percentage2_SF6] decimal(18,2) NOT NULL,
        [percentage2_NF3] decimal(18,2) NOT NULL,
        [non_move] decimal(18,4) NOT NULL,
        [move] decimal(18,4) NOT NULL,
        [process] decimal(18,4) NOT NULL,
        [escape] decimal(18,4) NOT NULL,
        [percentage_nonMove] decimal(18,2) NOT NULL,
        [percentage_Move] decimal(18,2) NOT NULL,
        [percentage_Process] decimal(18,2) NOT NULL,
        [percentage_Escape] decimal(18,2) NOT NULL,
        [percentage_Scope1] decimal(18,2) NOT NULL,
        [percentage_Scope2] decimal(18,2) NOT NULL,
        [Scope1] decimal(18,4) NOT NULL,
        [Scope2] decimal(18,4) NOT NULL,
        [All] decimal(18,3) NOT NULL,
        [cal_all] decimal(18,4) NOT NULL,
        [percentage_CalAll] decimal(18,2) NOT NULL,
        [no1_Grade] int NOT NULL,
        [no2_Grade] int NOT NULL,
        [no3_Grade] int NOT NULL,
        [avg_Grade] real NOT NULL,
        [all_Grade] nvarchar(max) NOT NULL,
        [ULL] decimal(18,2) NOT NULL,
        [UUL] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_Areas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Areas_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [Devices] (
        [Id] uniqueidentifier NOT NULL,
        [AreaId] uniqueidentifier NOT NULL,
        [Name] nvarchar(20) NOT NULL,
        [OtherName] nvarchar(20) NULL,
        [Scope] nvarchar(10) NOT NULL,
        [EmissionPattern] nvarchar(10) NOT NULL,
        [Material] nvarchar(max) NOT NULL,
        [Customize] bit NOT NULL,
        [AssetNo] nvarchar(20) NULL,
        [Provess] nvarchar(20) NULL,
        [Source] nvarchar(20) NULL,
        [Dept] nvarchar(20) NULL,
        [Unit] nvarchar(20) NULL,
        [Data_Correction] int NOT NULL,
        [Device_Correction] int NOT NULL,
        [CEF_Correction] int NOT NULL,
        [Grade] int NOT NULL,
        [Emissions] decimal(18,4) NOT NULL,
        [data_UUL] decimal(18,10) NOT NULL,
        [data_ULL] decimal(18,10) NOT NULL,
        [all_UUL] decimal(18,10) NOT NULL,
        [all_ULL] decimal(18,10) NOT NULL,
        [count_UUL] decimal(18,10) NOT NULL,
        [count_ULL] decimal(18,10) NOT NULL,
        [isDeleted] tinyint NOT NULL,
        [CreateTime] datetime2 NOT NULL,
        [ModifiedTime] datetime2 NULL,
        [DeleteTime] datetime2 NULL,
        CONSTRAINT [PK_Devices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Devices_Areas_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [Areas] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [ActivityDatas] (
        [id] int NOT NULL IDENTITY,
        [DeviceId] uniqueidentifier NOT NULL,
        [Num] decimal(18,4) NOT NULL,
        [Time] datetime2 NULL,
        [remark] nvarchar(max) NULL,
        CONSTRAINT [PK_ActivityDatas] PRIMARY KEY ([id]),
        CONSTRAINT [FK_ActivityDatas_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE TABLE [GHGs] (
        [Id] uniqueidentifier NOT NULL,
        [DeviceId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [GWP] decimal(18,10) NOT NULL,
        [CEF] decimal(18,10) NOT NULL,
        [all_UUL] decimal(18,10) NOT NULL,
        [all_ULL] decimal(18,10) NOT NULL,
        [CEF_UUL] decimal(18,10) NOT NULL,
        [CEF_ULL] decimal(18,10) NOT NULL,
        [Emission] decimal(18,10) NOT NULL,
        [isDeleted] tinyint NOT NULL,
        [CreateTime] datetime2 NOT NULL,
        [ModifiedTime] datetime2 NULL,
        [DeleteTime] datetime2 NULL,
        CONSTRAINT [PK_GHGs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GHGs_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Name', N'GWP_Year', N'Num') AND [object_id] = OBJECT_ID(N'[GWPs]'))
        SET IDENTITY_INSERT [GWPs] ON;
    EXEC(N'INSERT INTO [GWPs] ([Name], [GWP_Year], [Num])
    VALUES (N''海龍-1211'', 2022, 1930.0),
    (N''CH4'', 2022, 27.9),
    (N''CO2'', 2022, 1.0),
    (N''FM200'', 2022, 3600.0),
    (N''N2O'', 2022, 273.0),
    (N''NF3'', 2022, 17400.0),
    (N''R-12'', 2022, 12500.0),
    (N''R-134A'', 2022, 1530.0),
    (N''R-22'', 2022, 1960.0),
    (N''R-23'', 2022, 14600.0),
    (N''R-32'', 2022, 771.0),
    (N''R-404A'', 2022, 4728.0),
    (N''R-407C'', 2022, 1908.0),
    (N''R-410A'', 2022, 2256.0),
    (N''R-417A'', 2022, 2127.0),
    (N''R-507A'', 2022, 4475.0),
    (N''R-600A'', 2022, 0.006),
    (N''SF6'', 2022, 24300.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Name', N'GWP_Year', N'Num') AND [object_id] = OBJECT_ID(N'[GWPs]'))
        SET IDENTITY_INSERT [GWPs] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CEF_Correction', N'CH4CEF', N'CH4ULL', N'CH4UUL', N'CO2CEF', N'CO2ULL', N'CO2UUL', N'DataULL', N'DataUUL', N'EmissionPattern', N'HFCSCEF', N'HFCSULL', N'HFCSUUL', N'N2OCEF', N'N2OULL', N'N2OUUL', N'NF3CEF', N'NF3ULL', N'NF3UUL', N'Name', N'PFCSCEF', N'PFCSULL', N'PFCSUUL', N'SF6CEF', N'SF6ULL', N'SF6UUL', N'Scope', N'Unit', N'Year') AND [object_id] = OBJECT_ID(N'[Materials]'))
        SET IDENTITY_INSERT [Materials] ON;
    EXEC(N'INSERT INTO [Materials] ([Id], [CEF_Correction], [CH4CEF], [CH4ULL], [CH4UUL], [CO2CEF], [CO2ULL], [CO2UUL], [DataULL], [DataUUL], [EmissionPattern], [HFCSCEF], [HFCSULL], [HFCSUUL], [N2OCEF], [N2OULL], [N2OUUL], [NF3CEF], [NF3ULL], [NF3UUL], [Name], [PFCSCEF], [PFCSULL], [PFCSUUL], [SF6CEF], [SF6ULL], [SF6UUL], [Scope], [Unit], [Year])
    VALUES (1, 3, 0.0000246603, 0.7, 2.0, 2.3328598392, 0.077167019, 0.067653277, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000369904, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''自產煤'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (2, 3, 0.0000284702, 0.7, 2.0, 2.693284704, 0.077167019, 0.067653277, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000427054, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''原料煤'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (3, 3, 0.0000254557, 0.7, 2.0, 2.4081133824, 0.077167019, 0.067653277, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000381836, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''燃料煤'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (4, 3, 0.0000297263, 0.7, 2.0, 2.922093324, 0.0376398779, 0.0274669379, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000445894, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''無煙煤'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (5, 3, 0.0000284702, 0.7, 2.0, 2.693284704, 0.077167019, 0.067653277, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000427054, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''焦煤'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (6, 3, 0.0000254557, 0.7, 2.0, 2.4081133824, 0.0539112051, 0.0539112051, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000381836, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''煙煤'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (7, 3, 0.0000205153, 0.7, 2.0, 1.971522252, 0.03433923, 0.0405827263, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.000030773, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''亞煙煤(發電)'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (8, 3, 0.0000234461, 0.7, 2.0, 2.253168288, 0.03433923, 0.0405827263, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000351691, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''亞煙煤(其他)'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (9, 3, 0.0000119073, 0.7, 2.0, 1.2026331792, 0.1, 0.1386138614, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000178609, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''褐煤'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (10, 3, 0.0000089053, 0.7, 2.0, 0.9528696252, 0.1570093458, 0.1682242991, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.000013358, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''油頁岩'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (11, 3, 0.0000097678, 0.7, 2.0, 1.0353872664, 0.0566037736, 0.0188679245, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000146517, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''泥煤'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (12, 3, 0.0000159098, 0.7, 2.0, 1.5512094, 0.1046153846, 0.1179487179, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000238648, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''煤球'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (13, 3, 0.0000293076, 0.7, 2.0, 3.1359132, 0.1056074766, 0.1121495327, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000439614, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''焦炭'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (14, 3, 0.0001029953, 0.6666666667, 2.3333333333, 3.3473466, 0.1497435897, 0.1794871795, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000205991, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''石油焦'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (15, 3, 0.000094203, 0.6666666667, 2.3333333333, 2.19807, 0.0357142857, 0.0428571429, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000188406, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''航空汽油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (16, 3, 0.0001004832, 0.6666666667, 2.3333333333, 2.3948496, 0.0251748252, 0.0405594406, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000200966, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''航空燃油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (17, 3, 0.0001130436, 0.6666666667, 2.3333333333, 2.76203196, 0.0300136426, 0.0300136426, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000226087, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''原油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (18, 3, 0.0000825595, 0.6666666667, 2.3333333333, 2.1190274028, 0.1, 0.1090909091, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000165119, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''奧里油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (19, 3, 0.0001326881, 0.6666666667, 2.3333333333, 2.8395246038, 0.0919003115, 0.0965732087, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000265376, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''天然氣凝結油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''M3'', 0),
    (20, 3, 0.0001067634, 0.6666666667, 2.3333333333, 2.55876282, 0.0152990264, 0.0250347705, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000213527, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''煤油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (21, 3, 0.0001079943, 0.6666666667, 2.3333333333, 2.7945625586, 0.0750341064, 0.0804911323, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000215989, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''頁岩油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (22, 3, 0.0001055074, 0.6666666667, 2.3333333333, 2.606031792, 0.020242915, 0.0094466937, -0.01, -0.01, N''固定'', 0.0, 0.0, 0.0, 0.0000211015, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''柴油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (23, 3, 0.0000979711, 0.6666666667, 2.3333333333, 2.263132872, 0.025974026, 0.0533910534, -0.01, -0.01, N''固定'', 0.0, 0.0, 0.0, 0.0000195942, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''車用汽油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (24, 3, 0.0001205798, 0.6666666667, 2.3333333333, 3.110959872, 0.0245478036, 0.0180878553, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.000024116, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''蒸餘油 (燃料油)'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (25, 3, 0.0000277794, 0.7, 2.0, 1.7528812758, 0.0237717908, 0.0396196513, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000027779, 0.7, 2.0, 0.0, 0.0, 0.0, N''液化石油氣'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (26, 3, 0.0000979711, 0.6666666667, 2.3333333333, 2.393761032, 0.0545702592, 0.0409276944, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000195942, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''石油腦'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (27, 3, 0.000125604, 0.6666666667, 2.3333333333, 3.3787476, 0.0954151177, 0.1140024783, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000251208, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''柏油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (28, 3, 0.0001205798, 0.6666666667, 2.3333333333, 2.946167424, 0.0190995907, 0.0259208731, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.000024116, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''潤滑油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (29, 3, 0.0001130436, 0.6666666667, 2.3333333333, 2.76203196, 0.0150068213, 0.0150068213, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000226087, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''其他油品'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (30, 3, 0.0000464316, 0.7, 2.0, 2.8601872992, 0.0827922078, 0.1136363636, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000046432, 0.7, 2.0, 0.0, 0.0, 0.0, N''乙烷'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (31, 3, 0.0000334944, 0.7, 2.0, 1.87903584, 0.0320855615, 0.0392156863, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000033494, 0.7, 2.0, 0.0, 0.0, 0.0, N''天然氣'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''M3'', 0),
    (32, 3, 0.0000376812, 0.7, 2.0, 2.17043712, 0.1631944444, 0.1979166667, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000037681, 0.7, 2.0, 0.0, 0.0, 0.0, N''煉油氣'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''M3'', 0),
    (33, 3, 0.0000175846, 0.7, 2.0, 0.780754464, 0.1599099099, 0.2184684685, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000017585, 0.7, 2.0, 0.0, 0.0, 0.0, N''焦爐氣'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''M3'', 0),
    (34, 3, 0.0000032531, 0.7, 2.0, 0.845817336, 0.1576923077, 0.1846153846, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000003253, 0.7, 2.0, 0.0, 0.0, 0.0, N''高爐氣'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''M3'', 0),
    (35, 3, 0.0002549271, 0.6666666667, 2.3333333333, 0.7792272743, 0.2006543075, 0.3195201745, 0.0, 0.0, N''固定'', 0.0, 0.0, 0.0, 0.0000339903, 0.625, 2.75, 0.0, 0.0, 0.0, N''一般廢棄物'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''Kg'', 0),
    (36, 3, 0.000094203, 0.6666666667, 2.3333333333, 2.19807, 0.0357142857, 0.0428571429, 0.0, 0.0, N''移動'', 0.0, 0.0, 0.0, 0.0000188406, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''航空汽油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (37, 3, 0.0001004832, 0.6666666667, 2.3333333333, 2.3948496, 0.0251748252, 0.0405594406, 0.0, 0.0, N''移動'', 0.0, 0.0, 0.0, 0.0000200966, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''航空燃油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (38, 3, 0.000816426, 0.6666666667, 2.44, 2.263132872, 0.025974026, 0.0533910534, -0.01, -0.01, N''移動'', 0.0, 0.0, 0.0, 0.0002612563, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''車用汽油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (39, 3, 0.0001371596, 0.5897435897, 1.4358974359, 2.606031792, 0.020242915, 0.0094466937, -0.01, -0.01, N''移動'', 0.0, 0.0, 0.0, 0.0001371596, 0.6666666667, 2.0769230769, 0.0, 0.0, 0.0, N''柴油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (40, 3, 0.0001067634, 0.6666666667, 2.3333333333, 2.55876282, 0.0152990264, 0.0250347705, 0.0, 0.0, N''移動'', 0.0, 0.0, 0.0, 0.0000213527, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''煤油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (41, 3, 0.0001205798, 0.6666666667, 2.3333333333, 2.946167424, 0.0190995907, 0.0259208731, 0.0, 0.0, N''移動'', 0.0, 0.0, 0.0, 0.000024116, 0.6666666667, 2.3333333333, 0.0, 0.0, 0.0, N''潤滑油'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0),
    (42, 3, 0.0017223239, 0.0, 0.0, 1.7528812758, 0.0237717908, 0.0396196513, 0.0, 0.0, N''移動'', 0.0, 0.0, 0.0, 0.0000055559, 0.0, 0.0, 0.0, 0.0, 0.0, N''液化石油氣'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''L'', 0);
    INSERT INTO [Materials] ([Id], [CEF_Correction], [CH4CEF], [CH4ULL], [CH4UUL], [CO2CEF], [CO2ULL], [CO2UUL], [DataULL], [DataUUL], [EmissionPattern], [HFCSCEF], [HFCSULL], [HFCSUUL], [N2OCEF], [N2OULL], [N2OUUL], [NF3CEF], [NF3ULL], [NF3UUL], [Name], [PFCSCEF], [PFCSULL], [PFCSUUL], [SF6CEF], [SF6ULL], [SF6UUL], [Scope], [Unit], [Year])
    VALUES (43, 3, 0.0034666704, 0.4565217391, 15.7391304348, 2.11391532, 0.0320855615, 0.0392156863, 0.0, 0.0, N''移動'', 0.0, 0.0, 0.0, 0.0001130436, 0.6666666667, 24.6666666667, 0.0, 0.0, 0.0, N''液化天然氣'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N''M3'', 0),
    (44, 3, 0.0, 0.0, 0.0, 0.495, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 0),
    (57, 3, 0.0031875, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''廢水處理'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (58, 3, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''二氧化碳'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (59, 1, 0.0, 0.0, 0.0, 3.384165385, 0.0, 0.0, 0.0, 0.0, N''製程'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''乙炔'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (60, 1, 0.0, 0.0, 0.0, 3.6666666666, 0.0, 0.0, 0.0, 0.0, N''製程'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''焊條'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (61, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.003, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''冰箱'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (62, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.003, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''飲水機'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (63, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.055, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''商用冰箱'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (64, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.2, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''中、大型冰箱'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (65, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.33, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''低溫冷凍車'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (66, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.16, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''乾燥機'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (67, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.16, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''工業冷藏、冷凍'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (68, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.16, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''食品加工冷藏、冷凍'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (69, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.09, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''冰水主機'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (70, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.03, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''冷氣機'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (71, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.2, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''車用空調'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (72, 1, 0.0, 0.0, 0.0, 3.026, 0.0, 0.0, 0.0, 0.0, N''製程'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''丁烷'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (73, 3, 0.0, 0.0, 0.0, 0.509, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 110)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CEF_Correction', N'CH4CEF', N'CH4ULL', N'CH4UUL', N'CO2CEF', N'CO2ULL', N'CO2UUL', N'DataULL', N'DataUUL', N'EmissionPattern', N'HFCSCEF', N'HFCSULL', N'HFCSUUL', N'N2OCEF', N'N2OULL', N'N2OUUL', N'NF3CEF', N'NF3ULL', N'NF3UUL', N'Name', N'PFCSCEF', N'PFCSULL', N'PFCSUUL', N'SF6CEF', N'SF6ULL', N'SF6UUL', N'Scope', N'Unit', N'Year') AND [object_id] = OBJECT_ID(N'[Materials]'))
        SET IDENTITY_INSERT [Materials] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'EmissionPattern', N'Material', N'Name', N'Scope', N'Type') AND [object_id] = OBJECT_ID(N'[defaultDevices]'))
        SET IDENTITY_INSERT [defaultDevices] ON;
    EXEC(N'INSERT INTO [defaultDevices] ([Id], [EmissionPattern], [Material], [Name], [Scope], [Type])
    VALUES (1, N''固定'', N''柴油'', N''緊急發電機'', N''類別一'', N''''),
    (2, N''移動'', N''柴油'', N''公務車'', N''類別一'', N''''),
    (3, N''移動'', N''車用汽油'', N''公務車'', N''類別一'', N''''),
    (4, N''逸散'', N''R-410A'', N''冷氣機'', N''類別一'', N''''),
    (5, N''逸散'', N''R-134A'', N''飲水機'', N''類別一'', N''''),
    (6, N''逸散'', N''R-134A'', N''乾燥機'', N''類別一'', N''''),
    (7, N''逸散'', N''R-134A'', N''冰水主機'', N''類別一'', N''''),
    (8, N''逸散'', N''R-134A'', N''車用空調'', N''類別一'', N''''),
    (9, N''逸散'', N''廢水處理'', N''化糞池'', N''類別一'', N''''),
    (10, N''外購電力'', N''外購電力'', N''電力'', N''類別二'', N'''')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'EmissionPattern', N'Material', N'Name', N'Scope', N'Type') AND [object_id] = OBJECT_ID(N'[defaultDevices]'))
        SET IDENTITY_INSERT [defaultDevices] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Data_Correction', N'Device_Correction', N'EmissionPattern', N'Material', N'Name', N'Scope', N'unit') AND [object_id] = OBJECT_ID(N'[deviceDatas]'))
        SET IDENTITY_INSERT [deviceDatas] ON;
    EXEC(N'INSERT INTO [deviceDatas] ([Id], [Data_Correction], [Device_Correction], [EmissionPattern], [Material], [Name], [Scope], [unit])
    VALUES (1, 3, 3, N''逸散'', N''R-410A'', N''冷氣機'', N''類別一'', N''公斤''),
    (2, 3, 3, N''逸散'', N''R-134A'', N''冰水主機'', N''類別一'', N''公斤''),
    (3, 3, 3, N''逸散'', N''R-134A'', N''冰箱'', N''類別一'', N''公斤''),
    (4, 3, 3, N''逸散'', N''R-134A'', N''飲水機'', N''類別一'', N''公斤''),
    (5, 3, 3, N''逸散'', N''R-134A'', N''乾燥機'', N''類別一'', N''公斤''),
    (6, 3, 3, N''逸散'', N''R-134A'', N''車用空調'', N''類別一'', N''公斤''),
    (7, 3, 3, N''逸散'', N''R-134A'', N''工業冷藏、冷凍'', N''類別一'', N''公斤''),
    (8, 3, 3, N''固定'', N''柴油'', N''緊急發電機'', N''類別一'', N''公升''),
    (9, 2, 2, N''固定'', N''液化石油氣'', N''廚房'', N''類別一'', N''公斤''),
    (10, 2, 2, N''移動'', N''車用汽油'', N''公務車'', N''類別一'', N''公升''),
    (11, 2, 2, N''移動'', N''柴油'', N''堆高機'', N''類別一'', N''公升''),
    (12, 3, 3, N''逸散'', N''二氧化碳'', N''CO2滅火器'', N''類別一'', N''公斤''),
    (13, 3, 3, N''逸散'', N''二氧化碳'', N''二氧化碳'', N''類別一'', N''公斤''),
    (14, 3, 3, N''逸散'', N''二氧化碳'', N''WD40'', N''類別一'', N''公斤''),
    (15, 3, 3, N''逸散'', N''海龍-1211'', N''海龍1211'', N''類別一'', N''公斤''),
    (16, 3, 3, N''逸散'', N''FM200'', N''FM200'', N''類別一'', N''公斤''),
    (17, 3, 3, N''逸散'', N''廢水處理'', N''化糞池'', N''類別一'', N''人''),
    (18, 1, 1, N''外購電力'', N''外購電力'', N''電力'', N''類別二'', N''度''),
    (19, 3, 3, N''製程'', N''乙炔'', N''乙炔'', N''類別一'', N''公斤''),
    (20, 3, 3, N''製程'', N''焊條'', N''焊條'', N''類別一'', N''公斤''),
    (21, 3, 3, N''逸散'', N''R-134A'', N''工業冷藏、冷凍'', N''類別一'', N''公斤''),
    (22, 3, 3, N''逸散'', N''R-134A'', N''商用冰箱'', N''類別一'', N''公斤''),
    (23, 3, 3, N''逸散'', N''R-134A'', N''中、大型冰箱'', N''類別一'', N''公斤''),
    (24, 3, 3, N''逸散'', N''R-134A'', N''低溫冷凍車'', N''類別一'', N''公斤''),
    (25, 3, 3, N''逸散'', N''R-134A'', N''食品加工冷藏、冷凍'', N''類別一'', N''公斤''),
    (26, 3, 3, N''製程'', N''丁烷'', N''瓦斯罐'', N''類別一'', N''公斤'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Data_Correction', N'Device_Correction', N'EmissionPattern', N'Material', N'Name', N'Scope', N'unit') AND [object_id] = OBJECT_ID(N'[deviceDatas]'))
        SET IDENTITY_INSERT [deviceDatas] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE INDEX [IX_ActivityDatas_DeviceId] ON [ActivityDatas] ([DeviceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE INDEX [IX_Areas_CompanyId] ON [Areas] ([CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE INDEX [IX_Companies_UserId] ON [Companies] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE INDEX [IX_Devices_AreaId] ON [Devices] ([AreaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    CREATE INDEX [IX_GHGs_DeviceId] ON [GHGs] ([DeviceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240409153851_InitalCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240409153851_InitalCreate', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411114015_1'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240411114015_1', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411114244_2'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'Scope2Information');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [Scope2Information] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411114244_2'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'Scope1Information');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [Scope1Information] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411114244_2'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'ReportingInformation');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [ReportingInformation] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411114244_2'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'GHGInformation');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [GHGInformation] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411114244_2'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'CompanyInformation');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [CompanyInformation] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411114244_2'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'AddressInformation');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [AddressInformation] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411114244_2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240411114244_2', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240413131233_3'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CEF_Correction', N'CH4CEF', N'CH4ULL', N'CH4UUL', N'CO2CEF', N'CO2ULL', N'CO2UUL', N'DataULL', N'DataUUL', N'EmissionPattern', N'HFCSCEF', N'HFCSULL', N'HFCSUUL', N'N2OCEF', N'N2OULL', N'N2OUUL', N'NF3CEF', N'NF3ULL', N'NF3UUL', N'Name', N'PFCSCEF', N'PFCSULL', N'PFCSUUL', N'SF6CEF', N'SF6ULL', N'SF6UUL', N'Scope', N'Unit', N'Year') AND [object_id] = OBJECT_ID(N'[Materials]'))
        SET IDENTITY_INSERT [Materials] ON;
    EXEC(N'INSERT INTO [Materials] ([Id], [CEF_Correction], [CH4CEF], [CH4ULL], [CH4UUL], [CO2CEF], [CO2ULL], [CO2UUL], [DataULL], [DataUUL], [EmissionPattern], [HFCSCEF], [HFCSULL], [HFCSUUL], [N2OCEF], [N2OULL], [N2OUUL], [NF3CEF], [NF3ULL], [NF3UUL], [Name], [PFCSCEF], [PFCSULL], [PFCSUUL], [SF6CEF], [SF6ULL], [SF6UUL], [Scope], [Unit], [Year])
    VALUES (74, 3, 0.0, 0.0, 0.0, 0.025, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''WD40'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CEF_Correction', N'CH4CEF', N'CH4ULL', N'CH4UUL', N'CO2CEF', N'CO2ULL', N'CO2UUL', N'DataULL', N'DataUUL', N'EmissionPattern', N'HFCSCEF', N'HFCSULL', N'HFCSUUL', N'N2OCEF', N'N2OULL', N'N2OUUL', N'NF3CEF', N'NF3ULL', N'NF3UUL', N'Name', N'PFCSCEF', N'PFCSULL', N'PFCSUUL', N'SF6CEF', N'SF6ULL', N'SF6UUL', N'Scope', N'Unit', N'Year') AND [object_id] = OBJECT_ID(N'[Materials]'))
        SET IDENTITY_INSERT [Materials] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240413131233_3'
)
BEGIN
    EXEC(N'UPDATE [deviceDatas] SET [Material] = N''WD40''
    WHERE [Id] = 14;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240413131233_3'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240413131233_3', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240422053506_422'
)
BEGIN
    ALTER TABLE [Companies] ADD [ReportOpening] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240422053506_422'
)
BEGIN
    ALTER TABLE [Companies] ADD [ReportingPurposes] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240422053506_422'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [EmissionPattern] = N''製程''
    WHERE [Id] = 74;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240422053506_422'
)
BEGIN
    EXEC(N'UPDATE [deviceDatas] SET [EmissionPattern] = N''製程''
    WHERE [Id] = 14;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240422053506_422'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240422053506_422', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240423104116_423'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GHGs]') AND [c].[name] = N'GWP');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [GHGs] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [GHGs] ALTER COLUMN [GWP] decimal(18,2) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240423104116_423'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CH4CEF] = 0.003825
    WHERE [Id] = 57;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240423104116_423'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240423104116_423', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240502092448_502'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [Year] = 112
    WHERE [Id] = 44;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240502092448_502'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CEF_Correction', N'CH4CEF', N'CH4ULL', N'CH4UUL', N'CO2CEF', N'CO2ULL', N'CO2UUL', N'DataULL', N'DataUUL', N'EmissionPattern', N'HFCSCEF', N'HFCSULL', N'HFCSUUL', N'N2OCEF', N'N2OULL', N'N2OUUL', N'NF3CEF', N'NF3ULL', N'NF3UUL', N'Name', N'PFCSCEF', N'PFCSULL', N'PFCSUUL', N'SF6CEF', N'SF6ULL', N'SF6UUL', N'Scope', N'Unit', N'Year') AND [object_id] = OBJECT_ID(N'[Materials]'))
        SET IDENTITY_INSERT [Materials] ON;
    EXEC(N'INSERT INTO [Materials] ([Id], [CEF_Correction], [CH4CEF], [CH4ULL], [CH4UUL], [CO2CEF], [CO2ULL], [CO2UUL], [DataULL], [DataUUL], [EmissionPattern], [HFCSCEF], [HFCSULL], [HFCSUUL], [N2OCEF], [N2OULL], [N2OUUL], [NF3CEF], [NF3ULL], [NF3UUL], [Name], [PFCSCEF], [PFCSULL], [PFCSUUL], [SF6CEF], [SF6ULL], [SF6UUL], [Scope], [Unit], [Year])
    VALUES (45, 3, 0.0, 0.0, 0.0, 0.494, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 113)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CEF_Correction', N'CH4CEF', N'CH4ULL', N'CH4UUL', N'CO2CEF', N'CO2ULL', N'CO2UUL', N'DataULL', N'DataUUL', N'EmissionPattern', N'HFCSCEF', N'HFCSULL', N'HFCSUUL', N'N2OCEF', N'N2OULL', N'N2OUUL', N'NF3CEF', N'NF3ULL', N'NF3UUL', N'Name', N'PFCSCEF', N'PFCSULL', N'PFCSUUL', N'SF6CEF', N'SF6ULL', N'SF6UUL', N'Scope', N'Unit', N'Year') AND [object_id] = OBJECT_ID(N'[Materials]'))
        SET IDENTITY_INSERT [Materials] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240502092448_502'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240502092448_502', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    ALTER TABLE [Devices] ADD [NameRemark] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    ALTER TABLE [Devices] ADD [Remark] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Areas]') AND [c].[name] = N'Type');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Areas] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [Areas] ALTER COLUMN [Type] nvarchar(10) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Areas]') AND [c].[name] = N'Name');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Areas] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [Areas] ALTER COLUMN [Name] nvarchar(20) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CO2CEF] = 0.555, [Year] = 94
    WHERE [Id] = 44;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CO2CEF] = 0.5625, [Year] = 95
    WHERE [Id] = 45;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CH4CEF] = 0.0, [CO2CEF] = 0.533, [CO2ULL] = -0.07, [CO2UUL] = 0.07, [DataULL] = -0.01, [DataUUL] = -0.01, [EmissionPattern] = N''外購電力'', [Name] = N''外購電力'', [Scope] = N''類別二'', [Year] = 107
    WHERE [Id] = 57;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CO2CEF] = 0.509, [CO2ULL] = -0.07, [CO2UUL] = 0.07, [DataULL] = -0.01, [DataUUL] = -0.01, [EmissionPattern] = N''外購電力'', [Name] = N''外購電力'', [Scope] = N''類別二'', [Year] = 108
    WHERE [Id] = 58;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CEF_Correction] = 3, [CO2CEF] = 0.502, [CO2ULL] = -0.07, [CO2UUL] = 0.07, [DataULL] = -0.01, [DataUUL] = -0.01, [EmissionPattern] = N''外購電力'', [Name] = N''外購電力'', [Scope] = N''類別二'', [Year] = 109
    WHERE [Id] = 59;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CEF_Correction] = 3, [CO2CEF] = 0.509, [CO2ULL] = -0.07, [CO2UUL] = 0.07, [DataULL] = -0.01, [DataUUL] = -0.01, [EmissionPattern] = N''外購電力'', [Name] = N''外購電力'', [Scope] = N''類別二'', [Year] = 110
    WHERE [Id] = 60;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CO2CEF] = 0.495, [CO2ULL] = -0.07, [CO2UUL] = 0.07, [DataULL] = -0.01, [DataUUL] = -0.01, [EmissionPattern] = N''外購電力'', [HFCSCEF] = 0.0, [Name] = N''外購電力'', [Scope] = N''類別二'', [Year] = 111
    WHERE [Id] = 61;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CO2CEF] = 0.494, [CO2ULL] = -0.07, [CO2UUL] = 0.07, [DataULL] = -0.01, [DataUUL] = -0.01, [EmissionPattern] = N''外購電力'', [HFCSCEF] = 0.0, [Name] = N''外購電力'', [Scope] = N''類別二'', [Year] = 112
    WHERE [Id] = 62;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CH4CEF] = 0.003825, [HFCSCEF] = 0.0, [Name] = N''廢水處理''
    WHERE [Id] = 63;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CO2CEF] = 1.0, [HFCSCEF] = 0.0, [Name] = N''二氧化碳''
    WHERE [Id] = 64;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CEF_Correction] = 1, [CO2CEF] = 3.384165385, [EmissionPattern] = N''製程'', [HFCSCEF] = 0.0, [Name] = N''乙炔''
    WHERE [Id] = 65;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CEF_Correction] = 1, [CO2CEF] = 3.6666666666, [EmissionPattern] = N''製程'', [HFCSCEF] = 0.0, [Name] = N''焊條''
    WHERE [Id] = 66;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [HFCSCEF] = 0.003, [Name] = N''冰箱''
    WHERE [Id] = 67;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [HFCSCEF] = 0.003, [Name] = N''飲水機''
    WHERE [Id] = 68;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [HFCSCEF] = 0.055, [Name] = N''商用冰箱''
    WHERE [Id] = 69;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [HFCSCEF] = 0.2, [Name] = N''中、大型冰箱''
    WHERE [Id] = 70;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [HFCSCEF] = 0.33, [Name] = N''低溫冷凍車''
    WHERE [Id] = 71;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CEF_Correction] = 3, [CO2CEF] = 0.0, [EmissionPattern] = N''逸散'', [HFCSCEF] = 0.16, [Name] = N''乾燥機''
    WHERE [Id] = 72;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CO2CEF] = 0.0, [CO2ULL] = 0.0, [CO2UUL] = 0.0, [DataULL] = 0.0, [DataUUL] = 0.0, [EmissionPattern] = N''逸散'', [HFCSCEF] = 0.16, [Name] = N''工業冷藏、冷凍'', [Scope] = N''類別一'', [Year] = 0
    WHERE [Id] = 73;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CO2CEF] = 0.0, [EmissionPattern] = N''逸散'', [HFCSCEF] = 0.16, [Name] = N''食品加工冷藏、冷凍''
    WHERE [Id] = 74;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CEF_Correction', N'CH4CEF', N'CH4ULL', N'CH4UUL', N'CO2CEF', N'CO2ULL', N'CO2UUL', N'DataULL', N'DataUUL', N'EmissionPattern', N'HFCSCEF', N'HFCSULL', N'HFCSUUL', N'N2OCEF', N'N2OULL', N'N2OUUL', N'NF3CEF', N'NF3ULL', N'NF3UUL', N'Name', N'PFCSCEF', N'PFCSULL', N'PFCSUUL', N'SF6CEF', N'SF6ULL', N'SF6UUL', N'Scope', N'Unit', N'Year') AND [object_id] = OBJECT_ID(N'[Materials]'))
        SET IDENTITY_INSERT [Materials] ON;
    EXEC(N'INSERT INTO [Materials] ([Id], [CEF_Correction], [CH4CEF], [CH4ULL], [CH4UUL], [CO2CEF], [CO2ULL], [CO2UUL], [DataULL], [DataUUL], [EmissionPattern], [HFCSCEF], [HFCSULL], [HFCSUUL], [N2OCEF], [N2OULL], [N2OUUL], [NF3CEF], [NF3ULL], [NF3UUL], [Name], [PFCSCEF], [PFCSULL], [PFCSUUL], [SF6CEF], [SF6ULL], [SF6UUL], [Scope], [Unit], [Year])
    VALUES (46, 3, 0.0, 0.0, 0.0, 0.558, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 96),
    (47, 3, 0.0, 0.0, 0.0, 0.555, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 97),
    (48, 3, 0.0, 0.0, 0.0, 0.543, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 98),
    (49, 3, 0.0, 0.0, 0.0, 0.534, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 99),
    (50, 3, 0.0, 0.0, 0.0, 0.534, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 100),
    (51, 3, 0.0, 0.0, 0.0, 0.529, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 101),
    (52, 3, 0.0, 0.0, 0.0, 0.519, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 102),
    (53, 3, 0.0, 0.0, 0.0, 0.518, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 103),
    (54, 3, 0.0, 0.0, 0.0, 0.525, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 104),
    (55, 3, 0.0, 0.0, 0.0, 0.53, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 105),
    (56, 3, 0.0, 0.0, 0.0, 0.554, -0.07, 0.07, -0.01, -0.01, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''外購電力'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別二'', N'''', 106),
    (75, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.09, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''冰水主機'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (76, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.03, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''冷氣機'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (77, 3, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 0.2, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''車用空調'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (78, 1, 0.0, 0.0, 0.0, 3.026, 0.0, 0.0, 0.0, 0.0, N''製程'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''丁烷'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (79, 3, 0.0, 0.0, 0.0, 0.025, 0.0, 0.0, 0.0, 0.0, N''製程'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''WD40'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CEF_Correction', N'CH4CEF', N'CH4ULL', N'CH4UUL', N'CO2CEF', N'CO2ULL', N'CO2UUL', N'DataULL', N'DataUUL', N'EmissionPattern', N'HFCSCEF', N'HFCSULL', N'HFCSUUL', N'N2OCEF', N'N2OULL', N'N2OUUL', N'NF3CEF', N'NF3ULL', N'NF3UUL', N'Name', N'PFCSCEF', N'PFCSULL', N'PFCSUUL', N'SF6CEF', N'SF6ULL', N'SF6UUL', N'Scope', N'Unit', N'Year') AND [object_id] = OBJECT_ID(N'[Materials]'))
        SET IDENTITY_INSERT [Materials] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240506072633_503'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240506072633_503', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240513101333_513'
)
BEGIN
    EXEC(N'UPDATE [deviceDatas] SET [Name] = N''二氧化碳滅火器''
    WHERE [Id] = 12;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240513101333_513'
)
BEGIN
    EXEC(N'UPDATE [deviceDatas] SET [unit] = N''人-年''
    WHERE [Id] = 17;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240513101333_513'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240513101333_513', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240514071238_gwp'
)
BEGIN
    EXEC(N'UPDATE [GWPs] SET [Num] = 2255.5
    WHERE [Name] = N''R-410A'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240514071238_gwp'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240514071238_gwp', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240517115234_updateMaxLength'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'Name');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [Name] nvarchar(100) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240517115234_updateMaxLength'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'EasyName');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [Companies] ALTER COLUMN [EasyName] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240517115234_updateMaxLength'
)
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Areas]') AND [c].[name] = N'Name');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Areas] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [Areas] ALTER COLUMN [Name] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240517115234_updateMaxLength'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240517115234_updateMaxLength', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240517143922_WD40-CEF_Correction'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CEF_Correction] = 1
    WHERE [Id] = 79;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240517143922_WD40-CEF_Correction'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240517143922_WD40-CEF_Correction', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240518114234_CO2_CEF_Correction'
)
BEGIN
    EXEC(N'UPDATE [Materials] SET [CEF_Correction] = 1
    WHERE [Id] = 64;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240518114234_CO2_CEF_Correction'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240518114234_CO2_CEF_Correction', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240612121402_R125GWP'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Name', N'GWP_Year', N'Num') AND [object_id] = OBJECT_ID(N'[GWPs]'))
        SET IDENTITY_INSERT [GWPs] ON;
    EXEC(N'INSERT INTO [GWPs] ([Name], [GWP_Year], [Num])
    VALUES (N''R-125'', 2022, 3740.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Name', N'GWP_Year', N'Num') AND [object_id] = OBJECT_ID(N'[GWPs]'))
        SET IDENTITY_INSERT [GWPs] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240612121402_R125GWP'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240612121402_R125GWP', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240612133657_DeviceCountUULChange'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Devices]') AND [c].[name] = N'count_UUL');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Devices] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [Devices] ALTER COLUMN [count_UUL] decimal(25,10) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240612133657_DeviceCountUULChange'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Devices]') AND [c].[name] = N'count_ULL');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Devices] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [Devices] ALTER COLUMN [count_ULL] decimal(25,10) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240612133657_DeviceCountUULChange'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240612133657_DeviceCountUULChange', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240613053341_hfc236'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Name', N'GWP_Year', N'Num') AND [object_id] = OBJECT_ID(N'[GWPs]'))
        SET IDENTITY_INSERT [GWPs] ON;
    EXEC(N'INSERT INTO [GWPs] ([Name], [GWP_Year], [Num])
    VALUES (N''HFC-236fa'', 2022, 8690.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Name', N'GWP_Year', N'Num') AND [object_id] = OBJECT_ID(N'[GWPs]'))
        SET IDENTITY_INSERT [GWPs] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240613053341_hfc236'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Data_Correction', N'Device_Correction', N'EmissionPattern', N'Material', N'Name', N'Scope', N'unit') AND [object_id] = OBJECT_ID(N'[deviceDatas]'))
        SET IDENTITY_INSERT [deviceDatas] ON;
    EXEC(N'INSERT INTO [deviceDatas] ([Id], [Data_Correction], [Device_Correction], [EmissionPattern], [Material], [Name], [Scope], [unit])
    VALUES (27, 3, 3, N''逸散'', N''HFC-236fa'', N''六氟丙烷滅火器'', N''類別一'', N''公斤'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Data_Correction', N'Device_Correction', N'EmissionPattern', N'Material', N'Name', N'Scope', N'unit') AND [object_id] = OBJECT_ID(N'[deviceDatas]'))
        SET IDENTITY_INSERT [deviceDatas] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240613053341_hfc236'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240613053341_hfc236', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240613054230_addgwp'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Name', N'GWP_Year', N'Num') AND [object_id] = OBJECT_ID(N'[GWPs]'))
        SET IDENTITY_INSERT [GWPs] ON;
    EXEC(N'INSERT INTO [GWPs] ([Name], [GWP_Year], [Num])
    VALUES (N''R-1234yf'', 2022, 0.501),
    (N''R-407F'', 2022, 1965.3),
    (N''R-452A'', 2022, 2291.5603)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Name', N'GWP_Year', N'Num') AND [object_id] = OBJECT_ID(N'[GWPs]'))
        SET IDENTITY_INSERT [GWPs] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240613054230_addgwp'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240613054230_addgwp', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240613060021_updateHFCSDeviceData'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''海龍-1211'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240613060021_updateHFCSDeviceData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Name', N'GWP_Year', N'Num') AND [object_id] = OBJECT_ID(N'[GWPs]'))
        SET IDENTITY_INSERT [GWPs] ON;
    EXEC(N'INSERT INTO [GWPs] ([Name], [GWP_Year], [Num])
    VALUES (N''二氟一氯一溴甲烷'', 2022, 1930.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Name', N'GWP_Year', N'Num') AND [object_id] = OBJECT_ID(N'[GWPs]'))
        SET IDENTITY_INSERT [GWPs] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240613060021_updateHFCSDeviceData'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CEF_Correction', N'CH4CEF', N'CH4ULL', N'CH4UUL', N'CO2CEF', N'CO2ULL', N'CO2UUL', N'DataULL', N'DataUUL', N'EmissionPattern', N'HFCSCEF', N'HFCSULL', N'HFCSUUL', N'N2OCEF', N'N2OULL', N'N2OUUL', N'NF3CEF', N'NF3ULL', N'NF3UUL', N'Name', N'PFCSCEF', N'PFCSULL', N'PFCSUUL', N'SF6CEF', N'SF6ULL', N'SF6UUL', N'Scope', N'Unit', N'Year') AND [object_id] = OBJECT_ID(N'[Materials]'))
        SET IDENTITY_INSERT [Materials] ON;
    EXEC(N'INSERT INTO [Materials] ([Id], [CEF_Correction], [CH4CEF], [CH4ULL], [CH4UUL], [CO2CEF], [CO2ULL], [CO2UUL], [DataULL], [DataUUL], [EmissionPattern], [HFCSCEF], [HFCSULL], [HFCSUUL], [N2OCEF], [N2OULL], [N2OUUL], [NF3CEF], [NF3ULL], [NF3UUL], [Name], [PFCSCEF], [PFCSULL], [PFCSUUL], [SF6CEF], [SF6ULL], [SF6UUL], [Scope], [Unit], [Year])
    VALUES (80, 1, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''HFC-236fa'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0),
    (81, 1, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''逸散'', 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''二氟一氯一溴甲烷'', 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, N''類別一'', N'''', 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CEF_Correction', N'CH4CEF', N'CH4ULL', N'CH4UUL', N'CO2CEF', N'CO2ULL', N'CO2UUL', N'DataULL', N'DataUUL', N'EmissionPattern', N'HFCSCEF', N'HFCSULL', N'HFCSUUL', N'N2OCEF', N'N2OULL', N'N2OUUL', N'NF3CEF', N'NF3ULL', N'NF3UUL', N'Name', N'PFCSCEF', N'PFCSULL', N'PFCSUUL', N'SF6CEF', N'SF6ULL', N'SF6UUL', N'Scope', N'Unit', N'Year') AND [object_id] = OBJECT_ID(N'[Materials]'))
        SET IDENTITY_INSERT [Materials] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240613060021_updateHFCSDeviceData'
)
BEGIN
    EXEC(N'UPDATE [deviceDatas] SET [Material] = N''二氟一氯一溴甲烷''
    WHERE [Id] = 15;
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240613060021_updateHFCSDeviceData'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240613060021_updateHFCSDeviceData', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    ALTER TABLE [GWPs] DROP CONSTRAINT [PK_GWPs];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''二氟一氯一溴甲烷'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''CH4'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''CO2'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''FM200'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''HFC-236fa'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''N2O'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''NF3'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-12'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-1234yf'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-125'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-134A'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-22'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-23'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-32'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-404A'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-407C'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-407F'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-410A'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-417A'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-452A'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-507A'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''R-600A'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC(N'DELETE FROM [GWPs]
    WHERE [Name] = N''SF6'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    EXEC sp_rename N'[GWPs].[GWP_Year]', N'ARCount', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GWPs]') AND [c].[name] = N'Name');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [GWPs] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [GWPs] ALTER COLUMN [Name] nvarchar(max) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    ALTER TABLE [GWPs] ADD [Id] int NOT NULL IDENTITY;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    ALTER TABLE [GWPs] ADD CONSTRAINT [PK_GWPs] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ARCount', N'Name', N'Num') AND [object_id] = OBJECT_ID(N'[GWPs]'))
        SET IDENTITY_INSERT [GWPs] ON;
    EXEC(N'INSERT INTO [GWPs] ([Id], [ARCount], [Name], [Num])
    VALUES (1, 6, N''CO2'', 1.0),
    (2, 6, N''CH4'', 27.9),
    (3, 6, N''N2O'', 273.0),
    (4, 6, N''R-12'', 12500.0),
    (5, 6, N''R-125'', 3740.0),
    (6, 6, N''R-1234yf'', 0.501),
    (7, 6, N''R-23'', 14600.0),
    (8, 6, N''R-32'', 771.0),
    (9, 6, N''R-134A'', 1530.0),
    (10, 6, N''FM200'', 3600.0),
    (11, 6, N''R-22'', 1960.0),
    (12, 6, N''R-410A'', 2255.5),
    (13, 6, N''R-600A'', 0.006),
    (14, 6, N''R-417A'', 2127.0),
    (15, 6, N''R-404A'', 4728.0),
    (16, 6, N''R-407C'', 1908.0),
    (17, 6, N''R-407F'', 1965.3),
    (18, 6, N''R-452A'', 2291.5603),
    (19, 6, N''R-507A'', 4475.0),
    (20, 6, N''NF3'', 17400.0),
    (21, 6, N''SF6'', 24300.0),
    (22, 6, N''二氟一氯一溴甲烷'', 1930.0),
    (23, 6, N''HFC-236fa'', 8690.0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ARCount', N'Name', N'Num') AND [object_id] = OBJECT_ID(N'[GWPs]'))
        SET IDENTITY_INSERT [GWPs] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240618175955_editGWP'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240618175955_editGWP', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624143850_AddAnalysis'
)
BEGIN
    ALTER TABLE [Areas] ADD [ARVersion] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624143850_AddAnalysis'
)
BEGIN
    CREATE TABLE [Analyses] (
        [Id] uniqueidentifier NOT NULL,
        [AreaId] uniqueidentifier NOT NULL,
        [_21A] int NOT NULL,
        [_21B] int NOT NULL,
        [_21C] int NOT NULL,
        [_22A] int NOT NULL,
        [_22B] int NOT NULL,
        [_22C] int NOT NULL,
        [_31A] int NOT NULL,
        [_31B] int NOT NULL,
        [_31C] int NOT NULL,
        [_32A] int NOT NULL,
        [_32B] int NOT NULL,
        [_32C] int NOT NULL,
        [_33A] int NOT NULL,
        [_33B] int NOT NULL,
        [_33C] int NOT NULL,
        [_34A] int NOT NULL,
        [_34B] int NOT NULL,
        [_34C] int NOT NULL,
        [_35A] int NOT NULL,
        [_35B] int NOT NULL,
        [_35C] int NOT NULL,
        [_41A] int NOT NULL,
        [_41B] int NOT NULL,
        [_41C] int NOT NULL,
        [_42A] int NOT NULL,
        [_42B] int NOT NULL,
        [_42C] int NOT NULL,
        [_43A] int NOT NULL,
        [_43B] int NOT NULL,
        [_43C] int NOT NULL,
        [_44A] int NOT NULL,
        [_44B] int NOT NULL,
        [_44C] int NOT NULL,
        [_45A] int NOT NULL,
        [_45B] int NOT NULL,
        [_45C] int NOT NULL,
        [_46A] int NOT NULL,
        [_46B] int NOT NULL,
        [_46C] int NOT NULL,
        [_47A] int NOT NULL,
        [_47B] int NOT NULL,
        [_47C] int NOT NULL,
        [_48A] int NOT NULL,
        [_48B] int NOT NULL,
        [_48C] int NOT NULL,
        [_49A] int NOT NULL,
        [_49B] int NOT NULL,
        [_49C] int NOT NULL,
        [_410A] int NOT NULL,
        [_410B] int NOT NULL,
        [_410C] int NOT NULL,
        [_411A] int NOT NULL,
        [_411B] int NOT NULL,
        [_411C] int NOT NULL,
        [_51A] int NOT NULL,
        [_51B] int NOT NULL,
        [_51C] int NOT NULL,
        [_52A] int NOT NULL,
        [_52B] int NOT NULL,
        [_52C] int NOT NULL,
        [_53A] int NOT NULL,
        [_53B] int NOT NULL,
        [_53C] int NOT NULL,
        [_54A] int NOT NULL,
        [_54B] int NOT NULL,
        [_54C] int NOT NULL,
        [_55A] int NOT NULL,
        [_55B] int NOT NULL,
        [_55C] int NOT NULL,
        [_61A] int NOT NULL,
        [_61B] int NOT NULL,
        [_61C] int NOT NULL,
        [_21] int NOT NULL,
        [_22] int NOT NULL,
        [_31] int NOT NULL,
        [_32] int NOT NULL,
        [_33] int NOT NULL,
        [_34] int NOT NULL,
        [_35] int NOT NULL,
        [_41] int NOT NULL,
        [_42] int NOT NULL,
        [_43] int NOT NULL,
        [_44] int NOT NULL,
        [_45] int NOT NULL,
        [_46] int NOT NULL,
        [_47] int NOT NULL,
        [_48] int NOT NULL,
        [_49] int NOT NULL,
        [_410] int NOT NULL,
        [_411] int NOT NULL,
        [_51] int NOT NULL,
        [_52] int NOT NULL,
        [_53] int NOT NULL,
        [_54] int NOT NULL,
        [_55] int NOT NULL,
        [_61] int NOT NULL,
        [isDeleted] tinyint NOT NULL,
        [CreateTime] datetime2 NOT NULL,
        [ModifiedTime] datetime2 NULL,
        [DeleteTime] datetime2 NULL,
        CONSTRAINT [PK_Analyses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Analyses_Areas_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [Areas] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624143850_AddAnalysis'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Analyses_AreaId] ON [Analyses] ([AreaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240624143850_AddAnalysis'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240624143850_AddAnalysis', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240628173830_mssql.local_migration_509'
)
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Discriminator');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [AspNetUsers] ALTER COLUMN [Discriminator] nvarchar(21) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240628173830_mssql.local_migration_509'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240628173830_mssql.local_migration_509', N'8.0.6');
END;
GO

COMMIT;
GO

