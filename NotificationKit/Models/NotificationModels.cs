using System;

namespace NotificationKit.Models
{
    /// <summary>
    /// 送信結果のログを格納するエンティティです
    /// </summary>
    public class SendLog
    {
        public int Id { get; set; }

        /// <summary>
        /// 送信したメッセージ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 送信対象となるタグの条件式
        /// </summary>
        public string TagExpression { get; set; }

        /// <summary>
        /// 送信に成功した件数
        /// </summary>
        public long Success { get; set; }

        /// <summary>
        /// 送信に失敗した件数
        /// </summary>
        public long Failure { get; set; }

        /// <summary>
        /// 送信日時
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }
    }

    /// <summary>
    /// スケジュール送信結果のログを格納するエンティティです
    /// </summary>
    public class ScheduleLog
    {
        public int Id { get; set; }

        /// <summary>
        /// スケジュールの ID
        /// </summary>
        public string NotificationId { get; set; }

        /// <summary>
        /// 送信先プラットフォーム
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// 送信したメッセージ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 送信対象となるタグの条件式
        /// </summary>
        public string TagExpression { get; set; }

        /// <summary>
        /// スケジュールされた時間
        /// </summary>
        public DateTimeOffset ScheduledOn { get; set; }

        /// <summary>
        /// キャンセルされた時間
        /// </summary>
        public DateTimeOffset? CancelledOn { get; set; }

        /// <summary>
        /// スケジュール登録日時
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; }
    }
}