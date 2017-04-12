//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="IBulkInsertConfiguration.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace SqlServerBulkInsert
{
    using System;
    using System.Collections.Generic;

    public interface IBulkInsertConfiguration
    {
        #region Properties

        Type ManagedType { get; }

        IEnumerable<Type> MappedTypes { get; }

        #endregion
    }
}