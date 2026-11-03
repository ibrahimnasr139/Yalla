using System.Net;

namespace Domain.Errors
{
    public sealed record Error(string Code, string Message, HttpStatusCode StatusCode)
    {
        public static Error None => new(string.Empty, string.Empty, HttpStatusCode.OK);
    }
}