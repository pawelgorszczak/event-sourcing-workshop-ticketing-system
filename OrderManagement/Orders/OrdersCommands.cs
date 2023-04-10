using OrderManagement.Orders.ValueObjects;

namespace OrderManagement.Orders;

public record CreateOrder(string UserId, string ShoppingCartId, List<OrderItem> OrderItems);
public record ConfirmOrder(string OrderId);
public record CancelOrder(string OrderId);
public record CancelOrderDueToTimeout(string OrderId);

public record AddReservedTicketsCommand(string OrderId, IEnumerable<ReservedTicket> ReservedTickets);
public record ConfirmOrderCommand(string OrderId);
public record ProcessPaymentSuccessCommand(string OrderId, string TransactionId);
public record ProcessPaymentFailureCommand(string OrderId, string TransactionId, string Reason);