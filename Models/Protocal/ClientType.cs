
using System;
using System.Collections.Generic;
using System.Text;

namespace MmWizard.Protocol
{
    /// <summary>
    /// ClientType
    /// </summary>
    public enum ClientType
    {
        /// <summary>
        /// web前端，需要检查接入ip或者域名
        /// </summary>
        QtWeb,

        /// <summary>
        /// 移动app
        /// </summary>
        QtApp,

        /// <summary>
        /// 微信
        /// </summary>
        WeiXin,

        /// <summary>
        /// 第三方
        /// </summary>
        ThirdPart,

        /// <summary>
        /// 内部微服务，前端接入层会拦截这个类型
        /// </summary>
        InnerRpc,

        /// <summary>
        /// 未知类型
        /// </summary>
        Unkunwn,
    }
}