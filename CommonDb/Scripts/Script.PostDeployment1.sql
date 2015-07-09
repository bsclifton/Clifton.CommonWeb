DECLARE @accountStatusRowCount INT

SELECT @accountStatusRowCount = COUNT(*)
FROM [dbo].[accountStatus]

IF @accountStatusRowCount = 0
BEGIN
	INSERT INTO [dbo].[accountStatus] (id, name, [description])
	VALUES
	(1, 'pending_activate', 'Pending email confirmation'),
	(2, 'active', 'Active'),
	(3, 'banned', 'Banned'),
	(4, 'removed', 'Removed')
END