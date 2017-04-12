//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ParentConfiguration.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace Demo.Configuration
{
    using System.Data.Entity.ModelConfiguration;

    using Demo.Entities;

    public class ParentConfiguration : EntityTypeConfiguration<Parent>
    {
        public ParentConfiguration()
        {
            // table name
            this.ToTable("Parents");

            // Key and relationships
            this.HasKey(a => a.Id);

            // Column mappings
            this.Property(a => a.Id).HasColumnName("parentId");
            this.Property(a => a.Name).HasColumnName("parentName");
            this.Property(a => a.ValueInt).HasColumnName("valueInt");

            // Row Version
            this.Property(p => p.RowVersion).IsRowVersion();
        }
    }
}