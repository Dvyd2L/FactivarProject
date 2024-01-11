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
    [Roles_IdRol]      INT              NOT NULL,
    [RefreshToken]     NVARCHAR (MAX)   NULL,
    [EnlaceCambioPass] NVARCHAR (50)    NULL,
    [FechaEnvioEnlace] DATETIME         NULL,

    PRIMARY KEY CLUSTERED ([IdUsuario] ASC),
    FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[DatosPersonales] ([Id]),
	CONSTRAINT [FK_Usuarios_Roles] FOREIGN KEY ([Roles_IdRol]) REFERENCES [dbo].[Roles] ([IdRol])
);

-- Paso 3: Crear la tabla de roles
CREATE TABLE [dbo].[Roles] (
    [IdRol]       INT           NOT NULL,
    [Descripcion] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([IdRol] ASC)
);

INSERT INTO [dbo].[Roles] ([IdRol], [Descripcion]) VALUES (1, N'Admin')
INSERT INTO [dbo].[Roles] ([IdRol], [Descripcion]) VALUES (2, N'User')

-- Paso 4: Creamos la tabla de logs
CREATE TABLE [dbo].[Registros] (
    [IdRegistro]         UNIQUEIDENTIFIER NOT NULL,
    [FechaAccion]        DATETIME         NOT NULL,
    [Proceso]            NVARCHAR (50)    NOT NULL,
    [Operacion]          NVARCHAR (50)    NOT NULL,
    [Observaciones]      NVARCHAR (150)   NULL,
    [Usuarios_IdUsuario] UNIQUEIDENTIFIER NULL,
    [Ip]                 NVARCHAR (50)    NULL,
    PRIMARY KEY CLUSTERED ([IdRegistro] ASC)
);

-- Paso 5: Crear el índice
CREATE UNIQUE INDEX idx_unico
ON Credenciales (EnlaceCambioPass)
WHERE EnlaceCambioPass IS NOT NULL;