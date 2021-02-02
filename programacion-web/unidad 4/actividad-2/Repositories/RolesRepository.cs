using System;
using System.Collections.Generic;
using System.Linq;
using Actividad2.Models;
using Microsoft.EntityFrameworkCore;

namespace Actividad2.Repositories
{
    public class RolesRepository<T> where T : class
    {
        public roleContext Context { get; set; }
        public RolesRepository(roleContext context)
        {
            Context = context;
        }
        public virtual IEnumerable<T> ObtenerTodo()
        {
            return Context.Set<T>();
        }
        public T ObtenerPorId(object id)
        {
            return Context.Find<T>(id);
        }

        public virtual void Insertar(T entidad)
        {
            if (Validaciones(entidad))
            {
                Context.Add(entidad);
                Context.SaveChanges();
            }
        }
        public virtual void Editar(T entidad)
        {
            if (Validaciones(entidad))
            {
                Context.Update<T>(entidad);
                Context.SaveChanges();
            }
        }
        public virtual void Eliminar(T entidad)
        {
            Context.Remove<T>(entidad);
            Context.SaveChanges();
        }

        public virtual bool Validaciones(T entidad)
        {
            return true;
        }
    }

    public class MaestroRepository : RolesRepository<Maestro>
    {
        public MaestroRepository(roleContext context) : base(context) { }

        public Maestro ObtenerMaestroPorNoControl(int nocontrol)
        {
            return Context.Maestro.FirstOrDefault(x => x.NoControl == nocontrol);
        }
        public Maestro ObtenerAlumnosPorMaestro(int idmaestro)
        {
            return Context.Maestro.Include(x => x.Alumno).FirstOrDefault(x => x.Id == idmaestro);
        }

        public override bool Validaciones(Maestro maestro)
        {
            if (maestro.NoControl <= 0)
                throw new Exception("Ingrese el número de control del maestro.");
            if (maestro.NoControl == 4444)
                throw new Exception("Este no es un número de control válido.");
            if (string.IsNullOrWhiteSpace(maestro.Nombre))
                throw new Exception("Ingrese el nombre del maestro.");
            if (string.IsNullOrWhiteSpace(maestro.Contrasena))
                throw new Exception("Ingrese la contraseña del maestro.");
            if (maestro.NoControl.ToString().Length > 4)
                throw new Exception("El número de control no debe exceder los 4 dígitos.");
            if (maestro.NoControl.ToString().Length < 4)
                throw new Exception("El número de control debe ser de 4 dígitos.");
            return true;
        }
    }

    public class AlumnosRepository : RolesRepository<Alumno>
    {
        public AlumnosRepository(roleContext context) : base(context) { }

        public Alumno ObtenerAlumnoPorNoControl(string noControl)
        {
            return Context.Alumno.FirstOrDefault(x => x.NoControl.ToLower() == noControl.ToLower());
        }

        public override bool Validaciones(Alumno alumno)
        {
            if (string.IsNullOrEmpty(alumno.NoControl))
                throw new Exception("Ingrese el número de control del alumno.");
            if (string.IsNullOrEmpty(alumno.Nombre))
                throw new Exception("Ingrese el nombre del alumno.");
            if (alumno.IdMaestro.ToString() == null || alumno.IdMaestro <= 0)
                throw new Exception("Debe asignarle un maestro al alumno.");
            if (alumno.NoControl == "4444")
                throw new Exception("Este no es un número de control válido.");
            if (alumno.NoControl.Length < 8)
                throw new Exception("El número de control debe ser de 8 dígitos.");
            if (alumno.NoControl.Length > 8)
                throw new Exception("El número de control no debe exceder los 8 dígitos.");
            return true;
        }
    }
}