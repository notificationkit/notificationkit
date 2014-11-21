using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

using CsvHelper;

using Microsoft.ServiceBus.Notifications;

using Newtonsoft.Json;

using NotificationKit.Models;

namespace NotificationKit.Controllers
{
    /// <summary>
    /// インポートやエクスポートを行う REST API を実装するコントローラーです
    /// </summary>
    public class BatchController : ApiController
    {
        private readonly NotificationHubClient _client = NotificationHubClient.CreateClientFromConnectionString(AppSettings.NotificationHubConnectionString, AppSettings.NotificationHubPath);

        /// <summary>
        /// CSV 形式で登録されている情報の一覧を出力します
        /// </summary>
        public async Task<HttpResponseMessage> Get(string platform = "", string expression = null)
        {
            // 登録情報を取得
            var result = string.IsNullOrEmpty(expression) ? await _client.GetAllRegistrationsAsync(0) : await _client.GetRegistrationsByTagExpressionAsync(expression);

            // プラットフォームでフィルタリング
            var platforms = platform.Split(',');

            var records = result.Select(p => new RegistrationModel(p)).Where(p => platforms.Contains(p.Platform));

            // CSV として出力する
            var writer = new StringWriter();

            var csvWriter = new CsvWriter(writer);

            csvWriter.Configuration.RegisterClassMap<RegistrationModelMap>();

            csvWriter.WriteRecords(records);

            // ダウンロード可能な形でレスポンスを作成
            var response = new HttpResponseMessage
            {
                Content = new StringContent(writer.ToString())
            };

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "export.csv" };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return response;
        }

        /// <summary>
        /// CSV 形式でアップロードされたファイルを元に登録情報の追加、更新を行います
        /// </summary>
        public async Task<HttpResponseMessage> Post()
        {
            // マルチパート以外の場合には拒否
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // マルチパートデータを読み込む
            var provider = await Request.Content.ReadAsMultipartAsync();
            var content = provider.Contents.FirstOrDefault(p => p.Headers.ContentDisposition.Name == "\"csv\"");

            // ファイルが見つからない場合にはエラー
            if (content == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            RegistrationModel[] records;

            try
            {
                // CSV としてファイルを解析する
                var csvData = await content.ReadAsStringAsync();
                var reader = new StringReader(csvData);

                var csvReader = new CsvReader(reader);

                csvReader.Configuration.RegisterClassMap<RegistrationModelMap>();

                records = csvReader.GetRecords<RegistrationModel>().ToArray();
            }
            catch (Exception ex)
            {
                return Failure(ex.Message);
            }

            // 予め登録済みのデータがあるかを非同期で一括取得する
            var registrations = (await Task.WhenAll(records.Where(p => !string.IsNullOrEmpty(p.RegistrationId)).Select(p => _client.GetRegistrationAsync<RegistrationDescription>(p.RegistrationId)))).Where(p => p != null).ToArray();

            long insert = 0, update = 0;

            var targets = new List<RegistrationDescription>();

            // 1 レコードごとに追加なのか、更新なのか RegistrationId の有無で処理を行う
            foreach (var record in records)
            {
                var registration = registrations.FirstOrDefault(p => p.RegistrationId == record.RegistrationId);

                if (registration == null)
                {
                    switch (record.Platform)
                    {
                        case Platforms.Windows:
                            registration = new WindowsRegistrationDescription(record.Handle);
                            break;
                        case Platforms.WindowsPhone:
                            registration = new MpnsRegistrationDescription(record.Handle);
                            break;
                        case Platforms.Apple:
                            registration = new AppleRegistrationDescription(record.Handle);
                            break;
                        case Platforms.Android:
                            registration = new GcmRegistrationDescription(record.Handle);
                            break;
                        default:
                            throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }

                    registration.RegistrationId = await _client.CreateRegistrationIdAsync();

                    insert += 1;
                }
                else
                {
                    switch (record.Platform)
                    {
                        case Platforms.Windows:
                            ((WindowsRegistrationDescription)registration).ChannelUri = new Uri(record.Handle);
                            break;
                        case Platforms.WindowsPhone:
                            ((MpnsRegistrationDescription)registration).ChannelUri = new Uri(record.Handle);
                            break;
                        case Platforms.Apple:
                            ((AppleRegistrationDescription)registration).DeviceToken = record.Handle;
                            break;
                        case Platforms.Android:
                            ((GcmRegistrationDescription)registration).GcmRegistrationId = record.Handle;
                            break;
                        default:
                            throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }

                    update += 1;
                }

                registration.Tags = record.Tags;

                targets.Add(registration);
            }

            try
            {
                // 処理対象を非同期で一括処理する
                await Task.WhenAll(targets.Select(p => _client.CreateOrUpdateRegistrationAsync(p)));
            }
            catch (Exception ex)
            {
                return Failure(ex.Message);
            }

            // 処理結果を返却
            return Success(string.Format("Imported from CSV : Insert {0}, Update {1}, Total {2}", insert, update, insert + update));
        }

        protected HttpResponseMessage Success(string message)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(message))
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }

        protected HttpResponseMessage Failure(string message)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { Message = message }))
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}
