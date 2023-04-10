namespace OrderManagement.Orders.ValueObjects;

public record ReservedTicket(string ConcertId, string TicketLevel, int Quantity);
