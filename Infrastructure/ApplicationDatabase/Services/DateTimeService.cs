
using Infrastructure.ApplicationDatabase.Common.Interfaces;

namespace Infrastructure.ApplicationDatabase.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
