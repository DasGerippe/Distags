using System.Collections.Concurrent;

namespace LevelTagApi.Security;

public class ConcurrentTicketStore<TTicketData> : ITicketStore<TTicketData>
{
    private readonly ConcurrentDictionary<string, TicketData> _Tickets = new ConcurrentDictionary<string, TicketData>();
    private readonly TimeSpan _TicketExpirationDelay;

    public ConcurrentTicketStore(LevelTagApiSettings levelTagApiSettings)
    {
        _TicketExpirationDelay = levelTagApiSettings.TicketExpirationDelay;
    }

    public void Store(string ticket, TTicketData data)
    {
        RemoveExpiredTickets();

        _Tickets[ticket] = new TicketData
        {
            Expiration = DateTimeOffset.UtcNow + _TicketExpirationDelay,
            Data = data,
        };
    }

    public TTicketData? Retrieve(string ticket)
    {
        if (!_Tickets.TryRemove(ticket, out TicketData? ticketData))
            return default;

        if (ticketData.Expiration < DateTimeOffset.UtcNow)
            return default;

        return ticketData.Data;
    }

    public void RemoveExpiredTickets()
    {
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;

        List<KeyValuePair<string, TicketData>> expiredTickets = _Tickets
            .Where(kvp => kvp.Value.Expiration < utcNow)
            .ToList();

        foreach (KeyValuePair<string, TicketData> expiredTicket in expiredTickets)
        {
            _Tickets.TryRemove(expiredTicket);
        }
    }

    private class TicketData
    {
        public required DateTimeOffset Expiration { get; init; }

        public required TTicketData Data { get; init; }
    }
}