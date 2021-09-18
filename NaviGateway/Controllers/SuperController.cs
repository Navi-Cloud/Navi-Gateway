using System.Collections.Generic;
using Io.Github.NaviCloud.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NaviGateway.Controllers
{
    public class SuperController: ControllerBase
    {
        protected delegate IActionResult LazyExecution();
        
        protected IActionResult HandleCase(Dictionary<ResultType, LazyExecution> handledCase, Result result)
        {
            if (!handledCase.ContainsKey(result.ResultType))
                return new ObjectResult(new {errorMessage = "Unknown Error occurred!"})
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };

            return handledCase[result.ResultType].Invoke();
        }
    }
}