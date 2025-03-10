﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using THYWebApp.Data;

#nullable disable

namespace THYWebApp.Migrations
{
    [DbContext(typeof(THYContext))]
    partial class THYContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("THYWebApp.Models.Aircraft", b =>
                {
                    b.Property<int>("AircraftID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AircraftID"));

                    b.Property<string>("AircraftManufacturingCompany")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AircraftModel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AircraftName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("AircraftProductionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("AircraftSeatCapacity")
                        .HasColumnType("int");

                    b.HasKey("AircraftID");

                    b.ToTable("Aircraft");
                });

            modelBuilder.Entity("THYWebApp.Models.AircraftErrorReport", b =>
                {
                    b.Property<int>("AircraftErrorReportID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AircraftErrorReportID"));

                    b.Property<string>("AircraftErrorDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("AircraftReportDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("AircraftStatusID")
                        .HasColumnType("int");

                    b.HasKey("AircraftErrorReportID");

                    b.ToTable("AircraftErrorReport");
                });

            modelBuilder.Entity("THYWebApp.Models.AircraftStatus", b =>
                {
                    b.Property<int>("AircraftStatusID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AircraftStatusID"));

                    b.Property<decimal>("AircraftFuelLevel")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("AircraftID")
                        .HasColumnType("int");

                    b.Property<DateTime>("AircraftLastMaintenanceDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("AirplaneStatus")
                        .HasColumnType("bit");

                    b.HasKey("AircraftStatusID");

                    b.ToTable("AircraftStatus");
                });

            modelBuilder.Entity("THYWebApp.Models.Airport", b =>
                {
                    b.Property<int>("AirportID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AirportID"));

                    b.Property<string>("AirportCity")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AirportCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AirportCountry")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AirportName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AirportID");

                    b.ToTable("Airport");
                });

            modelBuilder.Entity("THYWebApp.Models.AirportGate", b =>
                {
                    b.Property<int>("AirportGateID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AirportGateID"));

                    b.Property<string>("AirportGateNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AirportID")
                        .HasColumnType("int");

                    b.HasKey("AirportGateID");

                    b.ToTable("AirportGate");
                });

            modelBuilder.Entity("THYWebApp.Models.Baggage", b =>
                {
                    b.Property<int>("BaggageID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BaggageID"));

                    b.Property<string>("BaggageStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BaggageType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("BaggageWeight")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("FlightID")
                        .HasColumnType("int");

                    b.Property<int?>("NonMemberCustomerID")
                        .HasColumnType("int");

                    b.Property<int?>("UserID")
                        .HasColumnType("int");

                    b.HasKey("BaggageID");

                    b.ToTable("Baggage");
                });

            modelBuilder.Entity("THYWebApp.Models.BaggageGate", b =>
                {
                    b.Property<int>("BaggageGateID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BaggageGateID"));

                    b.Property<int>("AirportID")
                        .HasColumnType("int");

                    b.Property<string>("BaggageGateNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BaggageGateID");

                    b.ToTable("BaggageGate");
                });

            modelBuilder.Entity("THYWebApp.Models.BaggagePrice", b =>
                {
                    b.Property<int>("BaggagePriceID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BaggagePriceID"));

                    b.Property<int>("BaggageWeight")
                        .HasColumnType("int");

                    b.Property<decimal>("BaggageWeightPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("BaggagePriceID");

                    b.ToTable("BaggagePrice");
                });

            modelBuilder.Entity("THYWebApp.Models.Flight", b =>
                {
                    b.Property<int>("FlightID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FlightID"));

                    b.Property<int>("AircraftID")
                        .HasColumnType("int");

                    b.Property<int>("ArrivalAirportID")
                        .HasColumnType("int");

                    b.Property<int>("ArrivalGateID")
                        .HasColumnType("int");

                    b.Property<int>("DepartureAirportID")
                        .HasColumnType("int");

                    b.Property<int>("DepartureGateID")
                        .HasColumnType("int");

                    b.Property<string>("FlighNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FlightAirlineCompany")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FlightArrivalDateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FlightDelayTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FlightDepartureDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("FlightPriceID")
                        .HasColumnType("int");

                    b.Property<string>("FlightStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FlightID");

                    b.ToTable("Flight");
                });

            modelBuilder.Entity("THYWebApp.Models.FlightPrice", b =>
                {
                    b.Property<int>("FlightPriceID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FlightPriceID"));

                    b.Property<decimal>("FlightPriceAmount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("FlightPriceID");

                    b.ToTable("FlightPrice");
                });

            modelBuilder.Entity("THYWebApp.Models.FlyPackets", b =>
                {
                    b.Property<int>("FlyPacketsID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FlyPacketsID"));

                    b.Property<string>("BusinessFlight")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExtraFlight")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PremiumFlight")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegularFlight")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FlyPacketsID");

                    b.ToTable("FlyPacket");
                });

            modelBuilder.Entity("THYWebApp.Models.LoginRecord", b =>
                {
                    b.Property<int>("LoginRecordID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LoginRecordID"));

                    b.Property<DateTime>("LoginDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LoginMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("LoginRecordID");

                    b.ToTable("LoginRecord");
                });

            modelBuilder.Entity("THYWebApp.Models.Memberr", b =>
                {
                    b.Property<int>("MemberID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MemberID"));

                    b.Property<DateTime>("MemberJoinDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("MemberMiles")
                        .HasColumnType("int");

                    b.Property<string>("MembershipNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("MemberID");

                    b.ToTable("Memberr");
                });

            modelBuilder.Entity("THYWebApp.Models.NonMemberCustomer", b =>
                {
                    b.Property<int>("NonMemberCustomerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NonMemberCustomerID"));

                    b.Property<string>("PassengerAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PassengerDateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("PassengerEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassengerFirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassengerLastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassengerMiddleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassengerNationality")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassengerPassportNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassengerPhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassengerTCKimlik")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NonMemberCustomerID");

                    b.ToTable("NonMemberCustomer");
                });

            modelBuilder.Entity("THYWebApp.Models.Payment", b =>
                {
                    b.Property<int>("PaymentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentID"));

                    b.Property<decimal>("PaymentAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TicketID")
                        .HasColumnType("int");

                    b.HasKey("PaymentID");

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("THYWebApp.Models.Reservation", b =>
                {
                    b.Property<int>("ReservationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReservationID"));

                    b.Property<int>("FlightID")
                        .HasColumnType("int");

                    b.Property<int>("FlyPacketsID")
                        .HasColumnType("int");

                    b.Property<int>("NonMemberCustomerID")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReservationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReservationStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ReservationID");

                    b.ToTable("Reservation");
                });

            modelBuilder.Entity("THYWebApp.Models.Ticket", b =>
                {
                    b.Property<int>("TicketID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TicketID"));

                    b.Property<int>("FlightID")
                        .HasColumnType("int");

                    b.Property<int>("ReservationID")
                        .HasColumnType("int");

                    b.Property<decimal>("TicketPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("TicketStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TicketID");

                    b.ToTable("Ticket");
                });

            modelBuilder.Entity("THYWebApp.Models.UserType", b =>
                {
                    b.Property<int>("UserTypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserTypeID"));

                    b.Property<string>("UserTypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserTypes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserTypeID");

                    b.ToTable("UserType");
                });

            modelBuilder.Entity("THYWebApp.Models.Users", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"));

                    b.Property<string>("UserAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UserDateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserLastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserMiddleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserNationality")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserPassportNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserPhone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserTCKimlik")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<int>("UserTypeID")
                        .HasColumnType("int");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
