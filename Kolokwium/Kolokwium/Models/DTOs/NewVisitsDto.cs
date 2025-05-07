namespace Kolokwium.Models;

public class NewVisitsDto
{
    public int visitId { get; set; }
    public int clientId { get; set; }
    public string mechanicLicenceNumber { get; set; }
    public List<NewService> services { get; set; }

}

public class NewMechanicDto
{
    public string mechanicLicenceNumber { get; set; }
}

public class NewService
{
    public string serviceName { get; set; }
    public decimal serviceFee { get; set; }
}