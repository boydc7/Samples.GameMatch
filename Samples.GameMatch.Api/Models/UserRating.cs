using System;
using System.ComponentModel.DataAnnotations;

namespace Samples.GameMatch.Api
{
    public class UserRating : BaseModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [EnumDataType(typeof(GameType))]
        public GameType GameType { get; set; }

        [Required]
        public double Rating { get; set; }
    }

    public class AdminUserRatingRequest : UserRatingRequest
    {
        [Required]
        public Guid UserId { get; set; }
    }

    public class UserRatingRequest
    {
        [Required]
        [EnumDataType(typeof(GameType))]
        public GameType GameType { get; set; }

        [Required]
        public double Rating { get; set; }
    }
}
