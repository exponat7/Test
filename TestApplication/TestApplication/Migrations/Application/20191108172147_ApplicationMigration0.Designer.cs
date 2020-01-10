﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TestApplication.Subsystems.Database;

namespace TestApplication.Migrations.Application
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20191108172147_ApplicationMigration0")]
    partial class ApplicationMigration0
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("TestApplication.Models.Request.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("TestApplication.Models.RequestJournal.RequestsJournalRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CurrentState");

                    b.Property<int?>("RequestId");

                    b.Property<DateTime>("StateChanged");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.ToTable("JournalRecords");
                });

            modelBuilder.Entity("TestApplication.Models.RequestJournal.RequestsJournalRecord", b =>
                {
                    b.HasOne("TestApplication.Models.Request.Request", "Request")
                        .WithMany("JournalRecords")
                        .HasForeignKey("RequestId");
                });
#pragma warning restore 612, 618
        }
    }
}