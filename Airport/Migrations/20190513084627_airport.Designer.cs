﻿// <auto-generated />
using System;
using Airport.DL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Airport.Migrations
{
    [DbContext(typeof(AirportContext))]
    [Migration("20190513084627_airport")]
    partial class airport
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity("Airport.DL.Arrival", b =>
                {
                    b.Property<int>("ArrivalId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Number");

                    b.Property<DateTime>("Time");

                    b.HasKey("ArrivalId");

                    b.ToTable("Arrivals");
                });

            modelBuilder.Entity("Airport.DL.Departure", b =>
                {
                    b.Property<int>("DepartureId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Number");

                    b.Property<DateTime>("Time");

                    b.HasKey("DepartureId");

                    b.ToTable("Departures");
                });

            modelBuilder.Entity("Airport.DL.FlightMove", b =>
                {
                    b.Property<int>("FlightMoveId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Finish");

                    b.Property<int>("LineNumber");

                    b.Property<string>("Number");

                    b.Property<DateTime>("Start");

                    b.HasKey("FlightMoveId");

                    b.ToTable("Moves");
                });

            modelBuilder.Entity("Airport.DL.LineDB", b =>
                {
                    b.Property<int>("LineId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Direction");

                    b.Property<bool>("IsBusy");

                    b.Property<int>("LineSnapshotId");

                    b.Property<int>("Number");

                    b.HasKey("LineId");

                    b.HasIndex("LineSnapshotId");

                    b.ToTable("Lines");
                });

            modelBuilder.Entity("Airport.DL.LineSnapshot", b =>
                {
                    b.Property<int>("LineSnapshotId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Time");

                    b.HasKey("LineSnapshotId");

                    b.ToTable("LinesSnapshots");
                });

            modelBuilder.Entity("Airport.DL.LineDB", b =>
                {
                    b.HasOne("Airport.DL.LineSnapshot", "LineSnapshot")
                        .WithMany("Lines")
                        .HasForeignKey("LineSnapshotId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
