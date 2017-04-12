//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="Parent.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace Demo.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class Parent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int ValueInt { get; set; }

        public ICollection<Child> Children { get; set; }

        public byte[] RowVersion { get; set; }
    }
}