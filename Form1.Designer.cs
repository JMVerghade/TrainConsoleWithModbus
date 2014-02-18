namespace Cs_Modbus_Maitre
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.B_open = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.b_close = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.textNoEsclave = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btDecrease = new System.Windows.Forms.Button();
            this.btIncrease = new System.Windows.Forms.Button();
            this.slideSpeed = new System.Windows.Forms.TrackBar();
            this.txtLocoSpeed = new System.Windows.Forms.TextBox();
            this.btLocoStop = new System.Windows.Forms.Button();
            this.btEmergencyStop = new System.Windows.Forms.Button();
            this.txtLocoAddress = new System.Windows.Forms.TextBox();
            this.btAdressInc = new System.Windows.Forms.Button();
            this.btAdressDec = new System.Windows.Forms.Button();
            this.txtLocoName = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.slideSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // B_open
            // 
            this.B_open.Location = new System.Drawing.Point(1, 2);
            this.B_open.Name = "B_open";
            this.B_open.Size = new System.Drawing.Size(118, 19);
            this.B_open.TabIndex = 0;
            this.B_open.Text = "Open COM";
            this.B_open.UseVisualStyleBackColor = true;
            this.B_open.Click += new System.EventHandler(this.button1_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(1, 449);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.richTextBox1.Size = new System.Drawing.Size(330, 161);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // b_close
            // 
            this.b_close.Location = new System.Drawing.Point(317, 2);
            this.b_close.Name = "b_close";
            this.b_close.Size = new System.Drawing.Size(92, 19);
            this.b_close.TabIndex = 1;
            this.b_close.Text = "Close COM";
            this.b_close.UseVisualStyleBackColor = true;
            this.b_close.Click += new System.EventHandler(this.button2_Click);
            // 
            // serialPort1
            // 
            this.serialPort1.PortName = "COM4";
            this.serialPort1.ReadTimeout = 1000;
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "COM18",
            "COM17",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6"});
            this.comboBox1.Location = new System.Drawing.Point(125, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(58, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "1200",
            "9600",
            "19200",
            "115200"});
            this.comboBox2.Location = new System.Drawing.Point(189, 2);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(57, 21);
            this.comboBox2.TabIndex = 6;
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(252, 2);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(59, 21);
            this.comboBox3.TabIndex = 7;
            // 
            // textNoEsclave
            // 
            this.textNoEsclave.Location = new System.Drawing.Point(85, 24);
            this.textNoEsclave.Name = "textNoEsclave";
            this.textNoEsclave.Size = new System.Drawing.Size(21, 20);
            this.textNoEsclave.TabIndex = 27;
            this.textNoEsclave.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Slave address";
            // 
            // btDecrease
            // 
            this.btDecrease.Location = new System.Drawing.Point(10, 156);
            this.btDecrease.Name = "btDecrease";
            this.btDecrease.Size = new System.Drawing.Size(57, 32);
            this.btDecrease.TabIndex = 29;
            this.btDecrease.Text = "<<";
            this.btDecrease.UseVisualStyleBackColor = true;
            this.btDecrease.Click += new System.EventHandler(this.btDecrease_Click);
            // 
            // btIncrease
            // 
            this.btIncrease.Location = new System.Drawing.Point(203, 156);
            this.btIncrease.Name = "btIncrease";
            this.btIncrease.Size = new System.Drawing.Size(58, 32);
            this.btIncrease.TabIndex = 30;
            this.btIncrease.Text = ">>";
            this.btIncrease.UseVisualStyleBackColor = true;
            this.btIncrease.Click += new System.EventHandler(this.btIncrease_Click);
            // 
            // slideSpeed
            // 
            this.slideSpeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.slideSpeed.Location = new System.Drawing.Point(10, 194);
            this.slideSpeed.Maximum = 127;
            this.slideSpeed.Minimum = -127;
            this.slideSpeed.Name = "slideSpeed";
            this.slideSpeed.Size = new System.Drawing.Size(251, 45);
            this.slideSpeed.TabIndex = 32;
            this.slideSpeed.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.slideSpeed.ValueChanged += new System.EventHandler(this.slideSpeed_ValueChanged);
            // 
            // txtLocoSpeed
            // 
            this.txtLocoSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLocoSpeed.Location = new System.Drawing.Point(85, 156);
            this.txtLocoSpeed.Name = "txtLocoSpeed";
            this.txtLocoSpeed.Size = new System.Drawing.Size(100, 38);
            this.txtLocoSpeed.TabIndex = 33;
            this.txtLocoSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtLocoSpeed.TextChanged += new System.EventHandler(this.txtLocoSpeed_TextChanged);
            // 
            // btLocoStop
            // 
            this.btLocoStop.Image = ((System.Drawing.Image)(resources.GetObject("btLocoStop.Image")));
            this.btLocoStop.Location = new System.Drawing.Point(203, 97);
            this.btLocoStop.Name = "btLocoStop";
            this.btLocoStop.Size = new System.Drawing.Size(58, 58);
            this.btLocoStop.TabIndex = 34;
            this.btLocoStop.Text = "Stop";
            this.btLocoStop.UseVisualStyleBackColor = true;
            this.btLocoStop.Click += new System.EventHandler(this.btLocoStop_Click);
            // 
            // btEmergencyStop
            // 
            this.btEmergencyStop.Image = ((System.Drawing.Image)(resources.GetObject("btEmergencyStop.Image")));
            this.btEmergencyStop.Location = new System.Drawing.Point(9, 97);
            this.btEmergencyStop.Name = "btEmergencyStop";
            this.btEmergencyStop.Size = new System.Drawing.Size(58, 58);
            this.btEmergencyStop.TabIndex = 40;
            this.btEmergencyStop.UseVisualStyleBackColor = true;
            this.btEmergencyStop.Click += new System.EventHandler(this.btEmergencyStop_Click);
            // 
            // txtLocoAddress
            // 
            this.txtLocoAddress.Font = new System.Drawing.Font("Swis721 BlkOul BT", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLocoAddress.Location = new System.Drawing.Point(102, 102);
            this.txtLocoAddress.Name = "txtLocoAddress";
            this.txtLocoAddress.Size = new System.Drawing.Size(69, 40);
            this.txtLocoAddress.TabIndex = 41;
            this.txtLocoAddress.Text = "3";
            this.txtLocoAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btAdressInc
            // 
            this.btAdressInc.Location = new System.Drawing.Point(176, 103);
            this.btAdressInc.Name = "btAdressInc";
            this.btAdressInc.Size = new System.Drawing.Size(24, 38);
            this.btAdressInc.TabIndex = 42;
            this.btAdressInc.Text = ">";
            this.btAdressInc.UseVisualStyleBackColor = true;
            this.btAdressInc.Click += new System.EventHandler(this.btAdressInc_Click);
            // 
            // btAdressDec
            // 
            this.btAdressDec.Location = new System.Drawing.Point(72, 102);
            this.btAdressDec.Name = "btAdressDec";
            this.btAdressDec.Size = new System.Drawing.Size(24, 38);
            this.btAdressDec.TabIndex = 43;
            this.btAdressDec.Text = "<";
            this.btAdressDec.UseVisualStyleBackColor = true;
            this.btAdressDec.Click += new System.EventHandler(this.btAdressDec_Click);
            // 
            // txtLocoName
            // 
            this.txtLocoName.Font = new System.Drawing.Font("Swis721 BlkOul BT", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLocoName.Location = new System.Drawing.Point(12, 56);
            this.txtLocoName.Name = "txtLocoName";
            this.txtLocoName.Size = new System.Drawing.Size(248, 40);
            this.txtLocoName.TabIndex = 44;
            this.txtLocoName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(337, 449);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 30);
            this.button1.TabIndex = 45;
            this.button1.Text = "Stop refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 622);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtLocoName);
            this.Controls.Add(this.btAdressDec);
            this.Controls.Add(this.btAdressInc);
            this.Controls.Add(this.txtLocoAddress);
            this.Controls.Add(this.btEmergencyStop);
            this.Controls.Add(this.btLocoStop);
            this.Controls.Add(this.txtLocoSpeed);
            this.Controls.Add(this.slideSpeed);
            this.Controls.Add(this.btIncrease);
            this.Controls.Add(this.btDecrease);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textNoEsclave);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.b_close);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.B_open);
            this.Name = "Form1";
            this.Text = "Train Console with Modbus";
            ((System.ComponentModel.ISupportInitialize)(this.slideSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_open;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button b_close;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.TextBox textNoEsclave;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btDecrease;
        private System.Windows.Forms.Button btIncrease;
        private System.Windows.Forms.TrackBar slideSpeed;
        private System.Windows.Forms.TextBox txtLocoSpeed;
        private System.Windows.Forms.Button btLocoStop;
        private System.Windows.Forms.Button btEmergencyStop;
        private System.Windows.Forms.TextBox txtLocoAddress;
        private System.Windows.Forms.Button btAdressInc;
        private System.Windows.Forms.Button btAdressDec;
        private System.Windows.Forms.TextBox txtLocoName;
        private System.Windows.Forms.Button button1;
    }
}

