using System;
using SampleSecureWeb.Models;

namespace SampleSecureWeb.Data;

public class UserData : IUser
{
    private readonly ApplicationDbContext _db;
    public UserData(ApplicationDbContext db)
    {
        _db = db;
    }

    public User GetUserByUsername(string username)
    {
       return _db.Users.FirstOrDefault(u => u.Username == username);
    }

    public User Login(User user)
    {
        var _user = _db.Users.FirstOrDefault(u => u.Username == user.Username);
        if (_user == null)
        {
            throw new Exception("User not found");
        }
        if (!BCrypt.Net.BCrypt.Verify(user.Password, _user.Password))
        {
            throw new Exception("Password is incorrect");
        }
        return _user;
    }

    public User Registration(User user)
    {
        try
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

public void UpdateUser(User user)
{
    // Mengambil pengguna berdasarkan username
    var existingUser = _db.Users.FirstOrDefault(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase));
    
    if (existingUser != null)
    {
        // Hash password baru sebelum menyimpannya
        existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        _db.SaveChanges();
    }
    else
    {
        throw new Exception("User not found.");
    }
}






}