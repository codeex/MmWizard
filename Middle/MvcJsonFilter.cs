using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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

    public class MvcRemoveJsonFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is MyController)
            {
                var p = context.Result;
                if (p is ObjectResult objectResult)
                {
                    if (objectResult.DeclaredType.IsGenericType &&
                        objectResult.DeclaredType.GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        return;
                    }

                    objectResult.Value = 
                }
            }
            //throw new NotImplementedException();
        }
    }
}
