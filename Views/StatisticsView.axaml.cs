using Avalonia.Controls;
using Projet_Chess_db.Models;
using Projet_Chess_db.Services;
using System.Collections.Generic;
using System.Linq;

namespace Projet_Chess_db.Views
{
    public partial class StatisticsView : UserControl
    {
        private readonly IDataService _dataService;
        private List<Player> _players;
        private List<Competition> _competitions;

        public StatisticsView(IDataService dataService)
        {
            InitializeComponent();

            _dataService = dataService;
            LoadData();
            DisplayGlobalStats();
            DisplayRanking();
        }

        private void LoadData()
        {
            _players = _dataService.LoadPlayers();
            _competitions = _dataService.LoadCompetitions();
        }

        private void DisplayGlobalStats()
        {
            var totalPlayers = _players.Count;
            var totalGames = _competitions.SelectMany(c => c.Games).Count();
            var avgElo = _players.Count > 0 ? (int)_players.Average(p => p.EloRating) : 0;
            var topPlayer = _players.OrderByDescending(p => p.EloRating).FirstOrDefault();

            TxtGlobalStats.Text = $"Joueurs inscrits: {totalPlayers}\n" +
                                  $"Parties jouées: {totalGames}\n" +
                                  $"ELO moyen: {avgElo}\n" +
                                  $"Meilleur joueur: {topPlayer?.FullName ?? "N/A"} ({topPlayer?.EloRating ?? 0})";
        }

        private void DisplayRanking()
        {
            var ranking = _players
                .OrderByDescending(p => p.EloRating)
                .Select((player, index) => new
                {
                    Rank = index + 1,
                    Player = player
                })
                .ToList();

            RankingListBox.ItemsSource = ranking;
        }

        private void RankingListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RankingListBox.SelectedItem == null)
                return;

            // Extraire le joueur depuis l'objet anonyme
            var selectedItem = RankingListBox.SelectedItem;
            var playerProperty = selectedItem.GetType().GetProperty("Player");
            var player = playerProperty?.GetValue(selectedItem) as Player;

            if (player == null)
                return;

            DisplayPlayerStats(player);
        }

        private void DisplayPlayerStats(Player player)
        {
            TxtPlayerName.Text = $"{player.FullName} (ELO: {player.EloRating})";

            // Récupérer toutes les parties du joueur
            var allGames = _competitions
                .SelectMany(c => c.Games)
                .Where(g => g.WhitePlayerId == player.Id || g.BlackPlayerId == player.Id)
                .ToList();

            var totalGames = allGames.Count;
            var wins = 0;
            var losses = 0;
            var draws = 0;

            foreach (var game in allGames)
            {
                if (game.Result == GameResult.Draw)
                {
                    draws++;
                }
                else if ((game.Result == GameResult.WhiteWin && game.WhitePlayerId == player.Id) ||
                         (game.Result == GameResult.BlackWin && game.BlackPlayerId == player.Id))
                {
                    wins++;
                }
                else if (game.Result != GameResult.InProgress)
                {
                    losses++;
                }
            }

            var winRate = totalGames > 0 ? (wins * 100.0 / totalGames) : 0;
            var drawRate = totalGames > 0 ? (draws * 100.0 / totalGames) : 0;
            var lossRate = totalGames > 0 ? (losses * 100.0 / totalGames) : 0;

            // Stats générales
            TxtPlayerStats.Text = $"Parties jouées: {totalGames}\n" +
                                  $"Victoires: {wins}\n" +
                                  $"Défaites: {losses}\n" +
                                  $"Nuls: {draws}\n" +
                                  $"Date d'inscription: {player.RegistrationDate:dd/MM/yyyy}";

            // Performance
            TxtPerformance.Text = $"Taux de victoire: {winRate:F1}%\n" +
                                  $"Taux de nul: {drawRate:F1}%\n" +
                                  $"Taux de défaite: {lossRate:F1}%\n" +
                                  $"ELO actuel: {player.EloRating}";

            // Dernières parties
            var recentGames = allGames
                .OrderByDescending(g => g.GameDate)
                .Take(10)
                .Select(game =>
                {
                    var isWhite = game.WhitePlayerId == player.Id;
                    var opponentId = isWhite ? game.BlackPlayerId : game.WhitePlayerId;
                    var opponent = _players.FirstOrDefault(p => p.Id == opponentId);
                    var opponentName = opponent?.FullName ?? $"ID {opponentId}";

                    var eloBefore = isWhite ? game.WhiteEloBeforeGame : game.BlackEloBeforeGame;
                    var eloAfter = player.EloRating;

                    string resultText;
                    string eloChange;

                    if (game.Result == GameResult.Draw)
                    {
                        resultText = "Match nul";
                        eloChange = "~0";
                    }
                    else if ((game.Result == GameResult.WhiteWin && isWhite) ||
                             (game.Result == GameResult.BlackWin && !isWhite))
                    {
                        resultText = "Victoire";
                        eloChange = "+?";
                    }
                    else
                    {
                        resultText = "Défaite";
                        eloChange = "-?";
                    }

                    return new
                    {
                        game.GameDate,
                        Description = $"{(isWhite ? "Blancs" : "Noirs")} vs {opponentName}",
                        ResultText = resultText,
                        EloBefore = eloBefore,
                        EloAfter = eloAfter,
                        EloChange = eloChange
                    };
                })
                .ToList();

            RecentGamesListBox.ItemsSource = recentGames;
        }
    }
}