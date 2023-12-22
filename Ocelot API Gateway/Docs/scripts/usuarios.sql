CREATE TABLE [dbo].[Usuarios] (
    [Id]               UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Email]            NVARCHAR (100)   NOT NULL,
    [Password]         NVARCHAR (500)   NOT NULL,
    [Salt]             VARBINARY (MAX)  NULL,
    [EnlaceCambioPass] NVARCHAR (50)    NULL,
    [FechaEnvioEnlace] DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE ([Email]),
    UNIQUE ([EnlaceCambioPass])
);

CREATE NONCLUSTERED INDEX IX_Usuarios_Email
ON [dbo].[Usuarios] (Email);