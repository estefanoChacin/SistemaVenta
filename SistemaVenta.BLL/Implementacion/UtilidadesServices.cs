using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SistemaVenta.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class UtilidadesServices : IUtilidadesServices
    {


        public string ConvertirSha256(string texto)
        {
           StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create()) {
                Encoding enc = Encoding.UTF8;

                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (var item in result)
                {
                    sb.Append(item.ToString("x2"));
                }
            }
            return sb.ToString();
        }



        public string GenerarClave()
        {
            string clave = new Guid().ToString("N").Substring(0,6);
            return clave;
        }
    }
}
