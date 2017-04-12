//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IBulkInsertWriter.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace SqlServerBulkInsert
{
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public interface IBulkInsertWriter
    {
        #region Properties

        IBulkInsertConfiguration Configuration { get; }

        IList<object> ObjectsToWrite { get; }

        #endregion

        void Write(SqlConnection connection);
    }
}