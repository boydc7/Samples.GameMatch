using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Samples.GameMatch.Api
{
    public class MatchesController : ApiControllerBase
    {
        private readonly IMatchQueue _matchQueue;
        private readonly IMatchPairRepository _matchPairRepository;
        private readonly ITransformer<MatchRequest, MakeMatch> _transformer;

        public MatchesController(IMatchQueue matchQueue, IMatchPairRepository matchPairRepository,
                                 ITransformer<MatchRequest, MakeMatch> transformer)
        {
            _matchQueue = matchQueue;
            _matchPairRepository = matchPairRepository;
            _transformer = transformer;
        }

        /// <summary>
        ///     Returns matches for the <see cref="User" /> that is authenticated based on the <see cref="QueryMatchesRequest" />
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET /matches/me?skip=0&amp;take=100&amp;maxMmrGap=1.2
        ///
        /// </remarks>
        /// <returns>Matches for the <see cref="User" /> that is authenticated based on the <see cref="QueryMatchesRequest" /></returns>
        /// <response code="200">Matches for the <see cref="User" /> that is authenticated based on the <see cref="QueryMatchesRequest" /></response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<GmApiResults<MatchResponse>> GetMyMatches([FromQuery] QueryMatchesRequest request)
        {
            var myMatches = _matchPairRepository.QueryByRequest(User.GetUserId(), request);

            return myMatches.AsOkGmApiResults();
        }

        /// <summary>
        ///     Admin required. Returns matches for any/all users based on the <see cref="QueryMatchesRequest" /> requsted.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET /matches?skip=0&amp;take=100&amp;maxMmrGap=1.2
        ///
        /// </remarks>
        /// <returns>Matches for the <see cref="User" /> that is authenticated based on the <see cref="QueryMatchesRequest" /></returns>
        /// <response code="200">Matches for the <see cref="User" /> that is authenticated based on the <see cref="QueryMatchesRequest" /></response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "admin")]
        public ActionResult<GmApiResults<MatchResponse>> GetMatches([FromQuery] QueryMatchesRequest request)
        {
            var myMatches = _matchPairRepository.QueryByRequest(User.GetUserId(), request);

            return myMatches.AsOkGmApiResults();
        }

        /// <summary>
        ///     Queues the current authenticated <see cref="User" /> for match making to a game of some type
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST /matches/me
        ///         {
        ///             "Type": "Chess",
        ///             "MaxMmrGap": null,
        ///             "MatchType": "Any"
        ///         }
        ///
        /// </remarks>
        /// <param name="request">The <see cref="MatchRequest" /> definition</param>
        /// <response code="204">Successfully enqueued for matching</response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        [HttpPost("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public NoContentResult PostMatchMe([FromBody] MatchRequest request)
        {
            var makeMatch = _transformer.To(request);

            makeMatch.RequestedByUserId = User.GetUserId();

            _matchQueue.Enqueue(makeMatch);

            return NoContent();
        }

        /// <summary>
        ///     Admin required. Queues a match request with the definition of the specified <see cref="AdminMatchRequest" />
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST /matches
        ///         {
        ///             "UserId": "b9763dff-acc5-4530-a1be-133cff404092",
        ///             "GameType": "Chess",
        ///             "MaxMmrGap": null,
        ///             "MatchType": "Any"
        ///         }
        ///
        /// </remarks>
        /// <param name="request">The <see cref="AdminMatchRequest" /> definition</param>
        /// <response code="204">Successfully enqueued for matching</response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "admin")]
        public NoContentResult PostMatch([FromBody] AdminMatchRequest request)
        {
            var makeMatch = _transformer.To(request);

            makeMatch.RequestedByUserId = request.UserId;

            _matchQueue.Enqueue(makeMatch);

            return NoContent();
        }
    }
}
