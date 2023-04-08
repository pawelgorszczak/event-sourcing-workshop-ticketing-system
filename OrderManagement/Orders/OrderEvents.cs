using OrderManagement.Orders.ValueObjects;

namespace OrderManagement.Orders;

public record OrderCreated(string OrderId, string UserId, string ShoppingCartId, List<OrderItem> OrderItems, CustomerInfo CustomerInfo);
public record OrderConfirmed(string OrderId);
public record OrderCancelled(string OrderId);
public record OrderCancelledDueToTimeout(string OrderId);
