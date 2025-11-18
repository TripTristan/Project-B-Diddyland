public class AttractieModel
{
    public int ID { get; set; } // Primary Key// Database Generated
    public int AttractieID { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int MinHeightInCM { get; set; }
    public int Capacity { get; set; }

    public AttractieModel( int attractieID, string name, string type, int minHeightInCM, int capacity)
    {
        AttractieID = attractieID;
        Name = name;
        Type = type;
        MinHeightInCM = minHeightInCM;
        Capacity = capacity;
    }
}