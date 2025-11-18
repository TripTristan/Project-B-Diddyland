public class Ticket
{
    public int Id { get; set; } // Primary Key// Database Generated
    public string OrderNr { get; set; } = "";

    public string VisitDate { get; set; }
    public SessionTimes Session{ get; set; }

    public string CustomerName { get; set; }
    public decimal Price { get; set; }

    public string? CustomerID { get; set; }


    public Ticket ( string orderNr, 
                    string visitDate, 
                    SessionTimes session, 
                    string customerName, 
                    decimal price, 
                    string? customerID)
    {
        Id = id;
        OrderNr = orderNr;
        VisitDate = visitDate;
        Session = session;
        CustomerName = customerName;
        Price = price;
        CustomerID = customerID;
    }

}
