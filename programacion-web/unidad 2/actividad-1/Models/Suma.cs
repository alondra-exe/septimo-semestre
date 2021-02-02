using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC2Unidad1Actividad.Models
{
    public class Suma
    {
        public int NumeroUno { get; set; }
        public int NumeroDos { get; set; }

        public int Total(int n1, int n2)
        {
            int total = n1 + n2;
            return total;
        }
    }
}
