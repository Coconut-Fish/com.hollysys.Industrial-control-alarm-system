﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using com.hollysys.Industrial_control_alarm_system.Data;

#nullable disable

namespace com.hollysys.Industrial_control_alarm_system.Migrations
{
    [DbContext(typeof(IndustrialControlAlarmSystemContext))]
    partial class IndustrialControlAlarmSystemContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("com.hollysys.Industrial_control_alarm_system.Models.Alarm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AdditionalInfo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("AlarmTime")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ConfirmTime")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsRecovered")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<DateTime?>("RecoverTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Alarm", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}