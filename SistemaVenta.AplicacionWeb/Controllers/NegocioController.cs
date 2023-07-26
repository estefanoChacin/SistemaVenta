using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Implementacion;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DALL.Implemtancion;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class NegocioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;


        public NegocioController(IMapper mapper, INegocioService negocioService)
        {
            _mapper= mapper;
            _negocioService= negocioService;
        }



        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Obtener() 
        {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();
            try
            {
                VMNegocio vmNegocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());
                gResponse.Estado = true;
                gResponse.Objecto = vmNegocio;

            }
            catch (Exception e)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = e.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }



        [HttpPost]
        public async Task<IActionResult> GuardarCambios([FromForm]IFormFile logo, [FromForm] string modelo)
                 {
            GenericResponse<VMNegocio> gResponse = new GenericResponse<VMNegocio>();
            try
            {
                VMNegocio vmNegocio = JsonConvert.DeserializeObject<VMNegocio>(modelo);

                string nombreLogo = "";
                Stream logoStream = null;

                if (logo != null)
                {
                    string nombreCodigo = Guid.NewGuid().ToString("N");
                    string extesion = Path.GetExtension(logo.FileName);
                    nombreLogo = string.Concat(nombreCodigo, extesion);
                    logoStream = logo.OpenReadStream();
                }
                Negocio negocioEditado = await _negocioService.GuardarCambios(_mapper.Map<Negocio>(vmNegocio), logoStream, nombreLogo);
                vmNegocio = _mapper.Map<VMNegocio>(negocioEditado);

                gResponse.Estado = true;
                gResponse.Objecto = vmNegocio;

            }
            catch (Exception e)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = e.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
