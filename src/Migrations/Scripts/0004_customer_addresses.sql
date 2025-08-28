-- Migration: 0004_customer_addresses.sql
-- Description: Create CustomerAddresses table for storing customer shipping addresses
-- Date: 2024-01-XX

-- Create CustomerAddresses table
CREATE TABLE CustomerAddresses (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    CustomerId BIGINT NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    RecipientName NVARCHAR(200) NOT NULL,
    AddressLine1 NVARCHAR(500) NOT NULL,
    AddressLine2 NVARCHAR(500) NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    IsDefault BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedAt DATETIME2 NULL
);

-- Create indexes for better performance
CREATE INDEX IX_CustomerAddresses_CustomerId ON CustomerAddresses(CustomerId);
CREATE INDEX IX_CustomerAddresses_IsActive ON CustomerAddresses(IsActive);
CREATE INDEX IX_CustomerAddresses_IsDefault ON CustomerAddresses(IsDefault);
CREATE INDEX IX_CustomerAddresses_CreatedAt ON CustomerAddresses(CreatedAt);

-- Create foreign key constraint to AppUsers table
ALTER TABLE CustomerAddresses 
ADD CONSTRAINT FK_CustomerAddresses_AppUsers 
FOREIGN KEY (CustomerId) REFERENCES AppUsers(Id);

-- Add constraint to ensure only one default address per customer
CREATE UNIQUE INDEX UX_CustomerAddresses_CustomerId_IsDefault 
ON CustomerAddresses(CustomerId, IsDefault) 
WHERE IsDefault = 1 AND IsActive = 1;

-- Insert sample data for testing (optional)
-- INSERT INTO CustomerAddresses (CustomerId, Title, RecipientName, AddressLine1, City, State, PostalCode, Phone, IsDefault, IsActive, CreatedAt)
-- VALUES 
--     (1, 'Ev Adresi', 'Ahmet Yılmaz', 'Atatürk Caddesi No:123', 'İstanbul', 'Kadıköy', '34700', '+90 555 123 4567', 1, 1, GETUTCDATE()),
--     (1, 'İş Adresi', 'Ahmet Yılmaz', 'Levent Mahallesi No:456', 'İstanbul', 'Beşiktaş', '34330', '+90 555 123 4567', 0, 1, GETUTCDATE()),
--     (2, 'Ana Adres', 'Ayşe Demir', 'Kızılay Meydanı No:789', 'Ankara', 'Çankaya', '06420', '+90 555 987 6543', 1, 1, GETUTCDATE());

-- Add comments for documentation
EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Customer shipping addresses table', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Unique identifier for the address', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'Id';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Reference to the customer (AppUser)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'CustomerId';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Address title (e.g., Home, Work, Vacation)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'Title';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Name of the person receiving the package', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'RecipientName';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Primary address line', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'AddressLine1';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Secondary address line (apartment, suite, etc.)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'AddressLine2';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'City name', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'City';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'State/Province name', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'State';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Postal/ZIP code', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'PostalCode';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Contact phone number for delivery', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'Phone';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indicates if this is the default address for the customer', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'IsDefault';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indicates if the address is active (soft delete)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'IsActive';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Timestamp when the address was created', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'CreatedAt';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Timestamp when the address was last modified', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'CustomerAddresses', 
    @level2type = N'COLUMN', @level2name = N'ModifiedAt';
