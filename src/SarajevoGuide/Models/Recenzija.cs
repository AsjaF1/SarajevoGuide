namespace SarajevoGuide.Models
{
    public class Recenzija
    {
        public int Id { get; set; }
        public int KorisnikId { get; set; }
        public int EventId { get; set; }
        public string Komentar { get; set; }
        public int Ocjena { get; set; }

        public Recenzija() { }

        public Recenzija(int id, int korisnikId, int eventId, string komentar, int ocjena)
        {
            this.Id = id;
            this.KorisnikId = korisnikId;
            this.EventId = eventId;
            this.Komentar = komentar;
            this.Ocjena = ocjena;
        }
    }
}
