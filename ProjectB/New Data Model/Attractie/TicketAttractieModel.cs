public class TicketAttractieModel
{
    public int ID { get; set; } // Primary Key// Database Generated
    public int Nr { get; set; }
    public int AttractieID { get; set; }

    public TicketAttractieModel(int nr, int attractieID)
    {
        Nr = nr;
        AttractieID = attractieID;
    }
}