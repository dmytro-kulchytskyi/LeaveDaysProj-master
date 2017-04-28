using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Models
{
    public class Result<T>
        where T : class
    {
        private Result() { }

        public bool Succed { get; private set; }
        private string Message;
        private T ResultObject;

        public static Result<T> Error(string message = null)
        {
            return new Result<T>()
            {
                Succed = false,
                Message = message
            };

        }

        public static Result<T> Success(T result = null, string message = null)
        {
            return new Result<T>()
            {
                ResultObject = result,
                Succed = true,
                Message = message
            };

        }


        public string GetMessage()
        {
            return this.Message;
        }

        public T GetResult()
        {
            return this.ResultObject;
        }
    }
}
