using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MmWizard.Models
{
    /// <summary>
    /// 元信息
    /// </summary>
    public class MetaInfo : BaseModel
    {
        /// <summary>
        /// Key关联业务Id
        /// </summary>
        public long KeyReleatedId { get; set; }

        /// <summary>
        /// Key ，业务id下同
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Key值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Key类型
        /// </summary>
        public KeyType KeyType { get; set; }
    }

    public enum KeyType
    {
        String = 0,
        Int = 1,
        Double = 2,
        Date = 3,
        Time = 4,
        DateTime = 5,
        Json = 6,
        Xml = 7,
        Razor = 8
    }
}
