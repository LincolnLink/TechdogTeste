using Dev.Business.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Business.Interfaces
{
    public interface IRepository<T> : IDisposable where T : Entity
    {
        Task Adicionar(T entity);

        Task<T> ObterPorId(Guid id);

        Task<List<T>> ObterTodos();

        Task Atualizar(T Obj);

        Task Remover(Guid id);

        /// <summary>int é o numero de linhas afetadas.</summary>       
        Task<int> SaveChanges();

    }
}
