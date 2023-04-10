using OrderManagement.Orders.ValueObjects;

namespace OrderManagement.Orders;

public record OrderCreated(string OrderId, string UserId, List<OrderItem> OrderItems, CustomerInfo CustomerInfo);
public record OrderConfirmed(string OrderId);
public record OrderCancelled(string OrderId);
public record OrderCancelledDueToTimeout(string OrderId);

public record ReservedTicket(string ConcertId, string TicketLevel, int Quantity, decimal Price);
public record ReservedTicketsAdded(string OrderId, List<ReservedTicket> ReservedTickets);
public record PaymentSucceeded(string OrderId, string TransactionId);
public record PaymentFailed(string OrderId, string TransactionId, string Reason);
