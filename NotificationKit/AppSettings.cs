using System.Configuration;

namespace NotificationKit
{
    [System.Diagnostics.DebuggerNonUserCodeAttribute]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static class AppSettings
    {
        public static string ClientValidationEnabled
        {
            get { return ConfigurationManager.AppSettings["ClientValidationEnabled"]; }
        }

        public static string NotificationHubConnectionString
        {
            get { return ConfigurationManager.AppSettings["NotificationHubConnectionString"]; }
        }

        public static string NotificationHubPath
        {
            get { return ConfigurationManager.AppSettings["NotificationHubPath"]; }
        }

        public static string UnobtrusiveJavaScriptEnabled
        {
            get { return ConfigurationManager.AppSettings["UnobtrusiveJavaScriptEnabled"]; }
        }

        public static string UseStandardTier
        {
            get { return ConfigurationManager.AppSettings["UseStandardTier"]; }
        }

        public static class Webpages
        {
            public static string Enabled
            {
                get { return ConfigurationManager.AppSettings["webpages:Enabled"]; }
            }

            public static string Version
            {
                get { return ConfigurationManager.AppSettings["webpages:Version"]; }
            }
        }
    }
}

