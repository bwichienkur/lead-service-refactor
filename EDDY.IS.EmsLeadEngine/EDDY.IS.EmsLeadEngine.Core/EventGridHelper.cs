using EDDY.IS.EmsLeadEngine.Core.Properties;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.EmsLeadEngine.Core
{
    public class EventGridHelper
    {
        EventGridClient client = new EventGridClient(new TopicCredentials(Settings.Default.EventGridTopicKey));
        
        public void SendEventGridRequest(List<EventGridEvent> eventList)
        {
            string topicHostName = new Uri(ConfigurationManager.AppSettings["EventGridUrl"]).Host;

            client.PublishEventsAsync(topicHostName, eventList).GetAwaiter().GetResult();
        }

    }
}
