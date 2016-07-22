using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ThreeOldFloor.Data.MicroOrm.Attributes;
using ThreeOldFloor.Data.MicroOrm.Attributes.Joins;
using ThreeOldFloor.Data.MicroOrm.Attributes.LogicalDelete;
using ThreeOldFloor.Data.MicroOrm.Extensions;

namespace ThreeOldFloor.Data.MicroOrm.SqlGenerator
{
    public class SqlGenerator<TEntity> : ThreeOldFloor.Data.MicroOrm.SqlGenerator.ISqlGenerator<TEntity>
        where TEntity : class
    {
        public SqlGenerator(ThreeOldFloor.Data.MicroOrm.SqlGenerator.ESqlConnector sqlConnector)
        {
            SqlConnector = sqlConnector;
            var entityType = typeof (TEntity);
            var entityTypeInfo = entityType.GetTypeInfo();
            var aliasAttribute = entityTypeInfo.GetCustomAttribute<TableAttribute>();

            this.TableName = aliasAttribute != null ? aliasAttribute.Name : entityTypeInfo.Name;
            AllProperties = entityType.GetProperties();
            //Load all the "primitive" entity properties
            var props = AllProperties.Where(ExpressionHelper.GetPrimitivePropertiesPredicate()).ToArray();

            //Filter the non stored properties
            this.BaseProperties =
                props.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any())
                    .Select(p => new PropertyMetadata(p));

            //Filter key properties
            this.KeyProperties =
                props.Where(p => p.GetCustomAttributes<KeyAttribute>().Any())
                    .Select(p => new PropertyMetadata(p));

            //Use identity as key pattern
            var identityProperty = props.FirstOrDefault(p => p.GetCustomAttributes<IdentityAttribute>().Any());
            this.IdentityProperty = identityProperty != null
                ? new PropertyMetadata(identityProperty)
                : null;

            //Status property (if exists, and if it does, it must be an enumeration)
            var statusProperty = props.FirstOrDefault(p => p.GetCustomAttributes<StatusAttribute>().Any());

            if (statusProperty == null) return;
            StatusProperty = new PropertyMetadata(statusProperty);

            if (statusProperty.PropertyType.IsBool())
            {
                var deleteProperty = props.FirstOrDefault(p => p.GetCustomAttributes<DeletedAttribute>().Any());
                if (deleteProperty == null) return;

                LogicalDelete = true;
                LogicalDeleteValue = 1; // true
            }
            else if (statusProperty.PropertyType.IsEnum())
            {
                var deleteOption =
                    statusProperty.PropertyType.GetFields()
                        .FirstOrDefault(f => f.GetCustomAttribute<DeletedAttribute>() != null);

                if (deleteOption == null) return;

                var enumValue = Enum.Parse(statusProperty.PropertyType, deleteOption.Name);

                if (enumValue != null)
                    LogicalDeleteValue = Convert.ChangeType(enumValue,
                        Enum.GetUnderlyingType(statusProperty.PropertyType));

                LogicalDelete = true;
            }
        }

        public SqlGenerator()
            : this(ThreeOldFloor.Data.MicroOrm.SqlGenerator.ESqlConnector.MSSQL)
        {
        }

        public ThreeOldFloor.Data.MicroOrm.SqlGenerator.ESqlConnector SqlConnector { get; set; }

        public bool IsIdentity
        {
            get { return this.IdentityProperty != null; }
        }

        public bool LogicalDelete { get; private set; }

        public string TableName { get; private set; }

        public PropertyInfo[] AllProperties { get; private set; }

        public PropertyMetadata IdentityProperty { get; private set; }

        public IEnumerable<PropertyMetadata> KeyProperties { get; private set; }

        public IEnumerable<PropertyMetadata> BaseProperties { get; private set; }

        public PropertyMetadata StatusProperty { get; private set; }

        public object LogicalDeleteValue { get; private set; }

        private string GetPropertyValue(TEntity entity, PropertyInfo property)
        {
            if (property.PropertyType == typeof(String) || property.PropertyType == typeof(DateTime?))
            {
                return string.Format("'{0}'", property.GetValue(entity));
            }
            var result = property.GetValue(entity) == null
                ? null
                : property.GetValue(entity).ToString();
            return result;
        }

        private string GetPropertyValue(QueryParameter parameter)
        {
            if (parameter.PropertyValue is string ||
                parameter.PropertyValue is DateTime)
            {
                return string.Format("'{0}'", parameter.PropertyValue);
            }

            return parameter.PropertyValue.ToString();
        }

        public virtual SqlQuery GetInsert(TEntity entity)
        {
            List<PropertyMetadata> properties = (this.IsIdentity
                ? this.BaseProperties.Where(
                    p => !p.Name.Equals(this.IdentityProperty.Name, StringComparison.OrdinalIgnoreCase))
                : this.BaseProperties).ToList();

            string columNames = string.Join(", ", properties.Select(p => p.Name));
            string values = string.Join(", ", properties.Select(p => GetPropertyValue(entity, p.PropertyInfo) ?? "NULL"));
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("INSERT INTO {0} {1} {2} ",
                this.TableName,
                string.IsNullOrEmpty(columNames) ? "" : "(" + columNames + ")",
                string.IsNullOrEmpty(values) ? "" : " VALUES (" + values + ")");

            if (this.IsIdentity)
            {
                switch (SqlConnector)
                {
                    case ThreeOldFloor.Data.MicroOrm.SqlGenerator.ESqlConnector.MSSQL:
                        sqlBuilder.Append("SELECT CAST(SCOPE_IDENTITY() AS INT) AS " + this.IdentityProperty.ColumnName);
                        break;

                    case ThreeOldFloor.Data.MicroOrm.SqlGenerator.ESqlConnector.MySQL:
                        sqlBuilder.Append("; SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS " +
                                          this.IdentityProperty.ColumnName);
                        break;

                    case ThreeOldFloor.Data.MicroOrm.SqlGenerator.ESqlConnector.PostgreSQL:
                        sqlBuilder.Append("RETURNING " + this.IdentityProperty.ColumnName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new SqlQuery(sqlBuilder.ToString(), entity);
        }

        public virtual SqlQuery GetUpdate(TEntity entity)
        {
            var properties =
                this.BaseProperties.Where(
                    p => !this.KeyProperties.Any(k => k.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));

            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("UPDATE {0} SET {1} WHERE {2}", this.TableName,
                string.Join(", ",
                    properties.Select(
                        p => string.Format("{0}={1}", p.ColumnName, GetPropertyValue(entity, p.PropertyInfo) ?? "NULL"))),
                string.Join(" AND ",
                    this.KeyProperties.Select(
                        p => string.Format("{0}={1}", p.ColumnName, GetPropertyValue(entity, p.PropertyInfo) ?? "NULL"))));

            return new SqlQuery(sqlBuilder.ToString().TrimEnd(), entity);
        }

        #region Get Select

        public virtual SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, true, null, includes);
        }

        public virtual SqlQuery GetSelectFirst(Expression<Func<TEntity, bool>> predicate,
            List<Expression<Func<TEntity, object>>> selectColumns, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, true, selectColumns, includes);
        }


        public virtual SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, false, null, includes);
        }

        public virtual SqlQuery GetSelectAll(Expression<Func<TEntity, bool>> predicate,
            List<Expression<Func<TEntity, object>>> selectColumns, params Expression<Func<TEntity, object>>[] includes)
        {
            return GetSelect(predicate, false, selectColumns, includes);
        }

        private static MemberExpression GetMemberInfo(Expression method)
        {
            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null)
                throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr =
                    ((UnaryExpression) lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr;
        }


        private StringBuilder InitBuilderSelect(bool firstOnly, List<Expression<Func<TEntity, object>>> selectColumns)
        {
            if (selectColumns != null)
            {
                var columns = selectColumns.Select(c => GetMemberInfo(c).Member.Name).ToList();
            }

            var builder = new StringBuilder();
            var select = "SELECT ";

            if (firstOnly && SqlConnector == ThreeOldFloor.Data.MicroOrm.SqlGenerator.ESqlConnector.MSSQL)
                select += "TOP 1 ";

            // convert the query parms into a SQL string and dynamic property object
            //builder.Append("{select} {GetFieldsSelect(TableName, BaseProperties)}");
            builder.AppendFormat("{0} {1}", select, GetFieldsSelect(TableName, BaseProperties));

            return builder;
        }

        private StringBuilder AppendJoinToSelect(StringBuilder originalBuilder,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var joinsBuilder = new StringBuilder();

            foreach (var include in includes)
            {
                var propertyName = ExpressionHelper.GetPropertyName(include);
                var joinProperty = AllProperties.First(x => x.Name == propertyName);
                var attrJoin = joinProperty.GetCustomAttribute<JoinAttributeBase>();
                if (attrJoin != null)
                {
                    var joinString = "";
                    if (attrJoin is LeftJoinAttribute)
                    {
                        joinString = "LEFT JOIN ";
                    }
                    else if (attrJoin is InnerJoinAttribute)
                    {
                        joinString = "INNER JOIN ";
                    }
                    else if (attrJoin is RightJoinAttribute)
                    {
                        joinString = "RIGHT JOIN ";
                    }

                    var joinType = joinProperty.PropertyType.IsGenericType()
                        ? joinProperty.PropertyType.GenericTypeArguments[0]
                        : joinProperty.PropertyType;

                    var properties = joinType.GetProperties().Where(ExpressionHelper.GetPrimitivePropertiesPredicate());
                    var props =
                        properties.Where(p => !p.GetCustomAttributes<NotMappedAttribute>().Any())
                            .Select(p => new PropertyMetadata(p));
                    originalBuilder.Append(", " + GetFieldsSelect(attrJoin.TableName, props));


                    //joinsBuilder.Append("{joinString} {attrJoin.TableName} ON {TableName}.{attrJoin.Key} = {attrJoin.TableName}.{attrJoin.ExternalKey} ");
                    joinsBuilder.AppendFormat("{0} {1} ON {2}.{3} = {4}.{5} ", joinString, attrJoin.TableName, TableName,
                        attrJoin.Key, attrJoin.TableName, attrJoin.ExternalKey);
                }
            }
            return joinsBuilder;
        }

        private static string GetFieldsSelect(string tableName,
            IEnumerable<PropertyMetadata> properties)
        {
            //Projection function
            Func<PropertyMetadata, string> projectionFunction =
                (p) => !string.IsNullOrEmpty(p.Alias)
                    ? string.Format("{0}.{1} AS {2}", tableName, p.ColumnName, p.Name)
                    : string.Format("{0}.{1}", tableName, p.ColumnName);

            return string.Join(", ", properties.Select(projectionFunction));
        }


        private SqlQuery GetSelect(Expression<Func<TEntity, bool>> predicate, bool firstOnly,
            List<Expression<Func<TEntity, object>>> selectColumns, params Expression<Func<TEntity, object>>[] includes)
        {
            var builder = InitBuilderSelect(firstOnly, selectColumns);

            if (includes.Any())
            {
                var joinsBuilder = AppendJoinToSelect(builder, includes);
                builder.Append(" FROM " + TableName);
                builder.Append(joinsBuilder);
            }
            else
            {
                builder.Append(" FROM " + TableName);
            }

            IDictionary<string, object> expando = new ExpandoObject();

            if (predicate != null)
            {
                // WHERE
                var queryProperties = new List<QueryParameter>();
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(predicate.Body), ExpressionType.Default,
                    ref queryProperties);

                builder.Append(" WHERE ");


                for (int i = 0; i < queryProperties.Count; i++)
                {
                    var item = queryProperties[i];
                    if (!string.IsNullOrEmpty(item.LinkingOperator) && i > 0)
                    {
                        builder.Append(string.Format("{0} {1}.{2} {3} {4} ", item.LinkingOperator, TableName,
                            item.PropertyName, item.QueryOperator, GetPropertyValue(item)));
                    }
                    else
                    {
                        builder.Append(string.Format("{0}.{1} {2} {3} ", TableName, item.PropertyName,
                            item.QueryOperator, GetPropertyValue(item)));
                    }

                    expando[item.PropertyName] = item.PropertyValue;
                }
            }

            if (firstOnly &&
                (SqlConnector == ThreeOldFloor.Data.MicroOrm.SqlGenerator.ESqlConnector.MySQL ||
                 SqlConnector == ThreeOldFloor.Data.MicroOrm.SqlGenerator.ESqlConnector.PostgreSQL))
                builder.Append("LIMIT 1");


            return new SqlQuery(builder.ToString().TrimEnd(), expando);
        }

        public virtual SqlQuery GetSelectBetween(object from, object to, Expression<Func<TEntity, object>> btwField,
            Expression<Func<TEntity, bool>> expression)
        {
            var filedName = ExpressionHelper.GetPropertyName(btwField);
            var queryResult = GetSelectAll(expression);
            var op = expression == null ? "WHERE" : "AND";

            queryResult.AppendToSql(string.Format(" {0} {1} BETWEEN '{2}' AND '{3}'", op, filedName, from, to));

            return queryResult;
        }

        public virtual SqlQuery GetDelete(TEntity entity)
        {
            var sqlBuilder = new StringBuilder();

            if (!LogicalDelete)
            {
                sqlBuilder.AppendFormat("DELETE FROM {0} WHERE {1}", this.TableName,
                    string.Join(" AND ",
                        this.KeyProperties.Select(
                            p => string.Format("{0} = {1}", p.ColumnName, GetPropertyValue(entity, p.PropertyInfo)??"NULL"))));
            }
            else
            {
                sqlBuilder.AppendFormat("UPDATE {0} SET {1} WHERE {2}", this.TableName,
                    string.Format("{0}={1}", StatusProperty.ColumnName, LogicalDeleteValue),
                    string.Join(" AND ",
                        this.KeyProperties.Select(
                            p => string.Format("{0}={1}", p.ColumnName, GetPropertyValue(entity, p.PropertyInfo)??"NULL"))));
            }

            return new SqlQuery(sqlBuilder.ToString(), entity);
        }

        #endregion Get Select

        /// <summary>
        /// Fill query properties
        /// </summary>
        /// <param name="body">The body.</param>
        /// <param name="linkingType">Type of the linking.</param>
        /// <param name="queryProperties">The query properties.</param>
        private static void FillQueryProperties(BinaryExpression body, ExpressionType linkingType,
            ref List<QueryParameter> queryProperties)
        {
            if (body.NodeType != ExpressionType.AndAlso && body.NodeType != ExpressionType.OrElse)
            {
                //string propertyName = ExpressionHelper.GetPropertyName(body);
                string propertyName = ExpressionHelper.GetColumnName(body);
                object propertyValue = ExpressionHelper.GetValue(body.Right);
                string opr = ExpressionHelper.GetSqlOperator(body.NodeType);
                string link = ExpressionHelper.GetSqlOperator(linkingType);

                queryProperties.Add(new QueryParameter(link, propertyName, propertyValue, opr));
            }
            else
            {
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(body.Left), body.NodeType, ref queryProperties);
                FillQueryProperties(ExpressionHelper.GetBinaryExpression(body.Right), body.NodeType, ref queryProperties);
            }
        }
    }
}