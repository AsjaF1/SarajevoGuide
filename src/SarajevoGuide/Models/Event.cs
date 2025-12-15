using SarajevoGuide.Enums;

namespace SarajevoGuide.Models
{
    public class Event
    {
        public Event()
        {
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public Kategorija Kategorija { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public double Lat { get; set; }
        public double Lng { get; set; }

        public double Price { get; set; }



        public Event(int id, string name, string description, DateTime? startDate, DateTime? endDate, double latitude, double longitude, Kategorija kategorija, double price)
        {
            Id = id;
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Lat = latitude;
            Lng = longitude;
            Kategorija = kategorija;
            Price = price;
        }

    }
}
