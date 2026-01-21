using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontOffice.Application.Features.Paiements.Commands.HandleMomoWebhook
{
    public class HandleMomoWebhookCommand : IRequest
    {
        public MomoWebhookPayload Payload { get; set; }

        public HandleMomoWebhookCommand(MomoWebhookPayload payload) => Payload = payload;
    }
}
