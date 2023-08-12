using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.Entity;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DALL.Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace SistemaVenta.BLL.Implementacion
{
    public  class ProductoService: IProductoService
    {
        private readonly IGenericRepository<Producto> _repository;
        private readonly IFireBaseServices _FireBaseServices;
        private readonly IUtilidadesServices _UtilidadesServices;

        public ProductoService(IGenericRepository<Producto> repository, IFireBaseServices fireBaseServices, IUtilidadesServices utilidadesServices)
        {
            _repository = repository;
            _FireBaseServices = fireBaseServices;
        }



        public async Task<List<Producto>> Lista()
        {
            IQueryable<Producto> query = await _repository.Consultar();
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }



        public async Task<Producto> Crear(Producto entidad, Stream imagen = null, string NombreIMagen = "")
        {
            Producto producto_existe = await _repository.Obtener(p => p.CodigoBarra == entidad.CodigoBarra);
            if (producto_existe != null) 
            { 
                throw new TaskCanceledException("El codigo de barra ya existe");
            }
            try
            {
                entidad.NombreImagen = NombreIMagen;
                if (imagen != null) 
                {
                    string urlImagen = await _FireBaseServices.SubirStorage(imagen, "carpeta_producto", NombreIMagen);
                    entidad.UrlImagen= urlImagen;
                }
                Producto producto_creado = await _repository.Crear(entidad);

                if (producto_creado.IdProducto == 0)
                {
                    throw new TaskCanceledException("No se pudo crar el producto");
                }
                IQueryable<Producto> query = await _repository.Consultar(p => p.IdProducto == producto_creado.IdProducto);
                producto_creado = query.Include(c=> c.IdCategoriaNavigation).First();

                return producto_creado;
            }
            catch (Exception)
            {
                throw;
            }
        }




        public async Task<Producto> Editar(Producto entidad, Stream imagen = null)
        {
            Producto producto_Existe = await _repository.Obtener(p => p.CodigoBarra == entidad.CodigoBarra && p.IdProducto != entidad.IdProducto);
            if (producto_Existe != null) 
            {
                throw new TaskCanceledException("El codigo de barras ya existe");
            }
            try
            {
                IQueryable<Producto> queryProducto = await _repository.Consultar(p => p.IdProducto == entidad.IdProducto);
                Producto productoEditar = queryProducto.First();

                productoEditar.CodigoBarra= entidad.CodigoBarra;
                productoEditar.Marca= entidad.Marca;
                productoEditar.Descripcion = entidad.Descripcion;
                productoEditar.IdCategoria= entidad.IdCategoria;
                productoEditar.Stock= entidad.Stock;

                if (imagen != null) 
                {
                    string urlImagen = await _FireBaseServices.SubirStorage(imagen, "carpeta_producto", productoEditar.NombreImagen);
                    productoEditar.UrlImagen= urlImagen;
                }

                bool respuesta = await _repository.Editar(productoEditar);
                if (!respuesta) 
                { 
                    throw new TaskCanceledException("No se pudo editar el producto");
                }
                Producto productoEditado = queryProducto.Include(c=> c.IdCategoriaNavigation).First();

                return productoEditado;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<bool> Eliminar(int id)
        {
            try
            {
                Producto productoEncontrado = await _repository.Obtener(e => e.IdProducto == id);
                if (productoEncontrado == null) 
                { 
                    throw new TaskCanceledException("El producto no existe");
                }
                string nombreImagen = productoEncontrado.NombreImagen;

                bool respuesta = await  _repository.Eliminar(productoEncontrado);
                if (respuesta)
                {
                    await _FireBaseServices.EliminarStorage("carpeta_producto", nombreImagen);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
