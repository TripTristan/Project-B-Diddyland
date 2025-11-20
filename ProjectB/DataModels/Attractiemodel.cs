public class AttractieModel
{
    public int ID { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int MinHeightInCM { get; set; }
    public int Capacity { get; set; }

    public AttractieModel() { }

    public AttractieModel(int id, string name, string type, int minHeightInCM, int capacity)
    {
        ID = id;
        Name = name;
        Type = type;
        MinHeightInCM = minHeightInCM;
        Capacity = capacity;
    }
}


