using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SubscriptionService.Web.Services;
using SubscriptionService.Web.Models.DTO.Commands;
using MediatR;
using SubscriptionService.Web.Models.DTO.Query;

namespace SubscriptionService.Web.Controllers
{
    [Route("api/user/{userId:guid}/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private IUserAccountService _userAccountService;
        private readonly IMediator _mediator;

        public UserAccountController(IUserAccountService userAccountService, IMediator mediator)
        {
            _userAccountService = userAccountService;
            _mediator = mediator;
        }

        /// <summary>
        /// Returns accounts associated with a User for a given user identifier
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET api/user/991e8431-9fdd-4cdf-b1f2-03cad2202f48/account
        /// </remarks>
        /// <param name="userId">User Id</param>
        /// <returns>List of accounts for a given User Identifier</returns>
        /// <response code="200">Returns Accounts</response>
        /// <response code="404">Returns when there are no accounts for the given User Identifier</response>
        [ProducesResponseType(typeof(List<GetUserAccountResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet()]
        public async Task<IActionResult> GetAccountsForUser([FromRoute] Guid userId)
        {
            var accounts = await _mediator.Send(new GetUserAccountRequest { UserId = userId });

            if (accounts == null)
                return NotFound();

            return Ok(accounts);
        }

        /// <summary>
        /// Creates an association between an account and a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST api/user/991e8431-9fdd-4cdf-b1f2-03cad2202f48/account
        /// </remarks>        
        /// <param name="userId">User Id that the account will be associated with</param>
        /// <param name="userAccountRequest">Dto to be used when creating the association</param>
        /// <response code="204">Returns when a user/account association is created successfully</response>
        /// <response code="400">Returns when a validation fails</response>
        /// <returns>Status NoContent(Http:204) when a user/account association is created successfully</returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpPost()]
        public async Task<IActionResult> CreateAccountForUser([FromRoute] Guid userId, [FromBody] CreateUserAccountRequest userAccountRequest)
        {
            if (userId != userAccountRequest.UserId)
                return BadRequest("User Id different to the one provided in the path");

            await _mediator.Send(userAccountRequest);
            return NoContent();
        }
    }
}
