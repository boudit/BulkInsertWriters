//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BulkInsertPropertyInfo.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace SqlServerBulkInsert
{
    using System.Reflection;

    public class BulkInsertPropertyInfo
    {
        #region Constructors and Destructors

        public BulkInsertPropertyInfo(PropertyInfo propertyInfo, bool isNavigationProperty = false)
        {
            this.PropertyInfo = propertyInfo;
            this.IsNavigationProperty = isNavigationProperty;
        }

        #endregion

        #region Properties

        public PropertyInfo PropertyInfo { get; private set; }

        public bool IsNavigationProperty { get; set; }

        #endregion
    }
}