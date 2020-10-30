using System;
using System.Collections.Generic;
using System.Linq;

namespace Samples.GameMatch.Api
{
    public class InMemoryMatchRepository : BaseInMemoryModelRepository<Match>, IMatchRepository
    {
        public Guid AddIgnore(Match match)
        {
            var existingBy = _dbModels.Values.FirstOrDefault(v => v.GameType == match.GameType &&
                                                                  v.MatchType == match.MatchType &&
                                                                  Math.Abs(v.MaxMmrGap - match.MaxMmrGap) < 0.0000001);

            return existingBy?.Id ?? Add(match);
        }
    }

    public class InMemoryMatchPairRepository : BaseInMemoryModelRepository<MatchPair>, IMatchPairRepository
    {
        private readonly IMatchRepository _matchRepository;

        public InMemoryMatchPairRepository(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public IEnumerable<MatchResponse> QueryByRequest(Guid? byUserId, QueryMatchesRequest query)
        {
            var yielded = 0;
            var enumerated = 0;

            var take = query.Take > 0
                           ? query.Take
                           : 100;

            // Just a simple iterative implementation to page through matched pairs...
            foreach (var match in _matchRepository.Query(m => (query.GameType == null || m.GameType == query.GameType) &&
                                                              (query.MatchType == null || m.MatchType == query.MatchType) &&
                                                              (query.MaxMmrGap == null || m.MaxMmrGap <= query.MaxMmrGap))
                                                  .OrderByDescending(m => m.CreatedOn))
            {
                var pairs = new List<MatchPairResponse>();

                foreach (var matchPair in _dbModels.Values.Where(v => (byUserId == null || byUserId == v.RequestedUserId) &&
                                                                      v.MatchId == match.Id)
                                                   .OrderByDescending(v => v.CreatedOn))
                {
                    enumerated++;

                    if (enumerated <= query.Skip)
                    {
                        continue;
                    }

                    pairs.Add(new MatchPairResponse
                              {
                                  RequestedUserId = matchPair.RequestedUserId,
                                  RequestedUserMmr = matchPair.RequestedUserMmr,
                                  MatchedUserId = matchPair.MatchedUserId,
                                  MatchedUserMmr = matchPair.MatchedUserMmr,
                                  MatchDate = matchPair.CreatedOn
                              });

                    yielded++;

                    if (yielded >= take)
                    {
                        break;
                    }
                }

                if (pairs.Count > 0)
                {
                    yield return new MatchResponse
                                 {
                                     GameType = match.GameType,
                                     MatchType = match.MatchType,
                                     MaxMmrGap = match.MaxMmrGap,
                                     Matches = pairs.AsListReadOnly()
                                 };
                }

                if (yielded >= take)
                {
                    break;
                }
            }
        }
    }
}
