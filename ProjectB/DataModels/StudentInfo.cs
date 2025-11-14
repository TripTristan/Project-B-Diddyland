public class StudentInfo
{
    public string studentName { get; set; }
    public string studentId { get; set; }
    public int Age { get; set; }
    public int Height { get; set; }
    public int Weight { get; set; }
    public string StudentParentName { get; set; }
    public string EmergencyContactName { get; set; }
    public string EmergencyContactPhone { get; set; }

    public StudentInfo(string studentName, string studentId, int age, int height, int weight)
    {
        this.studentName = studentName;
        this.studentId = studentId;
        this.Age = age;
        this.Height = height;
        this.Weight = weight;
        this.StudentParentName = studentParentName;
        this.EmergencyContactName = emergencyContactName;
        this.EmergencyContactPhone = emergencyContactPhone;
    }
    
}
