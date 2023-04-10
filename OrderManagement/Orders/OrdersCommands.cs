using OrderManagement.Orders.ValueObjects;

namespace OrderManagement.Orders;

public record CreateOrder(string UserId, string ShoppingCartId, List<OrderItem> OrderItems);
public record ConfirmOrder(string OrderId);
public record CancelOrder(string OrderId);
public record CancelOrderDueToTimeout(string OrderId);

public record AddReservedTickets(string OrderId, IEnumerable<ReservedTicket> ReservedTickets);
public record ProcessPaymentSuccess(string OrderId, string TransactionId);
public record ProcessPaymentFailure(string OrderId, string TransactionId, string Reason);