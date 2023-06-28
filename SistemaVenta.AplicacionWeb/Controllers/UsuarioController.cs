using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;



namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class UsuarioController : Controller
    {
        
        private readonly IUsuarioServices _usuarioServices;
        private readonly IRolServices _rolServices;
        private readonly IMapper _mapper;




        #region constructor
        public UsuarioController(IUsuarioServices usuarioServices, IRolServices rolServices, IMapper mapper)
        {
            _usuarioServices = usuarioServices;
            _rolServices = rolServices;
            _mapper = mapper;
        }
        #endregion



        #region index
        public IActionResult Index()
        {
            return View();
        }
        #endregion



        #region listar los roles
        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(await _rolServices.Lista());
            return StatusCode(StatusCodes.Status200OK, vmListaRoles);
        }
        #endregion



        #region listar 
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMUsuario> vmListaUsuario = _mapper.Map<List<VMUsuario>>(await _usuarioServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmListaUsuario });
        }
        #endregion



        #region Crear 
        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
                string nombreFoto = "";
                Stream fotoStream = null;
                if (foto != null)
                {
                    string nombreCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombreCodigo, extension);
                    fotoStream = foto.OpenReadStream();
                }
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
                Usuario usuarioCreado = await _usuarioServices.Crear(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);
                vmUsuario = _mapper.Map<VMUsuario>(usuarioCreado);

                gResponse.Estado = true;
                gResponse.Objecto = vmUsuario;
            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }
        #endregion



        #region Editar 
        [HttpPost]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
                string nombreFoto = "";
                Stream fotoStream = null;
                if (foto != null)
                {
                    string nombreCodigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombreCodigo, extension);
                    fotoStream = foto.OpenReadStream();
                }
                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
                Usuario usuarioEditado = await _usuarioServices.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);
                vmUsuario = _mapper.Map<VMUsuario>(usuarioEditado);

                gResponse.Estado = true;
                gResponse.Objecto = vmUsuario;
            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }
        #endregion



        #region Eliminar
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _usuarioServices.Eliminar(idUsuario);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
        #endregion

    }
}
