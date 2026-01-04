// See https://aka.ms/new-console-template for more information

using Amazon.Lambda.SNSEvents;
using HotelCreatedEventHandler;
using System.Text.Json;

Environment.SetEnvironmentVariable("host", "https://my-deployment-aaee25.es.ap-south-1.aws.elastic-cloud.com");
Environment.SetEnvironmentVariable("userName", "elastic");
Environment.SetEnvironmentVariable("password", "QKsmyurTF9sww5KDbfvvGC3J");
Environment.SetEnvironmentVariable("indexName", "event");

var hotel = new Hotel()
{
    Name = "Continental",
    City = "Paris",
    Price = 100,
    Rating = 4,
    Id = "12",
    UserId = "ABC",
    CreatedDateTime = DateTime.Now
};
var snsEvent = new SNSEvent()
{
    Records = new List<SNSEvent.SNSRecord>()
    {
        new SNSEvent.SNSRecord()
        {
            Sns = new SNSEvent.SNSMessage()
            {
                MessageId = "100",
                Message = JsonSerializer.Serialize(hotel)
            }
        }
    }
};
var handler = new HotelCreatedEventHandler.HotelCreatedEventHandler();
await handler.Handler(snsEvent);
Console.WriteLine("Hello, World!");
