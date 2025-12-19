using Avalonia.Controls;
using Avalonia.Interactivity;
using Projet_Chess_db.Models;
using System;

namespace Projet_Chess_db.Views
{
    public partial class AddPlayerDialog : Window
    {
        public AddPlayerDialog()
        {
            InitializeComponent();

            DateBirth.SelectedDate = new DateTimeOffset(DateTime.Now.AddYears(-20));
        }

        private void Save(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFirstName.Text) ||
                string.IsNullOrWhiteSpace(TxtLastName.Text))
            {
                return;
            }

            var player = new Player
            {
                FirstName = TxtFirstName.Text,
                LastName = TxtLastName.Text,
                DateOfBirth = DateBirth.SelectedDate?.DateTime ?? DateTime.Now.AddYears(-20),
                Email = TxtEmail.Text ?? "",
                PhoneNumber = TxtPhone.Text ?? "",
                EloRating = (int)NumElo.Value
            };

            Close(player);
        }

        private void Cancel(object? sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}