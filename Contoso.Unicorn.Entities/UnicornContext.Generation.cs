﻿

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated.
// 
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once IdentifierTypo
namespace Contoso.Unicorn.Entities
{
    /// <inheritdoc />
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    [GeneratedCode("TextTemplatingFileGenerator", "1.0.0")]
    public partial class UnicornContext : DbContext
    {
        /// <summary>
        /// Gets or sets table category.
        /// </summary>
        public virtual DbSet<CategoryEntity> Categories { get; set; }

        /// <summary>
        /// Gets or sets table customer.
        /// </summary>
        public virtual DbSet<CustomerEntity> Customers { get; set; }

        /// <summary>
        /// Gets or sets table employee.
        /// </summary>
        public virtual DbSet<EmployeeEntity> Employees { get; set; }

        /// <summary>
        /// Gets or sets table order.
        /// </summary>
        public virtual DbSet<OrderEntity> Orders { get; set; }

        /// <summary>
        /// Gets or sets table order detail.
        /// </summary>
        public virtual DbSet<OrderDetailEntity> OrderDetails { get; set; }

        /// <summary>
        /// Gets or sets table product.
        /// </summary>
        public virtual DbSet<ProductEntity> Products { get; set; }

        /// <summary>
        /// Gets or sets table shipper.
        /// </summary>
        public virtual DbSet<ShipperEntity> Shippers { get; set; }

        /// <summary>
        /// Gets or sets table supplier.
        /// </summary>
        public virtual DbSet<SupplierEntity> Suppliers { get; set; }
    }
}
