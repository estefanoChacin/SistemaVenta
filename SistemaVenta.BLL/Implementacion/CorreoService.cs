using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DALL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implemtancion
{
    public class CorreoService : ICorreoServices
    {

        private readonly IGenericRepository<Configuracion> _repositorio;



        public async Task<bool> EnviarCorreo(string CorreoDestino, string Asunto, string mensaje)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Servicio_Correo"));
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var Credenciales = new NetworkCredential(Config["correo"], Config["clave"]);

                var correo = new MailMessage()
                {
                    From = new MailAddress(Config["correo"], Config["clave"]),
                    Subject = Asunto,
                    Body = mensaje,
                    IsBodyHtml = true
                };
                correo.To.Add(CorreoDestino);

                var clienteServidor = new SmtpClient()
                {
                    Host = Config["host"],
                    Port = int.Parse(Config["port"]),
                    Credentials= Credenciales,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                };

                clienteServidor.Send(correo);
                return true;
            }
            catch (Exception e)
            {
                string error = e.Message;
                return false;
            }
        }
    }
}
