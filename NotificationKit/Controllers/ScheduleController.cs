using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.ServiceBus.Notifications;

using NotificationKit.Models;

namespace NotificationKit.Controllers
{
    /// <summary>
    /// Schedule タブで必要な REST API を実装するコントローラーです
    /// </summary>
    public class ScheduleController : ApiController
    {
        private readonly ApplicationDbContext _context = ApplicationDbContext.Create();
        private readonly NotificationHubClient _client = NotificationHubClient.CreateClientFromConnectionString(AppSettings.NotificationHubConnectionString, AppSettings.NotificationHubPath);

        /// <summary>
        /// スケジュール履歴の一覧を返します
        /// </summary>
        public IEnumerable<ScheduleLog> Get()
        {
            // 履歴を登録日時の降順で取得する
            var result = _context.ScheduleLogs.OrderByDescending(p => p.CreatedOn).ToList();

            return result;
        }

        /// <summary>
        /// 新しい通知スケジュールを作成します
        /// </summary>
        public async Task<IHttpActionResult> Post(ScheduleFormModel data)
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
            var tasks = notifications.Select(p => _client.ScheduleNotificationAsync(p, data.ScheduledOn.Value, data.TagExpression));

            // 全ての通知リクエストの完了を待機する
            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                // スケジュールログを DB に保存
                var scheduleLog = new ScheduleLog
                {
                    NotificationId = result.ScheduledNotificationId,
                    Platform = result.Payload.GetPlatform(),
                    Message = data.Message,
                    TagExpression = data.TagExpression,
                    ScheduledOn = data.ScheduledOn.Value,
                    CreatedOn = DateTimeOffset.UtcNow
                };

                _context.ScheduleLogs.Add(scheduleLog);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// スケジュールされた通知を削除します
        /// </summary>
        public async Task<ScheduleLog> Delete(CancelScheduleFormModel data)
        {
            // 送信されたデータが正しいか検証する
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // スケジュールキャンセルリクエストを非同期で送信
            await _client.CancelNotificationAsync(data.ScheduledNotificationId);

            // キャンセル日時を DB へ保存
            var entity = _context.ScheduleLogs.First(p => p.NotificationId == data.ScheduledNotificationId);

            entity.CancelledOn = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
