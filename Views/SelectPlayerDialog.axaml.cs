using Avalonia.Controls;
using Avalonia.Interactivity;
using Projet_Chess_db.Models;
using System.Collections.Generic;

namespace Projet_Chess_db.Views
{
    public partial class SelectPlayerDialog : Window
    {
        public SelectPlayerDialog(List<Player> players)
        {
            InitializeComponent();

            PlayerListBox.ItemsSource = players;
        }

        private void Select(object sender, RoutedEventArgs e)
        {
            if (PlayerListBox.SelectedItem is Player selectedPlayer)
            {
                Close(selectedPlayer);
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}