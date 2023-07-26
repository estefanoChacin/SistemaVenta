using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DALL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    internal class CategoriaService : ICategoriaServices
    {
        private readonly IGenericRepository<Categoria> _repository;



        public CategoriaService(IGenericRepository<Categoria> repository)
        {
            _repository = repository;
        }




        public async Task<List<Categoria>> Lista()
        {
            var query = await _repository.Consultar();
            return query.ToList();
        }




        public async Task<Categoria> Crear(Categoria entidad)
        {
            try
            {
                Categoria categoriaEncontrada = await _repository.Crear(entidad);
                if(categoriaEncontrada.IdCategoria == 0)
                    throw new TaskCanceledException("No se pudo crear la categoria.");

                return categoriaEncontrada;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<Categoria> Editar(Categoria entidad)
        {
            try
            {
                Categoria categoriaEncontrada = await _repository.Obtener(c => c.IdCategoria == entidad.IdCategoria);
                categoriaEncontrada.Descripcion= entidad.Descripcion;
                categoriaEncontrada.EsActivo = entidad.EsActivo;

                bool respuesta = await _repository.Editar(entidad);

                if(!respuesta)
                    throw new TaskCanceledException("No se pudo editar la categoria");

                return categoriaEncontrada;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<bool> Eliminar(int idCategoria)
        {
            try
            {
                Categoria categoriaEncontrada = await _repository.Obtener(c => c.IdCategoria == idCategoria);

                if (categoriaEncontrada == null)
                    throw new TaskCanceledException("La categoria no existe");

                bool respuesta = await _repository.Eliminar(categoriaEncontrada);
                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
