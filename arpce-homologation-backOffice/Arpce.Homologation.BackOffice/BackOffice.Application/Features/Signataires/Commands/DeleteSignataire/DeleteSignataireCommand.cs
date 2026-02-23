using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Features.Signataires.Commands.DeleteSignataire
{
    public class DeleteSignataireCommand : IRequest<bool> { public Guid Id { get; set; } }
}
