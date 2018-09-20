using System;

namespace MmWizard.Protocol
{
    /// <summary>
    /// 统一参数的基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
   public class BasicArgs<T>
    {
        private string checkSum;
        /// <summary>
        /// 校验值
        /// </summary>
        public string cs
        {
            get { return checkSum; }
            set { checkSum = value; }
        }

        private T _value;

        /// <summary>
        /// 业务数据
        /// </summary>
        public T v
        {
            get { return _value; }
            set { _value = value; }
        }

        private bool isCiphertext;
        /// <summary>
        /// 当前传输的是否密文,这里说的加密指的是对有效业务数据的加密指的是_Value;
        /// </summary>
        public bool icp
        {
            get { return isCiphertext; }
            set { isCiphertext = value; }
        }

        private string routerUri;
        /// <summary>
        /// 可以路由的Uri
        /// </summary>
        public string uri
        {
            get { return routerUri; }
            set { routerUri = value; }
        }

        private string language;

        /// <summary>
        /// 语言
        /// </summary>
        public string lg
        {
            get { return language; }
            set { language = value; }
        }

    }
}
