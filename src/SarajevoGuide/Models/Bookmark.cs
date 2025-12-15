namespace SarajevoGuide.Models
{
    public class Bookmark
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        // Navigation properties
        public virtual Event Event { get; set; }
        public virtual RegistrovaniKorisnik User { get; set; }
    }
}
