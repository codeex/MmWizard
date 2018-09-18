using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MmWizard.Models
{
    public class Artical : BaseSortModel
    {
        /// <summary>
        /// 模板名
        /// </summary>
        public string TemplateName { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWords { get; set; }

        /// <summary>
        /// 内容 
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 最后更新时间，排序用
        /// </summary>
        public DateTime UpdatedDate { get; set; }

    }
}
