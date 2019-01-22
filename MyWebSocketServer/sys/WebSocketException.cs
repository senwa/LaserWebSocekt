using System;
using System.Net;

namespace MyWebSocketServer.Sys
{
    public class WebSocketException : Exception
    {
        public HttpStatusCode Code { get; set; }

        public WebSocketException(HttpStatusCode code, string message)
            : base(message)
        {
            this.Code = code;
        }

        public static void ThrowForbidden(string message) 
        {
            throw new WebSocketException(HttpStatusCode.Forbidden, message);
        }

        public static void ThrowUnauthorized(string message)
        {
            throw new WebSocketException(HttpStatusCode.Unauthorized, message);
        }

        public static void ThrowNotFound(string message)
        {
            throw new WebSocketException(HttpStatusCode.NotFound, message);
        }

        public static void ThrowServerError(string message)
        {
            throw new WebSocketException(HttpStatusCode.InternalServerError, message);
        }
    }
}
