using System;
using System.ComponentModel.DataAnnotations;

namespace Samples.GameMatch.Api
{
    public class MatchUser : BaseModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid MatchId { get; set; }
    }
}
