using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace SarajevoGuide.Models

{
    public class RegistrovaniKorisnik 
    {
        public int id { get; set; }

        public string? ime { get; set; }    
        public string? prezime { get; set; }    
        [Required, EmailAddress]
        public string email { get; set; } = default!;
        [Required, DataType(DataType.Password)]
        public string lozinka { get; set; } = default!;
        public string? username { get; set; }  

        public RegistrovaniKorisnik() { }
        public RegistrovaniKorisnik(int id, string ime, string prezime, string email, string lozinka, string username)
        {
            this.id = id;
            this.ime = ime;
            this.prezime = prezime;
            this.email = email;
            this.lozinka = lozinka;
            this.username = username;
        }



    }

    }
