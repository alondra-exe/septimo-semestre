﻿using System.Collections.Generic;

namespace Actividad2.Models.ViewModels
{
    public class AlumnoViewModel
    {
        public Alumno Alumno { get; set; }
        public Maestro Maestro { get; set; }
        public IEnumerable<Maestro> Maestros { get; set; }
    }
}