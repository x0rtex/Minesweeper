using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using Minesweeper.Data;

namespace Minesweeper;

public partial class LoginWindow
{
    public LoginWindow() => InitializeComponent();

    private async void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string username = TxtUsername.Text;
            string password = TxtPassword.Password;

            if (await Authentication.LoginUser(username, password))
            {
                using var db = new MinesweeperData();
            
                var user = await db.Users
                    .SingleOrDefaultAsync(u => u.Name == username);

                if (user != null)
                {
                    Session.CurrentUser = user;
                    MessageBox.Show("Login successful!");
                    Close();
                }
                else
                    MessageBox.Show("User not found in the database.");
            }
            else
                MessageBox.Show("Invalid username or password.");
        }
        catch (Exception error)
        {
            Console.WriteLine($@"Error occured: {error.Message}");
            throw; // TODO handle exception
        }
    }

    private async void BtnRegister_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string username = TxtUsername.Text;
            string password = TxtPassword.Password;

            MessageBox.Show(await Authentication.RegisterUser(username, password) 
                ? "Registration successful!" : "Username already exists.");
        }
        catch (Exception error)
        {
            Console.WriteLine($@"Error occured: {error.Message}");
            throw; // TODO handle exception
        }
    }
}