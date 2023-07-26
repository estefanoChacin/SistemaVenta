using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DALL.Interfaces;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.Entity;
using System.Net;

namespace SistemaVenta.BLL.Implementacion
{
    public class UsuarioServices : IUsuarioServices
    {
        private readonly IGenericRepository<Usuario> _repository;
        private readonly IFireBaseServices _fireBaseServices;
        private readonly IUtilidadesServices _utilidadesServices;
        private readonly ICorreoServices _correoServices;





        public UsuarioServices(IGenericRepository<Usuario> repository,
                               IFireBaseServices fireBaseServices,
                               IUtilidadesServices utilidadesServices,
                               ICorreoServices correoServices)
        {
            _repository = repository;
            _fireBaseServices = fireBaseServices;
            _utilidadesServices = utilidadesServices;
            _correoServices = correoServices;
        }





        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repository.Consultar();
            return query.Include(rol => rol.IdRolNavigation).ToList();
        }





        public async Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            Usuario usuarioExiste = await _repository.Obtener(u => u.Correo == entidad.Correo);
            if (usuarioExiste != null)
            {
                throw new TaskCanceledException("El correo ya Existe");
            }
            try
            {
                string clave_generada = _utilidadesServices.GenerarClave();
                entidad.Clave = _utilidadesServices.ConvertirSha256(clave_generada);
                entidad.NombreFoto = NombreFoto;

                if (Foto != null)
                {
                    string urlFoto = await _fireBaseServices.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                    entidad.UrlFoto = urlFoto;
                }
                Usuario usuarioCreado = await _repository.Crear(entidad);

                if (usuarioCreado.IdUsuario == 0)
                {
                    throw new TaskCanceledException("No se puede crear el usuario");
                }
                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuarioCreado.Correo).Replace("[clave]", clave_generada);
                    string htmlCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;
                            readerStream = new StreamReader(dataStream);

                            if (response.CharacterSet == null)
                            {
                                readerStream = new StreamReader(dataStream);
                            }
                            else
                            {
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                                htmlCorreo = readerStream.ReadToEnd();
                                response.Close();
                                readerStream.Close();
                            }
                        }
                    }
                    if (htmlCorreo != "")
                    {
                        await _correoServices.EnviarCorreo(usuarioCreado.Correo, "Cuenta Creada", htmlCorreo);
                    }
                }
                IQueryable<Usuario> query = await _repository.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);
                usuarioCreado = query.Include(rol => rol.IdRolNavigation).First();

                return usuarioCreado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }





        public async Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "")
        {
            Usuario usuarioExiste = await _repository.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);
            if (usuarioExiste != null)
            {
                throw new TaskCanceledException("El correo ya Existe");
            }
            try
            {
                IQueryable<Usuario> queryUsuario = await _repository.Consultar(u => u.IdUsuario == entidad.IdUsuario);
                Usuario usuario_editar = queryUsuario.First();

                usuario_editar.Nombre = entidad.Nombre;
                usuario_editar.Correo = entidad.Correo;
                usuario_editar.Telefono = entidad.Telefono;
                usuario_editar.IdRol = entidad.IdRol;
                usuario_editar.EsActivo = entidad.EsActivo;
                if (usuario_editar.NombreFoto == "")
                {
                    usuario_editar.NombreFoto = NombreFoto;
                }
                if (Foto != null)
                {
                    string urlFoto = await _fireBaseServices.SubirStorage(Foto, "carpeta_usuario", usuario_editar.NombreFoto);
                    usuario_editar.UrlFoto = urlFoto;
                }
                bool respuesta = await _repository.Editar(usuario_editar);

                if (!respuesta)
                {
                    throw new TaskCanceledException("Nose pudo modificar el usuario");
                }
                Usuario usuario_editado = queryUsuario.Include(r => r.IdRolNavigation).First();
                return usuario_editado;

            }
            catch (Exception)
            {
                throw;
            }
        }





        public async Task<bool> Eliminar(int IdUsuario)
        {
            try
            {
                Usuario usuario_Encontrado = await _repository.Obtener(u => u.IdUsuario == IdUsuario);

                if (usuario_Encontrado == null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }
                string nombreFoto = usuario_Encontrado.NombreFoto;
                bool resuesta = await _repository.Eliminar(usuario_Encontrado);

                if (resuesta)
                {
                    await _fireBaseServices.EliminarStorage("carpeta_usuario", nombreFoto);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }





        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string claveEncritada = _utilidadesServices.ConvertirSha256(clave);
            Usuario UsuarioEncontrado = await _repository.Obtener(u => u.Correo.Equals(correo) && u.Clave.Equals(claveEncritada));

            return UsuarioEncontrado;
        }





        public async Task<Usuario> ObtenerPorID(int idUsuario)
        {
            IQueryable<Usuario> usuario = await _repository.Consultar(e => e.IdUsuario == idUsuario);
            Usuario resultado = usuario.Include(rol => rol.IdUsuario).FirstOrDefault();

            return resultado;
        }





        public async Task<bool> GuardarPerfil(Usuario usuario)
        {
            try
            {
                Usuario usuarioEncontrado = await _repository.Obtener(e => e.IdUsuario == usuario.IdUsuario);
                if (usuarioEncontrado == null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }
                usuarioEncontrado.Correo = usuario.Correo;
                usuarioEncontrado.Telefono = usuario.Telefono;

                bool respuesta = await _repository.Editar(usuarioEncontrado);
                return respuesta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }





        public async Task<bool> CambiarClave(Usuario usuario, string calveActual, string claveNueva)
        {
            try
            {
                Usuario usuarioEncontrado = await _repository.Obtener(u => u.IdUsuario == usuario.IdUsuario);

                if (usuarioEncontrado == null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }
                if (usuarioEncontrado.Clave != _utilidadesServices.ConvertirSha256(calveActual))
                {
                    throw new TaskCanceledException("La contraseña ingresada como actual no es correcta");
                }
                usuarioEncontrado.Clave = _utilidadesServices.ConvertirSha256(claveNueva);
                bool respuesta = await _repository.Editar(usuarioEncontrado);
                return respuesta;
            }
            catch (Exception)
            {
                throw;
            }
        }





        public async Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo)
        {
            try
            {
                Usuario usuarioEncontrado = await _repository.Obtener(u => u.Correo == correo);

                if (usuarioEncontrado == null)
                {
                    throw new TaskCanceledException("El Correo ingresado no existe.");
                }
                string claveGenerada = _utilidadesServices.GenerarClave();
                usuarioEncontrado.Clave = _utilidadesServices.ConvertirSha256(claveGenerada);


                UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[clave]", claveGenerada);
                string htmlCorreo = "";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader readerStream = null;
                        readerStream = new StreamReader(dataStream);

                        if (response.CharacterSet == null)
                        {
                            readerStream = new StreamReader(dataStream);
                        }
                        else
                        {
                            readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();
                        }
                    }
                }

                bool CorreoEnviado = false;
                if (htmlCorreo != "")
                {
                    CorreoEnviado = await _correoServices.EnviarCorreo(correo, "Contraseña Restablecida", htmlCorreo);
                }
                if (!CorreoEnviado)
                {
                    throw new TaskCanceledException("Tenemos problemas. Por favor intentelo mas tarde.");
                }
                bool respuesta = await _repository.Editar(usuarioEncontrado);
                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
