using Exiled.API.Interfaces;
using System.ComponentModel;

namespace test
{
    public class Config : IConfig
    {
        [Description("这个控制插件启用")]
        public bool IsEnabled { get; set; } = true;

        [Description("这个控制是否输出调试文本")]
        public bool Debug { get; set; } = false;
    }
}
