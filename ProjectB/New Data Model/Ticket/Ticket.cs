public class Ticket
{
    public int Id { get; set; } // Primary Key// Database Generated
    public string TicketNr { get; set; }
    public string OrderNr { get; set; }
    public string VisitDate { get; set; }
    public string SessionNr{ get; set; }
    public decimal Price { get; set; }

    public string? CustomerID { get; set; } // = "Guest + Customer Name"


    public Ticket ( 
        string ticketNr, 
        string orderNr, 
        string visitDate, 
        string sessionNr, 
        decimal price, 
        string? customerID)
    {
        TicketNr = ticketNr;
        OrderNr = orderNr;
        VisitDate = visitDate;
        SessionNr = sessionNr;
        Price = price;
        CustomerID = customerID;
    }

}
