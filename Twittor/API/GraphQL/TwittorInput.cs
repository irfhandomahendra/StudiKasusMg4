using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.GraphQL
{
    public record TwittorInput
    (
        int UserId,
        string Message
    );
}