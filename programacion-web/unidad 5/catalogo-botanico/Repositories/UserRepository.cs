using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CatalogoBotanico.Models;

namespace CatalogoBotanico.Repositories
{
    public class UserRepository : Repository<Userdata>
    {
        public UserRepository(sistem14_botanicaContext context) : base(context) { }

        public Userdata GetUserById(int id)
        {
            return Context.Userdata.FirstOrDefault(x => x.Id == id);
        }
        public Userdata GetUserByEmail(string email)
        {
            return Context.Userdata.FirstOrDefault(x => x.Email == email);
        }
        public Userdata GetPlantByUser(int id)
        {
            return Context.Userdata.Include(x => x.Plantdata).FirstOrDefault(x => x.Id == id);
        }

        public override bool Validar(Userdata entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Name))
                throw new Exception("Debe crear un nombre de usuario.");
            if (entidad.Name.Length > 15)
                throw new Exception("El nombre de usuario no debe pasar de 15 carácteres.");

            if (string.IsNullOrWhiteSpace(entidad.Email))
                throw new Exception("Debe añadir un correo electrónico.");

            if (string.IsNullOrWhiteSpace(entidad.Password))
                throw new Exception("Debe crear una contraseña.");
            if (entidad.Password.Length < 5)
                throw new Exception("La contraseña debe tener mínimo 5 carácteres.");
            return true;
        }
    }
}