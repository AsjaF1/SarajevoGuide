namespace SarajevoGuide.Models
{
    public class Kupovina
    {
        public int Id { get; set; }
        public DateTime datumKupovine { get; set; }
        public int brojUlaznica { get; set; }
        public int korisnikId { get; set; }
        public int eventId { get; set; }


        public Kupovina(int id, DateTime datumKupovine, int brojUlaznica, int korisnikId, int eventId)
        {
            Id = id;
            this.datumKupovine = datumKupovine;
            this.brojUlaznica = brojUlaznica;
            this.korisnikId = korisnikId;
            this.eventId = eventId;
        }


    }
}
