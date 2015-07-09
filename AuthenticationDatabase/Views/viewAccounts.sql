CREATE VIEW [dbo].[viewAccounts]
AS
SELECT a.*
FROM account a
JOIN accountStatus s ON a.accountStatusId = s.id
WHERE (s.name <> 'removed')