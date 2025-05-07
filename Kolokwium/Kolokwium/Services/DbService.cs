using Kolokwium.Models;
using Microsoft.Data.SqlClient;

namespace Kolokwium.Services;


public class DbService : IDbService
{
    private readonly string _connectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=Superstrongpassword123;TrustServerCertificate=True;";
    
    public async Task<VisitDto> GetVisitAsync(int idClient)
    {

        var visit = new VisitDto
        {
            client = new ClientDTO(),
            MechanicDto = new MechanicDto(),
            visitServices = new List<VisitServiceDto>()
        };
        
        string cmd =
            @"select distinct date , C.first_name , C.last_name , date_of_birth , M.mechanic_id , licence_number , name , service_fee
                        from Visit 
                        join Client C on Visit.client_id = C.client_id
                        join Mechanic M on Visit.mechanic_id = M.mechanic_id
                        join Visit_Service VS on Visit.visit_id = VS.visit_id
                        join Service S on VS.service_id = S.service_id
                        where C.client_id = @idclient ";
        
        
        

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command =new SqlCommand(cmd,connection))
            {
                command.Parameters.AddWithValue("@idclient", idClient);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {

                    while (await reader.ReadAsync())
                    {
                        visit.date = reader.GetDateTime(0);
                        visit.client.firstName = reader.GetString(1);
                        visit.client.lastName = reader.GetString(2);
                        visit.client.dateOfBirt = reader.GetDateTime(3);
                        visit.MechanicDto.mechanicId = reader.GetInt32(4);
                        visit.MechanicDto.licenceNumbe = reader.GetString(5);

                        visit.visitServices.Add(new VisitServiceDto
                        {
                            name = reader.GetString(6),
                            serviceFee = (int)reader.GetDecimal(7)
                        });
                    }
                }
                
                
            }
            
        }

        return visit;
    }

    public async Task AddNewNewVisitsDtoAsync(NewVisitsDto visitor)
    {
        string cmd = @"INSERT INTO Visit (visit_id, client_id, mechanic_id, date)  VALUES (@visit_id, @client_id, @mechanic_id, @date)";
        string getServiceId = "SELECT service_id FROM Service WHERE name = @name";
        string cmdVisitService = @"INSERT INTO Visit_Service (visit_id, service_id, service_fee) VALUES (@visit_id, @service_id, @service_fee)";
        string getMechanicId = "SELECT mechanic_id FROM Mechanic WHERE licence_number = @licence_number";

        string checkClient = "SELECT COUNT(*) FROM Client WHERE client_id = @client_id";
        string checkMechanic = "SELECT COUNT(*) FROM Mechanic WHERE licence_number = @licence_number";
        string checkService = "SELECT COUNT(*) FROM Service WHERE name = @name";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            try
            {

                using (SqlCommand command = new SqlCommand(checkClient ,connection))
                {
                    command.Parameters.AddWithValue("@client_id", visitor.clientId);
                    int clientExists = (int)await command.ExecuteScalarAsync();
                    if (clientExists == 0)
                        throw new Exception($"Klient o ID {visitor.clientId} nie istnieje");
                    
                }

                using (SqlCommand command = new SqlCommand(checkMechanic , connection))
                {
                    command.Parameters.AddWithValue("@licence_number", visitor.mechanicLicenceNumber);
                    int mechanicExists = (int)await command.ExecuteScalarAsync();
                    if (mechanicExists == 0)
                        throw new Exception($"Mechanik o numerze licencji {visitor.mechanicLicenceNumber} nie istnieje");

                }

                foreach (var service in visitor.services)
                {
                    using (SqlCommand command = new SqlCommand(checkService, connection))
                    {
                        command.Parameters.AddWithValue("@name", service.serviceName);
                        int serviceExists = (int)await command.ExecuteScalarAsync();
                        if (serviceExists == 0)
                            throw new Exception($"Usługa {service.serviceName} nie istnieje");
                    }
                }
                
                
                int mechanicId;
                using (SqlCommand command = new SqlCommand(getMechanicId, connection))
                {
                    command.Parameters.AddWithValue("@licence_number", visitor.mechanicLicenceNumber);
                    var result = await command.ExecuteScalarAsync();
                    mechanicId = (int)result;
                }

                using (SqlCommand command = new SqlCommand(cmd, connection))
                {
                    command.Parameters.AddWithValue("@visit_id", visitor.visitId);
                    command.Parameters.AddWithValue("@client_id", visitor.clientId);
                    command.Parameters.AddWithValue("@mechanic_id", mechanicId);
                    command.Parameters.AddWithValue("@date", DateTime.Now); 
 
    
                    await command.ExecuteNonQueryAsync();
                }

                
                foreach (var service in visitor.services)
                {
                    int serviceId;
                    using (SqlCommand command = new SqlCommand(getServiceId, connection))
                    {
                        command.Parameters.AddWithValue("@name", service.serviceName);
                        var result = await command.ExecuteScalarAsync();
                        serviceId = (int)result;
                    }
                    using (SqlCommand command = new SqlCommand(cmdVisitService, connection))
                    {
                        command.Parameters.AddWithValue("@visit_id", visitor.visitId);
                        command.Parameters.AddWithValue("@service_id", serviceId);
                        command.Parameters.AddWithValue("@service_fee", service.serviceFee);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}