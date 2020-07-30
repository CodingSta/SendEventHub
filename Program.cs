using System;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO.Ports;
using Microsoft.Azure.EventHubs;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SendEventHub
{
    class Program
    {
        private const string EVHConnStr = "Endpoint=sb://..........";
        private static EventHubClient eventHubClient;
        private const string EventHubName = "hub01";
        static float temperature = 0.0f;
        static float humidity = 0.0f;

        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
            //Console.WriteLine("Hello World!");
        }

        private static async Task MainAsync(string[] args){

            var connStrBuilder = new EventHubsConnectionStringBuilder(EVHConnStr){
                EntityPath = EventHubName
            };
            eventHubClient = EventHubClient.CreateFromConnectionString(connStrBuilder.ToString());

            StringBuilder builderA = new StringBuilder();
            while(true){

                Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                builderA.Append("{");

                // device -> Partition Key
                builderA.Append("\"device\":");
                builderA.Append("\"0001\"");
                builderA.Append(",");
                // time -> Row Key
                builderA.Append("\"time\":");
                builderA.Append("\""+ unixTimestamp +"\"");
                builderA.Append(",");

                // First Data
                builderA.Append("\"temperature\":");
                builderA.Append(temperature+=1);
                builderA.Append(",");

                // Second Data
                builderA.Append("\"humidity\":");
                builderA.Append(humidity+=1);
                
                builderA.Append("}\n");

                Console.WriteLine(builderA);
                await SendMessageToEventHub(builderA.ToString());
                builderA.Clear();
                await Task.Delay(1000);
            }
        }
        private static async Task SendMessageToEventHub(string message){
        try{
            await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
        } catch(Exception e) {
            Console.WriteLine(e.Message);
        }

    }


    }
}
