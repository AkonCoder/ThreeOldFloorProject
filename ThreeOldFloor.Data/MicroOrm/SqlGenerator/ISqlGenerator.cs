﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ThreeOldFloor.Data.MicroOrm.SqlGenerator
{
    /// <summary>
    /// Universal SqlGenerator for Tables
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ISqlGenerator<TEntity> where TEntity : class
    {
        string TableName { get; }

        bool IsIdentity { get; }

        ThreeOldFloor.Data.MicroOrm.SqlGenerator.ESqlConnector SqlConnector { get; set; }

        IEnumerable<PropertyMetadata> KeyProperties { get; }

        IEnumerable<PropertyMetadata> BaseProperties { get; }

        PropertyMetadata IdentityProperty { get; }

        PropertyMetadata StatusProperty { get; }

        object LogicalDeleteValue { get; }

        bool LogicalDelete { get; }

        SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate,
            List<Expression<Func<TEntity, object>>> selectColumns, params Expression<Func<TEntity, object>>[] includes);

        SqlQuery GetSelectFirst<T>(Expression<Func<TEntity, bool>> predicate, TEntity entity, Expression<Func<T, dynamic>> fields);

        SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate,
            List<Expression<Func<TEntity, object>>> selectColumns, params Expression<Func<TEntity, object>>[] includes);

        SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        SqlQuery GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwFiled,
            Expression<Func<TEntity, bool>> predicate);

        SqlQuery GetInsert(TEntity entity);

        SqlQuery GetUpdate(TEntity entity);
        SqlQuery GetUpdate<T>(TEntity entity, Expression<Func<T, dynamic>> fields);

        SqlQuery GetDelete(TEntity entity);
    }
}