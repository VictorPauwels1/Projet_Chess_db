using Avalonia.Controls;
using Avalonia.Interactivity;
using Projet_Chess_db.Models;
using System;

namespace Projet_Chess_db.Views
{
    public partial class EditPlayerDialog : Window
    {
        private readonly Player _originalPlayer;

        public EditPlayerDialog(Player player)
        {
            InitializeComponent();

            _originalPlayer = player;

            // Pré-remplir avec les données actuelles
            TxtFirstName.Text = player.FirstName;
            TxtLastName.Text = player.LastName;
            DateBirth.SelectedDate = new DateTimeOffset(player.DateOfBirth);
            TxtEmail.Text = player.Email;
            TxtPhone.Text = player.PhoneNumber;
            NumElo.Value = player.EloRating;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFirstName.Text) ||
                string.IsNullOrWhiteSpace(TxtLastName.Text))
            {
                return;
            }

            // Créer le joueur modifié (en gardant le même ID)
            var updatedPlayer = new Player
            {
                Id = _originalPlayer.Id,
                FirstName = TxtFirstName.Text,
                LastName = TxtLastName.Text,
                DateOfBirth = DateBirth.SelectedDate?.DateTime ?? DateTime.Now.AddYears(-20),
                Email = TxtEmail.Text ?? "",
                PhoneNumber = TxtPhone.Text ?? "",
                EloRating = (int)NumElo.Value,
                RegistrationDate = _originalPlayer.RegistrationDate
            };

            Close(updatedPlayer);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}