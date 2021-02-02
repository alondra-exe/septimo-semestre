using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVCUnidad2Actividad4.Models;

namespace MVCUnidad2Actividad4.Models
{
    public class DatosPeliculaViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreOriginal { get; set; }
        public DateTime? FechaEstreno { get; set; }
        public string Descripcion { get; set; }
        public IEnumerable<Apariciones> Apariciones { get; set; }
    }
}