//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BulkInsertWriter.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace SqlServerBulkInsert
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using SqlServerBulkInsert.Mapping;
    using SqlServerBulkInsert.Options;

    public class BulkInsertWriter<T> : SqlServerBulkInsert<T>, IBulkInsertWriter
    {
        #region Constructors and Destructors

        public BulkInsertWriter(AbstractMap<T> mapping)
            : base(mapping)
        {
            this.Configuration = mapping as IBulkInsertConfiguration;
            this.ObjectsToWrite = new List<object>();
        }

        public BulkInsertWriter(AbstractMap<T> mapping, BulkCopyOptions options)
            : base(mapping, options)
        {
            this.Configuration = mapping as IBulkInsertConfiguration;
            this.ObjectsToWrite = new List<object>();
        }

        #endregion

        #region Properties

        public IBulkInsertConfiguration Configuration { get; private set; }

        public IList<object> ObjectsToWrite { get; private set; }

        #endregion

        public void Write(SqlConnection connection)
        {
            if (!this.ObjectsToWrite.Any())
            {
                return;
            }

            this.Write(connection, this.ObjectsToWrite.Cast<T>());
        }
    }
}