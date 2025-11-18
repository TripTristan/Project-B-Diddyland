public class TicketAttractieModel
{
    public int ID { get; set; } // Primary Key// Database Generated
    public int TicketID { get; set; }
    public int AttractieID { get; set; }

    public TicketAttractieModel(int ticketID, int attractieID)
    {
        TicketID = ticketID;
        AttractieID = attractieID;
    }
}