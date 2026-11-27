using Application.Common;
using Application.Constants;
using Application.Features.Auth.Commands.CompleteProfile;
using Application.Features.Auth.Commands.LoginRequestOtp;
using Application.Features.Auth.Commands.LoginVerifyOtp;
using Application.Features.Auth.Commands.Logout;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.SignUpRequestOtp;
using Application.Features.Auth.Commands.SignUpVerifyOtp;
using Application.Features.Auth.Commands.UpdateCurrentUser;
using Application.Features.Auth.Queries.CurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [EnableRateLimiting("OtpByIp")]
        [HttpPost("signup/request-otp")]
        public async Task<IActionResult> SignUpRequestOtp([FromBody] SignUpRequestOtpCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                 success => Created(string.Empty, success),
                 error => StatusCode((int)error.StatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [EnableRateLimiting("OtpByIp")]
        [HttpPost("login/request-otp")]
        public async Task<IActionResult> LoginRequestOtp([FromBody] LoginRequestOtpCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                 success => Created(string.Empty, success),
                 error => StatusCode((int)error.StatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("signup/verify-otp")]
        public async Task<IActionResult> SignUpVerifyOtp([FromBody] SignUpVerifyOtpCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                 success => Ok(success),
                 error => StatusCode((int)error.StatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("login/verify-otp")]
        public async Task<IActionResult> LoginVerifyOtp([FromBody] LoginVerifyOtpCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                 success => Ok(success),
                 error => StatusCode((int)error.StatusCode, new ErrorDto { Error = error.Message })
            );
        }



        [Authorize(Roles = $"{RoleConstants.ADMIN},{RoleConstants.PASSENGER},{RoleConstants.CAPTAIN}")]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
            return result.Match<IActionResult>(
                 success => Ok(success),
                 error => StatusCode((int)error.StatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [Authorize(Roles = $"{RoleConstants.ADMIN},{RoleConstants.PASSENGER},{RoleConstants.CAPTAIN}")]
        [HttpPatch("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateCurrentUserCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                 success => Ok(success),
                 error => StatusCode((int)error.StatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [Authorize(Roles = $"{RoleConstants.ADMIN},{RoleConstants.PASSENGER},{RoleConstants.CAPTAIN}")]
        [HttpPost("complete-profile")]
        public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                 success => Ok(success),
                 error => StatusCode((int)error.StatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                 success => Ok(success),
                 error => StatusCode((int)error.StatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [Authorize(Roles = $"{RoleConstants.ADMIN},{RoleConstants.PASSENGER},{RoleConstants.CAPTAIN}")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                 success => NoContent(),
                 error => StatusCode((int)error.StatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}