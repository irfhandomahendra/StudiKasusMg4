using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.GraphQL
{
    public record CommentInput
    (
        int TwittorId,
        string Message
    );
}