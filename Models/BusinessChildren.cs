using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MmWizard.Models
{
    public class BusinessChildren : BaseSortModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 业务父id
        /// </summary>

        public long ParentId { get; set; }
    }
}
