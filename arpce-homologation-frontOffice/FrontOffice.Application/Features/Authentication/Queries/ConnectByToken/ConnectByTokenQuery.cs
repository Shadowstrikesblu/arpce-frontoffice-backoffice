using MediatR;

namespace FrontOffice.Application.Features.Authentication.Queries.ConnectByToken;

/// <summary>
/// Requête pour valider une session existante à l'aide d'un token JWT.
/// </summary>
public class ConnectByTokenQuery : IRequest<ConnectByTokenResult> 
{
    
}