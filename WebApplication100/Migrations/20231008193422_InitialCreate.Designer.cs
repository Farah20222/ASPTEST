﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication100.Models;

#nullable disable

namespace WebApplication100.Migrations
{
    [DbContext(typeof(AssignmentDBContext))]
    [Migration("20231008193422_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebApplication100.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<bool?>("Availability")
                        .HasColumnType("bit");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedTime")
                        .HasColumnType("datetime2");

                    b.HasKey("ProductId");

                    b.HasIndex("CreatedBy");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("WebApplication100.Models.ProductVendor", b =>
                {
                    b.Property<int>("ProductVendorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductVendorId"));

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int?>("UserProfileId")
                        .HasColumnType("int");

                    b.HasKey("ProductVendorId");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserProfileId");

                    b.ToTable("ProductVendors");
                });

            modelBuilder.Entity("WebApplication100.Models.Purchase", b =>
                {
                    b.Property<int>("PurchaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PurchaseId"));

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<decimal?>("PurchaseCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("PurchasedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserProfileId")
                        .HasColumnType("int");

                    b.HasKey("PurchaseId");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("WebApplication100.Models.UserProfile", b =>
                {
                    b.Property<int>("UserProfileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserProfileId"));

                    b.Property<DateTime?>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ForgetCreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ForgetPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordSalt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Phone")
                        .HasColumnType("int");

                    b.Property<string>("RoleName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserProfileId");

                    b.ToTable("UserProfiles");
                });

            modelBuilder.Entity("WebApplication100.Models.Product", b =>
                {
                    b.HasOne("WebApplication100.Models.UserProfile", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedBy");

                    b.Navigation("CreatedByUser");
                });

            modelBuilder.Entity("WebApplication100.Models.ProductVendor", b =>
                {
                    b.HasOne("WebApplication100.Models.Product", "Product")
                        .WithMany("ProductVendors")
                        .HasForeignKey("ProductId");

                    b.HasOne("WebApplication100.Models.UserProfile", "UserProfile")
                        .WithMany()
                        .HasForeignKey("UserProfileId");

                    b.Navigation("Product");

                    b.Navigation("UserProfile");
                });

            modelBuilder.Entity("WebApplication100.Models.Product", b =>
                {
                    b.Navigation("ProductVendors");
                });
#pragma warning restore 612, 618
        }
    }
}
