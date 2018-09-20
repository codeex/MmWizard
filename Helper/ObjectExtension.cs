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
        public static Result<T> ToResult<T>(Func<T> func)
        {
            var ret = new Result<T>()
            {
                rid = Guid.NewGuid().ToString("N")
            };
            try
            {
                var t = func();
                ret.v = t;
                ret.c = StatusCode.OK.code;
                ret.msg = StatusCode.OK.msg;
            }
            catch (Exception ex)
            {
                ret.v = default(T);
                ret.rid = Guid.NewGuid().ToString("N");
                ret.c = StatusCode.ServerError.code;
                ret.msg = $"{StatusCode.ServerError.msg}:{GetExceptionString(ex)}";
            }
            return ret;
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
    }
}
