CREATE TABLE [dbo].[accountStatusHistory] (
    [id]      UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL DEFAULT (NEWID()),
    [accountId]    UNIQUEIDENTIFIER    NOT NULL,
	[accountStatusId] INT NOT NULL,
    [from] DATETIME         NOT NULL DEFAULT (GETDATE()), 
	[to] DATETIME         NULL, 
    CONSTRAINT [PK_accountStatusHistory] PRIMARY KEY ([id]), 
    CONSTRAINT [FK_accountStatusHistory_AccountId] FOREIGN KEY ([accountId]) REFERENCES [account]([id]), 
    CONSTRAINT [FK_accountStatusHistory_AccountStatusId] FOREIGN KEY ([accountStatusId]) REFERENCES [accountStatus]([id]) 
);