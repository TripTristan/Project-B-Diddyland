public class StudentInfo
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public string GuardianName { get; set; }
    public string GuardianContact { get; set; }

    public StudentInfo(string name, int age, Gender gender, string guardianName, string guardianContact)
    {
        Name = name;
        Age = age;
        Gender = gender;
        GuardianName = guardianName;
        GuardianContact = guardianContact;
    }
}