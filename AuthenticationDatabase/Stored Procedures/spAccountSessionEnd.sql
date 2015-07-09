CREATE PROCEDURE [dbo].[spAccountSessionEnd]
	@sessionId UNIQUEIDENTIFIER
AS
BEGIN
	UPDATE accountSession
	SET ended = GETDATE()
	WHERE id = @sessionId
END