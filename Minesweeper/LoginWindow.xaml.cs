using System.Linq;
using System.Windows;
using Minesweeper.Data;

namespace Minesweeper;

public partial class LoginWindow
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        string username = TxtUsername.Text;
        string password = TxtPassword.Password;

        if (Authentication.LoginUser(username, password))
        {
            using var db = new MinesweeperData();
            
            var user = db.Users
                .SingleOrDefault(u => u.Name == username);

            if (user != null)
            {
                Session.CurrentUser = user; // Set the full user object from the database
                MessageBox.Show("Login successful!");
                Close();
            }
            else
            {
                MessageBox.Show("User not found in the database.");
            }
        }
        else
        {
            MessageBox.Show("Invalid username or password.");
        }
    }

    private void BtnRegister_Click(object sender, RoutedEventArgs e)
    {
        string username = TxtUsername.Text;
        string password = TxtPassword.Password;

        MessageBox.Show(Authentication.RegisterUser(username, password) 
            ? "Registration successful!" : "Username already exists.");
    }
}