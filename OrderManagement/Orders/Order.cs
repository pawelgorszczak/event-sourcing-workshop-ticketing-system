using Core.Aggregates;
using OrderManagement.Orders.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderManagement.Orders;

public enum OrderStatus { Pending, Confirmed, Paid, Cancelled }

public class Order: Aggregate
{
    private List<ReservedTicket> _reservedTicketsSource = new List<ReservedTicket>();
    private List<OrderItem> _orderItems = new List<OrderItem>();
    public string UserId { get; private set; } = default!;
    public OrderStatus Status { get; private set; } = default!;
    public decimal TotalAmount { get; private set; } = default!;
    public IReadOnlyList<ReservedTicket> ReservedTickets => _reservedTicketsSource.AsReadOnly();
    public IReadOnlyList<ReservedTicket> OrderItems => _reservedTicketsSource.AsReadOnly();
    public string? PaymentTransactionId { get; private set; } = default!;
    public CustomerInfo CustomerInfo { get; private set; } = default!;
    public bool IsConfirmed { get; private set; } = default!;
    public bool IsPaymentSucceeded { get; private set; } = default!;
    public string TransactionId { get; private set; } = default!;

    private Order() { } // For Marten

    public Order(string id, string userId, List<OrderItem> orderItems, CustomerInfo customerInfo)
    {
        Id = id;
        UserId = userId;
        ApplyAndEnqueue(new OrderCreated(id, userId, orderItems, customerInfo));
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

    public void ProcessPaymentSuccess(string transactionId)
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException("The payment can only be processed when the order is in confirmed status.");
        }

        ApplyAndEnqueue(new PaymentSucceeded(Id, transactionId));
    }

    public void ProcessPaymentFailure(string transactionId, string reason)
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException("The payment failure can only be processed when the order is in confirmed status.");
        }

        ApplyAndEnqueue(new PaymentFailed(Id, transactionId, reason));
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
        UserId = e.UserId;
        _orderItems.AddRange(e.OrderItems);
        CustomerInfo = e.CustomerInfo;
        Status = OrderStatus.Pending;
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
        _reservedTicketsSource.AddRange(e.ReservedTickets);
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
        TransactionId = default!;
    }
}