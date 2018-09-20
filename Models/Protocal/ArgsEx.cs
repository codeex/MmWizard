using System;
using System.Collections.Generic;
using System.Text;

namespace MmWizard.Protocol
{
    /// <summary>
    /// ArgsEx
    /// </summary>
   public static class ArgsEx
    {
        /// <summary>
        /// 拷贝一个当前上下文的args，作为下一个内部调用的args使用，
        /// 这里除了m和v没有拷贝，需要调用时赋值，其他都会原样拷贝结构和值
        /// </summary>
        /// <param name="args">当前上下文的args</param>
        /// <returns>返回除了m和v之外全新的Args</returns>
        public static Args<object> Copy(this Args<object> args)
        {
            return new Args<object>()
            {
                cs = args.cs,
                ct = args.ct,
                cv = args.cv,
                Headers = args.Headers,
                icp = args.icp,
                lg = args.lg,
                rid = args.rid,
                mv = args.mv,
                tk = args.tk,
                uri = args.uri,
            };
        }
    }
}
