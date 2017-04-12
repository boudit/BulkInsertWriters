//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BulkInsertConfiguration.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace SqlServerBulkInsert
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using SqlServerBulkInsert.Mapping;
    using SqlServerBulkInsert.Model;

    public class BulkInsertConfiguration<T> : AbstractMap<T>, IBulkInsertConfiguration
    {
        #region Fields and Constants

        private readonly HashSet<Type> mappedTypes = new HashSet<Type>();

        #endregion

        #region Constructors and Destructors

        public BulkInsertConfiguration(string tableName, Dictionary<string, BulkInsertPropertyInfo> mappings)
            : base(tableName)
        {
            foreach (var mapping in mappings)
            {
                var lambda = CreateLambdaFromPropertyInfo(mapping.Value);

                Type columnType;
                if (mapping.Value.IsNavigationProperty)
                {
                    columnType = typeof(ColumnDefinition<,>).MakeGenericType(typeof(T), typeof(Guid));
                }
                else
                {
                    columnType = typeof(ColumnDefinition<,>).MakeGenericType(
                        typeof(T), 
                        mapping.Value.PropertyInfo.PropertyType);
                }

                var column = Activator.CreateInstance(columnType, mapping.Key, lambda);
                this.Columns.Add(column as ColumnDefinition<T>);

                this.mappedTypes.Add(mapping.Value.PropertyInfo.PropertyType);
            }
        }

        #endregion

        #region Properties

        public Type ManagedType
        {
            get
            {
                return typeof(T);
            }
        }

        public IEnumerable<Type> MappedTypes
        {
            get
            {
                return this.mappedTypes;
            }
        }

        #endregion

        private static Delegate CreateLambdaFromPropertyInfo(BulkInsertPropertyInfo customInfo)
        {
            // Create a function that takes the input object and return the property specified in the PropertyInfo.
            var parameter = Expression.Parameter(typeof(T), "entity");
            var property = Expression.Property(parameter, customInfo.PropertyInfo);
            var funcType = typeof(Func<,>).MakeGenericType(typeof(T), customInfo.PropertyInfo.PropertyType);

            // If the property is a navigation property, get the property Id of the object.
            if (customInfo.IsNavigationProperty)
            {
                property = Expression.Property(property, "Id");
                funcType = typeof(Func<,>).MakeGenericType(typeof(T), typeof(Guid));
            }

            return Expression.Lambda(funcType, property, parameter).Compile();
        }
    }
}