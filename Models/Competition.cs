using System;
using System.Collections.Generic;

namespace Projet_Chess_db.Models
{
    public class Competition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Liste des IDs des joueurs inscrits
        public List<int> RegisteredPlayerIds { get; set; }

        // Liste des parties jouées
        public List<Game> Games { get; set; }

        public CompetitionStatus Status { get; set; }
        public int MaxParticipants { get; set; }


        public Competition()
        {
            RegisteredPlayerIds = new List<int>();
            Games = new List<Game>();
            Status = CompetitionStatus.Planned;
            MaxParticipants = 0; // 0 = pas de limite
        }

        // Vérifie si la compétition est complète
        public bool IsFull()
        {
            if (MaxParticipants == 0)
                return false;

            return RegisteredPlayerIds.Count >= MaxParticipants;
        }

        // Ajoute un joueur
        public bool RegisterPlayer(int playerId)
        {
            if (IsFull())
                return false;

            if (RegisteredPlayerIds.Contains(playerId))
                return false;

            RegisteredPlayerIds.Add(playerId);
            return true;
        }

        // Retire un joueur
        public bool UnregisterPlayer(int playerId)
        {
            return RegisteredPlayerIds.Remove(playerId);
        }
    }


    public enum CompetitionStatus
    {
        Planned,
        InProgress,
        Completed,
        Cancelled
    }
}