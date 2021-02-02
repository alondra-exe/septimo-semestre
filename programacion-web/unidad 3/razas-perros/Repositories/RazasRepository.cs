using System;
using System.Collections.Generic;
using System.Linq;
using RazasPerros.Models;
using RazasPerros.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace RazasPerros.Repositories
{
    public class RazasRepository : Repository<Razas>
    {
        public RazasRepository(sistem14_razasContext context) : base(context) { }
        public IEnumerable<RazaViewModel> GetRazas()
        {
            return Context.Razas.OrderBy(x => x.Nombre)
                .Select(x => new RazaViewModel
                {
                    Id = x.Id,
                    Nombre = x.Nombre
                });
        }
        public Razas GetRazaById(uint id)
        {
            return Context.Razas.Include(x => x.Estadisticasraza)
                .Include(x => x.Caracteristicasfisicas)
                .Include(x => x.IdPaisNavigation)
                .FirstOrDefault(x => x.Id == id);
        }
        public IEnumerable<RazaViewModel> GetRazasByLetraInicial(string letra)
        {
            return GetRazas().Where(x => x.Nombre.StartsWith(letra));
        }
        public IEnumerable<char> GetLetrasIniciales()
        {
            return Context.Razas.OrderBy(x => x.Nombre).Select(x => x.Nombre.First());
        }
        public Razas GetRazaByNombre(string nombre)
        {
            nombre = nombre.Replace("-", " ");
            return Context.Razas
                .Include(x => x.Estadisticasraza)
                .Include(x => x.Caracteristicasfisicas)
                .Include(x => x.IdPaisNavigation)
                .FirstOrDefault(x => x.Nombre == nombre);
        }
        public IEnumerable<RazaViewModel> Get4RandomRazasExcept(string nombre)
        {
            nombre = nombre.Replace("-", " ");
            Random r = new Random();
            return Context.Razas
                .Where(x => x.Nombre != nombre)
                .ToList()
                .OrderBy(x => r.Next())
                .Take(4)
                .Select(x => new RazaViewModel { Id = x.Id, Nombre = x.Nombre });
        }
        public IEnumerable<Paises> GetRazasByPais()
        {
            return Context.Paises.Include(x => x.Razas).OrderBy(x => x.Nombre);
        }
        public IEnumerable<Paises> GetPaises()
        {
            return Context.Paises.OrderBy(x => x.Nombre);
        }
        public override bool Validate(Razas raza)
        {
            // Validaciones datos
            if (raza.Id <= 0)
                throw new Exception("Ingrese una ID válida.");
            if (string.IsNullOrWhiteSpace(raza.Nombre))
                throw new Exception("Ingrese un nombre.");
            if (string.IsNullOrWhiteSpace(raza.Descripcion))
                throw new Exception("Ingrese una descripción.");
            if (string.IsNullOrWhiteSpace(raza.OtrosNombres))
                throw new Exception("Ingrese otro nombre de la raza.");
            if (!Context.Paises.Any(x => x.Id == raza.IdPais))
                throw new Exception("No existe el país específicado.");
            if (raza.PesoMin <= 0)
                throw new Exception("Ingrese un peso mínimo válido.");
            if (raza.PesoMax <= 0)
                throw new Exception("Ingrese un peso máximo válido.");
            if (raza.AlturaMin <= 0)
                throw new Exception("Ingrese una altura mínima válida.");
            if (raza.AlturaMax <= 0)
                throw new Exception("Ingrese una altura máxima válida.");
            if (raza.EsperanzaVida <= 0)
                throw new Exception("Ingrese una esperanza de vida válida.");
            if (Context.Razas.Any(x => x.Nombre == raza.Nombre && x.Id != raza.Id))
                throw new Exception("Ya existe una raza registrada con el mismo nombre.");
            // Validaciones características físicas
            if (string.IsNullOrWhiteSpace(raza.Caracteristicasfisicas.Patas))
                throw new Exception("Ingrese una característica física sobre sus patas.");
            if (string.IsNullOrWhiteSpace(raza.Caracteristicasfisicas.Cola))
                throw new Exception("Ingrese una característica física sobre su cola.");
            if (string.IsNullOrWhiteSpace(raza.Caracteristicasfisicas.Hocico))
                throw new Exception("Ingrese una característica física sobre su hocico.");
            if (string.IsNullOrWhiteSpace(raza.Caracteristicasfisicas.Pelo))
                throw new Exception("Ingrese una característica física sobre su pelo.");
            if (string.IsNullOrWhiteSpace(raza.Caracteristicasfisicas.Color))
                throw new Exception("Ingrese una característica física sobre su color.");
            // Validaciones estadísticas
            if (raza.Estadisticasraza.NivelEnergia < 0 || raza.Estadisticasraza.NivelEnergia > 10)
                throw new Exception("Ingrese una estadística sobre su nivel de energía del 0 al 10.");
            if (raza.Estadisticasraza.FacilidadEntrenamiento < 0 || raza.Estadisticasraza.FacilidadEntrenamiento > 10)
                throw new Exception("Ingrese una estadística sobre su facilidad de entrenamiento del 0 al 10.");
            if (raza.Estadisticasraza.EjercicioObligatorio < 0 || raza.Estadisticasraza.EjercicioObligatorio > 10)
                throw new Exception("Ingrese una estadística sobre su ejercicio obligatorio del 0 al 10.");
            if (raza.Estadisticasraza.AmistadDesconocidos < 0 || raza.Estadisticasraza.AmistadDesconocidos > 10)
                throw new Exception("Ingrese una estadística sobre su amistad con desconocidos del 0 al 10.");
            if (raza.Estadisticasraza.AmistadPerros < 0 || raza.Estadisticasraza.AmistadPerros > 10)
                throw new Exception("Ingrese una estadística sobre su amistad con otros perros del 0 al 10.");
            if (raza.Estadisticasraza.NecesidadCepillado < 0 || raza.Estadisticasraza.NecesidadCepillado > 10)
                throw new Exception("Ingrese una estadística sobre su necesidad de cepillado del 0 al 10.");
            //
            return true;
        }
    }
}