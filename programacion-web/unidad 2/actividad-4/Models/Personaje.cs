using System;
using System.Collections.Generic;

namespace MVCUnidad2Actividad4.Models
{
    public partial class Personaje
    {
        public Personaje()
        {
            Apariciones = new HashSet<Apariciones>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Apariciones> Apariciones { get; set; }
    }
}
