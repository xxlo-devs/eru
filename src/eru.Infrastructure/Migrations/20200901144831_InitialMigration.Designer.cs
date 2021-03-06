﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eru.Infrastructure.Persistence;

namespace eru.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200901144831_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7");

            modelBuilder.Entity("eru.Domain.Entity.Class", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.Property<string>("Section")
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Classes");
                });

            modelBuilder.Entity("eru.Domain.Entity.Subscriber", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.Property<string>("Platform")
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.Property<string>("Class")
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.Property<string>("PreferredLanguage")
                        .HasColumnType("TEXT")
                        .HasMaxLength(255);

                    b.HasKey("Id", "Platform");

                    b.ToTable("Subscribers");
                });
#pragma warning restore 612, 618
        }
    }
}
