﻿// <auto-generated />
using System;
using CraApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CraApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240626163916_reloadDB")]
    partial class reloadDB
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CraApp.Model.Activity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Day")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("time(0)");

                    b.Property<int>("MonthlyActivitiesId")
                        .HasColumnType("int");

                    b.Property<int>("Project")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("time(0)");

                    b.HasKey("Id");

                    b.HasIndex("MonthlyActivitiesId");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("CraApp.Model.MonthlyActivities", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Month")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("MonthlyActivities");
                });

            modelBuilder.Entity("CraApp.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Ahmed",
                            Password = "Password123#",
                            Role = "admin",
                            UserName = "shiinoo"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Marouane",
                            Password = "Password123#",
                            Role = "admin",
                            UserName = "PipInstallGeek"
                        });
                });

            modelBuilder.Entity("CraApp.Model.Activity", b =>
                {
                    b.HasOne("CraApp.Model.MonthlyActivities", "MonthlyActivities")
                        .WithMany("Activities")
                        .HasForeignKey("MonthlyActivitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MonthlyActivities");
                });

            modelBuilder.Entity("CraApp.Model.MonthlyActivities", b =>
                {
                    b.Navigation("Activities");
                });
#pragma warning restore 612, 618
        }
    }
}
