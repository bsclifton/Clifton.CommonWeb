CREATE PROCEDURE [dbo].[spAccountSessionBegin]
	@accountId UNIQUEIDENTIFIER,
	@clientId NVARCHAR (50),
	@sessionId UNIQUEIDENTIFIER OUTPUT,
	@expires DATETIME OUTPUT
AS
BEGIN
	SELECT @sessionId = NEWID(), @expires = DATEADD(MONTH, 24, GETUTCDATE())

	UPDATE accountSession
	SET ended = GETDATE()
	WHERE accountId = @accountId
	AND clientId = @clientId
	AND ended IS NULL

	INSERT INTO accountSession (id, accountId, clientId, expires)
	VALUES (@sessionId, @accountId, @clientId, @expires)
END