using System;
using System.Collections.Generic;

namespace Actividad1.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string CorreoElectronico { get; set; }
        public string Contrasena { get; set; }
        public ulong? Activo { get; set; }
        public int Codigo { get; set; }
    }
}
