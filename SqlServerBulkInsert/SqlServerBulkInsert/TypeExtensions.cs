//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="TypeExtensions.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace SqlServerBulkInsert
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class TypeExtensions
    {
        public static IEnumerable<PropertyInfo> GetPropertiesHierarchical(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }

            if (type == typeof(object))
            {
                return type.GetTypeInfo().DeclaredProperties;
            }

            return type.GetTypeInfo().DeclaredProperties.Concat(type.GetTypeInfo().BaseType.GetPropertiesHierarchical());
        }

        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            var list = new List<FieldInfo>();

            for (var type1 = type; type1 != typeof(object); type1 = type1.GetTypeInfo().BaseType)
            {
                list.AddRange(type1.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
            }

            return list.ToArray();
        } 
    }
}