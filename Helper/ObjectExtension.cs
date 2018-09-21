using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MmWizard.Protocol;

namespace MmWizard.Helper
{
    public static class ObjectExtension
    {
       
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
    }
}
