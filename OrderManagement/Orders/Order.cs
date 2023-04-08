using Core.Aggregates;
using OrderManagement.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
//using Marten.Schema;

namespace OrderManagement.Orders;

public record ReservedTicket(string ConcertId, string TicketLevel, int Quantity);

public enum OrderStatus { Pending, Confirmed, Paid, Cancelled }

public class Order: Aggregate
{
    public string UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public IReadOnlyList<ReservedTicket> ReservedTickets { get; private set; } = new List<ReservedTicket>();
    public string? PaymentTransactionId { get; private set; }
    public CustomerInfo CustomerInfo { get; private set; }
    public bool IsConfirmed { get; private set; }
    public bool IsPaymentSucceeded { get; private set; }
    public string TransactionId { get; private set; }

    private Order() { } // For Marten

    public Order(string id, string userId, CustomerInfo customerInfo)
    {
        Id = id;
        UserId = userId;
        ApplyAndEnqueue(new OrderCreated(id, userId, CustomerInfo));
    }

    public void AddReservedTickets(IEnumerable<ReservedTicket> reservedTickets)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Reserved tickets can only be added when the order is in pending status.");
        }

        ApplyAndEnqueue(new ReservedTicketsAdded(Id, reservedTickets.ToList()));
    }

    public void ConfirmOrder()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("The order can only be confirmed when it is in pending status.");
        }

        ApplyAndEnqueue(new OrderConfirmed(Id));
    }

    public void ProcessPaymentSuccess(decimal amount, string transactionId)
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException("The payment can only be processed when the order is in confirmed status.");
        }

        ApplyAndEnqueue(new PaymentSucceeded(Id, amount, transactionId));
    }

    public void ProcessPaymentFailure(string transactionId)
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException("The payment failure can only be processed when the order is in confirmed status.");
        }

        ApplyAndEnqueue(new PaymentFailed(Id, transactionId));
    }

    public void CancelOrder()
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException("The order can only be cancelled when it is in pending or confirmed status.");
        }

        ApplyAndEnqueue(new OrderCancelled(Id));
    }

    public void CancelOrderDueToTimeout()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("The order can only be cancelled due to timeout when it is in pending status.");
        }

        ApplyAndEnqueue(new OrderCancelledDueToTimeout(Id));
    }

    private void Apply(OrderCreated e)
    {
        Id = e.OrderId;
        _tickets = e.Tickets;
        _userId = e.UserId;
        _customerInfo = e.CustomerInfo;
        _status = OrderStatus.Created;
    }

    private void Apply(OrderCancelled e)
    {
        Status = OrderStatus.Cancelled;
    }

    private void Apply(OrderCancelledDueToTimeout e)
    {
        Status = OrderStatus.Cancelled;
    }

    private void Apply(ReservedTicketsAdded e)
    {
        _reservedTickets.AddRange(e.ReservedTickets);
    }

    private void Apply(OrderConfirmed e)
    {
        IsConfirmed = true;
    }

    private void Apply(PaymentSucceeded e)
    {
        IsPaymentSucceeded = true;
        TransactionId = e.TransactionId;
    }

    private void Apply(PaymentFailed e)
    {
        IsPaymentSucceeded = false;
        TransactionId = null;
    }
}

public record ReservedTicket(string ConcertId, string TicketLevel, int Quantity, decimal Price);
public record ReservedTicketsAdded(string OrderId, List<ReservedTicket> ReservedTickets);
public record OrderConfirmed(string OrderId);
public record PaymentSucceeded(string OrderId, decimal Amount, string TransactionId);
public record PaymentFailed(string OrderId, decimal Amount, string Reason);

public record AddReservedTicketsCommand(string OrderId, IEnumerable<ReservedTicket> ReservedTickets);
public record ConfirmOrderCommand(string OrderId);
public record ProcessPaymentSuccessCommand(string OrderId, decimal Amount, string TransactionId);
public record ProcessPaymentFailureCommand(string OrderId, decimal Amount, string Reason);

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
