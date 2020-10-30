using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Samples.GameMatch.Api
{
    public class Match : BaseModel
    {
        [Required]
        [EnumDataType(typeof(GameType))]
        public GameType GameType { get; set; }

        [Required]
        [EnumDataType(typeof(MatchType))]
        public MatchType MatchType { get; set; }

        [Required]
        public double MaxMmrGap { get; set; }
    }

    public class MatchPair : BaseModel
    {
        // At some point perhaps the MatchPair includes definition about the matched user
        // at the time of matching, but for now....
        [Required]
        public Guid MatchId { get; set; }

        [Required]
        public Guid RequestedUserId { get; set; }

        [Required]
        public double RequestedUserMmr { get; set; }

        [Required]
        public Guid MatchedUserId { get; set; }

        [Required]
        public double MatchedUserMmr { get; set; }
    }

    public class QueryMatchesRequest
    {
        public GameType? GameType { get; set; }
        public MatchType? MatchType { get; set; }
        public double? MaxMmrGap { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }

    public class MatchResponse
    {
        public GameType GameType { get; set; }
        public MatchType MatchType { get; set; }
        public double MaxMmrGap { get; set; }
        public IReadOnlyList<MatchPairResponse> Matches { get; set; }
    }

    public class MatchPairResponse
    {
        public Guid RequestedUserId { get; set; }
        public double RequestedUserMmr { get; set; }
        public Guid MatchedUserId { get; set; }
        public double MatchedUserMmr { get; set; }
        public DateTime MatchDate { get; set; }
    }

    public class AdminMatchRequest : MatchRequest
    {
        [Required]
        public Guid UserId { get; set; }
    }

    public class MatchRequest
    {
        [Required]
        [EnumDataType(typeof(GameType))]
        public GameType Type { get; set; }

        public MatchType MatchType { get; set; }

        public double? MaxMmrGap { get; set; }
    }
}
