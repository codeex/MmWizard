using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MmWizard.Models
{
    /// <summary>
    /// 基础类
    /// </summary>
    public abstract class BaseModel
    {
        /// <summary>
        /// 标识
        /// </summary>
        public long Id { get; set; }

    }


    /// <summary>
    /// 基础排序类
    /// </summary>
    public abstract class BaseSortModel : BaseModel
    {
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

    }
}
