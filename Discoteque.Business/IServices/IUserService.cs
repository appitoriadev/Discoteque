using Discoteque.Data.Models;

namespace Discoteque.Business.IServices;

public interface IUserService
{
    Task<User> CreateUser(User user);
    Task<User> GetUserById(int id);
    Task<User> GetUserByUsername(string username);
    Task<IEnumerable<User>> GetAllUsers();
    Task<User> UpdateUser(User user);
    Task DeleteUser(int id);
    Task<bool> ValidateUser(string username, string password);
} 