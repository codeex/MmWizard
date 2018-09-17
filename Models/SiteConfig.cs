using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MmWizard.Models
{
    public static class SiteConfig
    {
        private static IConfigurationSection _appSection = null;

        /// <summary>
        /// API域名地址
        /// </summary>
        public static string ConnString()
        {
            var connKey = "ConnString";
            string str = string.Empty;
            if (_appSection.GetSection(connKey) != null)
            {
                str = _appSection.GetSection(connKey).Value;
            }
            return str;
        }

        public static void SetAppSetting(IConfigurationSection section)
        {
            _appSection = section;
            if (section == null)
            {
                throw new Exception("未配置站点节点Config");
            }
        }

        public static RedisServer RedisConfig()
        {
            var connKey = "Redis";
            string str = string.Empty;
            if (_appSection.GetSection(connKey) != null)
            {
                return _appSection.GetSection(connKey).Get<RedisServer>();
            }
            return null;
        }
    }


}
