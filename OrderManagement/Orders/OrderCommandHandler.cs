using Core.EventStore;

namespace OrderManagement.Orders;

public class OrderCommandHandler
{
    private readonly IEventStore eventStore;

    public OrderCommandHandler(IEventStore eventStore)
    {
        this.eventStore = eventStore;
    }

    public async Task HandleAsync(AddReservedTickets command)
    {
        var order = await eventStore.AggregateStreamAsync<Order>(command.OrderId);
        order.AddReservedTickets(command.ReservedTickets);
        await eventStore.SaveChangesAsync();
    }

    public async Task HandleAsync(ConfirmOrder command)
    {
        var order = await eventStore.AggregateStreamAsync<Order>(command.OrderId);
        order.ConfirmOrder();
        await eventStore.SaveChangesAsync();
    }

    public async Task HandleAsync(ProcessPaymentSuccess command)
    {
        var order = await eventStore.AggregateStreamAsync<Order>(command.OrderId);
        order.ProcessPaymentSuccess(command.TransactionId);
        await eventStore.SaveChangesAsync();
    }

    public async Task HandleAsync(ProcessPaymentFailure command)
    {
        var order = await eventStore.AggregateStreamAsync<Order>(command.OrderId);
        order.ProcessPaymentFailure(command.TransactionId, command.Reason);
        await eventStore.SaveChangesAsync();
    } 
    
    public async Task HandleAsync(CancelOrder command)
    {
        var order = await eventStore.AggregateStreamAsync<Order>(command.OrderId);
        order.CancelOrder();
        await eventStore.SaveChangesAsync();
    }

    public async Task HandleAsync(CancelOrderDueToTimeout command)
    {
        var order = await eventStore.AggregateStreamAsync<Order>(command.OrderId);
        order.CancelOrderDueToTimeout();
        await eventStore.SaveChangesAsync();
    }
}