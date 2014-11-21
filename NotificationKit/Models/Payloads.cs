using Microsoft.ServiceBus.Notifications;

namespace NotificationKit.Models
{
    /// <summary>
    /// 通知ハブに送信する各プラットフォーム用のペイロードを定義します
    /// </summary>
    public class Payloads
    {
        public class Windows
        {
            public const int Badge = 1;
            public const int Tile = 2;
            public const int Toast = 3;

            public const string BadgeTemplate = @"<badge value=""attention""/>";
            public const string TileTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?><tile><visual><binding template=""TileSquareText04""><text id=""1"">{0}</text></binding></visual></tile>";
            public const string ToastTemplate = @"<toast><visual><binding template=""ToastText01""><text id=""1"">{0}</text></binding></visual></toast>";

            public static WindowsNotification Create(int type, params object[] args)
            {
                switch (type)
                {
                    case Badge:
                        return new WindowsNotification(BadgeTemplate);
                    case Tile:
                        return new WindowsNotification(string.Format(TileTemplate, args));
                    case Toast:
                        return new WindowsNotification(string.Format(ToastTemplate, args));
                }

                return null;
            }
        }

        public class WindowsPhone
        {
            public const int Tile = 1;
            public const int Toast = 2;

            public const string TileTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?><wp:Notification xmlns:wp=""WPNotification""><wp:Tile><wp:BackgroundImage>{{enter background image}}</wp:BackgroundImage><wp:Count>1</wp:Count><wp:Title>Notification Hub</wp:Title><wp:BackBackgroundImage>{{enter back background image}}</wp:BackBackgroundImage><wp:BackTitle></wp:BackTitle><wp:BackContent>{0}</wp:BackContent></wp:Tile></wp:Notification>";
            public const string ToastTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?><wp:Notification xmlns:wp=""WPNotification""><wp:Toast><wp:Text1>NotificationHub</wp:Text1><wp:Text2>{0}</wp:Text2></wp:Toast></wp:Notification>";

            public static MpnsNotification Create(int type, params object[] args)
            {
                switch (type)
                {
                    case Tile:
                        return new MpnsNotification(string.Format(TileTemplate, args));
                    case Toast:
                        return new MpnsNotification(string.Format(ToastTemplate, args));
                }

                return null;
            }
        }

        public class Apple
        {
            public const int Alert = 1;

            public const string AlertTemplate = @"{{""aps"":{{""alert"":""{0}""}}}}";

            public static AppleNotification Create(int type, params object[] args)
            {
                switch (type)
                {
                    case Alert:
                        return new AppleNotification(string.Format(AlertTemplate, args));
                }

                return null;
            }
        }

        public class Android
        {
            public const int Message = 1;

            public const string MessageTemplate = @"{{""data"":{{""msg"":""{0}""}}}}";

            public static GcmNotification Create(int type, params object[] args)
            {
                switch (type)
                {
                    case Message:
                        return new GcmNotification(string.Format(MessageTemplate, args));
                }

                return null;
            }
        }
    }
}