﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using eru.Domain.Entity;
using eru.Infrastructure.Persistence;

namespace eru.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200912132616_ChangeSubstitutionsTypeToJsonInSubstitutionsRecord")]
    partial class ChangeSubstitutionsTypeToJsonInSubstitutionsRecord
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("eru.Domain.Entity.Class", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Section")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("SubstitutionId")
                        .HasColumnType("character varying(255)");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SubstitutionId");

                    b.ToTable("Classes");
                });

            modelBuilder.Entity("eru.Domain.Entity.Subscriber", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Platform")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Class")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("PreferredLanguage")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id", "Platform");

                    b.ToTable("Subscribers");
                });

            modelBuilder.Entity("eru.Domain.Entity.Substitution", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<bool>("Cancelled")
                        .HasColumnType("boolean");

                    b.Property<string>("Groups")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<int>("Lesson")
                        .HasColumnType("integer");

                    b.Property<string>("Note")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Room")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Subject")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Substituting")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.Property<string>("Teacher")
                        .HasColumnType("character varying(255)")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Substitution");
                });

            modelBuilder.Entity("eru.Domain.Entity.SubstitutionsRecord", b =>
                {
                    b.Property<DateTime>("UploadDateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<IEnumerable<Substitution>>("Substitutions")
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("SubstitutionsDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("UploadDateTime");

                    b.ToTable("SubstitutionsRecords");
                });

            modelBuilder.Entity("eru.Domain.Entity.Class", b =>
                {
                    b.HasOne("eru.Domain.Entity.Substitution", null)
                        .WithMany("Classes")
                        .HasForeignKey("SubstitutionId");
                });
#pragma warning restore 612, 618
        }
    }
}
