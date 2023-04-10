/*using Marten.Schema;
using Core.EventStore;

namespace OrderManagement.Orders;

public class OrderCommandHandler
{
    private readonly IEventStore eventStore;

    public OrderCommandHandler(IEventStore eventStore)
    {
        this.eventStore = eventStore;
    }

    public async Task HandleAsync(AddReservedTicketsCommand command)
    {
        var order = await eventStore.LoadAsync<Order>(command.OrderId);
        order.AddReservedTickets(command.ReservedTickets);
        await eventStore.SaveChangesAsync();
    }

    public async Task HandleAsync(ConfirmOrderCommand command)
    {
        var order = await eventStore.LoadAsync<Order>(command.OrderId);
        order.ConfirmOrder();
        await eventStore.SaveChangesAsync();
    }

    public async Task HandleAsync(ProcessPaymentSuccessCommand command)
    {
        var order = await eventStore.LoadAsync<Order>(command.OrderId);
        order.ProcessPaymentSuccess(command.Amount, command.TransactionId);
        await eventStore.SaveChangesAsync();
    }

    public async Task HandleAsync(ProcessPaymentFailureCommand command)
    {
        var order = await eventStore.LoadAsync<Order>(command.OrderId);
        order.ProcessPaymentFailure(command.Amount, command.Reason);
        await eventStore.SaveChangesAsync();
    } 
    
    public async Task HandleAsync(CancelOrderCommand command)
    {
        var order = await eventStore.LoadAsync<Order>(command.OrderId);
        order.CancelOrder();
        await eventStore.SaveChangesAsync();
    }

    public async Task HandleAsync(CancelOrderDueToTimeoutCommand command)
    {
        var order = await eventStore.LoadAsync<Order>(command.OrderId);
        order.CancelOrderDueToTimeout();
        await eventStore.SaveChangesAsync();
    }
}
*/