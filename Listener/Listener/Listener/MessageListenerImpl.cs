using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Spring.Messaging.Nms;
using NMS;

using System.Drawing;
using System.Net.Json;

namespace Listener
{
    class MessageListenerImpl : IMessageListener
    {
        private ListBox outputListBox;
        private JsonTextParser parser = new JsonTextParser();
        private Dictionary<string, Light> map;  //取出家電指令的value並做mapping map對應開 map2對應關
        private Dictionary<string, Light> map2;

        public MessageListenerImpl(ListBox output)
        {
            outputListBox = output;
            map = new Dictionary<string, Light>();
            map2 = new Dictionary<string, Light>();
            map.Add("BEDROOM-LIGHT_ON", Form1.bedroom_light);
            map.Add("DOOR-LIGHT_ON", Form1.door_light);
            map.Add("LIVINGROOM-LIGHT_ON", Form1.livingroom_light);
            map.Add("STUDYROOM-LIGHT_ON", Form1.studyroom_light);
            map.Add("KITCHEN-LIGHT_ON", Form1.kitchen_light);
            map2.Add("BEDROOM-LIGHT_OFF", Form1.bedroom_light);
            map2.Add("DOOR-LIGHT_OFF", Form1.door_light);
            map2.Add("LIVINGROOM-LIGHT_OFF", Form1.livingroom_light);
            map2.Add("STUDYROOM-LIGHT_OFF", Form1.studyroom_light);
            map2.Add("KITCHEN-LIGHT_OFF", Form1.kitchen_light);
        }

        public void OnMessage(IMessage message)
        {
            string msg = ((ITextMessage)message).Text.Trim();
            JsonObjectCollection obj;
            string value;
            try
            {
                obj = (JsonObjectCollection)parser.Parse(msg);
                if (!"value".Equals(obj[0].Name))
                {
                    outputListBox.Items.Add("Message Received:不正確的指令格式");
                    return;
                }
                value = obj[0].GetValue().ToString();
                try
                {
                    map[value].switch_on();
                }
                catch (KeyNotFoundException)
                {
                    try
                    {
                        map2[value].switch_off();
                    }
                    catch (KeyNotFoundException)
                    {
                        if (value.Equals("TV_ON"))
                        {
                            Form1.tv.switch_on();
                        }
                        else if (value.Equals("TV_OFF"))
                        {
                            Form1.tv.switch_off();
                        }
                        else if (value.Equals("FAN_START"))
                        {
                            Form1.fan.switch_on();
                        }
                        else if (value.Equals("FAN_STOP"))
                        {
                            Form1.fan.switch_off();
                        }
                        else if (value.Equals("TV_PREV") && Form1.tv.is_on())
                        {
                            Form1.tv.channel_prev();
                        }
                        else if (value.Equals("TV_NEXT") && Form1.tv.is_on())
                        {
                            Form1.tv.channel_next();
                        }
                        else
                        {
                            outputListBox.Items.Add("Message Received:不支援的指令");
                            return;
                        }
                    }
                }
                outputListBox.Items.Add("Message Received:" + msg);
            }
            catch (NullReferenceException)
            {
                outputListBox.Items.Add("Message Received:非正確的JSON訊息格式");
            }
            catch (Exception)
            {
                outputListBox.Items.Add("Message Received:非JSON的訊息格式");
            }
        }
    }
}
