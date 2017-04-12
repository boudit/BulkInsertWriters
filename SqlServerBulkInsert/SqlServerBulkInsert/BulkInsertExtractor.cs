//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BulkInsertExtractor.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace SqlServerBulkInsert
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection;
    
    public static class BulkInsertExtractor
    {
        public static IEnumerable<IBulkInsertWriter> GetOrderedBulkInsertWriters(DbContext context)
        {
            var bulkInserts = new List<IBulkInsertWriter>();

            var fieldInfo = context.GetType().GetAllFields().First(fi => fi.Name == "_internalContext");
            var internalContext = fieldInfo.GetValue(context);

            var prop = internalContext.GetType().GetPropertiesHierarchical().First(p => p.Name == "CodeFirstModel");
            var codeFirstModel = prop.GetValue(internalContext);

            prop = codeFirstModel.GetType().GetPropertiesHierarchical().First(fi => fi.Name == "CachedModelBuilder");
            var modelBuilder = prop.GetValue(codeFirstModel);

            prop = modelBuilder.GetType().GetPropertiesHierarchical().First(fi => fi.Name == "Configurations");
            var configurations = prop.GetValue(modelBuilder);

            fieldInfo = configurations.GetType().GetAllFields().FirstOrDefault(fi => fi.Name == "_modelConfiguration");
            var modelConfiguration = fieldInfo.GetValue(configurations);

            prop =
                modelConfiguration.GetType()
                    .GetPropertiesHierarchical()
                    .FirstOrDefault(fi => fi.Name == "ActiveEntityConfigurations");
            var entityConfiguration = prop.GetValue(modelConfiguration) as IEnumerable;

            foreach (var entityTypeConfiguration in entityConfiguration)
            {
                bulkInserts.Add(ExtractEntityTypeConfiguration(entityTypeConfiguration));
            }

            return GetOrderedMappings(bulkInserts);
        }

        private static IBulkInsertWriter ExtractEntityTypeConfiguration(object entityTypeConfiguration)
        {
            var allEntityTypeConfigurationProperties =
                entityTypeConfiguration.GetType().GetPropertiesHierarchical().ToList();

            var prop = allEntityTypeConfigurationProperties.First(p => p.Name == "ClrType");
            var clrType = prop.GetValue(entityTypeConfiguration) as Type;

            prop = allEntityTypeConfigurationProperties.First(p => p.Name == "TableName");
            var tableName = prop.GetValue(entityTypeConfiguration) as string;

            var mappings = new Dictionary<string, BulkInsertPropertyInfo>();

            prop = allEntityTypeConfigurationProperties.First(p => p.Name == "PrimitivePropertyConfigurations");
            var primitiveConfigurations = prop.GetValue(entityTypeConfiguration) as IEnumerable;

            if (primitiveConfigurations != null)
            {
                foreach (var primitiveConfiguration in primitiveConfigurations)
                {
                    ExtractPrimitiveMappings(primitiveConfiguration, mappings);
                }
            }

            var field =
                entityTypeConfiguration.GetType()
                    .GetAllFields()
                    .First(p => p.Name == "_navigationPropertyConfigurations");
            var navigationConfigurations = field.GetValue(entityTypeConfiguration) as IEnumerable;

            if (navigationConfigurations != null)
            {
                foreach (var navigationConfiguration in navigationConfigurations)
                {
                    ExtractNavigationMappings(navigationConfiguration, mappings);
                }
            }

            var typeOfMapping = typeof(BulkInsertConfiguration<>).MakeGenericType(clrType);
            var customMapping = Activator.CreateInstance(typeOfMapping, tableName, mappings);

            var typeOfSqlServerBulkInsert = typeof(BulkInsertWriter<>).MakeGenericType(clrType);
            return Activator.CreateInstance(typeOfSqlServerBulkInsert, customMapping) as IBulkInsertWriter;
        }

        private static void ExtractPrimitiveMappings(
            object primitiveConfiguration,
            IDictionary<string, BulkInsertPropertyInfo> mappings)
        {
            var allPrimitiveConfigurationProperties =
                primitiveConfiguration.GetType().GetPropertiesHierarchical().ToList();

            var prop = allPrimitiveConfigurationProperties.First(p => p.Name == "Key");
            var mappedProperty = ((IEnumerable<PropertyInfo>)prop.GetValue(primitiveConfiguration)).FirstOrDefault();
            if (mappedProperty == null)
            {
                return;
            }

            prop = allPrimitiveConfigurationProperties.First(p => p.Name == "Value");
            var mapping = prop.GetValue(primitiveConfiguration);

            prop = mapping.GetType().GetPropertiesHierarchical().First(fi => fi.Name == "ColumnName");
            var columnName = prop.GetValue(mapping) as string;
            if (string.IsNullOrEmpty(columnName))
            {
                return;
            }

            mappings.Add(columnName, new BulkInsertPropertyInfo(mappedProperty));
        }

        private static void ExtractNavigationMappings(
            object navigationConfiguration,
            IDictionary<string, BulkInsertPropertyInfo> mappings)
        {
            var allConfigurationProperties = navigationConfiguration.GetType().GetPropertiesHierarchical().ToList();

            var prop = allConfigurationProperties.First(p => p.Name == "Key");
            var mappedProperty = (PropertyInfo)prop.GetValue(navigationConfiguration);
            if (mappedProperty == null)
            {
                return;
            }

            prop = allConfigurationProperties.First(p => p.Name == "Value");
            var mapping = prop.GetValue(navigationConfiguration);

            prop =
                mapping.GetType().GetPropertiesHierarchical().First(fi => fi.Name == "AssociationMappingConfiguration");
            var associationConfiguration = prop.GetValue(mapping);

            var field = associationConfiguration.GetType().GetAllFields().First(fi => fi.Name == "_keyColumnNames");
            var columnNames = field.GetValue(associationConfiguration) as IEnumerable<string>;
            if (columnNames == null || columnNames.Count() != 1)
            {
                return;
            }

            mappings.Add(columnNames.First(), new BulkInsertPropertyInfo(mappedProperty, true));
        }

        private static IEnumerable<IBulkInsertWriter> GetOrderedMappings(IEnumerable<IBulkInsertWriter> mappings)
        {
            var result = new List<IBulkInsertWriter>();

            var allManagedTypes = new HashSet<Type>(mappings.Select(map => map.Configuration.ManagedType));
            var managedTypesAlreadyMapped = new List<Type>();

            var missingMappings = mappings.ToList();
            while (missingMappings.Any())
            {
                var mappingsWithNoDependency =
                    missingMappings.Where(
                        map =>
                        !map.Configuration.MappedTypes.Intersect(allManagedTypes)
                             .Except(managedTypesAlreadyMapped)
                             .Any()).ToList();

                result.AddRange(mappingsWithNoDependency);
                missingMappings = missingMappings.Except(mappingsWithNoDependency).ToList();

                managedTypesAlreadyMapped.AddRange(
                    mappingsWithNoDependency.Select(map => map.Configuration.ManagedType));
            }

            return result;
        }
    }
}