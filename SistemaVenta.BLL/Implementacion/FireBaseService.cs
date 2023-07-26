using Firebase.Auth;
using Firebase.Storage;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DALL.Implemtancion;
using SistemaVenta.DALL.Interfaces;
using SistemaVenta.Entity;


namespace SistemaVenta.BLL.Implementacion
{
    public class FireBaseService : IFireBaseServices
    {

        private readonly IGenericRepository<Configuracion> _repositorio;



        public FireBaseService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }




        public async Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string UrlImagen = "";

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);
                var pruena1 = Config["email"];
                var pruena2 = Config["clave"];

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                //token de cancelacion                                
                var cancellation = new CancellationTokenSource();

                //creamos una tarea que ejecute el servicio de firebase storage
                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions //opciones de firebase 
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true //si ocurre un error que lo cancele
                    })
                    .Child(Config[CarpetaDestino]) //creamos las carpetas
                    .Child(NombreArchivo)  //creamos nombre de archivo
                    .PutAsync(StreamArchivo, cancellation.Token); // PutAsync copiamos el archivo como un formato de Stream

                UrlImagen = await task;
            }
            catch(Exception ex)
            {
                UrlImagen = "";
            }
            return UrlImagen;
        }




        public async Task<bool> EliminarStorage(string CarpetaDestino, string NombreArchivo)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                //token de cancelacion                                
                var cancellation = new CancellationTokenSource();

                //creamos una tarea que ejecute el servicio de firebase storage
                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions //opciones de firebase 
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true //si ocurre un error que lo cancele
                    })
                    .Child(Config[CarpetaDestino]) //creamos las carpetas
                    .Child(NombreArchivo)  //creamos nombre de archivo
                    .DeleteAsync(); // PutAsync copiamos el archivo como un formato de Stream

                await task;
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
