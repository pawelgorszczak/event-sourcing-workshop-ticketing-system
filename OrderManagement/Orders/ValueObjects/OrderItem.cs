namespace OrderManagement.Orders.ValueObjects;

public record OrderItem(string ConcertId, string TicketLevel, int Quantity);