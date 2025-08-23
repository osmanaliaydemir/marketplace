USE [Marketplace]
GO
/****** Object:  Table [dbo].[app_users]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[app_users](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[email] [nvarchar](255) NOT NULL,
	[password_hash] [nvarchar](255) NOT NULL,
	[full_name] [nvarchar](255) NULL,
	[role] [nvarchar](50) NOT NULL,
	[created_at] [datetime2](3) NULL,
	[modified_at] [datetime2](3) NULL,
	[is_active] [bit] NULL,
 CONSTRAINT [PK__app_user__3213E83F909B2C60] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ__app_user__AB6E616409F41B3D] UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[categories]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[categories](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[parent_id] [bigint] NULL,
	[name] [nvarchar](255) NOT NULL,
	[description] [nvarchar](max) NULL,
	[slug] [nvarchar](255) NULL,
	[is_active] [bit] NULL,
	[is_featured] [bit] NULL,
	[display_order] [int] NULL,
	[created_at] [datetime2](7) NULL,
	[image_url] [nvarchar](500) NULL,
	[icon_class] [nvarchar](100) NULL,
	[meta_title] [nvarchar](255) NULL,
	[meta_description] [nvarchar](500) NULL,
	[modified_at] [datetime2](7) NULL,
 CONSTRAINT [PK__categori__3213E83F8FFA2CCE] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ__categori__32DD1E4CE94DDE39] UNIQUE NONCLUSTERED 
(
	[slug] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[inventories]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[inventories](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[variant_id] [bigint] NOT NULL,
	[on_hand] [int] NULL,
	[reserved] [int] NULL,
	[updated_at] [datetime2](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ledger_postings]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ledger_postings](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[transaction_id] [bigint] NOT NULL,
	[account] [nvarchar](50) NOT NULL,
	[debit] [decimal](19, 4) NOT NULL,
	[credit] [decimal](19, 4) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ledger_transactions]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ledger_transactions](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[ref_type] [nvarchar](50) NOT NULL,
	[ref_id] [bigint] NOT NULL,
	[ts] [datetime2](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[order_groups]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[order_groups](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[order_id] [bigint] NOT NULL,
	[seller_id] [bigint] NOT NULL,
	[items_total] [decimal](19, 4) NOT NULL,
	[shipping_total] [decimal](19, 4) NOT NULL,
	[group_total] [decimal](19, 4) NOT NULL,
	[status] [nvarchar](50) NOT NULL,
	[store_id] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[order_items]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[order_items](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[order_group_id] [bigint] NOT NULL,
	[product_id] [bigint] NOT NULL,
	[variant_id] [bigint] NULL,
	[name] [nvarchar](255) NOT NULL,
	[qty] [int] NOT NULL,
	[unit_price] [decimal](19, 4) NOT NULL,
	[total] [decimal](19, 4) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[orders]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[orders](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[buyer_id] [bigint] NULL,
	[email] [nvarchar](255) NULL,
	[phone] [nvarchar](50) NULL,
	[ship_address_json] [nvarchar](max) NULL,
	[total] [decimal](19, 4) NOT NULL,
	[currency] [char](3) NOT NULL,
	[status] [nvarchar](50) NOT NULL,
	[created_at] [datetime2](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[outbox_messages]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[outbox_messages](
	[id] [uniqueidentifier] NOT NULL,
	[type] [nvarchar](200) NOT NULL,
	[payload] [nvarchar](max) NOT NULL,
	[occurred_on] [datetime2](3) NOT NULL,
	[processed_on] [datetime2](3) NULL,
	[retries] [int] NOT NULL,
	[error] [nvarchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[payment_splits]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[payment_splits](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[payment_id] [bigint] NOT NULL,
	[order_group_id] [bigint] NOT NULL,
	[seller_id] [bigint] NOT NULL,
	[seller_amount] [decimal](19, 4) NOT NULL,
	[platform_commission] [decimal](19, 4) NOT NULL,
	[currency] [char](3) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[payments]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[payments](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[order_id] [bigint] NOT NULL,
	[provider] [nvarchar](32) NOT NULL,
	[provider_tx] [nvarchar](128) NULL,
	[status] [nvarchar](50) NOT NULL,
	[gross] [decimal](19, 4) NOT NULL,
	[fee_platform] [decimal](19, 4) NOT NULL,
	[fee_psp] [decimal](19, 4) NOT NULL,
	[net_to_sellers] [decimal](19, 4) NOT NULL,
	[created_at] [datetime2](3) NULL,
	[captured_at] [datetime2](3) NULL,
	[merchant_oid] [nvarchar](64) NOT NULL,
	[currency] [char](3) NULL,
	[is_test] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[provider_tx] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[product_variants]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product_variants](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[product_id] [bigint] NOT NULL,
	[sku] [nvarchar](100) NULL,
	[price] [decimal](19, 4) NOT NULL,
	[stock_qty] [int] NULL,
	[is_active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[sku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[products]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[seller_id] [bigint] NOT NULL,
	[category_id] [bigint] NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[slug] [nvarchar](255) NULL,
	[description] [nvarchar](max) NULL,
	[price] [decimal](19, 4) NOT NULL,
	[currency] [char](3) NOT NULL,
	[is_active] [bit] NULL,
	[created_at] [datetime2](3) NULL,
	[modified_at] [datetime2](3) NULL,
	[store_id] [bigint] NOT NULL,
	[store_category_id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[slug] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[refund_items]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[refund_items](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[refund_id] [bigint] NOT NULL,
	[order_item_id] [bigint] NOT NULL,
	[amount] [decimal](19, 4) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[refunds]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[refunds](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[payment_id] [bigint] NOT NULL,
	[order_group_id] [bigint] NULL,
	[amount] [decimal](19, 4) NOT NULL,
	[reason] [nvarchar](255) NULL,
	[status] [nvarchar](50) NULL,
	[created_at] [datetime2](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sellers]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sellers](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[paytr_submerchant_id] [nvarchar](64) NULL,
	[kyc_status] [tinyint] NULL,
	[commission_rate] [decimal](5, 2) NULL,
	[iban] [nvarchar](34) NULL,
	[created_at] [datetime2](3) NULL,
	[modified_at] [datetime2](3) NULL,
	[is_active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[shipments]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[shipments](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[order_group_id] [bigint] NOT NULL,
	[carrier] [nvarchar](100) NOT NULL,
	[service] [nvarchar](100) NULL,
	[tracking_number] [nvarchar](100) NULL,
	[label_url] [nvarchar](500) NULL,
	[status] [nvarchar](50) NOT NULL,
	[shipped_at] [datetime2](3) NULL,
	[delivered_at] [datetime2](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[tracking_number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[store_applications]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[store_applications](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[store_name] [nvarchar](255) NOT NULL,
	[slug] [nvarchar](255) NOT NULL,
	[description] [nvarchar](max) NULL,
	[contact_email] [nvarchar](255) NOT NULL,
	[contact_phone] [nvarchar](50) NULL,
	[business_address] [nvarchar](max) NULL,
	[tax_number] [nvarchar](50) NULL,
	[status] [tinyint] NOT NULL,
	[rejection_reason] [nvarchar](500) NULL,
	[approved_at] [datetime2](3) NULL,
	[approved_by_user_id] [bigint] NULL,
	[seller_id] [bigint] NULL,
	[created_at] [datetime2](3) NULL,
	[modified_at] [datetime2](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[store_categories]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[store_categories](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[store_id] [bigint] NOT NULL,
	[parent_id] [bigint] NULL,
	[name] [nvarchar](255) NOT NULL,
	[slug] [nvarchar](255) NOT NULL,
	[is_active] [bit] NULL,
	[global_category_id] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[stores]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[stores](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[seller_id] [bigint] NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[slug] [nvarchar](255) NULL,
	[logo_url] [nvarchar](500) NULL,
	[is_active] [bit] NULL,
	[created_at] [datetime2](3) NULL,
	[modified_at] [datetime2](3) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[slug] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webhook_deliveries]    Script Date: 24.08.2025 00:46:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webhook_deliveries](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[event_type] [nvarchar](100) NOT NULL,
	[payload] [nvarchar](max) NOT NULL,
	[signature] [nvarchar](255) NULL,
	[status] [nvarchar](50) NULL,
	[attempts] [int] NULL,
	[created_at] [datetime2](3) NULL,
	[delivered_at] [datetime2](3) NULL,
	[external_id] [nvarchar](128) NULL,
	[merchant_oid] [nvarchar](64) NULL,
	[processed_at] [datetime2](3) NULL,
	[error] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[app_users] ADD  CONSTRAINT [DF__app_users__creat__37A5467C]  DEFAULT (sysutcdatetime()) FOR [created_at]
GO
ALTER TABLE [dbo].[app_users] ADD  CONSTRAINT [DF__app_users__is_ac__38996AB5]  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[categories] ADD  CONSTRAINT [DF__categorie__is_ac__49C3F6B7]  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[inventories] ADD  DEFAULT ((0)) FOR [on_hand]
GO
ALTER TABLE [dbo].[inventories] ADD  DEFAULT ((0)) FOR [reserved]
GO
ALTER TABLE [dbo].[inventories] ADD  DEFAULT (sysutcdatetime()) FOR [updated_at]
GO
ALTER TABLE [dbo].[ledger_postings] ADD  DEFAULT ((0)) FOR [debit]
GO
ALTER TABLE [dbo].[ledger_postings] ADD  DEFAULT ((0)) FOR [credit]
GO
ALTER TABLE [dbo].[ledger_transactions] ADD  DEFAULT (sysutcdatetime()) FOR [ts]
GO
ALTER TABLE [dbo].[orders] ADD  DEFAULT (sysutcdatetime()) FOR [created_at]
GO
ALTER TABLE [dbo].[outbox_messages] ADD  DEFAULT (sysutcdatetime()) FOR [occurred_on]
GO
ALTER TABLE [dbo].[outbox_messages] ADD  DEFAULT ((0)) FOR [retries]
GO
ALTER TABLE [dbo].[payment_splits] ADD  DEFAULT ((0)) FOR [platform_commission]
GO
ALTER TABLE [dbo].[payments] ADD  DEFAULT (sysutcdatetime()) FOR [created_at]
GO
ALTER TABLE [dbo].[payments] ADD  DEFAULT ((0)) FOR [is_test]
GO
ALTER TABLE [dbo].[product_variants] ADD  DEFAULT ((0)) FOR [stock_qty]
GO
ALTER TABLE [dbo].[product_variants] ADD  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[products] ADD  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[products] ADD  DEFAULT (sysutcdatetime()) FOR [created_at]
GO
ALTER TABLE [dbo].[refunds] ADD  DEFAULT (sysutcdatetime()) FOR [created_at]
GO
ALTER TABLE [dbo].[sellers] ADD  DEFAULT ((0)) FOR [kyc_status]
GO
ALTER TABLE [dbo].[sellers] ADD  DEFAULT ((0)) FOR [commission_rate]
GO
ALTER TABLE [dbo].[sellers] ADD  DEFAULT (sysutcdatetime()) FOR [created_at]
GO
ALTER TABLE [dbo].[sellers] ADD  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[store_categories] ADD  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[stores] ADD  DEFAULT ((1)) FOR [is_active]
GO
ALTER TABLE [dbo].[stores] ADD  DEFAULT (sysutcdatetime()) FOR [created_at]
GO
ALTER TABLE [dbo].[webhook_deliveries] ADD  DEFAULT ((0)) FOR [attempts]
GO
ALTER TABLE [dbo].[webhook_deliveries] ADD  DEFAULT (sysutcdatetime()) FOR [created_at]
GO
ALTER TABLE [dbo].[categories]  WITH CHECK ADD  CONSTRAINT [FK__categorie__paren__48CFD27E] FOREIGN KEY([parent_id])
REFERENCES [dbo].[categories] ([id])
GO
ALTER TABLE [dbo].[categories] CHECK CONSTRAINT [FK__categorie__paren__48CFD27E]
GO
ALTER TABLE [dbo].[inventories]  WITH CHECK ADD FOREIGN KEY([variant_id])
REFERENCES [dbo].[product_variants] ([id])
GO
ALTER TABLE [dbo].[ledger_postings]  WITH CHECK ADD FOREIGN KEY([transaction_id])
REFERENCES [dbo].[ledger_transactions] ([id])
GO
ALTER TABLE [dbo].[order_groups]  WITH CHECK ADD FOREIGN KEY([order_id])
REFERENCES [dbo].[orders] ([id])
GO
ALTER TABLE [dbo].[order_groups]  WITH CHECK ADD FOREIGN KEY([seller_id])
REFERENCES [dbo].[sellers] ([id])
GO
ALTER TABLE [dbo].[order_groups]  WITH CHECK ADD FOREIGN KEY([store_id])
REFERENCES [dbo].[stores] ([id])
GO
ALTER TABLE [dbo].[order_items]  WITH CHECK ADD FOREIGN KEY([order_group_id])
REFERENCES [dbo].[order_groups] ([id])
GO
ALTER TABLE [dbo].[order_items]  WITH CHECK ADD FOREIGN KEY([product_id])
REFERENCES [dbo].[products] ([id])
GO
ALTER TABLE [dbo].[order_items]  WITH CHECK ADD FOREIGN KEY([variant_id])
REFERENCES [dbo].[product_variants] ([id])
GO
ALTER TABLE [dbo].[orders]  WITH CHECK ADD  CONSTRAINT [FK__orders__buyer_id__5EBF139D] FOREIGN KEY([buyer_id])
REFERENCES [dbo].[app_users] ([id])
GO
ALTER TABLE [dbo].[orders] CHECK CONSTRAINT [FK__orders__buyer_id__5EBF139D]
GO
ALTER TABLE [dbo].[payment_splits]  WITH CHECK ADD FOREIGN KEY([order_group_id])
REFERENCES [dbo].[order_groups] ([id])
GO
ALTER TABLE [dbo].[payment_splits]  WITH CHECK ADD FOREIGN KEY([payment_id])
REFERENCES [dbo].[payments] ([id])
GO
ALTER TABLE [dbo].[payment_splits]  WITH CHECK ADD FOREIGN KEY([seller_id])
REFERENCES [dbo].[sellers] ([id])
GO
ALTER TABLE [dbo].[payments]  WITH CHECK ADD FOREIGN KEY([order_id])
REFERENCES [dbo].[orders] ([id])
GO
ALTER TABLE [dbo].[product_variants]  WITH CHECK ADD FOREIGN KEY([product_id])
REFERENCES [dbo].[products] ([id])
GO
ALTER TABLE [dbo].[products]  WITH CHECK ADD  CONSTRAINT [FK__products__catego__4E88ABD4] FOREIGN KEY([category_id])
REFERENCES [dbo].[categories] ([id])
GO
ALTER TABLE [dbo].[products] CHECK CONSTRAINT [FK__products__catego__4E88ABD4]
GO
ALTER TABLE [dbo].[products]  WITH CHECK ADD FOREIGN KEY([seller_id])
REFERENCES [dbo].[sellers] ([id])
GO
ALTER TABLE [dbo].[products]  WITH CHECK ADD FOREIGN KEY([store_id])
REFERENCES [dbo].[stores] ([id])
GO
ALTER TABLE [dbo].[products]  WITH CHECK ADD FOREIGN KEY([store_category_id])
REFERENCES [dbo].[store_categories] ([id])
GO
ALTER TABLE [dbo].[refund_items]  WITH CHECK ADD FOREIGN KEY([order_item_id])
REFERENCES [dbo].[order_items] ([id])
GO
ALTER TABLE [dbo].[refund_items]  WITH CHECK ADD FOREIGN KEY([refund_id])
REFERENCES [dbo].[refunds] ([id])
GO
ALTER TABLE [dbo].[refunds]  WITH CHECK ADD FOREIGN KEY([order_group_id])
REFERENCES [dbo].[order_groups] ([id])
GO
ALTER TABLE [dbo].[refunds]  WITH CHECK ADD FOREIGN KEY([payment_id])
REFERENCES [dbo].[payments] ([id])
GO
ALTER TABLE [dbo].[sellers]  WITH CHECK ADD  CONSTRAINT [FK__sellers__user_id__3B75D760] FOREIGN KEY([user_id])
REFERENCES [dbo].[app_users] ([id])
GO
ALTER TABLE [dbo].[sellers] CHECK CONSTRAINT [FK__sellers__user_id__3B75D760]
GO
ALTER TABLE [dbo].[shipments]  WITH CHECK ADD FOREIGN KEY([order_group_id])
REFERENCES [dbo].[order_groups] ([id])
GO
ALTER TABLE [dbo].[store_categories]  WITH CHECK ADD  CONSTRAINT [FK__store_cat__globa__1DB06A4F] FOREIGN KEY([global_category_id])
REFERENCES [dbo].[categories] ([id])
GO
ALTER TABLE [dbo].[store_categories] CHECK CONSTRAINT [FK__store_cat__globa__1DB06A4F]
GO
ALTER TABLE [dbo].[store_categories]  WITH CHECK ADD FOREIGN KEY([parent_id])
REFERENCES [dbo].[store_categories] ([id])
GO
ALTER TABLE [dbo].[store_categories]  WITH CHECK ADD FOREIGN KEY([store_id])
REFERENCES [dbo].[stores] ([id])
GO
ALTER TABLE [dbo].[stores]  WITH CHECK ADD FOREIGN KEY([seller_id])
REFERENCES [dbo].[sellers] ([id])
GO
ALTER TABLE [dbo].[ledger_postings]  WITH CHECK ADD  CONSTRAINT [CK_ledger_postings_debit_or_credit] CHECK  (([debit]>=(0) AND [credit]=(0) OR [credit]>=(0) AND [debit]=(0)))
GO
ALTER TABLE [dbo].[ledger_postings] CHECK CONSTRAINT [CK_ledger_postings_debit_or_credit]
GO
ALTER TABLE [dbo].[order_items]  WITH CHECK ADD  CONSTRAINT [CK_order_items_amounts_non_negative] CHECK  (([unit_price]>=(0) AND [total]>=(0)))
GO
ALTER TABLE [dbo].[order_items] CHECK CONSTRAINT [CK_order_items_amounts_non_negative]
GO
ALTER TABLE [dbo].[order_items]  WITH CHECK ADD  CONSTRAINT [CK_order_items_qty_positive] CHECK  (([qty]>(0)))
GO
ALTER TABLE [dbo].[order_items] CHECK CONSTRAINT [CK_order_items_qty_positive]
GO
ALTER TABLE [dbo].[orders]  WITH CHECK ADD CHECK  ((isjson([ship_address_json])=(1)))
GO
ALTER TABLE [dbo].[payment_splits]  WITH CHECK ADD  CONSTRAINT [CK_payment_splits_non_negative] CHECK  (([seller_amount]>=(0) AND [platform_commission]>=(0)))
GO
ALTER TABLE [dbo].[payment_splits] CHECK CONSTRAINT [CK_payment_splits_non_negative]
GO
ALTER TABLE [dbo].[payments]  WITH CHECK ADD  CONSTRAINT [CK_payments_non_negative] CHECK  (([gross]>=(0) AND [fee_platform]>=(0) AND [fee_psp]>=(0) AND [net_to_sellers]>=(0)))
GO
ALTER TABLE [dbo].[payments] CHECK CONSTRAINT [CK_payments_non_negative]
GO
ALTER TABLE [dbo].[sellers]  WITH CHECK ADD  CONSTRAINT [CK_sellers_commission_rate_0_100] CHECK  (([commission_rate]>=(0) AND [commission_rate]<=(100)))
GO
ALTER TABLE [dbo].[sellers] CHECK CONSTRAINT [CK_sellers_commission_rate_0_100]
GO
