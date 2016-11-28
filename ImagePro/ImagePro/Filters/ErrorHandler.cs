using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using NLog;

namespace ImagePro.Filters
{
    public class ErrorHandler : FilterAttribute, IExceptionFilter
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public void OnException(ExceptionContext excContext)
        {
            _logger.Warn(string.Format("Invalid operation:{0}|Error message:{1}",
                                        excContext.ActionDescriptor, excContext.Exception.Message));
           // _logger.Error(string.Format("Operation was not perfomed. Reason: User with ID '{0}' is not exists.", userID));



           // throw new HttpResponseException(HttpStatusCode.NotFound); //почему ничего не выскакивает?
        }
    }
}