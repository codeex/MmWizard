using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MmWizard.Models
{
    /// <summary>
    /// 页面模板
    /// </summary>
    public class PageTemplate : BaseModel
    {
        /// <summary>
        /// 模板内容 razor
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 类型 0-page
        /// </summary>
        public int Class { get; set; }

        /// <summary>
        /// 最后更新时间，排序用
        /// </summary>
        public DateTime UpdatedDate { get; set; }
    }
}
