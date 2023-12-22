using Newtonsoft.Json;

namespace Ocelot_API_Gateway.Docs;

public partial class IArticulo
{
    [JsonProperty("datosBasicos")]
    public required DatosBasicos DatosBasicos { get; set; }

    [JsonProperty("datosEconomicos")]
    public required DatosEconomicos DatosEconomicos { get; set; }

    [JsonProperty("imagen")]
    public required Uri Imagen { get; set; }

    [JsonProperty("datosGestion")]
    public required DatosGestion DatosGestion { get; set; }
}

public partial class DatosBasicos
{
    [JsonProperty("codigoBarras")]
    public required string CodigoBarras { get; set; }

    [JsonProperty("referencia")]
    public required string Referencia { get; set; }

    [JsonProperty("nombre")]
    public required string Nombre { get; set; }

    [JsonProperty("familia")]
    public required string[] Familia { get; set; }

    [JsonProperty("proveedor")]
    public required string Proveedor { get; set; }

    [JsonProperty("ubicacion")]
    public required string Ubicacion { get; set; }

    [JsonProperty("fechaAlta")]
    public required string FechaAlta { get; set; }
}

public partial class DatosEconomicos
{
    [JsonProperty("coste")]
    public long Coste { get; set; }

    [JsonProperty("margenBeneficio")]
    public required long[] MargenBeneficio { get; set; }

    [JsonProperty("iva")]
    public required long[] Iva { get; set; }
}

public partial class DatosGestion
{
    [JsonProperty("stock")]
    public required Stock Stock { get; set; }

    [JsonProperty("unidadesCaja")]
    public long UnidadesCaja { get; set; }
}

public partial class Stock
{
    [JsonProperty("actual")]
    public long Actual { get; set; }

    [JsonProperty("minimo")]
    public long Minimo { get; set; }
}
