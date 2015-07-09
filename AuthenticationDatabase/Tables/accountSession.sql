CREATE TABLE [dbo].[accountSession] (
    [id]      UNIQUEIDENTIFIER DEFAULT (NEWID()) ROWGUIDCOL NOT NULL,
	[accountId]    UNIQUEIDENTIFIER    NOT NULL,
	[clientId]    NVARCHAR (50)   NOT NULL, 
    [created] DATETIME         NOT NULL DEFAULT (GETDATE()),
	[ended] DATETIME         NULL,
	[expires] DATETIME         NOT NULL,
    CONSTRAINT [PK_accountSession] PRIMARY KEY ([id]),
	CONSTRAINT [FK_accountSession_AccountId] FOREIGN KEY ([accountId]) REFERENCES [account]([id])
);