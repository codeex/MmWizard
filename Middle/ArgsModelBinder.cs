using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MmWizard.Middle
{
    public class ArgsModelBinder : IModelBinder
    {
        private readonly IModelBinder fallbackBinder;

        public ArgsModelBinder(IModelBinder binder)
        {
            this.fallbackBinder = binder;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var allValues = bindingContext.ValueProvider.GetValue("");

            if (allValues == null)
            {
                return fallbackBinder.BindModelAsync(bindingContext);
            }
            // Combine into single DateTime which is the end result
            var result = bindingContext.ModelType;
            //bindingContext.ModelState.SetModelValue(bindingContext.ModelName, result, $"{datePartValues.FirstValue} {timePartValues.FirstValue}");
            bindingContext.Result = ModelBindingResult.Success(result);
            return Task.CompletedTask;
        }
    }
}
