namespace Samples.GameMatch.Api
{
    public class UserRatingTransformer : ITransformer<UserRatingRequest, UserRating>
    {
        public UserRating To(UserRatingRequest source, UserRating existing = null)
            => new UserRating
               {
                   Id = existing?.Id ?? default,
                   CreatedOn = existing?.CreatedOn ?? default,
                   UserId = existing?.UserId ?? default,
                   GameType = source.GameType == GameType.Unspecified
                                  ? existing?.GameType ?? GameType.Unspecified
                                  : source.GameType,
                   Rating = source.Rating
               };
    }
}
