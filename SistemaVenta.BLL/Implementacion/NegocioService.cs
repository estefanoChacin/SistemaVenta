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
    public class NegocioService : INegocioService
    {
        private readonly IGenericRepository<Negocio> _repository;
        private readonly IFireBaseServices _firebaseServices;




        public NegocioService(IGenericRepository<Negocio> repository, IFireBaseServices firebaseServices)
        {
            _repository = repository;
            _firebaseServices = firebaseServices;
        }




        public async Task<Negocio> GuardarCambios(Negocio negocio, Stream logo = null, string NombreLogo = "")
        {
            try {
                Negocio negocioEncontrado = await _repository.Obtener(n => n.IdNegocio == 1);

                negocioEncontrado.NumeroDocumento = negocio.NumeroDocumento;
                negocioEncontrado.Nombre = negocio.Nombre;
                negocioEncontrado.Correo= negocio.Correo;
                negocioEncontrado.Direccion = negocio.Direccion;
                negocioEncontrado.Telefono= negocio.Telefono;
                negocioEncontrado.PorcentajeImpuesto = negocio.PorcentajeImpuesto;
                negocioEncontrado.SimboloMoneda = negocio.SimboloMoneda;

                negocioEncontrado.NombreLogo = negocioEncontrado.NombreLogo == "" ? NombreLogo : negocioEncontrado.NombreLogo;

                if (logo != null) 
                {
                    string urlLogo = await _firebaseServices.SubirStorage(logo, "carpeta_logo", negocioEncontrado.NombreLogo);
                    negocioEncontrado.UrlLogo= urlLogo;
                }

                await _repository.Editar(negocioEncontrado);
                return negocioEncontrado;
            } catch {
                 throw;
            }
        }

        public async Task<Negocio> Obtener()
        {
            try
            {
                Negocio negocioEncontrado = await _repository.Obtener(n => n.IdNegocio == 1);
                return negocioEncontrado;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
