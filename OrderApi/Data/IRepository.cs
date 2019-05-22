using System.Collections.Generic;
using OrderApi.Models;

namespace OrderApi.Data
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        T Add(T entity);
        void Edit(T entity);
        void Remove(int id);
        T UpdateStatus(int id, Order.OrderStatus status);
    }
}
