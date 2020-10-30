using System;
using System.Collections.Generic;
using System.Linq;

namespace Samples.GameMatch.Api
{
    public class InMemoryUserRatingRepository : BaseInMemoryModelRepository<UserRating>, IUserRatingRepository
    {
        private readonly ISettingsRepository _settingsRepository;

        public InMemoryUserRatingRepository(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public UserRating GetByUserGameType(Guid userId, GameType gameType)
            => _dbModels.Values.FirstOrDefault(u => u.UserId.Equals(userId) && u.GameType == gameType);

        public IEnumerable<UserRating> GetPossibleMatches(double rating, MakeMatch makeMatch)
        {
            var within = makeMatch.MaxMmrGap ?? _settingsRepository.GetDefaultSetting().DefaultMaxMmrGap;

            // If asked for Harder matches only, lower bound is the rating.
            // Otherwise, it is the rating minus the within requested
            var lower = makeMatch.MatchType == MatchType.HarderOnly
                            ? rating
                            : rating - Math.Abs(within);

            // If asked for easier matches only, upper bound is the rating
            // Otherwise, it is the rating plus the within requested
            var upper = makeMatch.MatchType == MatchType.EasierOnly
                            ? rating
                            : rating + Math.Abs(within);

            return _dbModels.Values.Where(v => v.GameType == makeMatch.GameType &&
                                               v.Rating >= lower &&
                                               v.Rating <= upper);
        }
    }
}
