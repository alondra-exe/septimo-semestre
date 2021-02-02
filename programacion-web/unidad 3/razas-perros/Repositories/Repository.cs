﻿using RazasPerros.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazasPerros.Repositories
{
    public abstract class Repository<T> where T : class
    {
        public sistem14_razasContext Context { get; set; }
        public Repository(sistem14_razasContext context)
        {
            Context = context;
        }
        public virtual IEnumerable<T> GetAll()
        {
            return Context.Set<T>();
        }
        public virtual T GetById(object id)
        {
            return Context.Find<T>(id);
        }
        public virtual void Save()
        {
            Context.SaveChanges();
        }
        public virtual bool Validate(T entidad)
        {
            return true;
        }
        public virtual void Insert(T entidad)
        {
            if (Validate(entidad))
            {
                Context.Add(entidad);
                Save();
            }
        }
        public virtual void Update(T entidad)
        {
            if (Validate(entidad))
            {
                Context.Update(entidad);
                Save();
            }
        }
        public virtual void Delete(T entidad)
        {
            Context.Remove(entidad);
            Save();
        }
    }
}