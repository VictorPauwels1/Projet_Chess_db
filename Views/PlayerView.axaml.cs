using Avalonia.Controls;
using Avalonia.Interactivity;
using Projet_Chess_db.Models;
using Projet_Chess_db.Services;
using Projet_Chess_db.ViewModels;

namespace Projet_Chess_db.Views
{
    public partial class PlayerView : UserControl
    {
        private readonly PlayerViewModel _viewModel;

        public PlayerView(IDataService dataService, IEloCalculator eloCalculator)
        {
            InitializeComponent();

            _viewModel = new PlayerViewModel(dataService, eloCalculator);

            PlayerListBox.ItemsSource = _viewModel.Players;
        }

        private void PlayerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlayerListBox.SelectedItem is Player selectedPlayer)
            {
                _viewModel.SelectedPlayer = selectedPlayer;
                TxtDetails.Text = $"Nom: {selectedPlayer.FullName}\n" +
                                  $"Date naissance: {selectedPlayer.DateOfBirth:dd/MM/yyyy}\n" +
                                  $"ELO: {selectedPlayer.EloRating}\n" +
                                  $"Email: {selectedPlayer.Email}\n" +
                                  $"Téléphone: {selectedPlayer.PhoneNumber}";
            }
        }

        private async void AddPlayer(object sender, RoutedEventArgs e)
        {
            var dialog = new AddPlayerDialog();
            var window = this.VisualRoot as Window;

            if (window != null)
            {
                var result = await dialog.ShowDialog<Player>(window);
                if (result != null)
                {
                    _viewModel.AddPlayer(result);
                }
            }
        }
        private async void EditPlayer(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedPlayer == null)
                return;

            var dialog = new EditPlayerDialog(_viewModel.SelectedPlayer);
            var window = this.VisualRoot as Window;

            if (window != null)
            {
                var result = await dialog.ShowDialog<Player>(window);
                if (result != null)
                {
                    _viewModel.UpdatePlayer(result);
                    RefreshList(sender, e);
                }
            }
        }
        private void DeletePlayer(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedPlayer != null)
            {
                _viewModel.DeletePlayer(_viewModel.SelectedPlayer.Id);
                TxtDetails.Text = "Sélectionnez un joueur";
            }
        }

        private void RefreshList(object sender, RoutedEventArgs e)
        {
            PlayerListBox.ItemsSource = null;
            PlayerListBox.ItemsSource = _viewModel.Players;
        }
    }
}