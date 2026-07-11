using Microsoft.EntityFrameworkCore;
using SunwinInventory.Data;
using SunwinInventory.Models;
using SunwinInventory.Repositories.Interfaces;

namespace SunwinInventory.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly SunwinInventoryContext _context;

        public AccountRepository(SunwinInventoryContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginAsync(string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        
    }
}