using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.ServiceBus.Notifications;

using NotificationKit.Models;

namespace NotificationKit.Controllers
{
    /// <summary>
    /// Send タブで必要な REST API を実装するコントローラーです
    /// </summary>
    public class SendController : ApiController
    {
        private readonly ApplicationDbContext _context = ApplicationDbContext.Create();
        private readonly NotificationHubClient _client = NotificationHubClient.CreateClientFromConnectionString(AppSettings.NotificationHubConnectionString, AppSettings.NotificationHubPath);

        /// <summary>
        /// 送信した通知の履歴一覧を返します
        /// </summary>
        public IEnumerable<SendLog> Get()
        {
            // 登録日時の降順にログを取得する
            var result = _context.SendLogs.OrderByDescending(p => p.CreatedOn).ToList();

            return result;
        }

        /// <summary>
        /// 新しく通知を送信します
        /// </summary>
        public async Task<IHttpActionResult> Post(SendFormModel data)
        {
            // 送信されたデータが正しいか検証する
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var notifications = new List<Notification>();

            // 送信するペイロードを作成
            if (data.Windows > 0)
            {
                notifications.Add(Payloads.Windows.Create(data.Windows, data.Message));
            }

            if (data.WindowsPhone > 0)
            {
                notifications.Add(Payloads.WindowsPhone.Create(data.WindowsPhone, data.Message));
            }

            if (data.Apple > 0)
            {
                notifications.Add(Payloads.Apple.Create(data.Apple, data.Message));
            }

            if (data.Android > 0)
            {
                notifications.Add(Payloads.Android.Create(data.Android, data.Message));
            }

            // 送信先が 1 つもない場合にはエラー
            if (notifications.Count == 0)
            {
                return BadRequest("Required Platform select.");
            }

            // 通知ハブへ通知リクエストを非同期で送信
            var tasks = notifications.Select(p => _client.SendNotificationAsync(p, data.TagExpression));

            // 全ての通知リクエストの完了を待機する
            var results = await Task.WhenAll(tasks);

            // 送信結果を DB に保存
            var sendLog = new SendLog
            {
                Message = data.Message,
                TagExpression = data.TagExpression,
                Success = results.Sum(p => p.Success),
                Failure = results.Sum(p => p.Failure),
                CreatedOn = DateTimeOffset.UtcNow
            };

            _context.SendLogs.Add(sendLog);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
