using Helpers.Enums;

namespace Services.Interfaces;
public interface IRastroService
{
    Task<int> AddRastro(string usuario, EnumTipoProcesoRastro proceso, EnumTipoAccionRastro operacion, string observaciones);
}
