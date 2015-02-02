using System;
using System.Collections.Generic;
using System.Text;
using Spring.Messaging.Nms;
using Spring.Messaging.Nms.Listener;
using NMS;
using ActiveMQ;

namespace NMSListener
{
    class Program : IMessageListener
    {
        static void Main(string[] args)
        {
            SimpleMessageListenerContainer container = new SimpleMessageListenerContainer();
            container.ConnectionFactory = new ConnectionFactory("tcp://localhost:61616");
            container.PubSubDomain = true;
            container.DestinationName = "APP.MyTopic";
            container.ConcurrentConsumers = 1;
            container.MessageListener = new Program();
            container.Initialize();
            container.Start();
        }

        public void OnMessage(IMessage message)
        {
            ITextMessage textMessage = (ITextMessage) message;
            Console.WriteLine(textMessage.Text);
           
        }

    }
}
