using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.SNSEvents;
using System.Text.Json;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using System.Reflection;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace HotelCreatedEventHandler
{
    public class HotelCreatedEventHandler
    {
        public async Task Handler(SNSEvent sNSEvent)
        {
            var dbClient = new AmazonDynamoDBClient();
            var table = Table.LoadTable(dbClient, "hotel-created-event-ids");

            //https://my-deployment-aaee25.es.ap-south-1.aws.elastic-cloud.com
            var host = Environment.GetEnvironmentVariable("host");
            var userName = Environment.GetEnvironmentVariable("userName");
            var password = Environment.GetEnvironmentVariable("password");
            var indexName = Environment.GetEnvironmentVariable("indexName");

            var settings = new ElasticsearchClientSettings(new Uri(host));
            settings.Authentication(new BasicAuthentication(userName, password));
            settings.DefaultIndex(indexName);
            settings.DefaultMappingFor<Hotel>(m => m.IdProperty(p => p.Id));
            var client = new ElasticsearchClient(settings);
            foreach (var eventRecord in sNSEvent.Records)
            {
                var eventId = eventRecord.Sns.MessageId;
                var foundItem = await table.GetItemAsync(eventId);
                if (foundItem == null)
                {
                    await table.PutItemAsync(new Document()
                    {
                        ["messageID"] = eventId
                    });
                    var hotel = JsonSerializer.Deserialize<Hotel>(eventRecord.Sns.Message);

                    if (!(await client.Indices.ExistsAsync(indexName)).Exists)
                    {
                        await client.Indices.CreateAsync(indexName);
                    }
                    await client.IndexAsync<Hotel>(hotel, new CancellationToken());
                }
            }
        }
    }
}




