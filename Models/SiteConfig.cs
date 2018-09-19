using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MmWizard.Models
{
    public static class SiteConfig
    {
        private static IConfigurationSection _appSection = null;
        private static Dictionary<string, string> _dictSql;

        /// <summary>
        /// API域名地址
        /// </summary>
        public static string GetConnString()
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

            try
            {
                var xmlPath = Path.Combine(Directory.GetCurrentDirectory(), "mysql.xml");
                var cfg = (new ConfigurationBuilder()).AddXmlFile(xmlPath).Build();
                _dictSql = cfg.Get<Dictionary<string, string>>();
            }
            catch(Exception ex)
            {
                throw new Exception("mysql.xml文件解析失败", ex);
            }

        }

        public static string GetSql(string key)
        {
            if (_dictSql.ContainsKey(key))
            {
                return _dictSql[key];
            }
            else
            {
                throw new Exception($"没有找到Key[{key}]指定的Sql");
            }
        }
        public static RedisServer GetRedisConfig()
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
