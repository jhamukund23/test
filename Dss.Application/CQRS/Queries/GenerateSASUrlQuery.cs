using Dss.Domain.MRM;
using MediatR;

namespace Dss.Application.Queries
{
    public record GenerateSASUrlQuery():IRequest<SASUrlResponse>;
 

}
