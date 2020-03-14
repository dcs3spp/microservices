using System;
using System.Collections.Generic;


namespace dcs3spp.courseManagementContainers.BuildingBlocks.Validation
{
    public class Notification
    {
        private class Error
        {
            public string Message { get; private set; }
            public Exception Exception { get; private set; }

            public Error(string message, Exception exception)
            {
                this.Message = message;
                this.Exception = exception;
            }
        }

        private List<Error> errors;

        public Notification()
        {
            errors = new List<Error>();
        }

        public void addError(string message, Exception exception)
        {
            errors.Add(new Error(message, exception));
        }

        public string errorMessage()
        {
            return String.Join("\n", errors);
        }

        public bool HasErrors()
        {
            return (errors.Count > 0);
        }
    }
}
