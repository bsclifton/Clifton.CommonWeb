CREATE PROCEDURE [dbo].[spAccountConfirm]
	@accountId UNIQUEIDENTIFIER,
	@code UNIQUEIDENTIFIER,
	@clientId NVARCHAR (50)
AS
BEGIN
	DECLARE @accountConfirmationId UNIQUEIDENTIFIER

	SELECT @accountConfirmationId = id
	FROM accountConfirmation
	WHERE id = @code
	AND accountId = @accountId
	AND confirmed IS NULL

	IF (@accountConfirmationId IS NOT NULL)
	BEGIN
		UPDATE accountConfirmation
		SET confirmed = GETDATE(),
			clientId = @clientId
		WHERE id = @accountConfirmationId

		EXEC spAccountStatusUpdate @accountId, 'active'
	END
END