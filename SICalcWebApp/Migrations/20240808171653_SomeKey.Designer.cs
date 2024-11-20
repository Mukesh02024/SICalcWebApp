﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SICalcWebApp.Data;

#nullable disable

namespace SICalcWebApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240808171653_SomeKey")]
    partial class SomeKey
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SICalcWebApp.Areas.SICalculator.Models.FC", b =>
                {
                    b.Property<int>("FCId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FCId"));

                    b.Property<decimal>("C_Fe")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("FCValue")
                        .HasColumnType("int");

                    b.HasKey("FCId");

                    b.ToTable("FCs");
                });

            modelBuilder.Entity("SICalcWebApp.Areas.SICalculator.Models.FCInfo", b =>
                {
                    b.Property<int>("FCInfoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FCInfoId"));

                    b.Property<int>("FCId")
                        .HasColumnType("int");

                    b.Property<decimal>("FeedRate")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TPDId")
                        .HasColumnType("int");

                    b.HasKey("FCInfoId");

                    b.HasIndex("FCId");

                    b.HasIndex("TPDId");

                    b.ToTable("FCInfos");
                });

            modelBuilder.Entity("SICalcWebApp.Areas.SICalculator.Models.TPDInfo", b =>
                {
                    b.Property<int>("TPDId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TPDId"));

                    b.Property<int>("KilnName")
                        .HasColumnType("int");

                    b.Property<int>("TPD")
                        .HasColumnType("int");

                    b.HasKey("TPDId");

                    b.ToTable("TPDInfos");
                });

            modelBuilder.Entity("SICalcWebApp.Areas.SICalculator.Models.FCInfo", b =>
                {
                    b.HasOne("SICalcWebApp.Areas.SICalculator.Models.FC", "FC")
                        .WithMany("FCInfos")
                        .HasForeignKey("FCId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SICalcWebApp.Areas.SICalculator.Models.TPDInfo", "TPDInfo")
                        .WithMany("FCInfos")
                        .HasForeignKey("TPDId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FC");

                    b.Navigation("TPDInfo");
                });

            modelBuilder.Entity("SICalcWebApp.Areas.SICalculator.Models.FC", b =>
                {
                    b.Navigation("FCInfos");
                });

            modelBuilder.Entity("SICalcWebApp.Areas.SICalculator.Models.TPDInfo", b =>
                {
                    b.Navigation("FCInfos");
                });
#pragma warning restore 612, 618
        }
    }
}