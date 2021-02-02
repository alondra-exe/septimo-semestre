using System;
using System.Linq;
using Actividad1.Models;

namespace Actividad1.Repositories
{
    public class UsuarioRepository<T> where T : class
    {
        public usuariosContext Context { get; set; }
        public UsuarioRepository(usuariosContext context)
        {
            Context = context;
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            return Context.Usuario.FirstOrDefault(x => x.Id == id);
        }
        public Usuario ObtenerUsuarioPorCorreo(string correo)
        {
            return Context.Usuario.FirstOrDefault(x => x.CorreoElectronico == correo);
        }
        public Usuario ObtenerUsuario(Usuario id)
        {
            return Context.Find<Usuario>(id);
        }

        public bool Validaciones(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.NombreUsuario))
                throw new Exception("Ingrese el nombre de usuario.");
            if (string.IsNullOrEmpty(usuario.CorreoElectronico))
                throw new Exception("Ingrese el correo electrónico del usuario.");
            if (string.IsNullOrEmpty(usuario.Contrasena))
                throw new Exception("Ingrese la contraseña del usuario.");
            //if (Context.Usuario.Any(x => x.CorreoElectronico.ToLower() == usuario.CorreoElectronico.ToLower()))
            //    throw new Exception("Ya se registró un usuario con este correo electrónico previamente.");
            //if (Context.Usuario.Any(x => x.NombreUsuario.ToLower() == usuario.NombreUsuario.ToLower()))
            //    throw new Exception("Ya se registró un usuario con este nombre previamente.");
            return true;
        }

        public virtual void Insertar(Usuario usuario)
        {
            if (Validaciones(usuario))
            {
                Context.Add(usuario);
                Context.SaveChanges();
            }
        }
        public virtual void Editar(Usuario usuario)
        {
            if (Validaciones(usuario))
            {
                Context.Update<Usuario>(usuario);
                Context.SaveChanges();
            }
        }
        public virtual void Eliminar(Usuario usuario)
        {
            Context.Remove<Usuario>(usuario);
            Context.SaveChanges();
        }
    }
}