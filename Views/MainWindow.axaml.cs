using Avalonia.Controls;
using Avalonia.Interactivity;
using Projet_Chess_db.Services;

namespace Projet_Chess_db.Views
{
    public partial class MainWindow : Window
    {
        private readonly IDataService _dataService;
        private readonly IEloCalculator _eloCalculator;

        public MainWindow()
        {
            InitializeComponent();

            _dataService = new JsonDataService();
            _eloCalculator = new ChessEloCalculator();

            // Afficher la vue des joueurs par défaut
            ShowPlayers(null, null);
        }

        private void ShowPlayers(object sender, RoutedEventArgs e)
        {
            var content = this.FindControl<ContentControl>("MainContent");
            if (content != null)
            {
                content.Content = new PlayerView(_dataService, _eloCalculator);
            }
        }

        private void ShowCompetitions(object sender, RoutedEventArgs e)
        {
            var content = this.FindControl<ContentControl>("MainContent");
            if (content != null)
            {
                content.Content = new CompetitionView(_dataService);
            }
        }

        private void ShowGames(object sender, RoutedEventArgs e)
        {
            var content = this.FindControl<ContentControl>("MainContent");
            if (content != null)
            {
                content.Content = new GameView(_dataService, _eloCalculator);
            }
        }

        private void ShowStatistics(object sender, RoutedEventArgs e)
        {
            var content = this.FindControl<ContentControl>("MainContent");
            if (content != null)
            {
                content.Content = new StatisticsView(_dataService);
            }
        }
    }
}