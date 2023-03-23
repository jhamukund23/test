using Dss.Domain.Models;
namespace Dss.application.Interfaces;
public interface IUserService
{
    Task<IList<User>> GetAllUserAsync();
    Task<User?> GetUserByIdAsync(Guid uGuid);
    Task<bool> IsUserExistsAsync(string email);
    Task<string> CreateUserAsync(User model);
    Task<bool> UpdateUserAsync(User model);
    Task<bool> DeleteUserAsync(User model);
}