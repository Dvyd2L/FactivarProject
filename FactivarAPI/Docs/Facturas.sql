CREATE TABLE [dbo].[Factura]
(
	[NumeroFactura] INT NOT NULL PRIMARY KEY,
	[Importe] INT NOT NULL,
	[IVA] INT NOT NULL,
	[Total] INT NOT NULL,
	[PendientePago] BIT NOT NULL,
	[DescripcionOperacion] NVARCHAR(500) NOT NULL,
	[FechaExpedicion] DATE NOT NULL,
	[FechaCobro] DATE NOT NULL,
	[ClienteId] CHAR(9) NOT NULL,

	FOREIGN KEY (ClienteId) REFERENCES Clientes(Cif)
)