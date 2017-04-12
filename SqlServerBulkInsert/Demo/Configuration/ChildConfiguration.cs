//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ChildConfiguration.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace Demo.Configuration
{
    using System.Data.Entity.ModelConfiguration;

    using Demo.Entities;

    public class ChildConfiguration : EntityTypeConfiguration<Child>
    {
        public ChildConfiguration()
        {
            // table name
            this.ToTable("Children");

            // Key and relationships
            this.HasKey(a => a.Id);
            this.HasRequired(a => a.Parent)
                .WithMany(o => o.Children)
                .Map(configuration => configuration.MapKey("parentId"));

            // Column mappings
            this.Property(a => a.Id).HasColumnName("childId");
            this.Property(a => a.Name).HasColumnName("childName");

            // Row Version
            this.Property(p => p.RowVersion).IsRowVersion();
        }
    }
}