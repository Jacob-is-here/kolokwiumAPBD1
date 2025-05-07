using Kolokwium.Models;

namespace Kolokwium.Services;

public interface IDbService
{

    Task<VisitDto> GetVisitAsync(int id);
    Task AddNewNewVisitsDtoAsync(NewVisitsDto visitor);
}