using MVCUnidad2Actividad4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCUnidad2Actividad4.Services
{
    public class ClasificacionService
    {
        public List<Clase> Clasificacion { get; set; }
        public ClasificacionService()
        {
            using (animalesContext context = new animalesContext())
            {
                Clasificacion = context.Clase.OrderBy(x => x.Nombre).ToList();
            }
        }
    }
}
