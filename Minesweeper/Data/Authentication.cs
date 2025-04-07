using System.Linq;

namespace Minesweeper.Data;

public class Authentication
{
    public static bool RegisterUser(string username, string password)
    {
        using var db = new MinesweeperData();
        
        if (db.Users.Any(u => u.Name == username))
            return false;

        var user = new User
        {
            Name = username,
            Password = password
        };
        db.Users.Add(user);
        db.SaveChanges();
        return true;
    }

    public static bool LoginUser(string username, string password)
    {
        using var db = new MinesweeperData();
        var user = db.Users.FirstOrDefault(u => u.Name == username);
        return user != null && user.Password == password;
    }
}