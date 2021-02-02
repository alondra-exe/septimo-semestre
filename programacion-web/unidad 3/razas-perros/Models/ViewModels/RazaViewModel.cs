using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RazasPerros.Models.ViewModels
{
    public class RazaViewModel
    {
        public uint Id { get; set; }
        public string Nombre { get; set; }
        //
        public Razas Raza { get; set; }
        public IEnumerable<Paises> Paises { get; set; }
        public IFormFile Archivo { get; set; }
        public string Imagen { get; set; }
    }
}