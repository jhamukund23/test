using Dss.application.Interfaces;
using Dss.Application.Common.Interfaces;
using Dss.Domain.Models;
using Dss.Infrastructure.Persistence;

public class UserService : RepositoryBase<User>, IUserService
{
    public UserService(ApplicationDBContext context) : base(context)
    { }
    public async Task<string> CreateUserAsync(User user)
    {
        Create(user);
        await SaveAsync();
        return user.Id.ToString();
    }

    public async Task<IList<User>> GetAllUserAsync()
    {
        var users = await FindAllAsync();
        return users.OrderBy(x => x.FullName).ThenBy(c => c.CreatedAt).ToList();
    }

    public async Task<User?> GetUserByIdAsync(Guid userGuid)
    {
        var res = await FindByConditionAync(o => o.UserGuid.Equals(userGuid));
        return res.FirstOrDefault();
    }

    public async Task<bool> IsUserExistsAsync(string emailId)
    {
        var userDetails = await FindByConditionAync(o => o.EmailId.Equals(emailId));
        return (userDetails.FirstOrDefault() != null) ? true : false;
    }
    public async Task<bool> UpdateUserAsync(User model)
    {
        Update(model);
        int rowsAffected = await Save();
        if (rowsAffected > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public async Task<bool> DeleteUserAsync(User model)
    {
        Delete(model);
        int rowsAffected = await Save();
        if (rowsAffected > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}