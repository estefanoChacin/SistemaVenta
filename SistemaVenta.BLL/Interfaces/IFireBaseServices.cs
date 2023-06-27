using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IFireBaseServices
    {
        Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, String NombreA);

        Task<bool> EliminarStorage(string CarpetaDestino, String NombreA);

    }
}
