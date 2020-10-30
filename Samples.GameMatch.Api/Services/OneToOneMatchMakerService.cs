using System.Linq;
using Microsoft.Extensions.Logging;

namespace Samples.GameMatch.Api
{
    public class OneToOneMatchMakerService : IMatchMakerService
    {
        private readonly IUserRatingRepository _userRatingRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IMatchPairRepository _matchPairRepository;
        private readonly ITransformer<MakeMatch, Match> _matchTransformer;
        private readonly ITransformer<UserRating, MatchPair> _matchPairTransformer;
        private readonly ILogger<OneToOneMatchMakerService> _log;

        public OneToOneMatchMakerService(IUserRatingRepository userRatingRepository,
                                         IMatchRepository matchRepository,
                                         IMatchPairRepository matchPairRepository,
                                         ITransformer<MakeMatch, Match> matchTransformer,
                                         ITransformer<UserRating, MatchPair> matchPairTransformer,
                                         ILogger<OneToOneMatchMakerService> log)
        {
            _userRatingRepository = userRatingRepository;
            _matchRepository = matchRepository;
            _matchPairRepository = matchPairRepository;
            _matchTransformer = matchTransformer;
            _matchPairTransformer = matchPairTransformer;
            _log = log;
        }

        public void Match(MakeMatch request)
        {
            var requestedByUserRating = _userRatingRepository.GetByUserGameType(request.RequestedByUserId,
                                                                                request.GameType);

            if (requestedByUserRating == null)
            {
                _log.LogWarning("User {UserId} does not have a rating for GameType of {GameType}", request.RequestedByUserId, request.GameType.ToString());

                return;
            }

            // Add the match definition as needed
            var match = _matchTransformer.To(request);

            var matchId = _matchRepository.AddIgnore(match);

            // For now, simply going to always take found possible matches and add them as matches you could pick to play. Naturally this could get
            // a lot more complex, including duplicate checking (though there would likely be some threshold at which the duplicates do not apply any
            // longer since time has gone on and things have changed), or to match only a certain number of a particular strategy (i..e. the easiest or most
            // difficult, or those I haven't played before, etc.)
            foreach (var userRating in _userRatingRepository.GetPossibleMatches(requestedByUserRating.Rating, request)
                                                            .Where(ur => ur.UserId != requestedByUserRating.UserId))
            {
                // Just insert matches
                var matchPair = _matchPairTransformer.To(userRating);

                matchPair.RequestedUserId = request.RequestedByUserId;
                matchPair.RequestedUserMmr = requestedByUserRating.Rating;
                matchPair.MatchId = matchId;

                _matchPairRepository.Add(matchPair);
            }
        }
    }
}
