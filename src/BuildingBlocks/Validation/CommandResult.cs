using Grpc.Core;
using System.Net;

namespace dcs3spp.courseManagementContainers.BuildingBlocks.Validation
{
    /// <summary>
    /// Generic class providing the ability to store success status and result/error for a CQRS command
    /// </summary>
    /// <typeparam name="T">The type for the command result</typeparam>
    public class CommandResult<T>
    {
        public CommandError Error {get; private set; }
        public T Result { get; private set; }
        public bool Success { get; private set; }
        
        /// <summary>
        /// Initial state is Success is false and Result set to default value.
        /// Errors property is empty string
        /// </summary>
        public CommandResult() {
            Success = false;
            Result = default(T);
            Error = new CommandError("", 0);
        }

        /// <summary>
        /// Set error
        /// </summary>
        /// <param name="error">Error message</param>
        public void SetError(string message, short httpStatusCode)
        {
            Error = new CommandError(message, httpStatusCode);
            Success = false;
        }

        /// <summary>
        /// Set result
        /// </summary>
        /// <param name="result">Command result</param>
        public void SetResult(T result)
        {
            Error = new CommandError("", 0);
            Success = true;
            Result = result;
        }

    }

    /// <summary>
    /// Represents an error received from a command containing message and http status code
    /// </summary>
    public class CommandError
    {
        public string Message { get; private set; }
        public int HTTPStatusCode { get; private set; }
        public CommandError(string message, int httpStatusCode)
        {
            Message = message;
            HTTPStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Utility method to map http error code to grpc status code
        /// </summary>
        /// <returns>
        /// Grpc status code equivalent of error code.null Returns <see cref="StatusCode.Unknown"/>
        /// if the http error is unrecognised
        /// </returns>
        public StatusCode MapToGrpStatusCode()
        {
            switch(HTTPStatusCode)
            {
                case (int) HttpStatusCode.Conflict:
                    return StatusCode.AlreadyExists;
                case (int) HttpStatusCode.NotFound:
                    return StatusCode.NotFound;
                case (int) HttpStatusCode.InternalServerError:
                    return StatusCode.Internal;
                case (int) HttpStatusCode.Forbidden:
                    return StatusCode.PermissionDenied;
                case (int) HttpStatusCode.Unauthorized:
                    return StatusCode.Unauthenticated;
                default:
                    return StatusCode.Unknown;
            }
        }
    }
}
