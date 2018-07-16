CREATE TABLE [dbo].[AccountProjectionCommitment]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1,1),
	[AccountProjectionId] BIGINT NOT NULL ,
	[CommitmentId] BIGINT NOT NULL 
)
GO
CREATE NONCLUSTERED INDEX [idx_accountprojection_commitment] ON [dbo].[AccountProjectionCommitment] (AccountProjectionId) INCLUDE(CommitmentId) WITH(ONLINE = ON)
GO