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



// public class StudentInfo
// {
//     public string studentName { get; set; }
//     public string studentId { get; set; }
//     public int Age { get; set; }
//     public Gender Gender { get; set; }
//     public int Height { get; set; }
//     public int Weight { get; set; }
//     public string StudentParentName { get; set; }
//     public string EmergencyContactName { get; set; }
//     public string EmergencyContactPhone { get; set; }

//     public StudentInfo(string studentName, string studentId, int age, int height, int weight)
//     {
//         this.studentName = studentName;
//         this.studentId = studentId;
//         this.Age = age;
//         this.Gender = gender;
//         this.Height = height;
//         this.Weight = weight;
//         this.StudentParentName = studentParentName;
//         this.EmergencyContactName = emergencyContactName;
//         this.EmergencyContactPhone = emergencyContactPhone;
//     }
    
// }
