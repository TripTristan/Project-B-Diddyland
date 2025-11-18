public class StudentInfo
{
    public int Id { get; set; }
    public string Nr { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public string GuardianName { get; set; }
    public string GuardianContact { get; set; }

    public StudentInfo(string nr, string name, int age, Gender gender, string guardianName, string guardianContact)
    {
        Nr = nr;
        Name = name;
        Age = age;
        Gender = gender;
        GuardianName = guardianName;
        GuardianContact = guardianContact;
    }
}