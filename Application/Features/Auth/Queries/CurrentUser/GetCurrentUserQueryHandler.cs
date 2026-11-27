using Application.Contracts.Repositories;
using Application.Features.Auth.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Queries.CurrentUser
{
    internal sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, OneOf<UserDto, Error>>
    {
        private readonly ICurrentUser _currentUser;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPassengerRepository _passengerRepository;
        private readonly IMapper _mapper;

        public GetCurrentUserQueryHandler(ICurrentUser currentUser, UserManager<ApplicationUser> userManager,
            IPassengerRepository passengerRepository, IMapper mapper)
        {
            _currentUser = currentUser;
            _userManager = userManager;
            _passengerRepository = passengerRepository;
            _mapper = mapper;
        }
        public async Task<OneOf<UserDto, Error>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;
            if (userId is null)
                return UserErrors.Unauthorized;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return UserErrors.UserNotFound;

            Passenger? passenger = null;
            //Captain? captin = null;
            if (user.Role is Role.PASSENGER)
            {
                passenger = await _passengerRepository.GetPassengerByUserIdAsync(userId, cancellationToken);
                if (passenger is null)
                    return UserErrors.UserNotFound;

            }
            //else if (role is Role.CAPTAIN)
            //{
            //    var captain = await _CaptainRepository.GetCaptinByUserIdAsync(userId, cancellationToken);
            //    if (captain is null)
            //        return UserErrors.Unauthorized;
            //}

            var result = _mapper.Map<UserDto>(user);
            if (passenger is not null)
                _mapper.Map(passenger, result);
            //else if (capation is not null)
            //_mapper.Map(captain, result);

            return result;
        }
    }
}