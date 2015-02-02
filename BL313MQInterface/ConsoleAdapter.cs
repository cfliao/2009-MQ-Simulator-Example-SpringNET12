using System;
using System.Collections.Generic;
using System.Text;
using Spring.Messaging.Nms.Core;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System.Windows.Forms;
using System.Threading;
using System.Net.Json;

namespace MQInterface
{
    class ConsoleAdapter : IMessageListener
    {
        private mqForm mq;

        private ConnectionFactory connectionFactory;
        private NmsTemplate template;
        private const int cool_down = 28;
        private System.Threading.Thread timer;
        private const int Interval = 60000;
        private bool sended = false;
        private JsonTextParser parser = new JsonTextParser();
        private string lastserverURL = "";

        public ConsoleAdapter(mqForm m)
        {
            mq = m;
            timer = new Thread(new ThreadStart(Count));
        }

        public void OnMessage(IMessage message)
        {
            if (mq.Count() >= 10) mq.Clear();
            string msg = "";
            if ("ssh.HCI.SR".Equals(mq.getDestination()) || "ssh.HCI.TTS".Equals(mq.getDestination()))
            {
                //msg = ((ITextMessage)message).Properties.GetString("content").Trim();
                msg = System.Text.Encoding.Default.GetString(Convert.FromBase64String(((ITextMessage)message).Text));
            }
            else
            {
                msg = ((ITextMessage)message).Text.Trim();
            }
            if("ssh.RAW_DATA".Equals(mq.getDestination()))
            {
                JsonObjectCollection obj;
                try
                {
                    obj = (JsonObjectCollection)parser.Parse(msg);
                    if ("type".Equals(obj[0].Name) && "taroko".Equals(obj[0].GetValue().ToString()) && "id".Equals(obj[1].Name) && "humidity".Equals(obj[2].Name) && "temperature".Equals(obj[3].Name))
                    {
                        int humidity = int.Parse(obj[2].GetValue().ToString());
                        if (humidity <= 100 && humidity >= 0 && double.Parse(obj[3].GetValue().ToString()) >= cool_down && !sended)
                        {
                            too_hot();
                            sended = true;
                            timer.Start();
                        }
                    }
                }
                catch (Exception){}
            }
            mq.Add("Recv >" + msg);
        }

        private void too_hot()
        {
            string serverURL = mq.getServerURL();
            try
            {
                if (!lastserverURL.Equals(serverURL))
                {
                    connectionFactory = new ConnectionFactory(serverURL);
                    template = new NmsTemplate(connectionFactory);
                    lastserverURL = serverURL;
                    template.PubSubDomain = true;
                }
                template.SendWithDelegate("ssh.COMMAND", delegate(ISession session)
                {
                    ITextMessage message = session.CreateTextMessage();
                    message.Text = "{\"value\":\"FAN_START\"}";
                    return message;
                });
            }
            catch (Exception)
            {
                
            }
        }

        private void Count()
        {
            Thread.Sleep(Interval);
            sended = false;
            timer.Abort();
        }
    }
}
