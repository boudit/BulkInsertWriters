//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="DataProvider.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace Demo.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Demo.Entities;

    using SqlServerBulkInsert;

    public static class DataProvider
    {
        private static readonly Random Random = new Random();

        private static readonly List<Parent> Parents = new List<Parent>();

        public static IEnumerable<Parent> GetParents()
        {
            return Parents;
        }

        public static IEnumerable<Child> GetChildren()
        {
            return GetParents().SelectMany(p => p.Children).ToList();
        }

        public static Parent CreateParent(IEnumerable<IBulkInsertWriter> bulkInserts = null)
        {
            var guid = Guid.NewGuid();

            var result = new Parent
            {
                Id = guid,
                Name = GenerateRandomString(10),
                ValueInt = GenerateRandomInt(50),
            };

            AddEntityToBulkInsertWriter(bulkInserts, result);

            result.Children = Enumerable.Range(1, GenerateRandomInt(50)).Select(a => CreateChild(result, bulkInserts)).ToList();

            Parents.Add(result);

            return result;
        }

        public static Child CreateChild(Parent parent, IEnumerable<IBulkInsertWriter> bulkInserts = null)
        {
            var guid = Guid.NewGuid();

            var result = new Child
            {
                Id = guid,
                Name = GenerateRandomString(10),
                Parent = parent,
            };

            AddEntityToBulkInsertWriter(bulkInserts, result);
            
            return result;
        }

        private static void AddEntityToBulkInsertWriter(IEnumerable<IBulkInsertWriter> bulkInserts, object result)
        {
            if (bulkInserts == null)
            {
                return;
            }

            var typeOfEntity = result.GetType();

            var bulkInsert = bulkInserts.FirstOrDefault(bi => bi.Configuration.ManagedType == typeOfEntity);

            if (bulkInsert != null)
            {
                bulkInsert.ObjectsToWrite.Add(result);
            }
        }

        private static string GenerateRandomString(int length)
        {
            int min = 40;
            int max = 122;

            return new string(Enumerable.Range(1, length).Select(i => (char)Random.Next(40, 122)).ToArray());
        }

        private static int GenerateRandomInt(int maxValue)
        {
            return Random.Next(1, maxValue);
        }
    }
}