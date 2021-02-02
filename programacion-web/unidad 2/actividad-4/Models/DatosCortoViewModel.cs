using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVCUnidad2Actividad4.Models;

namespace MVCUnidad2Actividad4.Models
{
    public class DatosCortoViewModel
    {
        public string Categoria { get; set; }
        public IEnumerable<Models.Cortometraje> Cortometrajes { get; set; }
    }
}
