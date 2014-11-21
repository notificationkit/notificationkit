using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.ServiceBus.Notifications;

namespace NotificationKit.Models
{
    public static class NotificationExtensions
    {
        public static async Task<IEnumerable<RegistrationDescription>> GetRegistrationsByTagExpressionAsync(this NotificationHubClient client, string tagExpression)
        {
            // タグ式を分解して、元々のオブジェクトに戻す
            var andExpressions = tagExpression.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();

            var tags = (from expression in andExpressions
                        let body = expression.Trim('!', '(', ')')
                        let orExpressions = body.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray()
                        let key = orExpressions[0].Split(':')[0]
                        select new TagModel
                        {
                            Key = key,
                            Values = orExpressions.Select(p => p.Substring(key.Length + 1)).ToArray(),
                            Exclude = expression.StartsWith("!")
                        }).ToArray();

            IEnumerable<RegistrationDescription> results;

            // 否定が入っている場合には全件取得する必要がある
            if (tags.Any(p => p.Exclude))
            {
                results = await client.GetAllRegistrationsAsync(0);
            }
            else
            {
                // フィルタリングに最低限必要なタグのみ取得する
                results = (await Task.WhenAll(tags.SelectMany(p => p.Values.Select(q => client.GetRegistrationsByTagAsync(p.Key + ":" + q, 0))))).SelectMany(p => p).Distinct(new RegistrationDescriptionComparer());
            }

            // フィルタリングした結果を返す
            return results.Where(p => tags.All(q =>
            {
                if (q.Exclude)
                {
                    return !q.Values.Any(r => p.Tags.Contains(q.Key + ":" + r));
                }
                
                return q.Values.Any(r => p.Tags.Contains(q.Key + ":" + r));
            }));
        }

        private class TagModel
        {
            public string Key { get; set; }

            public string[] Values { get; set; }

            public bool Exclude { get; set; }
        }

        private class RegistrationDescriptionComparer : EqualityComparer<RegistrationDescription>
        {
            public override bool Equals(RegistrationDescription val1, RegistrationDescription val2)
            {
                if (object.Equals(val1, val2))
                {
                    return true;
                }

                if (val1 == null || val2 == null)
                {
                    return false;
                }

                return (val1.RegistrationId == val2.RegistrationId);
            }

            public override int GetHashCode(RegistrationDescription value)
            {
                return value.RegistrationId.GetHashCode();
            }
        }

        public static string GetPlatform(this Notification notification)
        {
            if (notification is WindowsNotification)
            {
                return Platforms.Windows;
            }

            if (notification is MpnsNotification)
            {
                return Platforms.WindowsPhone;
            }

            if (notification is AppleNotification)
            {
                return Platforms.Apple;
            }

            if (notification is GcmNotification)
            {
                return Platforms.Android;
            }

            return null;
        }
    }
}