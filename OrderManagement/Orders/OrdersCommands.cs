namespace OrderManagement.Orders;

public record CreateOrder(string UserId, string ShoppingCartId, List<OrderItem> OrderItems);
public record ConfirmOrder(string OrderId);
public record CancelOrder(string OrderId);
public record CancelOrderDueToTimeout(string OrderId);