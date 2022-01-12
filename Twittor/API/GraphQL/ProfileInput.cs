using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.GraphQL
{
    public record ProfileInput
    (
        int? Id,
        string FullName,
        string Email,
        string Username,
        string Password
    );
}