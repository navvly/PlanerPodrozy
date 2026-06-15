namespace Planer_podróży.Models
{
    public class Trip
    {
        public string Name { get; set; }

        public string City { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Budget { get; set; }

        public override string ToString()
        {
            string status;

            DateTime today = DateTime.Today;

            if (today < StartDate)
                status = "Planowana";
            else if (today > EndDate)
                status = "Zakończona";
            else
                status = "W trakcie";

            return $"{Name} ({status})";
        }
    }
}