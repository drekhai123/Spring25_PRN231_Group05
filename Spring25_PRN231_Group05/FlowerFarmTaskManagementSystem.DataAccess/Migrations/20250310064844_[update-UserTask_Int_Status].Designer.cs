﻿// <auto-generated />
using System;
using FlowerFarmTaskManagementSystem.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FlowerFarmTaskManagementSystem.DataAccess.Migrations
{
    [DbContext(typeof(FlowerFarmTaskManagementSystemDbContext))]
    [Migration("20250310064844_[update-UserTask_Int_Status]")]
    partial class updateUserTask_Int_Status
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.Category", b =>
                {
                    b.Property<Guid>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CategoryImageUrl")
                        .HasColumnType("longtext");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.FarmToolCategories", b =>
                {
                    b.Property<Guid>("FarmToolCategoriesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FarmToolCategoriesDescription")
                        .HasColumnType("longtext");

                    b.Property<string>("FarmToolCategoriesName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("FarmToolCategoriesId");

                    b.ToTable("FarmToolCategories");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.FarmTools", b =>
                {
                    b.Property<Guid>("FarmToolsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("FarmToolCategoriesId")
                        .HasColumnType("char(36)");

                    b.Property<string>("FarmToolsDetails")
                        .HasColumnType("longtext");

                    b.Property<string>("FarmToolsName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("FarmToolsQuantity")
                        .HasColumnType("int");

                    b.Property<string>("FarmToolsUnit")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("FarmToolsId");

                    b.HasIndex("FarmToolCategoriesId");

                    b.ToTable("FarmTools");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.FarmToolsOfTask", b =>
                {
                    b.Property<Guid>("FarmToolsOfTaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("FarmToolsId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("UserTaskId")
                        .HasColumnType("char(36)");

                    b.HasKey("FarmToolsOfTaskId");

                    b.HasIndex("FarmToolsId");

                    b.HasIndex("UserTaskId");

                    b.ToTable("FarmToolsOfTasks");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.Field", b =>
                {
                    b.Property<Guid>("FieldId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FieldImageUrl")
                        .HasColumnType("longtext");

                    b.Property<string>("FieldName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<double>("Length")
                        .HasColumnType("double");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("Width")
                        .HasColumnType("double");

                    b.HasKey("FieldId");

                    b.ToTable("Fields");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.Product", b =>
                {
                    b.Property<Guid>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Note")
                        .HasColumnType("longtext");

                    b.Property<string>("ProductImageUrl")
                        .HasColumnType("longtext");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.ProductField", b =>
                {
                    b.Property<Guid>("ProductFieldId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("FieldId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("char(36)");

                    b.Property<double>("Productivity")
                        .HasColumnType("double");

                    b.Property<string>("ProductivityUnit")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ProductFieldId");

                    b.HasIndex("FieldId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductFields");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.Supplier", b =>
                {
                    b.Property<Guid>("SupplierId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("SupplierAddress")
                        .HasColumnType("longtext");

                    b.Property<string>("SupplierEmail")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("SupplierName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("SupplierPhone")
                        .HasColumnType("longtext");

                    b.HasKey("SupplierId");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.TaskWork", b =>
                {
                    b.Property<Guid>("TaskWorkId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("AssignedBy")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("AssignedTo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("longtext");

                    b.Property<string>("JobTitle")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("ProductFieldId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("TaskWorkId");

                    b.HasIndex("ProductFieldId");

                    b.ToTable("TaskWorks");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.TypeOfSupplier", b =>
                {
                    b.Property<Guid>("TypeOfSupplierId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("SupplierId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("TypeSupplierId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("TypeOfSupplierId");

                    b.HasIndex("SupplierId");

                    b.HasIndex("TypeSupplierId");

                    b.ToTable("TypeOfSuppliers");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.TypeSupplier", b =>
                {
                    b.Property<Guid>("TypeSupplierId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("TypeStatus")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("TypeSupplierDescription")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("TypeSupplierImageUrl")
                        .HasColumnType("longtext");

                    b.Property<string>("TypeSupplierName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("TypeSupplierId");

                    b.ToTable("TypeSuppliers");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Experience")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Phone")
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("WorkPosition")
                        .HasColumnType("longtext");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.UserTask", b =>
                {
                    b.Property<Guid>("UserTaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("FarmToolsId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("TaskWorkId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("UpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<string>("UserTaskDescription")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("UserTaskId");

                    b.HasIndex("FarmToolsId");

                    b.HasIndex("TaskWorkId");

                    b.HasIndex("UserId");

                    b.ToTable("UserTasks");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.FarmTools", b =>
                {
                    b.HasOne("FlowerFarmTaskManagementSystem.BusinessObject.Models.FarmToolCategories", "FarmToolCategories")
                        .WithMany()
                        .HasForeignKey("FarmToolCategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FarmToolCategories");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.FarmToolsOfTask", b =>
                {
                    b.HasOne("FlowerFarmTaskManagementSystem.BusinessObject.Models.FarmTools", "FarmTools")
                        .WithMany()
                        .HasForeignKey("FarmToolsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FlowerFarmTaskManagementSystem.BusinessObject.Models.UserTask", "UserTask")
                        .WithMany()
                        .HasForeignKey("UserTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FarmTools");

                    b.Navigation("UserTask");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.Product", b =>
                {
                    b.HasOne("FlowerFarmTaskManagementSystem.BusinessObject.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.ProductField", b =>
                {
                    b.HasOne("FlowerFarmTaskManagementSystem.BusinessObject.Models.Field", "Field")
                        .WithMany()
                        .HasForeignKey("FieldId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FlowerFarmTaskManagementSystem.BusinessObject.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Field");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.TaskWork", b =>
                {
                    b.HasOne("FlowerFarmTaskManagementSystem.BusinessObject.Models.ProductField", "ProductField")
                        .WithMany()
                        .HasForeignKey("ProductFieldId");

                    b.Navigation("ProductField");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.TypeOfSupplier", b =>
                {
                    b.HasOne("FlowerFarmTaskManagementSystem.BusinessObject.Models.Supplier", "Supplier")
                        .WithMany()
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FlowerFarmTaskManagementSystem.BusinessObject.Models.TypeSupplier", "TypeSupplier")
                        .WithMany()
                        .HasForeignKey("TypeSupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Supplier");

                    b.Navigation("TypeSupplier");
                });

            modelBuilder.Entity("FlowerFarmTaskManagementSystem.BusinessObject.Models.UserTask", b =>
                {
                    b.HasOne("FlowerFarmTaskManagementSystem.BusinessObject.Models.FarmTools", "FarmTools")
                        .WithMany()
                        .HasForeignKey("FarmToolsId");

                    b.HasOne("FlowerFarmTaskManagementSystem.BusinessObject.Models.TaskWork", "TaskWork")
                        .WithMany()
                        .HasForeignKey("TaskWorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FlowerFarmTaskManagementSystem.BusinessObject.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FarmTools");

                    b.Navigation("TaskWork");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
