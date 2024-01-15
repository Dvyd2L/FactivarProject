using DTOs.FactivarAPI;
using iText.Layout.Element;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CalculoIvaService
    {
        #region PROPIEDADES
        public readonly IServiceProvider _serviceProvider;
        #endregion

        #region CONSTRUCTOR
        public CalculoIvaService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        #endregion

        #region METODOS
        public IEnumerable<DTOIvas> CalculoIVA(List<DTOArticulo> articulos)
        {
            var ivas = new DTOIvas[5]; // 0 - 4 - 5 - 10 - 21
            for(int i = 0; i < ivas.Length; ++i) { ivas[i] = new DTOIvas(); }

            foreach (var articulo in articulos)
            {
                switch (articulo.Iva) {
                    case 0:
                        ivas[0].BImponible += articulo.BImponible;
                        ivas[0].Cuota += articulo.CuotaIva;
                        break;
                    case 4:
                        ivas[1].BImponible += articulo.BImponible;
                        ivas[1].Cuota += articulo.CuotaIva;
                        break;
                    case 5:
                        ivas[2].BImponible += articulo.BImponible;
                        ivas[2].Cuota += articulo.CuotaIva;
                        break;
                    case 10:
                        ivas[3].BImponible += articulo.BImponible;
                        ivas[3].Cuota += articulo.CuotaIva;
                        break;
                    case 21:
                        ivas[4].BImponible += articulo.BImponible;
                        ivas[4].Cuota += articulo.CuotaIva;
                        break;
                }
            }

            return ivas;
        }
        #endregion
    }
}
