using System;
using System.Collections.Generic;

namespace Samples.GameMatch.Api
{
    public interface IMatchRepository : IBaseModelRepository<Match>
    {
        Guid AddIgnore(Match match);
    }

    public interface IMatchPairRepository : IBaseModelRepository<MatchPair>
    {
        IEnumerable<MatchResponse> QueryByRequest(Guid? byUserId, QueryMatchesRequest query);
    }
}
