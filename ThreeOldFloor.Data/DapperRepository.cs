using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThreeOldFloor.Core.Data;

namespace ThreeOldFloor.Data
{
    public class DapperRepository<TEntity> : MicroOrm.BaseDapperRepository<TEntity>, IRepository<TEntity>
        where TEntity : class
    {
        public DapperRepository(DapperDbContext context) : base(context.Connection)
        {
        }

        private T ExecuteWithTryCatch<T>(Func<T> action)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                return action();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Connection.State == ConnectionState.Open && Connection != null)
                {
                    Connection.Close();
                }
            }
        }

        TEntity IRepository<TEntity>.Find(Expression<Func<TEntity, bool>> expression)
        {
            return ExecuteWithTryCatch(() => base.Find(expression));
        }

        TEntity IRepository<TEntity>.Find(Expression<Func<TEntity, bool>> expression,
            List<Expression<Func<TEntity, object>>> selectColumns)
        {
            return ExecuteWithTryCatch(() => base.Find(expression, selectColumns));
        }

        TEntity IRepository<TEntity>.Find<TChild1>(
            Expression<Func<TEntity, bool>> expression,
            Expression<Func<TEntity, object>> tChild1)
        {
            return ExecuteWithTryCatch(() => base.Find<TChild1>(expression, tChild1));
        }


        IEnumerable<TEntity> IRepository<TEntity>.FindAll()
        {
            return ExecuteWithTryCatch(() => base.FindAll());
        }

        IEnumerable<TEntity> IRepository<TEntity>.FindAll(
            Expression<Func<TEntity, bool>> expression)
        {
            return ExecuteWithTryCatch(() => base.FindAll(expression));
        }

        IEnumerable<TEntity> IRepository<TEntity>.FindAll<TChild1>(
            Expression<Func<TEntity, object>> tChild1)
        {
            return ExecuteWithTryCatch(() => base.FindAll<TChild1>(tChild1));
        }

        IEnumerable<TEntity> IRepository<TEntity>.FindAll<TChild1>(
            Expression<Func<TEntity, bool>> expression,
            Expression<Func<TEntity, object>> tChild1)
        {
            return ExecuteWithTryCatch(() => base.FindAll<TChild1>(expression, tChild1));
        }

        Task<IEnumerable<TEntity>> IRepository<TEntity>.FindAllAsync()
        {
            return ExecuteWithTryCatch(() => base.FindAllAsync());
        }

        Task<IEnumerable<TEntity>> IRepository<TEntity>.FindAllAsync(
            Expression<Func<TEntity, bool>> expression)
        {
            return ExecuteWithTryCatch(() => base.FindAllAsync(expression));
        }

        Task<IEnumerable<TEntity>> IRepository<TEntity>.FindAllAsync
            <TChild1>(Expression<Func<TEntity, object>> tChild1)
        {
            return ExecuteWithTryCatch(() => base.FindAllAsync<TChild1>(tChild1));
        }

        Task<IEnumerable<TEntity>> IRepository<TEntity>.FindAllAsync
            <TChild1>(Expression<Func<TEntity, bool>> expression,
                Expression<Func<TEntity, object>> tChild1)
        {
            return ExecuteWithTryCatch(() => base.FindAllAsync<TChild1>(expression, tChild1));
        }

        Task<TEntity> IRepository<TEntity>.FindAsync<TChild1>(
            Expression<Func<TEntity, bool>> expression,
            Expression<Func<TEntity, object>> tChild1)
        {
            return ExecuteWithTryCatch(() => base.FindAsync<TChild1>(expression, tChild1));
        }

        Task<TEntity> IRepository<TEntity>.FindAsync(
            Expression<Func<TEntity, bool>> expression)
        {
            return ExecuteWithTryCatch(() => base.FindAsync(expression));
        }

        bool IRepository<TEntity>.Insert(TEntity instance)
        {
            return ExecuteWithTryCatch(() => base.Insert(instance));
        }

        Task<bool> IRepository<TEntity>.InsertAsync(TEntity instance)
        {
            return ExecuteWithTryCatch(() => base.InsertAsync(instance));
        }

        bool IRepository<TEntity>.Delete(TEntity instance)
        {
            return ExecuteWithTryCatch(() => base.Delete(instance));
        }


        bool IRepository<TEntity>.Update(TEntity instance)
        {
            return ExecuteWithTryCatch(() => base.Update(instance));
        }
    }
}