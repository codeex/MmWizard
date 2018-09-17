using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MmWizard.Models
{
    /// <summary>
    /// 定义一个redis服务器对象
    /// </summary>
    public class RedisServer
    {
        /// <summary>
        /// 服务器名称或者ip
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 是否可以执行带有风险性指令
        /// </summary>
        public bool AllowAdmin { get; set; }

        /// <summary>
        /// 链接超时时间，一般用默认（配置为0表示默认）
        /// </summary>
        public int ConnectTimeout { get; set; }

        /// <summary>
        /// 是否需要密码
        /// </summary>
        public bool Ssl { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }
    }
}
