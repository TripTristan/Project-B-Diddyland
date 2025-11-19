public class TicketAttractieModel
{
    public int ID { get; set; } // Primary Key// Database Generated
    public int Nr { get; set; }
    public int AttractionID { get; set; }

    public TicketAttractieModel(int nr, int attractieID)
    {
        Nr = nr;
        AttractionID = attractionID;
    }
}