using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MmWizard.Db
{
    public class PageResult<T>
    {
        public PageInfo Page { set; get; }
        public List<T> ListValue { set; get; }
    }
}
