using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MmWizard.Helper
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
