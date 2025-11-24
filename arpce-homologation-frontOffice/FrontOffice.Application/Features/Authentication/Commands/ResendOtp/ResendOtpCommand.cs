using MediatR;

namespace FrontOffice.Application.Features.Authentication.Commands.ResendOtp;

public class ResendOtpCommand : IRequest<bool> { }