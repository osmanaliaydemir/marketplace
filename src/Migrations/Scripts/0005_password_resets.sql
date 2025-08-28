-- Migration: 0005_password_resets.sql
-- Description: Create PasswordResets table for password reset functionality
-- Date: 2024-01-XX

-- Create PasswordResets table
CREATE TABLE PasswordResets (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    IsUsed BIT NOT NULL DEFAULT 0,
    UsedAt DATETIME2 NULL,
    IpAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedAt DATETIME2 NULL
);

-- Create indexes for better performance
CREATE INDEX IX_PasswordResets_Email ON PasswordResets(Email);
CREATE INDEX IX_PasswordResets_Token ON PasswordResets(Token);
CREATE INDEX IX_PasswordResets_ExpiresAt ON PasswordResets(ExpiresAt);
CREATE INDEX IX_PasswordResets_IsUsed ON PasswordResets(IsUsed);
CREATE INDEX IX_PasswordResets_CreatedAt ON PasswordResets(CreatedAt);

-- Create unique index for token
CREATE UNIQUE INDEX UX_PasswordResets_Token ON PasswordResets(Token);

-- Add comments for documentation
EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Password reset tokens table', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'PasswordResets';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Unique identifier for the password reset', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'PasswordResets', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Email address of the user requesting password reset', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'PasswordResets', 
    @level2type = N'COLUMN', @level2name = N'Email';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Unique token for password reset', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'PasswordResets', 
    @level2type = N'COLUMN', @level2name = N'Token';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Expiration time of the token', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'PasswordResets', 
    @level2type = N'COLUMN', @level2name = N'ExpiresAt';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indicates if the token has been used', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'PasswordResets', 
    @level2type = N'COLUMN', @level2name = N'IsUsed';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Timestamp when the token was used', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'PasswordResets', 
    @level2type = N'COLUMN', @level2name = N'UsedAt';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'IP address of the user requesting password reset', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'PasswordResets', 
    @level2type = N'COLUMN', @level2name = N'IpAddress';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'User agent string of the browser', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'PasswordResets', 
    @level2type = N'COLUMN', @level2name = N'UserAgent';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Timestamp when the password reset was created', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'PasswordResets', 
    @level2type = N'COLUMN', @level2name = N'CreatedAt';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Timestamp when the password reset was last modified', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'PasswordResets', 
    @level2type = N'COLUMN', @level2name = N'ModifiedAt';
