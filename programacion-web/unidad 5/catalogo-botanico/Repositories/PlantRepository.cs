using System;
using System.Linq;
using CatalogoBotanico.Models;

namespace CatalogoBotanico.Repositories
{
    public class PlantRepository: Repository<Plantdata>
    {
        public PlantRepository(sistem14_botanicaContext context) : base(context) { }

        public Plantdata GetPlant(Plantdata plant)
        {
            return Context.Find<Plantdata>(plant);
        }
        public Plantdata GetPlantById(int id)
        {
            return Context.Plantdata.FirstOrDefault(x => x.Id == id);
        }
        public Plantdata GetPlantByCommonName(string name)
        {
            return Context.Plantdata.FirstOrDefault(x => x.CommonName == name);
        }

        public override bool Validar(Plantdata entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.CommonName))
                throw new Exception("Debe escribir el nombre común de la planta.");
            if (entidad.CommonName.Length > 50)
                throw new Exception("El nombre común de la planta no debe pasar los 50 carácteres.");

            if (string.IsNullOrWhiteSpace(entidad.ScientificName))
                throw new Exception("Debe escribir el nombre científico de la planta.");

            if (string.IsNullOrWhiteSpace(entidad.Division))
                throw new Exception("Debe escribir la división a la que pertenece la planta.");

            if (string.IsNullOrWhiteSpace(entidad.Family))
                throw new Exception("Debe escribir la familia a la que pertenece la planta.");

            if (string.IsNullOrWhiteSpace(entidad.Subfamily))
                throw new Exception("Debe escribir la subfamilia a la que pertenece la planta.");

            if (string.IsNullOrWhiteSpace(entidad.Gender))
                throw new Exception("Debe escribir el género de la planta.");

            if (string.IsNullOrWhiteSpace(entidad.Info))
                throw new Exception("Debe escribir una descripción sobre la planta.");
            if (entidad.Info.Length > 150)
                throw new Exception("La descripción sobre la planta no debe pasar los 150 carácteres.");

            if (entidad.IdUser.ToString() == null || entidad.IdUser <= 0)
                throw new Exception("Específique a que usuario pertenece este catálogo de plantas.");
            return true;
        }
    }
}