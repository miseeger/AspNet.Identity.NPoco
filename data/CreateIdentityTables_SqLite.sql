CREATE TABLE [Application] (
  [Id] VARCHAR(64) NOT NULL ON CONFLICT FAIL, 
  [Name] VARCHAR(128) NOT NULL ON CONFLICT FAIL, 
  CONSTRAINT [] PRIMARY KEY ([Id]) ON CONFLICT FAIL);


CREATE TABLE [Role] (
  [Id] VARCHAR(64) NOT NULL ON CONFLICT FAIL, 
  [Name] VARCHAR(256) NOT NULL ON CONFLICT FAIL, 
  CONSTRAINT [] PRIMARY KEY ([Id]) ON CONFLICT FAIL);


CREATE TABLE [User] (
  [Id] VARCHAR(64) NOT NULL ON CONFLICT FAIL COLLATE NOCASE, 
  [Email] VARCHAR(256), 
  [EmailConfirmed] BOOL NOT NULL ON CONFLICT FAIL, 
  [PasswordHash] VARCHAR(512), 
  [SecurityStamp] VARCHAR(512), 
  [PhoneNumber] VARCHAR(256), 
  [PhoneNumberConfirmed] BOOL NOT NULL ON CONFLICT FAIL, 
  [TwoFactorEnabled] BOOL NOT NULL ON CONFLICT FAIL, 
  [LockoutEndDateUtc] DATETIME, 
  [LockoutEnabled] BOOL NOT NULL ON CONFLICT FAIL, 
  [AccessFailedCount] INT NOT NULL ON CONFLICT FAIL DEFAULT 0, 
  [UserName] VARVARCHAR(256) NOT NULL ON CONFLICT FAIL, 
  CONSTRAINT [sqlite_autoindex_User_1] PRIMARY KEY ([Id]) ON CONFLICT FAIL);


CREATE TABLE [UserClaim] (
  [Id] VARCHAR(64) NOT NULL ON CONFLICT FAIL, 
  [UserId] VARCHAR(64) NOT NULL ON CONFLICT FAIL CONSTRAINT [FK_UserClaim_User] REFERENCES [User]([Id]) ON DELETE CASCADE ON UPDATE CASCADE, 
  [ClaimType] VARCHAR(256), 
  [ClaimValue] VARCHAR(256), 
  CONSTRAINT [sqlite_autoindex_UserClaim_1] PRIMARY KEY ([Id]) ON CONFLICT FAIL);

CREATE INDEX [IX_UserClaim_UserId] ON [UserClaim] ([UserId]);


CREATE TABLE [UserLogin] (
  [LoginProvider] VARCHAR(128) NOT NULL ON CONFLICT FAIL, 
  [ProviderKey] VARCHAR(128) NOT NULL ON CONFLICT FAIL, 
  [UserId] VARCHAR(64) NOT NULL ON CONFLICT FAIL CONSTRAINT [FK_UserLogin_User] REFERENCES [User]([Id]) ON DELETE CASCADE ON UPDATE CASCADE, 
  CONSTRAINT [sqlite_autoindex_UserLogin_1] PRIMARY KEY ([LoginProvider], [ProviderKey], [UserId]) ON CONFLICT FAIL);

CREATE INDEX [IX_UserLogin_UserId] ON [UserLogin] ([UserId]);


CREATE TABLE [UserRole] (
  [UserId] VARCHAR(64) NOT NULL ON CONFLICT FAIL CONSTRAINT [FK_UserRole_User] REFERENCES [User]([Id]) ON DELETE CASCADE ON UPDATE CASCADE, 
  [RoleId] VARCHAR(64) NOT NULL ON CONFLICT FAIL CONSTRAINT [FK_UserRole_Role] REFERENCES [Role]([Id]) ON DELETE CASCADE ON UPDATE CASCADE, 
  CONSTRAINT [sqlite_autoindex_UserRole_1] PRIMARY KEY ([UserId], [RoleId]));

CREATE INDEX [IX_UserRole_RoleId] ON [UserRole] ([RoleId]);

CREATE INDEX [IX_UserRole_UserId] ON [UserRole] ([UserId]);

