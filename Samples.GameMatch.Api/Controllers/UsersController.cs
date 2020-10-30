using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Samples.GameMatch.Api
{
    public class UsersController : ApiControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRatingRepository _userRatingRepository;
        private readonly ITransformer<SignupUserRequest, User> _signupTransformer;
        private readonly IConfiguration _configuration;
        private readonly ITransformer<UserRatingRequest, UserRating> _userRatingTransformer;

        public UsersController(IUserRepository userRepository,
                               IUserRatingRepository userRatingRepository,
                               ITransformer<SignupUserRequest, User> signupTransformer,
                               ITransformer<UserRatingRequest, UserRating> userRatingTransformer,
                               IConfiguration configuration)
        {
            _userRepository = userRepository;
            _userRatingRepository = userRatingRepository;
            _signupTransformer = signupTransformer;
            _configuration = configuration;
            _userRatingTransformer = userRatingTransformer;
        }

        /// <summary>
        ///     Creates a new <see cref="User" /> in the system with the given name, email, password
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST /users/signup
        ///         {
        ///             "FirstName": "Chad",
        ///             "LastName": "Boyd",
        ///             "Email": "chadboyd@gamematchdemo.io",
        ///             "Password": "SomeSuperSecretPasswordForChad"
        ///         }
        ///
        /// </remarks>
        /// <param name="request">The <see cref="SignupUserRequest" /> definition</param>
        /// <returns>The newly created <see cref="User" /></returns>
        /// <response code="200">The newly created <see cref="User" /></response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        /// <response code="409">If a <see cref="User" /> with the email specified already exists in the system</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [AllowAnonymous]
        public ActionResult<GmApiResult<User>> Post([FromBody] SignupUserRequest request)
        {
            var existingUser = _userRepository.GetByEmail(request.Email);

            if (existingUser != null)
            {
                return Conflict("User", "Email");
            }

            var newUser = _signupTransformer.To(request);

            _userRepository.Add(newUser);

            return newUser.AsOkGmApiResult();
        }

        /// <summary>
        ///     Authenticates a <see cref="User" /> and returns a valid JWT if successful
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST /users/login
        ///         {
        ///             "Email": "chadboyd@gamematchdemo.io",
        ///             "Password": "SomeSuperSecretPasswordForChad"
        ///         }
        ///
        /// </remarks>
        /// <param name="request">The <see cref="LoginRequest" /> definition</param>
        /// <returns>A valid JWT if successfull</returns>
        /// <response code="200">The JWT for the authenticated <see cref="User" /></response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public ActionResult<GmApiResult<string>> PostLogin([FromBody] LoginRequest request)
        {
            var existingUser = _userRepository.GetByEmail(request.Email);

            if (existingUser == null || !existingUser.Password.Equals(request.Password.ToSha256Base64(), StringComparison.Ordinal))
            {
                return BadRequest(new
                                  {
                                      Message = "Email or password is incorrect or invalid"
                                  });
            }

            return existingUser.GenerateJwtToken(_configuration)
                               .AsOkGmApiResult();
        }

        /// <summary>
        ///     Returns the <see cref="User" /> that is authenticated
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET /users/me
        ///
        /// </remarks>
        /// <returns>The <see cref="User" /> that is authenticated</returns>
        /// <response code="200">The <see cref="User" /> that is authenticated</response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        /// <response code="404">No user record for the authenticated user (would be invalid)</response>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GmApiResult<User>> GetMyUser()
        {
            var existingUser = _userRepository.GetById(User.GetUserId());

            return existingUser == null
                       ? NotFound()
                       : existingUser.AsOkGmApiResult();
        }

        /// <summary>
        ///     Requires admin.  Returns the <see cref="User" /> with the id specified
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET /users/b9763dff-acc5-4530-a1be-133cff404092
        ///
        /// </remarks>
        /// <returns>The <see cref="User" /> with the id requested</returns>
        /// <response code="200">The <see cref="User" /> with the id requested</response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        /// <response code="404">No user record with the id requested</response>
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GmApiResult<User>> GetUser(Guid id)
        {
            var existingUser = _userRepository.GetById(id);

            return existingUser == null
                       ? NotFound()
                       : existingUser.AsOkGmApiResult();
        }

        /// <summary>
        ///     Requires admin.  Returns all the <see cref="User" />s in the system, no paging or nuttin, so...careful
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET /users
        ///
        /// </remarks>
        /// <returns>All of the <see cref="User" />s</returns>
        /// <response code="200">All of the <see cref="User" />s</response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        [Authorize(Roles = "admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<GmApiResults<User>> GetUsers()
            => _userRepository.GetAll().AsOkGmApiResults();

        /// <summary>
        ///     Returns all the <see cref="UserRating" />s for the user that is authenticated
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         GET /users/me/ratings
        ///
        /// </remarks>
        /// <returns>All of the <see cref="UserRating" />s for the current user</returns>
        /// <response code="200">All of the <see cref="UserRating" />s for the current user</response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        [HttpGet("me/ratings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<GmApiResults<UserRating>> GetMyRatings()
            => _userRatingRepository.Query(r => r.UserId == User.GetUserId()).AsOkGmApiResults();

        /// <summary>
        ///     Stores the rating info from the request as the current rating info for the authenticated user in the GameType specified
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST /users/me/ratings
        ///         {
        ///             "GameType": "Chess",
        ///             "Rating": 1.23
        ///         }
        ///
        /// </remarks>
        /// <param name="request">The <see cref="UserRatingRequest" /> definition</param>
        /// <response code="204">Successfully updated rating info</response>
        /// <response code="400">If the request is invalid (will include additional validation error information)</response>
        [HttpPut("me/ratings")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public NoContentResult PutMyRating([FromBody] UserRatingRequest request)
        {
            var userId = User.GetUserId();

            var existingRating = _userRatingRepository.GetByUserGameType(userId, request.GameType);

            var userRating = _userRatingTransformer.To(request, existingRating);

            userRating.UserId = userId;

            _userRatingRepository.AddOrUpdate(userRating);

            return NoContent();
        }
    }
}
