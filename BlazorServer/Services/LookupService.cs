using SharedLibrary;
using SharedLibrary.Dto;
using SharedLibrary.Entities;
using SharedLibrary.Enums;
using SharedLibrary.Models;

namespace BlazorServer.Services;

public enum LookupEnums
{
    UserTypes = 1,
}


public class GetLookupQuery
{
    public int key { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
    public int? SelectedId { get; set; }
}
public interface ILookupService
{
    public Task<List<LookupDto>> GetLookup(GetLookupQuery query);
}


public class LookupService : ILookupService
{


    public LookupService()
    {

    }

    public async Task<List<LookupDto>> GetLookup(GetLookupQuery query)
    {
        if (query.key == (int)LookupEnums.UserTypes)
        {
            List<LookupDto> data = new List<LookupDto>()
            {
                new LookupDto() { Id = (int)UserType.NotDefined, Name = UserType.NotDefined.ToString() },
                new LookupDto() { Id = (int)UserType.Site, Name = UserType.Site.ToString() },
                new LookupDto() { Id = (int)UserType.Integrator, Name = UserType.Integrator.ToString() },
            };

            return data;
        }
        else if (true)
        {
            return new();
        }

    }

}
