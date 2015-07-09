CREATE PROCEDURE [dbo].[spAccountCreate]
	@name NVARCHAR (255),
	@email NVARCHAR (255),
	@hash NVARCHAR (100),
	@accountId UNIQUEIDENTIFIER OUTPUT,
	@code UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
	SELECT @accountId = NEWID(), @code = NEWID()

	INSERT INTO account (id, name, email, [hash])
	VALUES (@accountId, @name, @email, @hash)

	INSERT INTO accountConfirmation (id, accountId)
	VALUES (@code, @accountId)

	EXEC spAccountStatusUpdate @accountId, 'pending_activate'
END