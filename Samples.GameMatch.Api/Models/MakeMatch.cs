using System;
using System.ComponentModel.DataAnnotations;

namespace Samples.GameMatch.Api
{
    public class MakeMatch
    {
        [Required]
        public Guid RequestedByUserId { get; set; }

        [Required]
        public GameType GameType { get; set; }

        public MatchType MatchType { get; set; }

        public double? MaxMmrGap { get; set; }
    }
}
