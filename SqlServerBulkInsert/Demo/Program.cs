//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="Program.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace Demo
{
    using System.Data.SqlClient;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Demo.Context;

    using SqlServerBulkInsert;

    public class Program
    {
        private static DemoContext context;

        private static void Main(string[] args)
        {
            context = new DemoContext();
            
            var bulkInserts = BulkInsertExtractor.GetOrderedBulkInsertWriters(context).ToList();

            Enumerable.Range(1, 1000).Select(
                i =>
                    {
                        DataProvider.CreateParent(bulkInserts);
                        return i;
                    });


            using (var sqlConnection = new SqlConnection(context.Database.Connection.ConnectionString))
            {
                sqlConnection.Open();

                foreach (var bulkInsert in bulkInserts)
                {
                    bulkInsert.Write(sqlConnection);
                }

                sqlConnection.Close();
            }
        }
    }
}