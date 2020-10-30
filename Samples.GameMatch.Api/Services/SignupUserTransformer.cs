namespace Samples.GameMatch.Api
{
    public class SignupUserTransformer : ITransformer<SignupUserRequest, User>
    {
        public User To(SignupUserRequest source, User existing = null)
            => new User
               {
                   Id = existing?.Id ?? default,
                   CreatedOn = existing?.CreatedOn ?? default,
                   FirstName = source.FirstName,
                   LastName = source.LastName,
                   Email = source.Email,
                   Password = source.Password.ToSha256Base64(),
                   Role = UserRole.User
               };
    }
}
