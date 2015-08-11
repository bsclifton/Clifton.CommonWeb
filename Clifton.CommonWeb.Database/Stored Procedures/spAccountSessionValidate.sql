CREATE PROCEDURE [dbo].[spAccountSessionValidate]
	@accountId UNIQUEIDENTIFIER,
	@clientId NVARCHAR (50),
	@sessionId UNIQUEIDENTIFIER,
	@expires DATETIME OUTPUT
AS
BEGIN
	SELECT @expires = expires
	FROM accountSession
	WHERE accountId = @accountId
	AND clientId = @clientId
	AND id = @sessionId
	AND ended IS NULL
END