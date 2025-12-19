using System;

namespace Projet_Chess_db.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int EloRating { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime RegistrationDate { get; set; }


        public string FullName => $"{FirstName} {LastName}";


        public Player()
        {
            RegistrationDate = DateTime.Now;
            EloRating = 1200;
        }
    }
}