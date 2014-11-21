using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.ServiceBus.Notifications;

using NotificationKit.Models;

namespace NotificationKit.Controllers
{
    /// <summary>
    /// Registration タブで必要な REST API を実装するコントローラーです
    /// </summary>
    public class RegistrationController : ApiController
    {
        private readonly NotificationHubClient _client = NotificationHubClient.CreateClientFromConnectionString(AppSettings.NotificationHubConnectionString, AppSettings.NotificationHubPath);

        /// <summary>
        /// 通知ハブに登録されている全ての登録情報を返します
        /// </summary>
        public async Task<IEnumerable<RegistrationModel>> Get()
        {
            // 登録されている情報を全て取得
            var result = await _client.GetAllRegistrationsAsync(0);

            // モデルへマッピングして返す
            return result.Select(p => new RegistrationModel(p));
        }

        /// <summary>
        /// 通知ハブに登録されているタグに関連付いている登録情報を返します
        /// </summary>
        public async Task<IEnumerable<RegistrationModel>> Get(string expression)
        {
            // タグに関連付いている情報を全て取得
            var result = await _client.GetRegistrationsByTagExpressionAsync(expression);

            // モデルへマッピングして返す
            return result.Select(p => new RegistrationModel(p));
        }

        /// <summary>
        /// 指定された ID の登録を削除します
        /// </summary>
        public async Task<IHttpActionResult> Delete(DeleteRegistrationFormModel data)
        {
            // 送信されたデータが正しいか検証する
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ID に紐付く登録を削除
            await _client.DeleteRegistrationAsync(data.RegistrationId);

            return Ok();
        }
    }
}
