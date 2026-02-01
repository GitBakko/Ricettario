CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
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
GO


CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Icon] nvarchar(max) NOT NULL,
    [Color] nvarchar(max) NOT NULL,
    [SortOrder] int NOT NULL,
    [IsSystemDefault] bit NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [RecipePhaseTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Icon] nvarchar(max) NULL,
    [Color] nvarchar(max) NULL,
    [IsActiveWork] bit NOT NULL,
    [IsSystemDefault] bit NOT NULL,
    CONSTRAINT [PK_RecipePhaseTypes] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Tags] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Color] nvarchar(max) NOT NULL,
    [UsageCount] int NOT NULL,
    CONSTRAINT [PK_Tags] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [Recipes] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Instructions] nvarchar(max) NULL,
    [PrepTimeMinutes] float NOT NULL,
    [CookTimeMinutes] float NOT NULL,
    [Difficulty] nvarchar(max) NULL,
    [ImageUrl] nvarchar(max) NULL,
    [TotalFlourWeight] float NOT NULL,
    [ServingPieces] int NOT NULL,
    [PieceWeight] float NULL,
    [CategoryId] int NULL,
    [UserId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Recipes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Recipes_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Recipes_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL
);
GO


CREATE TABLE [Ingredients] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Quantity] float NOT NULL,
    [Unit] nvarchar(max) NOT NULL,
    [BakersPercentage] float NOT NULL,
    [RecipeId] int NOT NULL,
    CONSTRAINT [PK_Ingredients] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Ingredients_Recipes_RecipeId] FOREIGN KEY ([RecipeId]) REFERENCES [Recipes] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [RecipePhases] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [RecipePhaseTypeId] int NOT NULL,
    [Description] nvarchar(max) NULL,
    [DurationMinutes] float NOT NULL,
    [Temperature] int NULL,
    [OvenMode] nvarchar(max) NULL,
    [RecipeId] int NOT NULL,
    CONSTRAINT [PK_RecipePhases] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RecipePhases_RecipePhaseTypes_RecipePhaseTypeId] FOREIGN KEY ([RecipePhaseTypeId]) REFERENCES [RecipePhaseTypes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RecipePhases_Recipes_RecipeId] FOREIGN KEY ([RecipeId]) REFERENCES [Recipes] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [RecipeTags] (
    [RecipeId] int NOT NULL,
    [TagId] int NOT NULL,
    CONSTRAINT [PK_RecipeTags] PRIMARY KEY ([RecipeId], [TagId]),
    CONSTRAINT [FK_RecipeTags_Recipes_RecipeId] FOREIGN KEY ([RecipeId]) REFERENCES [Recipes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RecipeTags_Tags_TagId] FOREIGN KEY ([TagId]) REFERENCES [Tags] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [RecipeVideos] (
    [Id] int NOT NULL IDENTITY,
    [Url] nvarchar(max) NOT NULL,
    [RecipeId] int NOT NULL,
    CONSTRAINT [PK_RecipeVideos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RecipeVideos_Recipes_RecipeId] FOREIGN KEY ([RecipeId]) REFERENCES [Recipes] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [PhaseIngredient] (
    [Id] int NOT NULL IDENTITY,
    [IngredientName] nvarchar(max) NOT NULL,
    [Quantity] float NOT NULL,
    [Unit] nvarchar(max) NOT NULL,
    [RecipePhaseId] int NOT NULL,
    CONSTRAINT [PK_PhaseIngredient] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PhaseIngredient_RecipePhases_RecipePhaseId] FOREIGN KEY ([RecipePhaseId]) REFERENCES [RecipePhases] ([Id]) ON DELETE CASCADE
);
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Color', N'Description', N'Icon', N'IsSystemDefault', N'Name', N'SortOrder') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] ([Id], [Color], [Description], [Icon], [IsSystemDefault], [Name], [SortOrder])
VALUES (1, N'#dc3545', N'Pizze classiche, gourmet e regionali', N'fa-solid fa-pizza-slice', CAST(1 AS bit), N'Pizza', 1),
(2, N'#8B4513', N'Pane artigianale, filoni, pagnotte', N'fa-solid fa-bread-slice', CAST(1 AS bit), N'Pane', 2),
(3, N'#f0ad4e', N'Focacce liguri, pugliesi e varianti', N'fa-solid fa-border-all', CAST(1 AS bit), N'Focaccia', 3),
(4, N'#e91e8c', N'Dolci da forno, crostate, torte', N'fa-solid fa-cake-candles', CAST(1 AS bit), N'Pasticceria', 4),
(5, N'#ffc107', N'Panettone, pandoro, colomba', N'fa-solid fa-star', CAST(1 AS bit), N'Grandi Lievitati', 5),
(6, N'#fd7e14', N'Brioches dolci e salate, croissant', N'fa-solid fa-moon', CAST(1 AS bit), N'Brioche', 6),
(7, N'#795548', N'Biscotti, frollini, cantucci', N'fa-solid fa-cookie', CAST(1 AS bit), N'Biscotti', 7),
(8, N'#28a745', N'Quiche, torte rustiche, sfoglie', N'fa-solid fa-hexagon', CAST(1 AS bit), N'Torte Salate', 8),
(9, N'#17a2b8', N'Pasta all''uovo, ravioli, gnocchi', N'fa-solid fa-layer-group', CAST(1 AS bit), N'Pasta Fresca', 9),
(10, N'#c7a17a', N'Pain au chocolat, danish, sfoglia', N'fa-solid fa-diamond', CAST(1 AS bit), N'Viennoiserie', 10),
(11, N'#6c757d', N'Altre preparazioni da forno', N'fa-solid fa-ellipsis', CAST(1 AS bit), N'Altro', 99);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Color', N'Description', N'Icon', N'IsSystemDefault', N'Name', N'SortOrder') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Color', N'Description', N'Icon', N'IsActiveWork', N'IsSystemDefault', N'Name') AND [object_id] = OBJECT_ID(N'[RecipePhaseTypes]'))
    SET IDENTITY_INSERT [RecipePhaseTypes] ON;
INSERT INTO [RecipePhaseTypes] ([Id], [Color], [Description], [Icon], [IsActiveWork], [IsSystemDefault], [Name])
VALUES (1, N'#6c757d', NULL, N'fa-solid fa-circle', CAST(1 AS bit), CAST(1 AS bit), N'Impasto'),
(2, N'#6c757d', NULL, N'fa-solid fa-circle', CAST(0 AS bit), CAST(1 AS bit), N'Lievitazione'),
(3, N'#6c757d', NULL, N'fa-solid fa-circle', CAST(1 AS bit), CAST(1 AS bit), N'Cottura'),
(4, N'#6c757d', NULL, N'fa-solid fa-circle', CAST(1 AS bit), CAST(1 AS bit), N'Pre-Impasto'),
(5, N'#6c757d', NULL, N'fa-solid fa-circle', CAST(1 AS bit), CAST(1 AS bit), N'Pieghe'),
(6, N'#6c757d', NULL, N'fa-solid fa-circle', CAST(1 AS bit), CAST(1 AS bit), N'Formatura'),
(7, N'#6c757d', NULL, N'fa-solid fa-circle', CAST(0 AS bit), CAST(1 AS bit), N'Appretto'),
(8, N'#6c757d', NULL, N'fa-solid fa-circle', CAST(0 AS bit), CAST(1 AS bit), N'Autoli');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Color', N'Description', N'Icon', N'IsActiveWork', N'IsSystemDefault', N'Name') AND [object_id] = OBJECT_ID(N'[RecipePhaseTypes]'))
    SET IDENTITY_INSERT [RecipePhaseTypes] OFF;
GO


CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO


CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO


CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO


CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO


CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO


CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO


CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO


CREATE INDEX [IX_Ingredients_RecipeId] ON [Ingredients] ([RecipeId]);
GO


CREATE INDEX [IX_PhaseIngredient_RecipePhaseId] ON [PhaseIngredient] ([RecipePhaseId]);
GO


CREATE INDEX [IX_RecipePhases_RecipeId] ON [RecipePhases] ([RecipeId]);
GO


CREATE INDEX [IX_RecipePhases_RecipePhaseTypeId] ON [RecipePhases] ([RecipePhaseTypeId]);
GO


CREATE INDEX [IX_Recipes_CategoryId] ON [Recipes] ([CategoryId]);
GO


CREATE INDEX [IX_Recipes_UserId] ON [Recipes] ([UserId]);
GO


CREATE INDEX [IX_RecipeTags_TagId] ON [RecipeTags] ([TagId]);
GO


CREATE INDEX [IX_RecipeVideos_RecipeId] ON [RecipeVideos] ([RecipeId]);
GO


