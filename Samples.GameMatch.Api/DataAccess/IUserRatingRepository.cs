using System;
using System.Collections.Generic;

namespace Samples.GameMatch.Api
{
    public interface IUserRatingRepository : IBaseModelRepository<UserRating>
    {
        UserRating GetByUserGameType(Guid userId, GameType gameType);
        IEnumerable<UserRating> GetPossibleMatches(double rating, MakeMatch makeMatch);
    }
}
