CREATE TABLE [dbo].[accountConfirmation] (
    [id]      UNIQUEIDENTIFIER DEFAULT (NEWID()) ROWGUIDCOL NOT NULL,
	[accountId]    UNIQUEIDENTIFIER    NOT NULL,
    [created] DATETIME         NOT NULL DEFAULT (GETDATE()),
	[confirmed] DATETIME         NULL,
	[clientId]    NVARCHAR (50)   NULL, 
    CONSTRAINT [PK_accountConfirmation] PRIMARY KEY ([id]),
	CONSTRAINT [FK_accountConfirmation_AccountId] FOREIGN KEY ([accountId]) REFERENCES [account]([id])
);