using System.Windows;
using Minesweeper.Data;

namespace Minesweeper;

public partial class LoginWindow : Window
{
    private readonly Authentication _auth;
    
    public LoginWindow()
    {
        InitializeComponent();
        _auth = new Authentication();
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        string username = TxtUsername.Text;
        string password = TxtPassword.Password;

        if (Authentication.LoginUser(username, password))
        {
            var user = new User { Name = username }; 
            Session.CurrentUser = user;

            MessageBox.Show("Login successful!");
            Close();
        }
        else
            MessageBox.Show("Invalid username or password.");
    }

    private void BtnRegister_Click(object sender, RoutedEventArgs e)
    {
        string username = TxtUsername.Text;
        string password = TxtPassword.Password;

        MessageBox.Show(Authentication.RegisterUser(username, password) 
            ? "Registration successful!" : "Username already exists.");
    }
}