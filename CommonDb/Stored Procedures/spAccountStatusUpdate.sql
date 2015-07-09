CREATE PROCEDURE [dbo].[spAccountStatusUpdate]
	@accountId UNIQUEIDENTIFIER,
	@accountStatus NVARCHAR(50)
AS
BEGIN
	DECLARE @accountStatusId INT

	SELECT @accountStatusId = id
	FROM accountStatus
	WHERE name = @accountStatus

	IF (@accountStatusId IS NOT NULL)
	BEGIN
		DECLARE @currentStatusId INT

		SELECT @currentStatusId = accountStatusId
		FROM account
		WHERE id = @accountId

		IF (@currentStatusId <> @accountStatusId) OR (@currentStatusId IS NULL AND @accountStatusId IS NOT NULL)
		BEGIN
			UPDATE accountStatusHistory
			SET [to] = GETDATE()
			WHERE accountId = @accountId

			INSERT INTO accountStatusHistory (accountId, accountStatusId)
			VALUES (@accountId, @accountStatusId)

			UPDATE account
			SET accountStatusId = @accountStatusId
			WHERE id = @accountId
		END
	END
END
