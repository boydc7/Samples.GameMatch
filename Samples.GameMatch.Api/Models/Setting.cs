using System.ComponentModel.DataAnnotations;

namespace Samples.GameMatch.Api
{
    public class Setting : BaseModel
    {
        [Required]
        public double DefaultMaxMmrGap { get; set; }
    }

    public class SettingRequest
    {
        [Required]
        public double DefaultMaxMmrGap { get; set; }
    }
}
