public class TicketAttractieModel
{
    public int ID { get; set; }
    public int Nr { get; set; }
    public int AttractionID { get; set; }

    public TicketAttractieModel() { }

    public TicketAttractieModel(int nr, int attractionId)
    {
        Nr = nr;
        AttractionID = attractionId;
    }
}

