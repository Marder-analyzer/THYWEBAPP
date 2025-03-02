using Microsoft.EntityFrameworkCore;
using THYWebApp.Models;
using System.Linq;
using THYWebApp.Data;
using THYWebApp.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using THYWebApp.Controllers;

namespace THYWebApp.Services
{
    public class TicketService
    {
        private readonly THYContext _context;
        private readonly string _connectionString = "Data Source=MEHMETCAN; Initial Catalog=DBFly; Integrated Security=True; Encrypt=True; TrustServerCertificate=True";
        private readonly ILogger<TicketService> _logger;
        // Constructor - DI (Dependency Injection) ile ApplicationDbContext'i alır
        public TicketService(THYContext context,ILogger<TicketService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Ticket> GetTicketByReservationIDAndLastNameAsync(int reservationID, string lastName)
        {
            Ticket ticket = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // fn_GetTicketByReservationIDAndLastName fonksiyonunu çağırıyoruz
                var query = @"
        SELECT *
        FROM dbo.fn_GetTicketByReservationIDAndLastName(@ReservationID, @LastName)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReservationID", reservationID);
                    command.Parameters.AddWithValue("@LastName", lastName);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            ticket = new Ticket
                            {
                                TicketID = reader.GetInt32(reader.GetOrdinal("TicketID")),
                                ReservationID = reader.GetInt32(reader.GetOrdinal("ReservationID")),
                                LastName = reader["LastName"]?.ToString() ?? "Bilgi Bulunamadı",
                                FirstName = reader["FirstName"]?.ToString() ?? "Bilgi Bulunamadı",
                                DepartureAirportName = reader["DepartureAirportName"]?.ToString() ?? "Bilgi Bulunamadı",
                                ArrivalAirportName = reader["ArrivalAirportName"]?.ToString() ?? "Bilgi Bulunamadı",
                                ReservationDate = reader["ReservationDate"] != DBNull.Value
                                    ? reader.GetDateTime(reader.GetOrdinal("ReservationDate"))
                                    : DateTime.MinValue,
                                FlighNumber = reader["FlightID"]?.ToString() ?? "Bilgi Bulunamadı",
                                TicketStatus = reader["ReservationStatus"]?.ToString() ?? "Bilgi Bulunamadı",
                                PassengerEmail = reader["Email"]?.ToString() ?? "Bilgi Bulunamadı",
                                FlightAirlineCompany = reader["FlightAirlineCompany"]?.ToString() ?? "Bilgi Bulunamadı",
                                PassengerPhoneNumber = reader["PhoneNumber"]?.ToString() ?? "Bilgi Bulunamadı"
                            };
                        }
                    }
                }
            }

            return ticket;
        }
        public async Task<bool> DeleteTicketAsync(int ticketID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = "EXEC sp_DeleteTicket @TicketID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TicketID", ticketID);
                        var rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Bilet başarıyla silindi: TicketID={ticketID}");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine($"Bilet silinemedi: TicketID={ticketID}");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateTicketSeatNumberAsync(int ticketID, string newSeatNumber)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("sp_UpdateTicketSeatNumber", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@TicketID", ticketID);
                command.Parameters.AddWithValue("@NewSeatNumber", newSeatNumber);

                try
                {
                    await command.ExecuteNonQueryAsync();
                    return true;
                }
                catch (SqlException ex)
                {
                    // Hata loglama
                    _logger.LogError(ex, "Koltuk güncelleme sırasında hata oluştu. TicketID: {TicketID}, NewSeatNumber: {NewSeatNumber}", ticketID, newSeatNumber);
                    return false;
                }
            }
        }
        public async Task<bool> UpdateTicketAsync(Ticket ticket)
        {
            Console.WriteLine($"UpdateTicketAsync: TicketID={ticket.TicketID}, ReservationID={ticket.ReservationID}, Email={ticket.PassengerEmail}, Phone={ticket.PassengerPhoneNumber}");
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Prosedürü çağırmak için uygun SQL sorgusu
                    var query = "EXEC sp_UpdateTicket @TicketID, @ReservationID, @NewEmail, @NewPhoneNumber";

                    using (var command = new SqlCommand(query, connection))
                    {
                        // Parametreleri ekle
                        command.Parameters.AddWithValue("@TicketID", ticket.TicketID);
                        command.Parameters.AddWithValue("@ReservationID", ticket.ReservationID);

                        // Eğer kullanıcı Users tablosundaysa e-posta ve telefon bilgileri güncellenir
                        if (!string.IsNullOrEmpty(ticket.PassengerEmail))
                        {
                            command.Parameters.AddWithValue("@NewEmail", ticket.PassengerEmail);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@NewEmail", DBNull.Value); // Boş bırak
                        }

                        if (!string.IsNullOrEmpty(ticket.PassengerPhoneNumber))
                        {
                            command.Parameters.AddWithValue("@NewPhoneNumber", ticket.PassengerPhoneNumber);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@NewPhoneNumber", DBNull.Value); // Boş bırak
                        }

                        // Sorguyu çalıştır ve etkilenen satır sayısını kontrol et
                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0; // Eğer güncelleme başarılıysa true döner
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata loglama
                Console.WriteLine($"Hata: {ex.Message}");
                return false; // Hata durumunda false döner
            }
        }
        public async Task<(string TicketSeatNumber, string PassengerFullName, List<Seat> SeatInfoList)> GetSeatInfoByTicketIDAsync(int ticketID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("sp_GetSeatInfoByTicketID", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@TicketID", ticketID);

                string ticketSeatNumber = null;
                string passengerFullName = null;
                var seatInfoList = new List<Seat>();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    // İlk sorguyu oku: Biletin koltuk numarası ve yolcu bilgisi
                    if (await reader.ReadAsync())
                    {
                        ticketSeatNumber = reader["SeatNumber"].ToString();
                        passengerFullName = reader["PassengerFullName"].ToString();
                    }

                    // İkinci sorguya geç: Uçuşun tüm koltuk bilgileri
                    if (await reader.NextResultAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            seatInfoList.Add(new Seat
                            {
                                SeatNumber = reader["SeatNumber"].ToString(),
                                SeatStatus = reader["SeatStatus"].ToString(),
                                SeatID = reader.GetInt32(reader.GetOrdinal("SeatID")),
                                FlightID = reader.GetInt32(reader.GetOrdinal("FlightID")),
                            });
                        }
                    }
                }

                return (ticketSeatNumber, passengerFullName, seatInfoList);
            }
        }


        // Bilet detaylarını almak için method
        public async Task<Ticket> GetTicketDetailsAsync(int ticketId)
        {
            Ticket ticketDetails = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM fn_GetTicketDetails(@TicketID)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TicketID", ticketId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            ticketDetails = new Ticket
                            {
                                TicketID = reader.GetInt32(reader.GetOrdinal("TicketID")),
                                DepartureGateID = reader.GetInt32(reader.GetOrdinal("DepartureGateID")),
                                FirstName = reader["PassengerName"] != DBNull.Value
                                    ? reader.GetString(reader.GetOrdinal("PassengerName"))
                                    : "Bilgi Bulunamadı",
                                DepartureAirportName = reader["DepartureAirportName"] != DBNull.Value
                                    ? reader.GetString(reader.GetOrdinal("DepartureAirportName"))
                                    : "Bilgi Bulunamadı",
                                ArrivalAirportName = reader["ArrivalAirportName"] != DBNull.Value
                                    ? reader.GetString(reader.GetOrdinal("ArrivalAirportName"))
                                    : "Bilgi Bulunamadı",
                                DepartureDate = reader["FlightDate"] != DBNull.Value
                                    ? reader.GetDateTime(reader.GetOrdinal("FlightDate"))
                                    : DateTime.MinValue,
                                FlighNumber = reader["FlighNumber"] != DBNull.Value
                                    ? reader.GetString(reader.GetOrdinal("FlighNumber"))
                                    : "Bilgi Bulunamadı",
                                FlightAirlineCompany = reader["FlightAirlineCompany"] != DBNull.Value
                                    ? reader.GetString(reader.GetOrdinal("FlightAirlineCompany"))
                                    : "Bilgi Bulunamadı",
                                TicketStatus = reader["TicketStatus"] != DBNull.Value
                                    ? reader.GetString(reader.GetOrdinal("TicketStatus"))
                                    : "Bilgi Bulunamadı",
                                SeatNumber = reader["SeatNumber"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("SeatNumber")) :"Bilgi Bulunamadı",
                            };
                        }
                    }
                }
            }

            return ticketDetails;
        }
        

    }
}
