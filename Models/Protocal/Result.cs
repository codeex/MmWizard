


namespace MmWizard.Protocol
{
    /// <summary>
    /// 请求返回结果
    /// </summary>
    /// <typeparam name="R"></typeparam>
    public class Result<R>:BasicArgs<R>
    {
        private string responseId;
        /// <summary>
        /// 响应的Id，跟RequestId对应
        /// </summary>
        public string rid
        {
            get { return responseId; }
            set { responseId = value; }
        }

        private int code;
        /// <summary>
        /// 业务处理状态
        /// </summary>
        public int c
        {
            get { return code; }
            set { code = value; }
        }

        private string codeMsg;
        /// <summary>
        /// 状态码描述
        /// </summary>
        public string msg
        {
            get { return codeMsg; }
            set { codeMsg = value; }
        }

        private string[] _msgParam;

        public string[] MsgParam
        {
            get { return _msgParam; }
            set { _msgParam = value; }
        }
    }
}
