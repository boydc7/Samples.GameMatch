using System;
using System.Linq;

namespace Samples.GameMatch.Api
{
    public class InMemoryUserRepository : BaseInMemoryModelRepository<User>, IUserRepository
    {
        public User GetByEmail(string email)
            => _dbModels.Values.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}
