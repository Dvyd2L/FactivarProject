-- La cláusula WHERE EnlaceCambioPass IS NOT NULL en la creación del índice es necesaria. 
-- Esto se debe a que queremos que SQL Server ignore los valores NULL al considerar la unicidad del índice.
-- Queremos que el índice se aplique a todas las filas donde EnlaceCambioPass no es NULL, y que permita múltiples NULL

-- Paso 1: Crear la tabla de datos personales
CREATE TABLE [dbo].[DatosPersonales] (
    [Id]        UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Nombre]    NVARCHAR (25)    NOT NULL,
    [Apellidos] NVARCHAR (100)   NOT NULL,
    [Telefono]  CHAR (15)        NULL,
	[AvatarUrl] NVARCHAR (MAX) NULL,
    [Email]     NVARCHAR (100)   NOT NULL,

    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);

-- Paso 2: Crear la tabla de credenciales
CREATE TABLE [dbo].[Credenciales] (
    [IdUsuario]        UNIQUEIDENTIFIER NOT NULL,
    [Password]         NVARCHAR (500)   NOT NULL,
    [Salt]             VARBINARY (MAX)  NULL,
    [EnlaceCambioPass] NVARCHAR (50)    NULL,
    [FechaEnvioEnlace] DATETIME         NULL,

    PRIMARY KEY CLUSTERED ([IdUsuario] ASC),
    FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[DatosPersonales] ([Id])
);

-- Paso 3: Crear el índice
CREATE UNIQUE INDEX idx_unico
ON Credenciales (EnlaceCambioPass)
WHERE EnlaceCambioPass IS NOT NULL;