using System.Numerics;

public class GroupReservationDetails
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string OrganizationName { get; set; }
    public string ContactPerson { get; set; }
    public string ContactNumber { get; set; }
    public int GroupSize { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime ReservationDate { get; set; }

    public GroupReservationDetails(int id, int groupId, string organizationName, string contactPerson, string contactNumber, int groupSize, decimal totalPrice, DateTime reservationDate, decimal discount)
    {
        // or database???????????????
        // if (groupSize < 30)
        // {
        //     throw new ArgumentException("Group size must be greater than 30");
        // } 

        Id = id;
        GroupId = groupId;
        OrganizationName = organizationName;
        ContactPerson = contactPerson;
        ContactNumber = contactNumber;
        GroupSize = groupSize;
        TotalPrice = totalPrice;
        ReservationDate = reservationDate;
        Discount = discount;
    }
}

// public enum GroupType
// {
//     None = 0,
//     School = 1,
//     Corporate = 2
// }

// public class GroupReservationDetails
// {
//     public GroupType GroupType { get; set; }
// }
