CREATE TABLE [dbo].[account] (
    [id]      UNIQUEIDENTIFIER DEFAULT (NEWID()) ROWGUIDCOL NOT NULL,
    [name]    NVARCHAR (255)   NOT NULL,
    [email]   NVARCHAR (255)   NOT NULL,
	[hash]   NVARCHAR (100)   NOT NULL,
    [created] DATETIME         NOT NULL DEFAULT (GETDATE()),
	[accountStatusId] INT  NULL,
    CONSTRAINT [PK_account] PRIMARY KEY CLUSTERED ([id] ASC), 
	CONSTRAINT [UNQ_account_hash] UNIQUE ([hash]),
    CONSTRAINT [FK_account_AccountStatus] FOREIGN KEY ([accountStatusId]) REFERENCES [accountStatus]([id])
);