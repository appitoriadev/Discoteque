using System.Security.Cryptography;
using System.Text;
using Discoteque.Data;
using Discoteque.Data.Models;
using Discoteque.Business.IServices;
using Microsoft.EntityFrameworkCore;

namespace Discoteque.Business.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User> CreateUser(User user)
    {
        user.PasswordHash = HashPassword(user.PasswordHash);
        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveAsync();
        return user;
    }

    public async Task<User> GetUserById(int id)
    {
        return await _unitOfWork.UserRepository.FindAsync(id);
    }

    public async Task<User> GetUserByUsername(string username)
    {
        var users = await _unitOfWork.UserRepository.GetAllAsync(u => u.Username == username);
        return users.FirstOrDefault()!;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _unitOfWork.UserRepository.GetAllAsync();
    }

    public async Task<User> UpdateUser(User user)
    {
        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            user.PasswordHash = HashPassword(user.PasswordHash);
        }
        await _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveAsync();
        return user;
    }

    public async Task DeleteUser(int id)
    {
        var user = await GetUserById(id);
        if (user != null)
        {
            await _unitOfWork.UserRepository.Delete(user);
            await _unitOfWork.SaveAsync();
        }
    }

    public async Task<bool> ValidateUser(string username, string password)
    {
        var user = await GetUserByUsername(username);
        if (user == null)
            return false;

        return VerifyPassword(password, user.PasswordHash);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string inputPassword, string hashedPassword)
    {
        return HashPassword(inputPassword) == hashedPassword;
    }
} 