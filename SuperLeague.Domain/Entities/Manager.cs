namespace SuperLeague.Domain.Entities
{
    public class Manager
    {
        public int ManagerID { get; set; }
        public required string ManagerFirstName { get; set; }
        public required string ManagerLastName { get; set; }
        public required string Nationality { get; set; }
        public required int Age { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }

    }
}
