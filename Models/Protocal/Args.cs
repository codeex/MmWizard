/*----------------------------------------------------------------

 功能描述：定义一个客户端请求的数据结构，便于标准化
 
 {
    "m": "user/login",
    "uri":"ttid@quantum.com;u=321233;p=1000",
    "ct": "pc",
    "cv": "2.0.1",
    "rid": "0ad0ffcd5e6546eab1612885709adc49",
    "lg":"en",
    "tk":"234werwer23423423423rwerwerwer",
    "v": {
        "user_id": 11,
        "user_name": 1,
        "pass_word": 123456
    },
    "icp": false,
    "cs": "xx234rwerwerwerwer"
}

----------------------------------------------------------------*/

using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MmWizard.Protocol
{
    /// <summary>
    /// 客户端请求的数据结构
    /// </summary>
    /// <typeparam name="A"> A </typeparam>
    public class Args<A>:BasicArgs<A>
    {
        private string methodName;

        /// <summary>
        /// 请求的业务名称
        /// </summary>
        public string m
        {
            get { return methodName; }
            set { methodName = value; }
        }

        private string methodVersion;

        /// <summary>
        /// m的版本号
        /// </summary>
        public string mv
        {
            get { return methodVersion; }
            set { methodVersion = value; }
        }

        private string clientType;

        /// <summary>
        /// 客户端类型
        /// </summary>
        public string ct
        {
            get { return clientType; }
            set { clientType = value; }
        }

        private string clientVersion;

        /// <summary>
        /// 客户版本号
        /// </summary>
        public string cv
        {
            get { return clientVersion; }
            set { clientVersion = value; }
        }

        private string requestId;
        /// <summary>
        /// 请求的id，一般用Guid，便于日志对应请求和响应，要求返还的ResponseId原样复制
        /// </summary>
        public string rid
        {
            get { return requestId; }
            set { requestId = value; }
        }

        private string _token;

        /// <summary>
        /// 授权令牌
        /// </summary>
        public string tk
        {
            get { return _token; }
            set { _token = value; }
        }

        private Dictionary<string, HeaderValue> headers;

        public Dictionary<string, HeaderValue> Headers
        {
            get { return headers; }
            set { headers = value; }
        }
    }
}
