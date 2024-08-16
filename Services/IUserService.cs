using LinkShortenerAPI.Models;
using LinkShortenerAPI.Models.DTO;

namespace LinkShortenerAPI.Services;

public interface IUserService
{
    Task<UserLoginResponseDTO?> Login(string userName, string password);
    Task<UserLoginResponseDTO?> Login(string phoneNumber);
    Task<UserLoginResponseDTO?> Register(UserCreateDTO userCreateDto);
    public Task<List<User>> GetUsers();
    public Task<User?> GetUserByUserName(string userName);
}