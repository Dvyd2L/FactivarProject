CREATE TABLE [dbo].[Clientes] (
    [Cif]       CHAR (9)      NOT NULL,
    [Nombre]    NVARCHAR (30) NOT NULL,
    [Direccion] NVARCHAR (50) NOT NULL,
    [Telefono]  INT           NOT NULL,
    [Email]     NVARCHAR (50) NOT NULL,
    [FechaAlta] DATE          NOT NULL,
    [Eliminado] BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Cif] ASC)
);