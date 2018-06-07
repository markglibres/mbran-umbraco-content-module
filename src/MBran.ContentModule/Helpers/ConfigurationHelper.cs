using System;
using System.Configuration;
using Umbraco.Core;

namespace MBran.ContentModule.Helpers
{
    public class ConfigurationHelper
    {
        private const string _defaultRootPath = "~/Views/Partials/Content";
        private const string _rootPathConfigKey = "MBran.ContentModule.Root";

        private static readonly Lazy<ConfigurationHelper> _instance =
            new Lazy<ConfigurationHelper>(() => new ConfigurationHelper());

        private ConfigurationHelper()
        {
        }

        public static ConfigurationHelper Instance => _instance.Value;

        public string GetRootPath()
        {
#if DEBUG
            return GetRoothPathFromConfig();
#else
            var cacheName = string.Join("_", GetType().FullName, nameof(GetRootPath));

            return (string)ApplicationContext.Current.ApplicationCache
                .RuntimeCache
                .GetCacheItem(cacheName, GetRoothPathFromConfig);
#endif

        }

        private string GetRoothPathFromConfig()
        {
            var configValue = ConfigurationManager.AppSettings[_rootPathConfigKey];
            return string.IsNullOrWhiteSpace(configValue) ? _defaultRootPath : configValue.TrimEnd('/');
        }
    }
}