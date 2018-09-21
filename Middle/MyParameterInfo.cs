using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MmWizard.Middle
{
    public class MyParameterInfo : ParameterInfo
    {
        ParameterInfo _info;
        public MyParameterInfo(ParameterInfo info)
        {
            _info = info;
        }

        public Type ChangeType { get; set; }
        public override Type ParameterType => ChangeType != null ? ChangeType : _info.ParameterType;
    }
}
