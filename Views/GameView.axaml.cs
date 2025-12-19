using Avalonia.Controls;
using Avalonia.Interactivity;
using Projet_Chess_db.Models;
using Projet_Chess_db.Services;
using Projet_Chess_db.ViewModels;
using System.Linq;
using System.Text;

namespace Projet_Chess_db.Views
{
    public partial class GameView : UserControl
    {
        private readonly GameViewModel _viewModel;

        public GameView(IDataService dataService, IEloCalculator eloCalculator)
        {
            InitializeComponent();

            _viewModel = new GameViewModel(dataService, eloCalculator);

            // Charger les compétitions
            CmbCompetitions.ItemsSource = _viewModel.Competitions;

            // Événement quand on change de compétition
            CmbCompetitions.SelectionChanged += CmbCompetitions_SelectionChanged;

            // Charger l'historique des parties
            LoadGamesHistory();
        }

        // Filtrer les joueurs selon la compétition
        private void CmbCompetitions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbCompetitions.SelectedItem is Competition selectedComp)
            {
                // Filtrer uniquement les joueurs inscrits à cette compétition
                var registeredPlayers = _viewModel.Players
                    .Where(p => selectedComp.RegisteredPlayerIds.Contains(p.Id))
                    .ToList();

                CmbWhitePlayer.ItemsSource = registeredPlayers;
                CmbBlackPlayer.ItemsSource = registeredPlayers;

                // Réinitialiser les sélections
                CmbWhitePlayer.SelectedItem = null;
                CmbBlackPlayer.SelectedItem = null;
            }
        }

        private void LoadGamesHistory()
        {
            var allGames = _viewModel.Competitions
                .SelectMany(c => c.Games)
                .OrderByDescending(g => g.GameDate)
                .Take(10)
                .Select(game =>
                {
                    var whitePlayer = _viewModel.Players.FirstOrDefault(p => p.Id == game.WhitePlayerId);
                    var blackPlayer = _viewModel.Players.FirstOrDefault(p => p.Id == game.BlackPlayerId);
                    var competition = _viewModel.Competitions.FirstOrDefault(c => c.Id == game.CompetitionId);

                    return new
                    {
                        game.Id,
                        game.GameDate,
                        game.Result,
                        MovesCount = game.Moves.Count,
                        CompetitionName = competition?.Name ?? $"Compétition ID {game.CompetitionId}",
                        WhitePlayerName = whitePlayer?.FullName ?? $"ID {game.WhitePlayerId}",
                        BlackPlayerName = blackPlayer?.FullName ?? $"ID {game.BlackPlayerId}",
                        WhiteElo = game.WhiteEloBeforeGame,
                        BlackElo = game.BlackEloBeforeGame
                    };
                })
                .ToList();

            GamesListBox.ItemsSource = allGames;
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            if (CmbCompetitions.SelectedItem is Competition selectedComp &&
                CmbWhitePlayer.SelectedItem is Player whitePlayer &&
                CmbBlackPlayer.SelectedItem is Player blackPlayer)
            {
                // Vérification : pas le même joueur
                if (whitePlayer.Id == blackPlayer.Id)
                {

                    TxtGameInfo.Text = "ERREUR : Les deux joueurs doivent être différents !";
                    return;
                }

                // Vérification : les deux joueurs sont bien inscrits
                if (!selectedComp.RegisteredPlayerIds.Contains(whitePlayer.Id) ||
                    !selectedComp.RegisteredPlayerIds.Contains(blackPlayer.Id))
                {
                    TxtGameInfo.Text = "ERREUR : Les joueurs doivent être inscrits à la compétition !";
                    return;
                }

                _viewModel.StartNewGame(selectedComp.Id, whitePlayer.Id, blackPlayer.Id);

                // Afficher le panel de la partie
                GamePanel.IsVisible = true;

                TxtGameInfo.Text = $"Partie démarrée !\n\n" +
                                   $"Blancs: {whitePlayer.FullName} (ELO: {whitePlayer.EloRating})\n" +
                                   $"Noirs: {blackPlayer.FullName} (ELO: {blackPlayer.EloRating})\n" +
                                   $"Compétition: {selectedComp.Name}";

                TxtMovesList.Text = "";
            }
            else
            {
                // Message si des champs manquent
                if (CmbCompetitions.SelectedItem == null)
                    TxtGameInfo.Text = "Sélectionnez d'abord une compétition !";
                else if (CmbWhitePlayer.SelectedItem == null || CmbBlackPlayer.SelectedItem == null)
                    TxtGameInfo.Text = "Sélectionnez les deux joueurs !";
            }
        }

        private void AddMove(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtMoveNotation.Text))
                return;

            try
            {
                _viewModel.AddMove(TxtMoveNotation.Text);

                // Afficher tous les coups
                UpdateMovesList();

                TxtMoveNotation.Text = "";
            }
            catch (System.Exception)
            {
                // Erreur lors de l'ajout du coup
            }
        }

        private void UpdateMovesList()
        {
            if (_viewModel.CurrentGame == null)
                return;

            var sb = new StringBuilder();

            for (int i = 0; i < _viewModel.CurrentGame.Moves.Count; i += 2)
            {
                int moveNumber = (i / 2) + 1;
                string whiteMove = _viewModel.CurrentGame.Moves[i].Notation;
                string blackMove = i + 1 < _viewModel.CurrentGame.Moves.Count
                    ? _viewModel.CurrentGame.Moves[i + 1].Notation
                    : "";

                sb.AppendLine($"{moveNumber}. {whiteMove} {blackMove}");
            }

            TxtMovesList.Text = sb.ToString();
        }

        private void FinishGameWhiteWin(object sender, RoutedEventArgs e)
        {
            FinishGame(GameResult.WhiteWin);
        }

        private void FinishGameDraw(object sender, RoutedEventArgs e)
        {
            FinishGame(GameResult.Draw);
        }

        private void FinishGameBlackWin(object sender, RoutedEventArgs e)
        {
            FinishGame(GameResult.BlackWin);
        }

        private void FinishGame(GameResult result)
        {
            if (_viewModel.CurrentGame == null)
                return;

            _viewModel.FinishGame(result);

            // Masquer le panel
            GamePanel.IsVisible = false;

            // Recharger l'historique
            LoadGamesHistory();

            // Réinitialiser
            TxtMoveNotation.Text = "";
            TxtMovesList.Text = "";
            TxtGameInfo.Text = "Partie terminée et sauvegardée";
        }

        private void CancelGame(object sender, RoutedEventArgs e)
        {
            _viewModel.CancelGame();
            GamePanel.IsVisible = false;
            TxtGameInfo.Text = "Partie annulée";
        }


    }
}