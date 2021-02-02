using System.Collections.Generic;
using CatalogoBotanico.Models;

namespace CatalogoBotanico.Repositories
{
    public class Repository<T> where T : class
    {
        public sistem14_botanicaContext Context { get; set; }
        public Repository(sistem14_botanicaContext context)
        {
            Context = context;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Context.Set<T>();
        }
        public T GetById(object id)
        {
            return Context.Find<T>(id);
        }

        public virtual void Insert(T entidad)
        {
            if (Validar(entidad))
            {
                Context.Add(entidad);
                Context.SaveChanges();
            }
        }
        public virtual void Update(T entidad)
        {
            if (Validar(entidad))
            {
                Context.Update<T>(entidad);
                Context.SaveChanges();
            }
        }
        public virtual void Delete(T entidad)
        {
            Context.Remove<T>(entidad);
            Context.SaveChanges();
        }

        public virtual bool Validar(T entidad)
        {
            return true;
        }
    }
}