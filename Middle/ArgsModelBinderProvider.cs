using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;

namespace MmWizard.Middle
{
    public class ArgsModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder binder =
            new ArgsModelBinder(new SimpleTypeModelBinder(typeof(object),
                Startup.ServiceLocator.Instance.GetService(typeof(ILoggerFactory)) as ILoggerFactory));

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
           // return context.Metadata.ModelType == typeof(object) ? binder : null;
            return binder;
        }
    }
}
