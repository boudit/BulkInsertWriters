//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="DemoContext.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace Demo.Context
{
    using System.Data.Entity;

    using Demo.Configuration;

    public class DemoContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Adding configurations
            modelBuilder.Configurations.Add(new ParentConfiguration());
            modelBuilder.Configurations.Add(new ChildConfiguration());
        }
    }
}