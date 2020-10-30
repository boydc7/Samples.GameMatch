namespace Samples.GameMatch.Api
{
    public interface IUserRepository : IBaseModelRepository<User>
    {
        User GetByEmail(string email);
    }
}
