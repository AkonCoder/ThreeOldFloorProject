using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThreeOldFloor.Data.MicroOrm.SqlGenerator;

namespace ThreeOldFloor.Data.MicroOrm
{
    public interface IBaseDapperRepository<TEntity> where TEntity : class
    {
        IDbConnection Connection { get; }

        ISqlGenerator<TEntity> SqlGenerator { get; }

        TEntity Find(Expression<Func<TEntity, bool>> expression);

        TEntity Find(Expression<Func<TEntity, bool>> expression, List<Expression<Func<TEntity, object>>> selectColumns);


        TEntity Find<TChild1>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, object>> tChild1);

        IEnumerable<TEntity> FindAll();

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression);

        IEnumerable<TEntity> FindAll(SqlQuery sqlQuery);

        IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, object>> tChild1);

        IEnumerable<TEntity> FindAll<TChild1>(Expression<Func<TEntity, bool>> expression,
            Expression<Func<TEntity, object>> tChild1);

        IEnumerable<TEntity> FindAll<TChild1>(SqlQuery sqlQuery, Expression<Func<TEntity, object>> tChild1);

        Task<IEnumerable<TEntity>> FindAllAsync();

        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> expression);

        Task<IEnumerable<TEntity>> FindAllAsync(SqlQuery sqlQuery);

        Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, object>> tChild1);

        Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(Expression<Func<TEntity, bool>> expression,
            Expression<Func<TEntity, object>> tChild1);

        Task<IEnumerable<TEntity>> FindAllAsync<TChild1>(SqlQuery sqlQuery, Expression<Func<TEntity, object>> tChild1);

        Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, object>> tChild1);

        Task<TEntity> FindAsync<TChild1>(Expression<Func<TEntity, bool>> expression,
            Expression<Func<TEntity, object>> tChild1);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> expression);

        Task<TEntity> FindAsync();

        bool Insert(TEntity instance);

        Task<bool> InsertAsync(TEntity instance);

        bool Delete(TEntity instance);


        bool Update(TEntity instance);


        IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField);

        IEnumerable<TEntity> FindAllBetween(object from, object to, Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> expression);

        Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to,
            Expression<Func<TEntity, object>> btwField);

        Task<IEnumerable<TEntity>> FindAllBetweenAsync(object from, object to,
            Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression);

        IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField);

        IEnumerable<TEntity> FindAllBetween(DateTime from, DateTime to, Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> expression);

        Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to,
            Expression<Func<TEntity, object>> btwField);

        Task<IEnumerable<TEntity>> FindAllBetweenAsync(DateTime from, DateTime to,
            Expression<Func<TEntity, object>> btwField, Expression<Func<TEntity, bool>> expression);
    }
}