using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MmWizard.Db
{
    public class QueryModel
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public QueryModel()
        {
            Items = new List<ConditionItem>();
        }

        /// <summary>
        ///     查询条件
        /// </summary>
        public List<ConditionItem> Items { get; set; }
    }
}
