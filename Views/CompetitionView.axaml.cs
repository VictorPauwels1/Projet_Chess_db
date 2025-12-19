using Avalonia.Controls;
using Avalonia.Interactivity;
using Projet_Chess_db.Models;
using Projet_Chess_db.Services;
using Projet_Chess_db.ViewModels;
using System.Linq;

namespace Projet_Chess_db.Views
{
    public partial class CompetitionView : UserControl
    {
        private readonly CompetitionViewModel _viewModel;

        public CompetitionView(IDataService dataService)
        {
            InitializeComponent();

            _viewModel = new CompetitionViewModel(dataService);
            CompetitionListBox.ItemsSource = _viewModel.Competitions;
        }

        private void CompetitionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CompetitionListBox.SelectedItem is Competition selectedComp)
            {
                _viewModel.SelectedCompetition = selectedComp;

                TxtCompDetails.Text = $"Nom: {selectedComp.Name}\n" +
                                      $"Lieu: {selectedComp.Location}\n" +
                                      $"Début: {selectedComp.StartDate:dd/MM/yyyy}\n" +
                                      $"Fin: {selectedComp.EndDate:dd/MM/yyyy}\n" +
                                      $"Participants: {selectedComp.RegisteredPlayerIds.Count}\n" +
                                      $"Statut: {selectedComp.Status}";

                // Afficher ID + Nom des participants
                var participantInfo = selectedComp.RegisteredPlayerIds
                    .Select(id =>
                    {
                        var player = _viewModel.Players.FirstOrDefault(p => p.Id == id);
                        return player != null
                            ? $"[ID {id}] {player.FullName} (ELO: {player.EloRating})"
                            : $"[ID {id}] Joueur introuvable";
                    })
                    .ToList();

                ParticipantListBox.ItemsSource = participantInfo;

                // Créer des objets avec infos complètes pour les parties
                var gamesWithNames = selectedComp.Games.Select(game =>
                {
                    var whitePlayer = _viewModel.Players.FirstOrDefault(p => p.Id == game.WhitePlayerId);
                    var blackPlayer = _viewModel.Players.FirstOrDefault(p => p.Id == game.BlackPlayerId);

                    return new
                    {
                        game.Id,
                        game.GameDate,
                        game.Result,
                        MovesCount = game.Moves.Count,
                        WhitePlayerName = whitePlayer?.FullName ?? $"ID {game.WhitePlayerId}",
                        BlackPlayerName = blackPlayer?.FullName ?? $"ID {game.BlackPlayerId}",
                        WhiteElo = game.WhiteEloBeforeGame,
                        BlackElo = game.BlackEloBeforeGame
                    };
                }).ToList();

                GamesListBox.ItemsSource = gamesWithNames;
            }
        }

        private async void AddCompetition(object sender, RoutedEventArgs e)
        {
            var dialog = new AddCompetitionDialog();
            var window = this.VisualRoot as Window;

            if (window != null)
            {
                var result = await dialog.ShowDialog<Competition>(window);
                if (result != null)
                {
                    _viewModel.AddCompetition(result);
                }
            }
        }

        private async void RegisterPlayer(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedCompetition == null)
            {

                return;
            }

            var dialog = new SelectPlayerDialog(_viewModel.Players.ToList());
            var window = this.VisualRoot as Window;

            if (window != null)
            {
                var result = await dialog.ShowDialog<Player>(window);
                if (result != null)
                {
                    _viewModel.RegisterPlayer(_viewModel.SelectedCompetition.Id, result.Id);
                    CompetitionListBox_SelectionChanged(sender, null);
                }
            }
        }
    }
}