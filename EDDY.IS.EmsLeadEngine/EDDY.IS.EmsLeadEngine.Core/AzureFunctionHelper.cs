using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Services;
using Microsoft.ServiceBus.Messaging;
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage
using EDDY.IS.EmsLeadEngine.Core.Properties;
using Newtonsoft.Json;
using EDDY.IS.EmsLeadEngine.Entities.AzureFunction;
using System.IO;
using System.Net;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public class AzureFunctionHelper
    {
        static Microsoft.ServiceBus.Messaging.QueueClient InsertClient = Microsoft.ServiceBus.Messaging.QueueClient.CreateFromConnectionString(Settings.Default.ServiceBusConnectionString, Settings.Default.InsertQueueName);
        static Microsoft.ServiceBus.Messaging.QueueClient UpdateClient = Microsoft.ServiceBus.Messaging.QueueClient.CreateFromConnectionString(Settings.Default.ServiceBusConnectionString, Settings.Default.UpdateQueueName);
        static Azure.Storage.Queues.QueueClient FBConversionClient = new Azure.Storage.Queues.QueueClient(Settings.Default.FBConversionConnection, Settings.Default.FBConversionQueueName);
        static Azure.Storage.Queues.QueueClient FBAudienceClient = new Azure.Storage.Queues.QueueClient(Settings.Default.FBConversionConnection, Settings.Default.FBAudienceQueueName);
        static Azure.Storage.Queues.QueueClient GoogleConversionClient = new Azure.Storage.Queues.QueueClient(Settings.Default.GoogleConversionConnection, Settings.Default.GoogleConversionQueueName);
        static Azure.Storage.Queues.QueueClient GoogleAudienceClient = new Azure.Storage.Queues.QueueClient(Settings.Default.GoogleAudienceConnection, Settings.Default.GoogleAudienceQueueName);

        public void SendUpdateLeadRequest(UpdateLeadRequest request)
        {
            var jsonMessage = JsonConvert.SerializeObject(request);
            var message = new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage)), true);
            UpdateClient.Send(message);
        }

        public void SendInsertLeadRequest(InsertLeadRequest request)
        {
            var jsonMessage = JsonConvert.SerializeObject(request);
            var message = new BrokeredMessage(new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage)), true);
            InsertClient.Send(message);
        }

        public void SendFBConversionAPIRequest(FBConversionAPIRequest request)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            FBConversionClient.SendMessage(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request))));
        }

        public void SendFBAudienceRequest(FBAudienceRequest request)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            FBAudienceClient.SendMessage(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request))));
        }

        public void SendGoogleConversionAPIRequest(GoogleConversionAPIRequest request)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            GoogleConversionClient.SendMessage(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request))));
        }

        public void SendGoogleAudienceRequest(GoogleAudienceAPIRequest request)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            GoogleAudienceClient.SendMessage(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request))));
        }
    }
}
