using DTOs.FactivarAPI;

namespace Services;

public class CalculoIvaService(IServiceProvider serviceProvider)
{
    #region PROPIEDADES
    public readonly IServiceProvider _serviceProvider = serviceProvider;
    #endregion

    #region METODOS
    public IEnumerable<DTOIvas> CalculoIVA(List<DTOArticulo> articulos)
    {
        DTOIvas[] ivas = new DTOIvas[5]; // 0 - 4 - 5 - 10 - 21
        for (int i = 0; i < ivas.Length; ++i)
        {
            ivas[i] = new DTOIvas();
        }

        foreach (DTOArticulo articulo in articulos)
        {
            switch (articulo.Iva)
            {
                case IVA.CERO:
                    ivas[0].BImponible += articulo.BImponible;
                    ivas[0].Cuota += articulo.CuotaIva;
                    break;
                case IVA.SUPERREDUCIDO:
                    ivas[1].BImponible += articulo.BImponible;
                    ivas[1].Cuota += articulo.CuotaIva;
                    break;
                case IVA.ESPECIAL:
                    ivas[2].BImponible += articulo.BImponible;
                    ivas[2].Cuota += articulo.CuotaIva;
                    break;
                case IVA.REDUCIDO:
                    ivas[3].BImponible += articulo.BImponible;
                    ivas[3].Cuota += articulo.CuotaIva;
                    break;
                case IVA.GENERAL:
                    ivas[4].BImponible += articulo.BImponible;
                    ivas[4].Cuota += articulo.CuotaIva;
                    break;
            }
        }

        return ivas;
    }

    //public Dictionary<IVA, DTOIvas> DesgloseIVA(List<DTOArticulo> articulos)
    public Dictionary<string, DTOIvas> DesgloseIVA(List<DTOArticulo> articulos)
    {
        //Dictionary<IVA, DTOIvas> desgloseIva = new()
        Dictionary<string, DTOIvas> desgloseIva = new()
        {
            { IVA.CERO.ToString().ToLower(), new DTOIvas() },
            { IVA.SUPERREDUCIDO.ToString().ToLower(), new DTOIvas() },
            { IVA.ESPECIAL.ToString().ToLower(), new DTOIvas() },
            { IVA.REDUCIDO.ToString().ToLower(), new DTOIvas() },
            { IVA.GENERAL.ToString().ToLower(), new DTOIvas() }
        };

        foreach (DTOArticulo articulo in articulos)
        {
            desgloseIva[articulo.Iva.ToString().ToLower()].BImponible += articulo.BImponible;
            desgloseIva[articulo.Iva.ToString().ToLower()].Cuota += articulo.CuotaIva;
        }

        return desgloseIva;
    }
    #endregion
}
