CREATE TABLE [dbo].[AccountProjectionCommitment]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[AccountProjectionId] BIGINT NOT NULL constraint FK_AccountProjectionCommitment__AccountProjection FOREIGN KEY REFERENCES AccountProjection on Id,


)
