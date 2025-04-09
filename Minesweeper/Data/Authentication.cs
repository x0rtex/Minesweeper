using System.Data.Entity;
using System.Threading.Tasks;

namespace Minesweeper.Data;

public static class Authentication
{
    public static async Task<bool> RegisterUser(string username, string password)
    {
        using var db = new MinesweeperData();
        
        if (await db.Users
                .AnyAsync(u => u.Name == username))
            return false;

        var user = new User
        {
            Name = username,
            Password = PasswordHasher.HashPassword(password)
        };
        
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return true;
    }

    public static async Task<bool> LoginUser(string username, string password)
    {
        using var db = new MinesweeperData();
        
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Name == username);
        
        return user != null && PasswordHasher.VerifyPassword(password, user.Password);
    }
}