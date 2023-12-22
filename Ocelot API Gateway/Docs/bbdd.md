```json
{
	"articulos": {
		"datosBasicos": {
			"codigoBarras": 0123456789,
			"referencia": Ref,
			"nombre": "",
			"familia": ["f1", "f2", "f3"],
			"proveedor": Proveedor,
			"ubicacion": "",
			"fechaAlta": "01-01-2023"
		},
		"datosEconomicos": {
			"coste": decimal,
			"margenBeneficio": [10, 20, 30],
			"iva": [4, 10, 21]
			//"incremento": autocalculado a aprtir de coste * margen,
		},
		"imagen": "https://www.foo.com/api/v1/articulo/img/165618",
		"datosGestion": {
			"stock": {
				"actual": 0,
				"minimo": 0
			},
			"unidadesCaja": 0
		}
	}
}
```

clientes = {
}
proveedores = {
}
agentes = {
}