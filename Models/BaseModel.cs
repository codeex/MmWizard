using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MmWizard.Protocol;
using Newtonsoft.Json;

namespace MmWizard.Models
{
    public abstract class BModel
    {
        [JsonIgnore]
        public Args<object> _Args { get; set; }
        [JsonIgnore]
        public Result<object> _Result { get; set; }

    }

    /// <summary>
    /// 基础类
    /// </summary>
    public abstract class BaseModel : BModel
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
