using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Samples.GameMatch.Api
{
    public class RatingsController : ApiControllerBase
    {
        private readonly ITransformer<UserRatingRequest, UserRating> _userRatingTransformer;
        private readonly IUserRatingRepository _userRatingRepository;

        public RatingsController(ITransformer<UserRatingRequest, UserRating> userRatingTransformer,
                                 IUserRatingRepository userRatingRepository)
        {
            _userRatingTransformer = userRatingTransformer;
            _userRatingRepository = userRatingRepository;
        }

        /// <summary>
        ///     Requires admin. Upserts a rating for the user and gametype specified.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         PUT /ratings
        ///         {
        ///             "UserId": "b9763dff-acc5-4530-a1be-133cff404092",
        ///             "GameType": "Chess",
        ///             "Rating": 1.23
        ///         }
        ///
        /// </remarks>
        /// <param name="request">The <see cref="AdminUserRatingRequest" /> definition</param>
        /// <response code="204">Successfully updated rating info</response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "admin")]
        public NoContentResult PutUserRating([FromBody] AdminUserRatingRequest request)
        {
            var existingRating = _userRatingRepository.GetByUserGameType(request.UserId, request.GameType);

            var userRating = _userRatingTransformer.To(request, existingRating);

            if (userRating.UserId == default)
            {
                userRating.UserId = request.UserId;
            }

            _userRatingRepository.AddOrUpdate(userRating);

            return NoContent();
        }

        /// <summary>
        ///     Requires admin.  Returns all the <see cref="UserRating" />s in the system, no paging or nuttin, so...careful
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET /users
        ///
        /// </remarks>
        /// <returns>All of the <see cref="UserRating" />s</returns>
        /// <response code="200">All of the <see cref="UserRating" />s</response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        [Authorize(Roles = "admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<GmApiResults<UserRating>> GetUserRatings()
            => _userRatingRepository.GetAll().AsOkGmApiResults();
    }
}
