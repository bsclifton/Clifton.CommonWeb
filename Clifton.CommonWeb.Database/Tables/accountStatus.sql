CREATE TABLE [dbo].[accountStatus] (
    [id]      INT NOT NULL,
    [name]    NVARCHAR (50)    NOT NULL,
	[description]    NVARCHAR (50)    NOT NULL,
    [created] DATETIME         NOT NULL DEFAULT (GETDATE()), 
    CONSTRAINT [PK_accountStatus] PRIMARY KEY ([id]),
);