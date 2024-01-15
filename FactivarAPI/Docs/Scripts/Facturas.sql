CREATE TABLE [dbo].[Factura] (
    [NumeroFactura]        INT            NOT NULL,
    [Importe]              DECIMAL (9, 2) NOT NULL,
    [IVA]                  DECIMAL (9, 2) NOT NULL,
    [Total]                DECIMAL (9, 2) NOT NULL,
    [PendientePago]        BIT            NOT NULL,
    [DescripcionOperacion] NVARCHAR (500) NOT NULL,
    [FechaExpedicion]      DATE           NOT NULL,
    [FechaCobro]           DATE           NULL,
    [ClienteId]            CHAR (9)       NOT NULL,
    [Articulos]            VARCHAR (MAX)  NOT NULL,
    PRIMARY KEY CLUSTERED ([NumeroFactura] ASC),
    FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Clientes] ([Cif])
);