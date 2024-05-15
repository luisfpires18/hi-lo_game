namespace HiLoGame.Domain.Interfaces
{
    using System.Collections.Generic;

    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();

        void Insert(T entity);
    }
}