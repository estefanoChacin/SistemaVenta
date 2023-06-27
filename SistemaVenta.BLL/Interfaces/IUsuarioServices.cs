using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IUsuarioServices
    {
        Task<List<Usuario>> Lista();
        Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "");
        Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "");
        Task<bool> Eliminar(int IdUsuario);
        Task<Usuario> ObtenerPorCredenciales(string correo, string clave);
        Task<Usuario> ObtenerPorID(int idUsuario);
        Task<bool> GuardarPerfil(Usuario Usuario);
        Task<bool> CambiarClave(Usuario Usuario, string calveActual, string claveNueva);
        Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo);
    }
}
