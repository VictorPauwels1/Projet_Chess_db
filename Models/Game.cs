using System;
using System.Collections.Generic;

namespace Projet_Chess_db.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int CompetitionId { get; set; }
        public int WhitePlayerId { get; set; }
        public int BlackPlayerId { get; set; }
        public DateTime GameDate { get; set; }

        // Liste des coups
        public List<Move> Moves { get; set; }

        public GameResult Result { get; set; }

        // ELO avant la partie 
        public int WhiteEloBeforeGame { get; set; }
        public int BlackEloBeforeGame { get; set; }

        public Game()
        {
            Moves = new List<Move>();
            GameDate = DateTime.Now;
            Result = GameResult.InProgress;
        }

        // Ajoute un coup
        public void AddMove(Move move)
        {
            move.MoveNumber = Moves.Count + 1;
            Moves.Add(move);
        }

        // Obtient le score du joueur blanc 
        public double GetWhiteScore()
        {
            switch (Result)
            {
                case GameResult.WhiteWin:
                    return 1.0;
                case GameResult.BlackWin:
                    return 0.0;
                case GameResult.Draw:
                    return 0.5;
                default:
                    return 0.5;
            }
        }

        // Obtient le score du joueur noir
        public double GetBlackScore()
        {
            return 1.0 - GetWhiteScore();
        }

        // Vérifie si la partie est terminée
        public bool IsFinished()
        {
            return Result != GameResult.InProgress;
        }
    }

    // Classe Move 
    public class Move
    {
        public int MoveNumber { get; set; }
        public string Notation { get; set; } = string.Empty;

        public Move()
        {
        }

        public Move(string notation)
        {
            Notation = notation;
        }
    }


    public enum GameResult
    {
        InProgress,
        WhiteWin,
        BlackWin,
        Draw
    }
}
