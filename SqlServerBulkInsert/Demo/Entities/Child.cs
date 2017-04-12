//  --------------------------------------------------------------------------------------------------------------------
//  <copyright file="Child.cs" company="Eurofins">
//    Copyright (c) Eurofins. All rights reserved.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace Demo.Entities
{
    using System;

    public class Child
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Parent Parent { get; set; }

        public byte[] RowVersion { get; set; }
    }
}