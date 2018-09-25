using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MmWizard.Helper;
using MmWizard.Protocol;
using Newtonsoft.Json;

namespace MmWizard.Middle
{
    public class MvcParseJsonFilter : IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            // 如果是api控制器，则处理
            if (context.ActionDescriptor.FilterDescriptors.FirstOrDefault(x => x.Filter is ApiControllerAttribute) !=
                null)
            {
                var p = context.ActionDescriptor.Parameters[0] as ControllerParameterDescriptor;
                ControllerParameterDescriptor cpd = new ControllerParameterDescriptor()
                {
                    Name = p.Name,
                    BindingInfo = p.BindingInfo,
                    ParameterType = typeof(Args<>).MakeGenericType(p.ParameterType),
                    ParameterInfo = p.ParameterInfo,

                };
                p.ParameterType = typeof(Args<>).MakeGenericType(p.ParameterType);
                MyParameterInfo mp = new MyParameterInfo(p.ParameterInfo);
                mp.ChangeType = p.ParameterType;
                p.ParameterInfo = mp;

            }

            

        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }

    public class MvcAddResultJsonFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

        private static string GetExceptionString(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            while (ex != null)
            {
                sb.AppendFormat(
                    "\r\n[ExceptionMessage]:{0}\r\n[ExceptionStackTrace]:{1}\r\n-------------------------------------------------------------------------------\r\n",
                    ex.Message, ex.StackTrace);
                ex = ex.InnerException;
            }

            return sb.ToString();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is MyController)
            {
                if (context.Exception != null)
                {
                    context.ExceptionHandled = true;
                    var result = new Result<object>
                    {
                        v = null,
                        c = 500,
                        msg = GetExceptionString(context.Exception),
                    };
                    context.Result = new ObjectResult(result);
                    return;
                }

                var p = context.Result;
                if (p is ObjectResult objectResult)
                {
                    if (objectResult.DeclaredType.IsGenericType &&
                        objectResult.DeclaredType.GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        return;
                    }

                    objectResult.Value = new Result<object>
                    {
                        v = objectResult.Value,
                        c = 300,
                    };
                }
            }
            //throw new NotImplementedException();
        }
    }
}
