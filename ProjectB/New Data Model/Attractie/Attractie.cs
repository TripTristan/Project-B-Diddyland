using System;

public class AttractieModel
{
    public int Id { get; set; } // Primary Key// Database Generated
    public String Nr { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int MinHeightInCM { get; set; }
    public int Capacity { get; set; }

    public AttractieModel( String nr, string name, string type, int minHeightInCM, int capacity)
    {
        Nr = nr;
        Name = name;
        Type = type;
        MinHeightInCM = minHeightInCM;
        Capacity = capacity;
    }
}