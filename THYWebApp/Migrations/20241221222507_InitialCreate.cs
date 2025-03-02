using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace THYWebApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aircraft",
                columns: table => new
                {
                    AircraftID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AircraftName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AircraftModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AircraftProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AircraftSeatCapacity = table.Column<int>(type: "int", nullable: false),
                    AircraftManufacturingCompany = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aircraft", x => x.AircraftID);
                });
        
            migrationBuilder.CreateTable(
                name: "AircraftErrorReport",
                columns: table => new
                {
                    AircraftErrorReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AircraftStatusID = table.Column<int>(type: "int", nullable: false),
                    AircraftErrorDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AircraftReportDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AircraftErrorReport", x => x.AircraftErrorReportID);
                });

            migrationBuilder.CreateTable(
                name: "AircraftStatus",
                columns: table => new
                {
                    AircraftStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AircraftID = table.Column<int>(type: "int", nullable: false),
                    AircraftFuelLevel = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AircraftLastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AirplaneStatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AircraftStatus", x => x.AircraftStatusID);
                });

            migrationBuilder.CreateTable(
                name: "Airport",
                columns: table => new
                {
                    AirportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AirportName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AirportCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AirportCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AirportCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airport", x => x.AirportID);
                });

            migrationBuilder.CreateTable(
                name: "AirportGate",
                columns: table => new
                {
                    AirportGateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AirportID = table.Column<int>(type: "int", nullable: false),
                    AirportGateNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirportGate", x => x.AirportGateID);
                });

            migrationBuilder.CreateTable(
                name: "Baggage",
                columns: table => new
                {
                    BaggageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NonMemberCustomerID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    FlightID = table.Column<int>(type: "int", nullable: false),
                    BaggageWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BaggageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaggageStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baggage", x => x.BaggageID);
                });

            migrationBuilder.CreateTable(
                name: "BaggageGate",
                columns: table => new
                {
                    BaggageGateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AirportID = table.Column<int>(type: "int", nullable: false),
                    BaggageGateNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaggageGate", x => x.BaggageGateID);
                });

            migrationBuilder.CreateTable(
                name: "BaggagePrice",
                columns: table => new
                {
                    BaggagePriceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaggageWeight = table.Column<int>(type: "int", nullable: false),
                    BaggageWeightPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaggagePrice", x => x.BaggagePriceID);
                });

            migrationBuilder.CreateTable(
                name: "Flight",
                columns: table => new
                {
                    FlightID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlighNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AircraftID = table.Column<int>(type: "int", nullable: false),
                    DepartureAirportID = table.Column<int>(type: "int", nullable: false),
                    ArrivalAirportID = table.Column<int>(type: "int", nullable: false),
                    FlightDepartureDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlightArrivalDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlightDelayTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartureGateID = table.Column<int>(type: "int", nullable: false),
                    ArrivalGateID = table.Column<int>(type: "int", nullable: false),
                    FlightStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightAirlineCompany = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightPriceID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flight", x => x.FlightID);
                });

            migrationBuilder.CreateTable(
                name: "FlightPrice",
                columns: table => new
                {
                    FlightPriceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightPriceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightPrice", x => x.FlightPriceID);
                });

            migrationBuilder.CreateTable(
                name: "FlyPacket",
                columns: table => new
                {
                    FlyPacketsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegularFlight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraFlight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PremiumFlight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessFlight = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlyPacket", x => x.FlyPacketsID);
                });

            migrationBuilder.CreateTable(
                name: "LoginRecord",
                columns: table => new
                {
                    LoginRecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    LoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoginMethod = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginRecord", x => x.LoginRecordID);
                });

            migrationBuilder.CreateTable(
                name: "Memberr",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    MembershipNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemberMiles = table.Column<int>(type: "int", nullable: false),
                    MemberJoinDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberr", x => x.MemberID);
                });

            migrationBuilder.CreateTable(
                name: "NonMemberCustomer",
                columns: table => new
                {
                    NonMemberCustomerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PassengerFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassengerMiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassengerLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassengerTCKimlik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassengerPassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassengerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassengerPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassengerNationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassengerDateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PassengerAddress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonMemberCustomer", x => x.NonMemberCustomerID);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketID = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.PaymentID);
                });

            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    ReservationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NonMemberCustomerID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    FlightID = table.Column<int>(type: "int", nullable: false),
                    FlyPacketsID = table.Column<int>(type: "int", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReservationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservation", x => x.ReservationID);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    TicketID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationID = table.Column<int>(type: "int", nullable: false),
                    FlightID = table.Column<int>(type: "int", nullable: false),
                    TicketPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TicketStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.TicketID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserTypeID = table.Column<int>(type: "int", nullable: false),
                    UserTCKimlik = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserMiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserNationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserDateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "UserType",
                columns: table => new
                {
                    UserTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserTypes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserType", x => x.UserTypeID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aircraft");

            migrationBuilder.DropTable(
                name: "AircraftErrorReport");

            migrationBuilder.DropTable(
                name: "AircraftStatus");

            migrationBuilder.DropTable(
                name: "Airport");

            migrationBuilder.DropTable(
                name: "AirportGate");

            migrationBuilder.DropTable(
                name: "Baggage");

            migrationBuilder.DropTable(
                name: "BaggageGate");

            migrationBuilder.DropTable(
                name: "BaggagePrice");

            migrationBuilder.DropTable(
                name: "Flight");

            migrationBuilder.DropTable(
                name: "FlightPrice");

            migrationBuilder.DropTable(
                name: "FlyPacket");

            migrationBuilder.DropTable(
                name: "LoginRecord");

            migrationBuilder.DropTable(
                name: "Memberr");

            migrationBuilder.DropTable(
                name: "NonMemberCustomer");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Reservation");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserType");
        }
    }
}
