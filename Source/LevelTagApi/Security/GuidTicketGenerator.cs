namespace LevelTagApi.Security;

public class GuidTicketGenerator : ITicketGenerator
{
    public string GenerateTicket()
    {
        return Guid.NewGuid().ToString();
    }
}