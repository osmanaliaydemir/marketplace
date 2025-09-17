-- Add missing columns to existing products table
-- Check if columns exist before adding them

-- Add short_description column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'short_description')
    ALTER TABLE [dbo].[products] ADD [short_description] NVARCHAR(500) NULL;

-- Add sku column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'sku')
    ALTER TABLE [dbo].[products] ADD [sku] NVARCHAR(100) NOT NULL DEFAULT '';

-- Add compare_at_price column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'compare_at_price')
    ALTER TABLE [dbo].[products] ADD [compare_at_price] DECIMAL(18,2) NULL;

-- Add stock_qty column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'stock_qty')
    ALTER TABLE [dbo].[products] ADD [stock_qty] INT NULL DEFAULT 0;

-- Add is_featured column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'is_featured')
    ALTER TABLE [dbo].[products] ADD [is_featured] BIT NOT NULL DEFAULT 0;

-- Add is_published column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'is_published')
    ALTER TABLE [dbo].[products] ADD [is_published] BIT NOT NULL DEFAULT 1;

-- Add display_order column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'display_order')
    ALTER TABLE [dbo].[products] ADD [display_order] INT NOT NULL DEFAULT 0;

-- Add weight column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'weight')
    ALTER TABLE [dbo].[products] ADD [weight] DECIMAL(10,3) NOT NULL DEFAULT 0;

-- Add min_order_qty column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'min_order_qty')
    ALTER TABLE [dbo].[products] ADD [min_order_qty] INT NOT NULL DEFAULT 1;

-- Add max_order_qty column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'max_order_qty')
    ALTER TABLE [dbo].[products] ADD [max_order_qty] INT NOT NULL DEFAULT 999;

-- Add meta_title column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'meta_title')
    ALTER TABLE [dbo].[products] ADD [meta_title] NVARCHAR(255) NULL;

-- Add meta_description column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'meta_description')
    ALTER TABLE [dbo].[products] ADD [meta_description] NVARCHAR(500) NULL;

-- Add meta_keywords column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'meta_keywords')
    ALTER TABLE [dbo].[products] ADD [meta_keywords] NVARCHAR(500) NULL;

-- Add published_at column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'published_at')
    ALTER TABLE [dbo].[products] ADD [published_at] DATETIME2 NULL;

-- Add is_deleted column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'is_deleted')
    ALTER TABLE [dbo].[products] ADD [is_deleted] BIT NOT NULL DEFAULT 0;

-- Add deleted_at column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'deleted_at')
    ALTER TABLE [dbo].[products] ADD [deleted_at] DATETIME2 NULL;

-- Create additional indexes if they don't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'IX_products_slug')
    CREATE INDEX [IX_products_slug] ON [dbo].[products] ([slug]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'IX_products_sku')
    CREATE INDEX [IX_products_sku] ON [dbo].[products] ([sku]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'IX_products_is_active')
    CREATE INDEX [IX_products_is_active] ON [dbo].[products] ([is_active]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'IX_products_is_published')
    CREATE INDEX [IX_products_is_published] ON [dbo].[products] ([is_published]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'IX_products_is_featured')
    CREATE INDEX [IX_products_is_featured] ON [dbo].[products] ([is_featured]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[products]') AND name = 'IX_products_created_at')
    CREATE INDEX [IX_products_created_at] ON [dbo].[products] ([created_at]);

-- Note: New columns have default values, so existing records will automatically get them
-- No need to update existing records as the DEFAULT constraints will handle it