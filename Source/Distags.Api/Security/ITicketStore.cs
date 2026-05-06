namespace Distags.Api.Security;

public interface ITicketStore<TTicketData>
{
    public void Store(string ticket, TTicketData data);

    public TTicketData? Retrieve(string ticket);
}