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
    [Migration("20240831074127_Some")]
    partial class Some
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
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

            modelBuilder.Entity("SICalcWebApp.Areas.SICalculator.Models.InputOperand", b =>
                {
                    b.Property<int>("ProductID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductID"));

                    b.Property<decimal>("FeMSponge")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("FeT")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("FineLoss")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("FinesRealisation")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("FinesRealisationValue")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Gangue")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("GroundLoss")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("IronPrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("IronTypeId")
                        .HasColumnType("int");

                    b.Property<decimal>("Moisture")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Phos")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("Sidename")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TransferLoss")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("YLoss")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("Yield")
                        .HasPrecision(18, 4)
                        .HasColumnType("decimal(18,4)");

                    b.HasKey("ProductID");

                    b.HasIndex("IronTypeId");

                    b.ToTable("InputOperands");
                });

            modelBuilder.Entity("SICalcWebApp.Areas.SICalculator.Models.IronType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("IronTypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("IronTypes");
                });

            modelBuilder.Entity("SICalcWebApp.Areas.SICalculator.Models.PriceOfMaterial", b =>
                {
                    b.Property<int>("PriceInputId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PriceInputId"));

                    b.Property<decimal>("DolomiteRate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("FixedCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("MgfExpence")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("PriceInputId");

                    b.ToTable("PriceOfMaterials");
                });

            modelBuilder.Entity("SICalcWebApp.Areas.SICalculator.Models.TPDInfo", b =>
                {
                    b.Property<int>("TPDId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TPDId"));

                    b.Property<string>("KilnName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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

            modelBuilder.Entity("SICalcWebApp.Areas.SICalculator.Models.InputOperand", b =>
                {
                    b.HasOne("SICalcWebApp.Areas.SICalculator.Models.IronType", "IronType")
                        .WithMany()
                        .HasForeignKey("IronTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("IronType");
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
