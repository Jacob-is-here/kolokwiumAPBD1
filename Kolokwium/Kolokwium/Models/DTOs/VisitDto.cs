namespace Kolokwium.Models;


public class VisitDto
{
    public DateTime date { get; set; }
    public ClientDTO client { get; set; }
    public MechanicDto MechanicDto { get; set; }
    public List<VisitServiceDto> visitServices { get; set; }
}

public class ClientDTO
{
    
    public string firstName { get; set; }
    public string lastName { get; set; }    
    public DateTime dateOfBirt { get; set; }

}



public class MechanicDto
{
    public int mechanicId { get; set; }
    public string licenceNumbe { get; set; }
}

public class VisitServiceDto
{
    public string name { get; set; }
    public int serviceFee { get; set; }
}