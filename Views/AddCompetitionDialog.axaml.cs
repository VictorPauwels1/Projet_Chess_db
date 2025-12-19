using Avalonia.Controls;
using Avalonia.Interactivity;
using Projet_Chess_db.Models;
using System;

namespace Projet_Chess_db.Views
{
    public partial class AddCompetitionDialog : Window
    {
        public AddCompetitionDialog()
        {
            InitializeComponent();

            // Initialiser avec dates par défaut
            DateStart.SelectedDate = new DateTimeOffset(DateTime.Now.AddDays(7));
            DateEnd.SelectedDate = new DateTimeOffset(DateTime.Now.AddDays(10));
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text) ||
                string.IsNullOrWhiteSpace(TxtLocation.Text))
            {

                return;
            }

            var competition = new Competition
            {
                Name = TxtName.Text,
                Location = TxtLocation.Text,
                StartDate = DateStart.SelectedDate?.DateTime ?? DateTime.Now.AddDays(7),
                EndDate = DateEnd.SelectedDate?.DateTime ?? DateTime.Now.AddDays(10),
                MaxParticipants = (int)NumMaxParticipants.Value,
                Status = CompetitionStatus.Planned
            };

            Close(competition);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}