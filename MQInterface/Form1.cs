using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Spring.Messaging.Nms;
using Spring.Messaging.Nms.Listener;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Spring.Messaging.Nms.Core;


namespace MQInterface
{
    public partial class mqForm : Form
    {
        private string serverURL;
        private ConnectionFactory connectionFactory;
        private ConnectionFactory factory;
        private NmsTemplate template;
        private SimpleMessageListenerContainer container;
        private Thread listenerThread;
        private string nowserverURL = "failover://localhost:61616/(tcp://localhost:61616)";
        //private string nowserverURL = "tcp://localhost:61616";
        private string nowDestination = "ssh.COMMAND";

        public mqForm()
        {

            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(exit);
            serverURL = "failover://localhost:61616/(tcp://localhost:61616)";
            connectionFactory = new ConnectionFactory(serverURL);
            listenerThread = new Thread(new ThreadStart(Run));
            template = new NmsTemplate(connectionFactory);
            template.PubSubDomain = true;
            //textBox1.Focus();
        }



        private void exit(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                factory = new ConnectionFactory(mqUrlTextBox.Text);
                nowserverURL = mqUrlTextBox.Text;
                connectButton.Visible = false;
                disconnectButton.Visible = true;
                container = new SimpleMessageListenerContainer();
                CheckForIllegalCrossThreadCalls = false;
                listenerThread = new Thread(new ThreadStart(Run));
                listenerThread.Start();
                errormsgS.Text = "";
                mqUrlTextBox.Enabled = false;
                topicCb.Enabled = false;
            }
            catch (UriFormatException ex)
            {
                errormsgS.Text = ex.Message;
            }
        }

        private void Run()
        {
            container.ConnectionFactory = factory;
            container.PubSubDomain = true;
            container.DestinationName = topicCb.Text;
            container.ConcurrentConsumers = 1;
            container.MessageListener = new ConsoleAdapter(this);
            try
            {
                container.Initialize();
                container.Start();
                errormsgS.Text = "";
            }
            catch (System.Net.Sockets.SocketException e)
            {
                errormsgS.Text = e.Message;
            }
        }

        private void clearMessage_Click(object sender, EventArgs e)
        {
            SensorReader.Items.Clear();
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            container.Stop();
            listenerThread.Abort();
            connectButton.Visible = true;
            disconnectButton.Visible = false;
            mqUrlTextBox.Enabled = true;
            topicCb.Enabled = true;
        }

        private void changeServerURL_Click(object sender, EventArgs e)
        {
            if (!serverURL.Equals(URLofServer.Text))
            {
                serverURL = URLofServer.Text;
                try
                {
                    connectionFactory = new ConnectionFactory(serverURL);
                    template = new NmsTemplate(connectionFactory);
                    template.PubSubDomain = true;
                    errormsgS.Text = "";
                }
                catch (UriFormatException ex)
                {
                    errormsgS.Text = ex.Message;
                }
            }
        }

        private void clearsendBox_Click(object sender, EventArgs e)
        {
            sendBox.Items.Clear();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!nowDestination.Equals(destinationBox.Text))
                {
                    nowDestination = destinationBox.Text;
                    template = new NmsTemplate(connectionFactory);
                    template.PubSubDomain = true;
                }

                template.SendWithDelegate(destinationBox.Text, delegate(ISession session)
                {
                    ITextMessage message = session.CreateTextMessage();
                    if ("ssh.HCI.SR".Equals(destinationBox.Text) || "ssh.HCI.TTS".Equals(destinationBox.Text))
                    {

                        message.Text = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(inputBox.Text));
                        sendBox.Items.Add("Encoding: " + message.Text);
                        //message.Properties.SetString("content", inputBox.Text);
                        //message.Text = "HCI_SR_TTS";
                    }
                    else message.Text = inputBox.Text;
                    return message;
                });
                sendBox.Items.Add("Message Sent: " + inputBox.Text);
                errormsgS.Text = "";
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                errormsgS.Text = ex.Message;
            }
        }

        public string getServerURL()
        {
            return nowserverURL;
        }

        public void Add(string s)
        {
            SensorReader.Items.Add(s);
        }

        public int Count()
        {
            return SensorReader.Items.Count;
        }

        public void Clear()
        {
            SensorReader.Items.Clear();
        }

        public string getDestination()
        {
            return topicCb.Text;
        }

      
        private void mqForm_Load(object sender, EventArgs e)
        {

        }

        private void senderTab_Click(object sender, EventArgs e)
        {

        }

        //private void sendMessage(String dest, String msg)
        //{
        //    try
        //    {
        //        template = new NmsTemplate(connectionFactory);
        //        template.PubSubDomain = true;
        //        template.SendWithDelegate(dest, delegate(ISession session)
        //        {
        //            ITextMessage message = session.CreateTextMessage();
        //            if ("ssh.HCI.SR".Equals(topicCb.Text) || "ssh.HCI.TTS".Equals(topicCb.Text))
        //            {
        //               //message.Properties.SetString("content", msg);
        //               // message.Text = "HCI_SR_TTS";
        //                message.Text = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(inputBox.Text));
        //            }
        //            else message.Text = msg;
        //            return message;
        //        });
        //    }
        //    catch (System.Net.Sockets.SocketException ex)
        //    {
        //        sendBox.Text = ex.Message;
        //    }

        //}


    }
}