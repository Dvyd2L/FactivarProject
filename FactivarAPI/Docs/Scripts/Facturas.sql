CREATE TABLE [dbo].[Factura] (
    [FacturaID]            INT            IDENTITY (1, 1) NOT NULL,
    [NumeroFactura]        INT            NOT NULL,
    [PendientePago]        BIT            NOT NULL,
    [DescripcionOperacion] NVARCHAR (500) NOT NULL,
    [FechaExpedicion]      DATE           NOT NULL,
    [FechaCobro]           DATE           NULL,
    [ClienteId]            CHAR (9)       NOT NULL,
    [ProveedorId]          CHAR (9)       NOT NULL,
    [Articulos]            VARCHAR (MAX)  NOT NULL,
    PRIMARY KEY CLUSTERED ([FacturaID] ASC),
    FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Clientes] ([Cif]),
    FOREIGN KEY ([ProveedorId]) REFERENCES [dbo].[Clientes] ([Cif])
);