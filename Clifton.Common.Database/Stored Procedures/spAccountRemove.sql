CREATE PROCEDURE [dbo].[spAccountRemove]
	@accountId UNIQUEIDENTIFIER
AS
BEGIN
	UPDATE accountSession
	SET ended = GETDATE()
	WHERE accountId = @accountId
	AND ended IS NULL

	UPDATE accountConfirmation
	SET confirmed = GETDATE(),
		clientId = 'system'
	WHERE accountId = @accountId
	AND confirmed IS NULL

	EXEC spAccountStatusUpdate @accountId, 'removed'
END