using Microsoft.AspNetCore.Mvc;
using SistemaVenta.Entity;
using SistemaVenta.BLL.Interfaces;
using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.DALL.Implemtancion;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using Newtonsoft.Json;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductoService _productoService;

        public ProductosController(IMapper mapper, IProductoService productoService, ICategoriaServices categoriesService)
        {
            _mapper = mapper;
            _productoService = productoService;
        }



        public IActionResult Index()
        {
            return View();
        }





        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMProducto> vmProductoLista = _mapper.Map<List<VMProducto>>(await _productoService.Lista());
            return View(vmProductoLista);
        }




        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile imagen, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();
            try
            {
                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);

                string nombreImagen = "";
                Stream imagenStream = null;

                if (imagen != null) 
                { 
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extesion = Path.GetExtension(imagen.FileName);
                    nombreImagen = string.Concat(nombre_en_codigo, extesion);
                    imagenStream = imagen.OpenReadStream();
                }
                Producto productoCreado = await _productoService.Crear(_mapper.Map<Producto>(vmProducto), imagenStream, nombreImagen);
                vmProducto = _mapper.Map<VMProducto>(productoCreado);

                gResponse.Estado = true;
                gResponse.Objecto = vmProducto;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }




        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile imagen, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();
            try
            {
                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);
                Stream imagenStream = null;

                if (imagen != null)
                {
                    imagenStream = imagen.OpenReadStream();
                }
                Producto productoEditado = await _productoService.Editar(_mapper.Map<Producto>(vmProducto), imagenStream);
                vmProducto = _mapper.Map<VMProducto>(productoEditado);

                gResponse.Estado = true;
                gResponse.Objecto = vmProducto;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }






        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdProducto) 
        { 
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _productoService.Eliminar(IdProducto);
            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }


    }
}
