using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Samples.GameMatch.Api
{
    [Authorize(Roles = "admin")]
    public class SettingsController : ApiControllerBase
    {
        private readonly ISettingsRepository _settingsRepository;

        public SettingsController(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        /// <summary>
        ///     Admin required. Updates the default settings for match making.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT /settings
        ///         {
        ///             "DefaultMaxMmrGap": 0.1
        ///         }
        ///
        /// </remarks>
        /// <param name="request">The <see cref="SettingRequest" /> definition</param>
        /// <response code="204">Successfully updated settings</response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public NoContentResult PutSettings([FromBody] SettingRequest request)
        {
            var defaultSetting = _settingsRepository.GetDefaultSetting();

            defaultSetting.DefaultMaxMmrGap = request.DefaultMaxMmrGap;

            _settingsRepository.Update(defaultSetting);

            return NoContent();
        }
    }
}
