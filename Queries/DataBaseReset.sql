Drop table Projects;
drop table ProjectOwners;

CREATE TABLE [dbo].[Projects] (
    [GUID]                      UNIQUEIDENTIFIER NOT NULL,
    [Name]                      NVARCHAR (256)   NULL,    
    [OwnerGuid]                 UNIQUEIDENTIFIER NULL,
    [StartDate]                 DATETIME NULL,
    [FinishDate]                DATETIME NULL
);

CREATE TABLE [dbo].[ProjectOwners] (
[Guid] UNIQUEIDENTIFIER NOT NULL,   
[Name] NVARCHAR (256)   NULL
);
