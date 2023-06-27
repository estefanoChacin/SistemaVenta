using SistemaVenta.DALL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DALL.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace SistemaVenta.DALL.Implemtancion
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbventaContext _dbcontext;
        public GenericRepository(DbventaContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


        public async Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro)
        {
            try
            {
                TEntity entidad = await _dbcontext.Set<TEntity>().FirstOrDefaultAsync(filtro);
                return entidad;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<TEntity> Crear(TEntity entidad)
        {
            try
            {
                 _dbcontext.AddAsync(entidad);
                await _dbcontext.SaveChangesAsync();
                return entidad;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<bool> Editar(TEntity entidad)
        {
            try
            {
                _dbcontext.Update(entidad);
                await (_dbcontext.SaveChangesAsync());
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<bool> Eliminar(TEntity entidad)
        {
            try
            {
                _dbcontext.Remove(entidad);
                await (_dbcontext.SaveChangesAsync());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public async Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null)
        {
            IQueryable<TEntity> queryEntidad = filtro == null? _dbcontext.Set<TEntity>(): _dbcontext.Set<TEntity>().Where(filtro);
            return queryEntidad;
        }
    }
}
