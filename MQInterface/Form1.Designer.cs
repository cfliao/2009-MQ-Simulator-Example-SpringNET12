namespace MQInterface
{
    partial class mqForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.mqUrlTextBox = new System.Windows.Forms.ComboBox();
            this.topicCb = new System.Windows.Forms.ComboBox();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.clearMessage = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SensorReader = new System.Windows.Forms.ListBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.inputBox = new System.Windows.Forms.ComboBox();
            this.sendBox = new System.Windows.Forms.ListBox();
            this.changeServerURL = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.URLofServer = new System.Windows.Forms.ComboBox();
            this.destinationBox = new System.Windows.Forms.ComboBox();
            this.clearsendBox = new System.Windows.Forms.Button();
            this.errormsgS = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mqUrlTextBox
            // 
            this.mqUrlTextBox.FormattingEnabled = true;
            this.mqUrlTextBox.Items.AddRange(new object[] {
            "tcp://localhost:61616",
            "tcp://192.168.4.100:61616",
            "failover://localhost:61616/(tcp://localhost:61616,tcp://localhost:61616)"});
            this.mqUrlTextBox.Location = new System.Drawing.Point(96, 501);
            this.mqUrlTextBox.Name = "mqUrlTextBox";
            this.mqUrlTextBox.Size = new System.Drawing.Size(183, 22);
            this.mqUrlTextBox.TabIndex = 61;
            this.mqUrlTextBox.Text = "failover://localhost:61616/(tcp://localhost:61616)";
            // 
            // topicCb
            // 
            this.topicCb.FormattingEnabled = true;
            this.topicCb.Items.AddRange(new object[] {
            "swe.RAW_DATA",
            "swe.COMMAND",
            "swe.OM"});
            this.topicCb.Location = new System.Drawing.Point(96, 529);
            this.topicCb.Name = "topicCb";
            this.topicCb.Size = new System.Drawing.Size(183, 22);
            this.topicCb.TabIndex = 16;
            this.topicCb.Text = "swe.RAW_DATA";
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point(366, 500);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(75, 23);
            this.disconnectButton.TabIndex = 17;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Visible = false;
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // clearMessage
            // 
            this.clearMessage.Location = new System.Drawing.Point(435, 273);
            this.clearMessage.Name = "clearMessage";
            this.clearMessage.Size = new System.Drawing.Size(72, 23);
            this.clearMessage.TabIndex = 15;
            this.clearMessage.Text = "Clear";
            this.clearMessage.UseVisualStyleBackColor = true;
            this.clearMessage.Click += new System.EventHandler(this.clearMessage_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 532);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 14);
            this.label5.TabIndex = 14;
            this.label5.Text = "Sensor Topic";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(285, 500);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 13;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 504);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 14);
            this.label4.TabIndex = 12;
            this.label4.Text = "ActiveMQ URL";
            // 
            // SensorReader
            // 
            this.SensorReader.BackColor = System.Drawing.Color.Black;
            this.SensorReader.ForeColor = System.Drawing.Color.Lime;
            this.SensorReader.FormattingEnabled = true;
            this.SensorReader.ItemHeight = 14;
            this.SensorReader.Location = new System.Drawing.Point(9, 302);
            this.SensorReader.Name = "SensorReader";
            this.SensorReader.Size = new System.Drawing.Size(498, 186);
            this.SensorReader.TabIndex = 0;
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(436, 210);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(65, 22);
            this.sendButton.TabIndex = 17;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 177);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 14);
            this.label1.TabIndex = 14;
            this.label1.Text = "Destination Name:";
            // 
            // inputBox
            // 
            this.inputBox.FormattingEnabled = true;
            this.inputBox.Items.AddRange(new object[] {
            "{\"value\":\"FAN_ON\"}",
            "{\"value\":\"FAN_OFF\"}",
            "{\"value\":\"LAMP_ON\"}",
            "{\"value\":\"LAMP_OFF\"}",
            "",
            "{\"type\":\"taroko\",\"id\":\"1\",\"current\":\"999\"}",
            "{\"type\":\"taroko\",\"id\":\"2\",\"temperature\":\"20\",\"humidity\":\"40\"}"});
            this.inputBox.Location = new System.Drawing.Point(18, 210);
            this.inputBox.Name = "inputBox";
            this.inputBox.Size = new System.Drawing.Size(412, 22);
            this.inputBox.TabIndex = 21;
            // 
            // sendBox
            // 
            this.sendBox.FormattingEnabled = true;
            this.sendBox.HorizontalScrollbar = true;
            this.sendBox.ItemHeight = 14;
            this.sendBox.Location = new System.Drawing.Point(9, 34);
            this.sendBox.Margin = new System.Windows.Forms.Padding(0);
            this.sendBox.Name = "sendBox";
            this.sendBox.Size = new System.Drawing.Size(498, 130);
            this.sendBox.TabIndex = 18;
            // 
            // changeServerURL
            // 
            this.changeServerURL.Location = new System.Drawing.Point(272, 247);
            this.changeServerURL.Name = "changeServerURL";
            this.changeServerURL.Size = new System.Drawing.Size(65, 22);
            this.changeServerURL.TabIndex = 60;
            this.changeServerURL.Text = "Change";
            this.changeServerURL.UseVisualStyleBackColor = true;
            this.changeServerURL.Click += new System.EventHandler(this.changeServerURL_Click);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(15, 247);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 22);
            this.label8.TabIndex = 61;
            this.label8.Text = "Server URL";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(353, 248);
            this.label10.Margin = new System.Windows.Forms.Padding(0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(127, 22);
            this.label10.TabIndex = 62;
            this.label10.Text = "預設為本地端61616埠";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // URLofServer
            // 
            this.URLofServer.FormattingEnabled = true;
            this.URLofServer.Items.AddRange(new object[] {
            "tcp://localhost:61616",
            "tcp://192.168.4.100:61616",
            "tcp://192.168.1.110:61616",
            "failover://localhost:61616/(tcp://localhost:61616,tcp://localhost:61616)"});
            this.URLofServer.Location = new System.Drawing.Point(101, 248);
            this.URLofServer.Name = "URLofServer";
            this.URLofServer.Size = new System.Drawing.Size(157, 22);
            this.URLofServer.TabIndex = 63;
            // 
            // destinationBox
            // 
            this.destinationBox.FormattingEnabled = true;
            this.destinationBox.Items.AddRange(new object[] {
            "swe.RAW_DATA",
            "swe.COMMAND",
            "swe.OM"});
            this.destinationBox.Location = new System.Drawing.Point(128, 174);
            this.destinationBox.Name = "destinationBox";
            this.destinationBox.Size = new System.Drawing.Size(373, 22);
            this.destinationBox.TabIndex = 65;
            this.destinationBox.Text = "swe.RAW_DATA";
            // 
            // clearsendBox
            // 
            this.clearsendBox.Location = new System.Drawing.Point(442, 9);
            this.clearsendBox.Name = "clearsendBox";
            this.clearsendBox.Size = new System.Drawing.Size(65, 22);
            this.clearsendBox.TabIndex = 66;
            this.clearsendBox.Text = "Clear";
            this.clearsendBox.UseVisualStyleBackColor = true;
            this.clearsendBox.Click += new System.EventHandler(this.clearsendBox_Click);
            // 
            // errormsgS
            // 
            this.errormsgS.AutoSize = true;
            this.errormsgS.Location = new System.Drawing.Point(15, 277);
            this.errormsgS.Name = "errormsgS";
            this.errormsgS.Size = new System.Drawing.Size(0, 14);
            this.errormsgS.TabIndex = 67;
            // 
            // mqForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 558);
            this.Controls.Add(this.errormsgS);
            this.Controls.Add(this.clearsendBox);
            this.Controls.Add(this.mqUrlTextBox);
            this.Controls.Add(this.destinationBox);
            this.Controls.Add(this.URLofServer);
            this.Controls.Add(this.SensorReader);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.clearMessage);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.topicCb);
            this.Controls.Add(this.changeServerURL);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.sendBox);
            this.Controls.Add(this.inputBox);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.connectButton);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "mqForm";
            this.Text = "MQ 訊息模擬器";
            this.Load += new System.EventHandler(this.mqForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox topicCb;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button clearMessage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox SensorReader;
        private System.Windows.Forms.ComboBox mqUrlTextBox;
        private System.Windows.Forms.Button clearsendBox;
        private System.Windows.Forms.ComboBox destinationBox;
        private System.Windows.Forms.ComboBox URLofServer;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button changeServerURL;
        private System.Windows.Forms.ListBox sendBox;
        private System.Windows.Forms.ComboBox inputBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Label errormsgS;

    }
}

