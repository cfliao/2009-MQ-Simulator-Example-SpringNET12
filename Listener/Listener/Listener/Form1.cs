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
using NMS;
using ActiveMQ;

/*
 * 主要功能:1.聽取ssh.COMMAND並將家電指令反映在UI上
 *          2.模擬sensor並定時送出資訊
 *          3.模擬屋內有人走動 將People物件移到房間燈就會亮 移到沙發會送出壓力資訊
 *          4.與dependent版本差別在於此版電視僅由家電指令開啟 會主動發送電視的電流資訊到ssh.RAW_DATA
 */

namespace Listener
{

    public partial class Form1 : Form
    {

        private string serverURL;                           //for control
        private ConnectionFactory connectionFactory;
        private SimpleMessageListenerContainer container;
        private Thread thread;  //listen to ssh.COMMAND
        public static Light door_light;
        public static Light livingroom_light;
        public static Light kitchen_light;
        public static Light studyroom_light;
        public static Light bedroom_light;
        public static TV tv;
        public static Fan fan;

        private ConnectionFactory factoryS;         //for sensor
        private NmsTemplate nms;
        private NmsTemplate nmsC;
        private Thread TH1Thread;   //simulate temperature & humidity sensor
        private Thread TH2Thread;
        private Thread TH3Thread;
        private Thread co21Thread;  //simulate CO2 sensor
        private Thread co22Thread;
        private Thread co23Thread;
        private static int SENSING_INTERVAL = 5000;
        private string lastconnectedURL = "";
        private string lastDestination = "";

        private Graphics g;                                         //for graphics
        private SolidBrush brushb = new SolidBrush(Color.Black);
        private SolidBrush brushw = new SolidBrush(Color.White);
        private SolidBrush brushg = new SolidBrush(Color.LightGray);
        private Pen pen = new Pen(Color.Black);
        private const int ratio = 5;
        private int width;
        private int height;
        public EventArgs ea = new EventArgs();

        public Dictionary<int, Light> map;                  //for positioning
        private People[] person = new People[People.max];
        private int peopleInDoor = 0;   //計算各房間現有人數
        private int peopleInLiving = 0;
        private int peopleInKitchen = 0;
        private int peopleInStudy = 0;
        private int peopleInBed = 0;
        public int peopleOnSofa = 0;
        public bool sofaflag = false;
        private int DOOR = 0;
        private int LIVINGROOM = 1;
        private int KITCHEN = 2;
        private int STUDYROOM = 3;
        private int BEDROOM = 4;

        public Form1()
        {
            InitializeComponent();
            width = panel.Width;
            height = panel.Height;

            serverURL = "tcp://localhost:61616";                    //for connection with ActiveMQ
            connectionFactory = new ConnectionFactory(serverURL);
            container = new SimpleMessageListenerContainer();
            nmsC = new NmsTemplate(connectionFactory);
            nmsC.PubSubDomain = true;

            door_light = new Light(pictureDoorLight, stateDoorLight, labelD);                   //each appliance is about to have a class
            livingroom_light = new Light(pictureLivingroomLight, stateLivingroomLight, labelL);
            kitchen_light = new Light(pictureKitchenLight, stateKitchenLight, labelK);
            studyroom_light = new Light(pictureStudyroomLight, stateStudyroomLight, labelS);
            bedroom_light = new Light(pictureBedroomLight, stateBedroomLight, labelB);
            tv = new TV(pictureTV, stateTV, channel, nmsC, receiveBox);
            fan = new Fan(pictureFan, stateFan);

            TH1Thread = new Thread(new ThreadStart(TH1ThreadRun));  //for each sensor monitor
            TH2Thread = new Thread(new ThreadStart(TH2ThreadRun));
            TH3Thread = new Thread(new ThreadStart(TH3ThreadRun));
            co21Thread = new Thread(new ThreadStart(co21ThreadRun));
            co22Thread = new Thread(new ThreadStart(co22ThreadRun)); 
            co23Thread = new Thread(new ThreadStart(co23ThreadRun));

            this.MouseHover += new EventHandler(Display_Click);     //目前我是透過繪製panel的Graphics物件來表現電燈的開關狀態
            this.MouseUp += new MouseEventHandler(Display_Click);   //不過重新繪製後它不會馬上反映在panel上 所以透過增加這幾個
            panel.MouseHover += new EventHandler(Display_Click);    //EventHandler讓滑鼠在panel移動就會重繪
            panel.MouseUp += new MouseEventHandler(Display_Click);
            
            this.FormClosed += new FormClosedEventHandler(exit);

            map = new Dictionary<int, Light>();     //for positioning
            map.Add(DOOR, door_light);
            map.Add(LIVINGROOM, livingroom_light);
            map.Add(KITCHEN, kitchen_light);
            map.Add(STUDYROOM, studyroom_light);
            map.Add(BEDROOM, bedroom_light);

            InitMessageListenerThread();
        }

        private void exit(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void InitMessageListenerThread()
        {
            CheckForIllegalCrossThreadCalls = false;
            thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }

        private void Run()
        {
            container.ConnectionFactory = connectionFactory;
            container.PubSubDomain = true;
            container.DestinationName = "ssh.COMMAND";
            container.ConcurrentConsumers = 1;
            container.MessageListener = new MessageListenerImpl(receiveBox);
            try
            {
                container.Initialize();
                container.Start();
                errormsg.Text = "";
            }
            catch(System.Net.Sockets.SocketException e)
            {
                errormsg.Text = e.Message;
            }
        }

        private void TH1ThreadRun()
        {
            while (true)
            {
                Thread.Sleep(SENSING_INTERVAL);
                StringBuilder sb = new StringBuilder();
                sb.Append("{\"type\":\"taroko\", \"id\":\"" + TH1Id.Text + "\", \"humidity\":\"" + hygro1Value.Value + "\", \"temperature\":\"" + therom1Value.Value + "." + therom1Value2.Value + "\" }");
                try
                {
                    nms.ConvertAndSend(topicCb.Text, sb.ToString());
                    errormsgS.Text = "";
                }
                catch(System.Net.Sockets.SocketException e)
                {
                    errormsgS.Text = e.Message;
                }
            }
        }

        private void TH2ThreadRun()
        {
            while (true)
            {
                Thread.Sleep(SENSING_INTERVAL);
                StringBuilder sb = new StringBuilder();
                sb.Append("{\"type\":\"taroko\", \"id\":\"" + TH2Id.Text + "\", \"humidity\":\"" + hygro2Value.Value + "\", \"temperature\":\"" + therom2Value.Value + "." + therom2Value2.Value + "\" }");
                try
                {
                    nms.ConvertAndSend(topicCb.Text, sb.ToString());
                    errormsgS.Text = "";
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    errormsgS.Text = e.Message;
                }
            }
        }

        private void TH3ThreadRun()
        {
            while (true)
            {
                Thread.Sleep(SENSING_INTERVAL);
                StringBuilder sb = new StringBuilder();
                sb.Append("{\"type\":\"taroko\", \"id\":\"" + TH3Id.Text + "\", \"humidity\":\"" + hygro3Value.Value + "\", \"temperature\":\"" + therom3Value.Value + "." + therom3Value2.Value + "\" }");
                try
                {
                    nms.ConvertAndSend(topicCb.Text, sb.ToString());
                    errormsgS.Text = "";
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    errormsgS.Text = e.Message;
                }
            }
        }

        private void co21ThreadRun()
        {
            while (true)
            {
                Thread.Sleep(SENSING_INTERVAL);
                StringBuilder sb = new StringBuilder();
                sb.Append("{\"type\":\"taroko\", \"id\":\"" + co21Id.Text + "\", \"CO2\":\"" + co21Value.Value + "\" }");
                try
                {
                    nms.ConvertAndSend(topicCb.Text, sb.ToString());
                    errormsgS.Text = "";
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    errormsgS.Text = e.Message;
                }
            }
        }

        private void co22ThreadRun()
        {
            while (true)
            {
                Thread.Sleep(SENSING_INTERVAL);
                StringBuilder sb = new StringBuilder();
                sb.Append("{\"type\":\"taroko\", \"id\":\"" + co22Id.Text + "\", \"CO2\":\"" + co22Value.Value + "\" }");
                try
                {
                    nms.ConvertAndSend(topicCb.Text, sb.ToString());
                    errormsgS.Text = "";
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    errormsgS.Text = e.Message;
                }
            }
        }

        private void co23ThreadRun()
        {
            while (true)
            {
                Thread.Sleep(SENSING_INTERVAL);
                StringBuilder sb = new StringBuilder();
                sb.Append("{\"type\":\"taroko\", \"id\":\"" + co23Id.Text + "\", \"CO2\":\"" + co23Value.Value + "\" }");
                try
                {
                    nms.ConvertAndSend(topicCb.Text, sb.ToString());
                    errormsgS.Text = "";
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    errormsgS.Text = e.Message;
                }
            }
        }

        private void co21Enabled_CheckedChanged(object sender, EventArgs e)
        {
            if (co21Enabled.Checked && co21Thread.ThreadState == ThreadState.Unstarted)
            {
                co21Thread.Start();
            }
            else if (co21Enabled.Checked && co21Thread.ThreadState == ThreadState.Suspended)
            {
                co21Thread.Resume();
            }
            else
            {
                co21Thread.Suspend();
            }
        }

        private void co22Enabled_CheckedChanged(object sender, EventArgs e)
        {
            if (co22Enabled.Checked && co22Thread.ThreadState == ThreadState.Unstarted)
            {
                co22Thread.Start();
            }
            else if (co22Enabled.Checked && co22Thread.ThreadState == ThreadState.Suspended)
            {
                co22Thread.Resume();
            }
            else
            {
                co22Thread.Suspend();
            }
        }

        private void co23Enabled_CheckedChanged(object sender, EventArgs e)
        {
            if (co23Enabled.Checked && co23Thread.ThreadState == ThreadState.Unstarted)
            {
                co23Thread.Start();
            }
            else if (co23Enabled.Checked && co23Thread.ThreadState == ThreadState.Suspended)
            {
                co23Thread.Resume();
            }
            else
            {
                co23Thread.Suspend();
            }
        }

        public void sendPressure()
        {
            StringBuilder sb = new StringBuilder();
            if (peopleOnSofa > 0 && !sofaflag)
            {
                sb.Append("{\"type\":\"taroko\", \"id\":\"F1_1\", \"Pressure\":\"1\" }");
                sofaflag = true;
                try
                {
                    nmsC.ConvertAndSend("ssh.RAW_DATA", sb.ToString());
                }
                catch (Exception)
                {
                    receiveBox.Items.Add("壓力sensor發生錯誤");
                }
            }
            else if (peopleOnSofa == 0 && sofaflag)
            {
                sb.Append("{\"type\":\"taroko\", \"id\":\"F1_1\", \"Pressure\":\"0\" }");
                sofaflag = false;
                try
                {
                    nmsC.ConvertAndSend("ssh.RAW_DATA", sb.ToString());
                }
                catch (Exception)
                {
                    receiveBox.Items.Add("壓力sensor發生錯誤");
                }
            }
        }

        private void Initialize_Click(object sender, EventArgs e)
        {
            g = panel.CreateGraphics(); //開始繪製房間
            g.Clear(Color.LightGray);
            g.FillRectangle(brushb, 0            , 0                   , width       , ratio       );    //圍牆上
            g.FillRectangle(brushb, 0            , height - ratio      , width * 7/9 , ratio       );    //圍牆下
            g.FillRectangle(brushb, 0            , 0                   , ratio       , height      );    //圍牆左
            g.FillRectangle(brushb, width - ratio, 0                   , ratio       , height      );    //圍牆右
            g.FillRectangle(brushb, width / 3    , 0                   , ratio       , height * 3/5);    //臥房廚房隔間
            g.FillRectangle(brushb, width / 9    , height * 3/5 - ratio, width * 2/9 , ratio       );    //臥房書房隔間
            //g.FillRectangle(brushb, width * 13/27, 0                   , width * 2/27, height * 3/5);    //廚房客廳隔間
            g.DrawLine(pen, width * 13/27, 0, width * 5/9, height * 3/5);
            g.DrawRectangle(pen, width * 13/27, 0, width * 2/27, height * 3/5 - 1);
            door_light.switch_off();
            livingroom_light.switch_off();
            kitchen_light.switch_off();
            studyroom_light.switch_off();
            bedroom_light.switch_off();
            tv.switch_off();
            tv.clean_channel();
            fan.switch_off();
            g.Dispose();
            for (int i = 0; i < People.count; i++) person[i].Dispose();
            People.count = 0;
            pictureSofa.Image = global::Listener.Properties.Resources.sofaD;
            label35.Text = "";
            peopleInDoor = 0;
            peopleInLiving = 0;
            peopleInKitchen = 0;
            peopleInStudy = 0;
            peopleInBed = 0;
            peopleOnSofa = 0;
            sofaflag = false;
        }

        public void Display_Click(object sender, EventArgs e)
        {
            //比例 橫 臥房1.5:廚房1:客廳2 直 臥房/廚房/客廳1.5:書房/門口1 廚房客廳隔間佔廚房1/3
            g = panel.CreateGraphics();
            g.FillRectangle(brushb, 0            , 0                   , width       , ratio       );    //圍牆上
            g.FillRectangle(brushb, 0            , height - ratio      , width * 7/9 , ratio       );    //圍牆下
            g.FillRectangle(brushb, 0            , 0                   , ratio       , height      );    //圍牆左
            g.FillRectangle(brushb, width - ratio, 0                   , ratio       , height      );    //圍牆右
            g.FillRectangle(brushb, width / 3    , 0                   , ratio       , height * 3/5);    //臥房廚房隔間
            g.FillRectangle(brushb, width / 9    , height * 3/5 - ratio, width * 2/9 , ratio       );    //臥房書房隔間
            //g.FillRectangle(brushb, width * 13/27, 0                   , width * 2/27, height * 3/5);    //廚房客廳隔間
            g.DrawLine(pen, width * 13/27, 0, width * 5/9, height * 3/5);
            g.DrawRectangle(pen, width * 13/27, 0, width * 2/27, height * 3/5 - 1);
            if (bedroom_light.is_on())
            {
                g.FillRectangle(brushw, ratio          , ratio               , width/3 - ratio                , height * 3/5 - 2*ratio);    //開房間燈
                g.FillRectangle(brushw, ratio          , height * 3/5 - ratio, width/9 - ratio                , ratio                 );
            }
            else
            {
                g.FillRectangle(brushg, ratio, ratio, width / 3 - ratio, height * 3 / 5 - 2 * ratio);    //關房間燈
                g.FillRectangle(brushg, ratio, height * 3 / 5 - ratio, width / 9 - ratio, ratio);
            }
            if (kitchen_light.is_on())
            {
                g.FillRectangle(brushw, width/3 + ratio, ratio               , width * 13/27 - width/3 - ratio, height * 3/5 - ratio  );    //開廚房燈
            }
            else
            {
                g.FillRectangle(brushg, width / 3 + ratio, ratio, width * 13 / 27 - width / 3 - ratio, height * 3 / 5 - ratio);    //關廚房燈
            }
            if (livingroom_light.is_on())
            {
                g.FillRectangle(brushw, width * 5/9    , ratio               , width * 4/9 - ratio            , height * 3/5 - ratio  );    //開客廳燈
                pictureSofa.Image = global::Listener.Properties.Resources.sofaL;
            }
            else
            {
                g.FillRectangle(brushg, width * 5 / 9, ratio, width * 4 / 9 - ratio, height * 3 / 5 - ratio);    //關客廳燈
                pictureSofa.Image = global::Listener.Properties.Resources.sofaD;
            }
            if (studyroom_light.is_on())
            {
                g.FillRectangle(brushw, ratio          , height * 3/5        , width * 5/9 - ratio            , height * 2/5 - ratio  );    //開書房燈
            }
            else
            {
                g.FillRectangle(brushg, ratio, height * 3 / 5, width * 5 / 9 - ratio, height * 2 / 5 - ratio);    //關書房燈
            }
            if (door_light.is_on())
            {
                g.FillRectangle(brushw, width * 5/9    , height * 3/5        , width * 4/9 - ratio            , height * 2/5 - ratio  );    //開門口燈
            }
            else
            {
                g.FillRectangle(brushg, width * 5/9    , height * 3/5        , width * 4/9 - ratio            , height * 2/5 - ratio  );    //關門口燈
            }
            g.Dispose();
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Display_Click("", ea);
        }


        private void sendTestMessageButton_Click(object sender, EventArgs e)
        {
            if ("ssh.HCI.SR".Equals(topicCb.Text.Trim()) || "ssh.HCI.TTS".Equals(topicCb.Text.Trim()))
            {
                nms.SendWithDelegate(topicCb.Text, delegate(ISession session)
                {
                    ITextMessage msg = session.CreateTextMessage();
                    msg.Properties.SetString("content", testMessageTextBox.Text);
                    msg.Text = "HCI_SR_TTS";
                    return msg;
                });
            }
            else
            {
                try
                {
                    nms.ConvertAndSend(topicCb.Text, testMessageTextBox.Text);
                }
                catch(Exception ex)
                {
                    errormsgS.Text = ex.Message;
                }
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!lastconnectedURL.Equals(mqUrlTextBox.Text) || !lastDestination.Equals(topicCb.Text))
                {
                    factoryS = new ConnectionFactory(mqUrlTextBox.Text);
                    lastconnectedURL = mqUrlTextBox.Text;
                    lastDestination = topicCb.Text;
                    nms = new NmsTemplate(factoryS);
                    nms.PubSubDomain = true;
                    CheckForIllegalCrossThreadCalls = false;
                }
                TestMessagePanel.Visible = true;
                connectButton.Visible = false;
                disconnectButton.Visible = true;
                TH1Enabled.Enabled = true;
                TH2Enabled.Enabled = true;
                TH3Enabled.Enabled = true;
                co21Enabled.Enabled = true;
                co22Enabled.Enabled = true;
                co23Enabled.Enabled = true;
                mqUrlTextBox.Enabled = false;
                topicCb.Enabled = false;
                errormsgS.Text = "";
            }
            catch (UriFormatException ex)
            {
                errormsgS.Text = ex.Message;
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            if (TH1Thread.ThreadState == ThreadState.Running) TH1Thread.Suspend();
            if (TH2Thread.ThreadState == ThreadState.Running) TH2Thread.Suspend();
            if (TH3Thread.ThreadState == ThreadState.Running) TH3Thread.Suspend();
            if (co21Thread.ThreadState == ThreadState.Running) co21Thread.Suspend();
            if (co22Thread.ThreadState == ThreadState.Running) co22Thread.Suspend();
            if (co23Thread.ThreadState == ThreadState.Running) co23Thread.Suspend();
            connectButton.Visible = true;
            disconnectButton.Visible = false;
            TestMessagePanel.Visible = false;
            TH1Enabled.Checked = false;
            TH2Enabled.Checked = false;
            TH3Enabled.Checked = false;
            co21Enabled.Checked = false;
            co22Enabled.Checked = false;
            co23Enabled.Checked = false;
            TH1Enabled.Enabled = false;
            TH2Enabled.Enabled = false;
            TH3Enabled.Enabled = false;
            co21Enabled.Enabled = false;
            co22Enabled.Enabled = false;
            co23Enabled.Enabled = false;
            mqUrlTextBox.Enabled = true;
            topicCb.Enabled = true;
        }

        private void changeServerURL_Click(object sender, EventArgs e)
        {
            if (!serverURL.Equals(URLofServer.Text))
            {
                container.Stop();
                thread.Abort();
                serverURL = URLofServer.Text;
                try
                {
                    connectionFactory = new ConnectionFactory(serverURL);
                    container = new SimpleMessageListenerContainer();
                    InitMessageListenerThread();
                    Initialize_Click("", ea);
                    errormsg.Text = "";
                    nmsC = new NmsTemplate(connectionFactory);
                    nmsC.PubSubDomain = true;
                    CheckForIllegalCrossThreadCalls = false;
                }
                catch (UriFormatException ex)
                {
                    errormsg.Text = ex.Message;
                }
            }
        }

        private void clearreceiveBox_Click(object sender, EventArgs e)
        {
            receiveBox.Items.Clear();
        }

        private void TH1Enabled_CheckedChanged(object sender, EventArgs e)
        {
            if (TH1Enabled.Checked && TH1Thread.ThreadState == ThreadState.Unstarted)
            {
                TH1Thread.Start();
            }
            else if (TH1Enabled.Checked && TH1Thread.ThreadState == ThreadState.Suspended)
            {
                TH1Thread.Resume();
            }
            else
            {
                TH1Thread.Suspend();
            }
        }

        private void TH2Enabled_CheckedChanged(object sender, EventArgs e)
        {
            if (TH2Enabled.Checked && TH2Thread.ThreadState == ThreadState.Unstarted)
            {
                TH2Thread.Start();
            }
            else if (TH2Enabled.Checked && TH2Thread.ThreadState == ThreadState.Suspended)
            {
                TH2Thread.Resume();
            }
            else
            {
                TH2Thread.Suspend();
            }
        }

        private void TH3Enabled_CheckedChanged(object sender, EventArgs e)
        {
            if (TH3Enabled.Checked && TH3Thread.ThreadState == ThreadState.Unstarted)
            {
                TH3Thread.Start();
            }
            else if (TH3Enabled.Checked && TH3Thread.ThreadState == ThreadState.Suspended)
            {
                TH3Thread.Resume();
            }
            else
            {
                TH3Thread.Suspend();
            }
        }

        public int position(int x, int y, int mode)
        {
            if (y >= ratio && y < height * 3 / 5)
            {
                if (x >= ratio && x < width / 3 + ratio)
                {
                    if (mode == 1)
                    {
                        peopleInBed++;
                        return BEDROOM;
                    }
                    else if (mode == 2)
                    {
                        peopleInBed--;
                        if (peopleInBed == 0) return BEDROOM;
                    }
                }
                else if (x >= width / 3 + ratio && x < width * 5 / 9)
                {
                    if (mode == 1)
                    {
                        peopleInKitchen++;
                        return KITCHEN;
                    }
                    else if (mode == 2)
                    {
                        peopleInKitchen--;
                        if (peopleInKitchen == 0) return KITCHEN;
                    }
                }
                else if (x >= width * 5 / 9 && x < width - ratio)
                {
                    if (mode == 1)
                    {
                        peopleInLiving++;
                        return LIVINGROOM;
                    }
                    else if (mode == 2)
                    {
                        peopleInLiving--;
                        if (peopleInLiving == 0) return LIVINGROOM;
                    }
                }
            }
            else if (y >= height * 3 / 5 && y < height - ratio)
            {
                if (x >= ratio && x < width * 5 / 9)
                {
                    if (mode == 1)
                    {
                        peopleInStudy++;
                        return STUDYROOM;
                    }
                    else if (mode == 2)
                    {
                        peopleInStudy--;
                        if (peopleInStudy == 0) return STUDYROOM;
                    }
                }
                else if (x >= width * 5 / 9 && x < width - ratio)
                {
                    if (mode == 1)
                    {
                        peopleInDoor++;
                        return DOOR;
                    }
                    else if (mode == 2)
                    {
                        peopleInDoor--;
                        if (peopleInDoor == 0) return DOOR;
                    }
                }
            }
            return -1;
        }

        private void MakePeople_Click(object sender, EventArgs e)
        {
            if (People.count < People.max)
            {
                person[People.count] = new People(this);
                People.count++;
            }
            else label35.Text = "人數已達上限";
        }

    }

    public class Light
    {
        private bool on = false;
        private System.Windows.Forms.PictureBox picturebox;
        private System.Windows.Forms.Label show_state;
        private System.Windows.Forms.Label room_name;

        public Light(System.Windows.Forms.PictureBox p, System.Windows.Forms.Label l, System.Windows.Forms.Label l2)
        {
            picturebox = p;
            show_state = l;
            room_name = l2;
        }

        public void switch_on()
        {
            picturebox.Image = global::Listener.Properties.Resources.light_on;
            on = true;
            show_state.Text = "ON";
            room_name.BackColor = Color.White;
            if (Form1.tv.is_on()) Form1.tv.switch_on();
            else Form1.tv.switch_off();
            if (Form1.fan.is_on()) Form1.fan.switch_on();
            else Form1.fan.switch_off();
        }

        public void switch_off()
        {
            picturebox.Image = global::Listener.Properties.Resources.light_off;
            on = false;
            show_state.Text = "OFF";
            room_name.BackColor = Color.LightGray;
            if (Form1.tv.is_on()) Form1.tv.switch_on();
            else Form1.tv.switch_off();
            if (Form1.fan.is_on()) Form1.fan.switch_on();
            else Form1.fan.switch_off();
        }

        public bool is_on()
        {
            return on;
        }
    }

    public class TV
    {
        private bool on = false;
        private System.Windows.Forms.PictureBox picturebox;
        private System.Windows.Forms.Label show_state;
        private System.Windows.Forms.Label show_channel;
        private int channel = 50;
        private int pictureno = 1;
        private bool laststate = false;
        private NmsTemplate nmsC;
        private ListBox receiveBox;

        public TV(System.Windows.Forms.PictureBox p, System.Windows.Forms.Label l, System.Windows.Forms.Label l2, NmsTemplate n, ListBox lb)
        {
            picturebox = p;
            show_state = l;
            show_channel = l2;
            nmsC = n;
            receiveBox = lb;
        }

        public void switch_on()
        {
            if (Form1.livingroom_light.is_on())
            {
                if (pictureno == 1) picturebox.Image = global::Listener.Properties.Resources.tv_onL;
                else if (pictureno == 2) picturebox.Image = global::Listener.Properties.Resources.tv_onL2;
            }
            else
            {
                if (pictureno == 1) picturebox.Image = global::Listener.Properties.Resources.tv_onD;
                else if (pictureno == 2) picturebox.Image = global::Listener.Properties.Resources.tv_onD2;
            }
            on = true;
            show_state.Text = "ON";
            show_channel.Text = channel.ToString();
            sendCurrent();
        }

        public void switch_off()
        {
            if (Form1.livingroom_light.is_on()) picturebox.Image = global::Listener.Properties.Resources.tv_offL;
            else picturebox.Image = global::Listener.Properties.Resources.tv_offD;
            on = false;
            show_state.Text = "OFF";
            show_channel.Text = "";
            sendCurrent();
        }

        public bool is_on()
        {
            return on;
        }

        public void channel_prev()
        {
            channel++;
            if (channel >= 100) channel = 1;
            if (pictureno == 1) pictureno = 2;
            else if (pictureno == 2) pictureno = 1;
            switch_on();
        }

        public void channel_next()
        {
            channel--;
            if (channel <= 0) channel = 99;
            if (pictureno == 1) pictureno = 2;
            else if (pictureno == 2) pictureno = 1;
            switch_on();
        }

        public void clean_channel()
        {
            channel = 50;
            pictureno = 1;
        }

        public void sendCurrent()
        {
            StringBuilder sb = new StringBuilder();
            if (this.on && !laststate)
            {
                sb.Append("{\"type\":\"taroko\", \"id\":\"C5_1\", \"Current\":\"700\" }");
                laststate = true;
                try
                {
                    nmsC.ConvertAndSend("ssh.RAW_DATA", sb.ToString());
                }
                catch (System.Net.Sockets.SocketException)
                {
                    receiveBox.Items.Add("電流sensor發生錯誤");
                }
            }
            else if (!this.on && laststate)
            {
                sb.Append("{\"type\":\"taroko\", \"id\":\"C5_1\", \"Current\":\"0\" }");
                laststate = false;
                try
                {
                    nmsC.ConvertAndSend("ssh.RAW_DATA", sb.ToString());
                }
                catch (System.Net.Sockets.SocketException)
                {
                    receiveBox.Items.Add("電流sensor發生錯誤");
                }
            }
            
        }
    }

    public class Fan
    {
        private bool on = false;
        private System.Windows.Forms.PictureBox picturebox;
        private System.Windows.Forms.Label show_state;

        public Fan(System.Windows.Forms.PictureBox p, System.Windows.Forms.Label l)
        {
            picturebox = p;
            show_state = l;
        }

        public void switch_on()
        {
            if (Form1.livingroom_light.is_on()) picturebox.Image = global::Listener.Properties.Resources.fan_startL;
            else picturebox.Image = global::Listener.Properties.Resources.fan_startD;
            on = true;
            show_state.Text = "ON";
        }

        public void switch_off()
        {
            if (Form1.livingroom_light.is_on()) picturebox.Image = global::Listener.Properties.Resources.fan_stopL;
            else picturebox.Image = global::Listener.Properties.Resources.fan_stopD;
            on = false;
            show_state.Text = "OFF";
        }

        public bool is_on()
        {
            return on;
        }
    }

    public class People : System.Windows.Forms.PictureBox
    {
        private Point PersonClicked = new Point();
        private bool TempMove = false;
        public const int max = 5;
        public static int count = 0;
        private Form1 form;
        private bool seated = false;

        //透過滑鼠的click drag和set來計算People instance的位置
        private void PersonClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            TempMove = true;
            PersonClicked.X = e.X;
            PersonClicked.Y = e.Y;
            try
            {
                form.map[form.position(this.Left + this.Width / 2, this.Top + this.Height / 2, 2)].switch_off();
            }
            catch (KeyNotFoundException) { }
        }

        private void PersonDrag(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (TempMove)
            {
                this.Left += e.X - PersonClicked.X;
                this.Top += e.Y - PersonClicked.Y;
            }
        }

        private void PersonSet(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            TempMove = false;
            try
            {
                form.map[form.position(this.Left + this.Width / 2, this.Top + this.Height / 2, 1)].switch_on();
            }
            catch (KeyNotFoundException) { }
            if ((this.Top + this.Height > form.pictureSofa.Top && form.pictureSofa.Top + form.pictureSofa.Height > this.Top) && (this.Left + this.Width > form.pictureSofa.Left && form.pictureSofa.Left + form.pictureSofa.Width > this.Left))
            {
                if (!seated)
                {
                    form.peopleOnSofa++;
                    form.sendPressure();
                    seated = true;
                }
            }
            else if (seated)
            {
                form.peopleOnSofa--;
                form.sendPressure();
                seated = false;
            }
            form.Display_Click("", form.ea);
        }

        public People(Form1 f)
        {
            form = f;
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            f.tabPage1.Controls.Add(this);
            f.tabPage1.Controls.SetChildIndex(this,1);
            this.Image = global::Listener.Properties.Resources.Person;
            this.Location = new System.Drawing.Point(430, 273);
            this.Name = "Person";
            this.Size = new System.Drawing.Size(40, 40);
            this.TabStop = false;
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.MouseDown += new MouseEventHandler(PersonClick);
            this.MouseMove += new MouseEventHandler(PersonDrag);
            this.MouseUp += new MouseEventHandler(PersonSet);
        }
    }

}