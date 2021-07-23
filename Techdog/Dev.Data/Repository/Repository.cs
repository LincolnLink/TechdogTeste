using Dev.Business.Interfaces;
using Dev.Business.Models;
using Dev.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dev.Data.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : Entity, new()
    {
        protected readonly MeuDbContext Db;
        protected readonly DbSet<T> DbSet;

        public Repository(MeuDbContext db)
        {
            Db = db;
            DbSet = db.Set<T>();
        }


        public virtual async Task<T> ObterPorId(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<List<T>> ObterTodos()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task Adicionar(T entity)
        {
            DbSet.Add(entity);
            await SaveChanges();
        }

        public virtual async Task Atualizar(T entity)
        {
            DbSet.Update(entity);
            await SaveChanges();
        }

        public virtual async Task Remover(Guid id)
        {
            DbSet.Remove(new T { Id = id });
            await SaveChanges();
        }

        /// <summary>
        /// Salva no banco do contexto.
        /// Caso tenha algum tratamento, faça em apenas um método.
        /// </summary>        
        public async Task<int> SaveChanges()
        {
            return await Db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Db?.Dispose();
        }
    }
}
