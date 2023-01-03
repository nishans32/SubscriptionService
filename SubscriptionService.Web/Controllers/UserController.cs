using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Web.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SubscriptionService.Web.Handlers;
using SubscriptionService.Web.Models.DTO;
using SubscriptionService.Web.Services;
using SubscriptionService.Web.Models.DTO.Commands;
using SubscriptionService.Web.Models.DTO.Query;

namespace SubscriptionService.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMediator _mediator;

        public UserController(IUserService userService, IMediator mediator)
        {
            _userService = userService;
            _mediator = mediator;
        }

        /// <summary>
        /// Returns a User for a given User Identifier
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET api/user/991e8431-9fdd-4cdf-b1f2-03cad2202f48
        /// </remarks>
        /// <param name="userid">User Id</param>
        /// <returns>User for a given User Identifier</returns>
        /// <response code="200">Returns User</response>
        /// <response code="404">Unable to find a User for the given Identifier</response>
        [ProducesResponseType(typeof(GetUserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("{userid:guid}")]
        public async Task<IActionResult> GetUser(Guid userid)
        {
            var user = await _mediator.Send(new GetUserRequest { UserId = userid });
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Lists users
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET api/user/list
        /// </remarks>
        /// <returns>A list of Users</returns>
        /// <response code="200">Returns a list of Users</response>
        /// <response code="404">Unable to find  Users</response>
        [ProducesResponseType(typeof(IEnumerable<GetUserResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("list")]
        public async Task<IActionResult> ListUsers()
        {
            var users = await _userService.ListUsers();

            if (!users.Any())
                return NotFound();

            return Ok(users);
        }

        /// <summary>
        /// Creates a User
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST api/user
        /// </remarks>        
        /// <param name="userDto">A Dto to be used when creating the user</param>
        /// <response code="204">Returns when a user is created successfully</response>
        /// <response code="400">Returns when a validation fails</response>
        /// <returns>Status NoContent(Http:204) when a user is created successfully</returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPost()]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest userDto)
        {
            await _mediator.Send(userDto);

            return NoContent();
        }


    }
}
