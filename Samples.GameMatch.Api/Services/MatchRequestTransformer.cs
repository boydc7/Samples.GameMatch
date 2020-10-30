namespace Samples.GameMatch.Api
{
    public class MatchRequestTransformer : ITransformer<MatchRequest, MakeMatch>
    {
        public MakeMatch To(MatchRequest source, MakeMatch existing = null)
            => new MakeMatch
               {
                   RequestedByUserId = existing?.RequestedByUserId ?? default,
                   GameType = source.Type == GameType.Unspecified
                                  ? existing?.GameType ?? GameType.Unspecified
                                  : source.Type,
                   MaxMmrGap = source.MaxMmrGap,
                   MatchType = source.MatchType
               };
    }

    public class MakeMatchMatchTransformer : ITransformer<MakeMatch, Match>
    {
        public Match To(MakeMatch source, Match existing = null)
            => new Match
               {
                   Id = existing?.Id ?? default,
                   CreatedOn = existing?.CreatedOn ?? default,
                   GameType = source.GameType == GameType.Unspecified
                                  ? existing?.GameType ?? GameType.Unspecified
                                  : source.GameType,
                   MaxMmrGap = source.MaxMmrGap ?? existing?.MaxMmrGap ?? 0,
                   MatchType = source.MatchType
               };
    }

    public class MatchPairTransformer : ITransformer<UserRating, MatchPair>
    {
        public MatchPair To(UserRating source, MatchPair existing = null)
            => new MatchPair
               {
                   // At some point perhaps the MatchPair includes definition about the matched user
                   // at the time of matching, but for now....
                   Id = existing?.Id ?? default,
                   CreatedOn = existing?.CreatedOn ?? default,
                   MatchedUserId = source.UserId,
                   MatchedUserMmr = source.Rating
               };
    }
}
