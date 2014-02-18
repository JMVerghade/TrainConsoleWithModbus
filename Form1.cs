//--------------------------------------------------------------
// JMV 2014 : Train Console with Modbus communiation
//--------------------------------------------------------------
// This programm is a demonstration program to confirm that
// controling DCC Locomotives with an Arduino UNO or MEGA 
// connected to a PC through the USB port - RS232 and with Modbus 
// RTU protocol is possible.
// This programm is the Modbus master and poll Arduino to get
// informations. When the user change the throttle of a locomotive
// a modbus frame with special function code and datas is send to
// the Arduino. The Arduino answer to the Modbus master and execute
// the DCC command
//--------------------------------------------------------------
// I started this code thanks to the following source code because 
// it was very easy to use, and Modbus TCP sounds nice to me for
// further development of this train console with Modbus
//--------------------------------------------------------------
//  Client Modbus Maitre     B.Vannier www.va2i.com 31 juil 2008
//--------------------------------------------------------------
// Application test MODBUS
// Module associé: GModbus_seul_2008.cs 
//                 TMODBUS_S: gestion des protocoles MODBUS et MODBUS TCP
//--------------------------------------------------------------
// Ce logiciel est totalement libre de droits
//  Néanmoins, merci de m'envoyer un email en cas d'utilisation
//  avec éventuellement vos remarques à bruno.vannier@va2i.com
//--------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;


namespace Cs_Modbus_Maitre
{


    public partial class Form1 : Form
    {
        //--------------------------------------------------------------
        //  déclaration globales de la classe
        //--------------------------------------------------------------
        static Form1 pForm;     // permet l'appel des methodes non statiques à partir de méthodes statiques 
        delegate void SetTextCallback(string text);

        TMODBUS_S Modbus_s = new TMODBUS_S();    // traite les trames Modbus et revoie une trame reponse
        ushort[] TabMots = new ushort[512];
        bool[] TabBits = new bool[1024];

        string[] sTabVit = { "1200", "9600", "19200", "115200" };
        int[] iTabVit = { 1200, 9600, 19200, 115200 };
        string[] sTabPar = { "None", "Odd", "Even" };//{ "Sans", "Impaire", "Paire" };

        const int TBUF = 512;
        static byte[] Buf_emi = new byte[TBUF];  // buffer emission RS232
        static byte[] Buf_rec = new byte[TBUF];  // buffer réception RS232
        static byte[] Buf_temp = new byte[TBUF]; // buffer de réception temporaire
        static int index_buf = 0;               // index du buffer de réception temporaire

        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

       // static System.Windows.Forms.Timer MasterTimer = new System.Windows.Forms.Timer();

        static int compteur;        // pour le décompte du Time out (*10ms)
        static bool bTimeOutInterCar = false;
        
        const int timeOutIC = 1;   // time out inter caractère en *10ms
        const int timeOutTot = 10;  // time out total en *10ms

        const int MASTER_IDLE = 0;
        const int MASTER_SEND = 1;
        const int MASTER_WAIT_ANSWER = 2;
        static int IdleCounter=0; // compteur temps avant de renvoyer un trame
        const int IdleTimeMax = 10; // tems avant de renvoyer une trame
        static int TimeOutCounter = 0; //compteur de temps avant non réponse
        const int TimeOutMax = 100; // 1 seconde
        static int MasterStatus = MASTER_IDLE;

        static int FrameInNb = 0;
        const int LOCO_MAX_SPEED = 127;

        const int LOCO_DIR_FRONT = 0;
        const int LOCO_DIR_REVERSE = 1;

        static bool[] LocoFunctionsArray =new bool[12];
        static bool bUpdateLocoFunction = false;
        static byte UpdateFunctionNb = 0;

        static Locomotive[] Locos= new Locomotive[255];
        static int iActiveLocoIndex = 0;
        static int iLocosNumber = 0;


        static int iStationSlaveAddress = 1;

        LocoFunctionCheckBoxArray FxCheckBoxArray;
        static bool bPause;

        public Form1()
        {
            InitializeComponent();
            Locos[iLocosNumber] = new Locomotive();
            Locos[iLocosNumber].m_iAddress = 3;
            Locos[iLocosNumber].m_sName = "030 Jouef";
            Locos[iLocosNumber].m_sFunctionNameArray[0] = "Lumière";
            Locos[iLocosNumber].m_sFunctionNameArray[1] = "Son on";
            Locos[iLocosNumber].m_sFunctionNameArray[2] = "Sifflet";
            Locos[iLocosNumber].m_sFunctionNameArray[3] = "Sifflet court";
            iLocosNumber++;
            Locos[iLocosNumber] = new Locomotive();
            Locos[iLocosNumber].m_iAddress = 11;
            Locos[iLocosNumber].m_sName = "CC7200";
            iLocosNumber++;
            Locos[iLocosNumber] = new Locomotive();
            Locos[iLocosNumber].m_iAddress = 20;
            Locos[iLocosNumber].m_sName = "Cadeau";
            iLocosNumber++;
            for (int i = 0; i < 10; i++)
            {
                Locos[iLocosNumber] = new Locomotive();
                Locos[iLocosNumber].m_iAddress = iLocosNumber;
                iLocosNumber++;
            }

            comboBox1.SelectedIndex = 0;
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(sTabVit);
            comboBox2.SelectedIndex = 1;
            comboBox3.Items.AddRange(sTabPar);
            comboBox3.SelectedIndex = 0;
            richTextBox1.AppendText("DCC Train Console with Modbus by JMV");
            pForm = this;   // permet l'appel des methodes non statiques à partir de méthodes statiques (TimerEventProcessor)

            FxCheckBoxArray = new LocoFunctionCheckBoxArray(this);
            for (int i = 0; i < 12; i++) { FxCheckBoxArray.AddNewCheckBox(); FxCheckBoxArray[i].Text = "F" + i.ToString(); FxCheckBoxArray[i].Click += new System.EventHandler(ClickHandler);}

            LocoInfo_Update();
        }


        private void LocoInfo_Update()
        {
            txtLocoName.Text = Locos[iActiveLocoIndex].m_sName;
            txtLocoAddress.Text = Locos[iActiveLocoIndex].m_iAddress.ToString();
            slideSpeed.Value = Locos[iActiveLocoIndex].m_iSpeed;
            for (int i = 0; i < FxCheckBoxArray.Count-1; i++) { FxCheckBoxArray[i].Text = "F" + i.ToString() + ":" + Locos[iActiveLocoIndex].m_sFunctionNameArray[i]; }
            
        }
        //--------------------------------------------------------------
        //  Ouverture port et lancement thread réception
        //--------------------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.SelectedItem.ToString();
                serialPort1.BaudRate = iTabVit[comboBox2.SelectedIndex];
                switch (comboBox3.SelectedIndex)
                {
                    case 0: serialPort1.Parity = System.IO.Ports.Parity.None; break;
                    case 1: serialPort1.Parity = System.IO.Ports.Parity.Odd; break;
                    case 2: serialPort1.Parity = System.IO.Ports.Parity.Even; break;
                }

                serialPort1.Open();
                richTextBox1.AppendText("\nPort " + serialPort1.PortName + " Ouvert " + serialPort1.BaudRate.ToString() + "Bds " + serialPort1.Parity.ToString());
                pForm.Text = "MODBUS Maitre: Port " + serialPort1.PortName + " " + serialPort1.BaudRate.ToString() + "Bds " + serialPort1.Parity.ToString();

                myTimer.Tick += new EventHandler(TimerEventProcessor);
                // Sets the timer interval to 10ms.
                myTimer.Interval = 5; // in ms
                myTimer.Start();

                MasterStatus = MASTER_IDLE;
            }
            catch (Exception Er)
            {
                richTextBox1.AppendText("\nErreur ouverture Port" + serialPort1.PortName + " " + Er.Message);
                return; ;
            }
            finally
            {
            }
        }

        //--------------------------------------------------------------
        // Fermeture port
        //--------------------------------------------------------------
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();
                richTextBox1.AppendText("\nPort " + serialPort1.PortName + " Fermé");
                pForm.Text = "Serveur MODBUS: Port " + serialPort1.PortName + " Fermé";
                //MasterTimer.Stop();
            }
            catch (Exception Er)
            {
                richTextBox1.AppendText("\nErreur fermeture Port" + serialPort1.PortName + " " + Er.Message);
                return; ;
            }
            finally
            {
            }
        }

        //--------------------------------------------------------------
        // Gestion affichage
        //--------------------------------------------------------------
        private string ToHexa(byte[] trame, int lg)
        {
            string str = "";
            for (int i = 0; i < lg; i++)
            {
                str = str + trame[i].ToString("X2", null) + " ";
            }
            return (str);
        }
        //--------------------------------------------------------------
        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.richTextBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                string msg = "\nFrame ";
                msg += FrameInNb.ToString();
                msg += ":";
                msg += text;
                this.richTextBox1.AppendText(msg);
                this.richTextBox1.ScrollToCaret();
            }
        }


        //---------------------------------------------------------------------------------------------
        // Gestion de la reception RS232
        //---------------------------------------------------------------------------------------------
        // Les TimeOut sont remis à 0 à la réception d'un caractère (serialPort1_DataReceived)
        // Dans ce cas le message contenu dans Buf_temp est transféré dans Buf_rec et affiché et traité

        //-------------------------------------------------------------
        // Emission de trame
        //-------------------------------------------------------------
        private void Serial_Send(ref byte[] Frame,byte FrameLength)
        {
            pForm.serialPort1.Write(Frame, 0, FrameLength);
            pForm.SetText("Send: " + pForm.ToHexa(Frame, FrameLength));
        }


        //--------------------------------------------------------------
        // Réception d'un caratère
        //--------------------------------------------------------------
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if ((TBUF - index_buf) > 0)
            {
                try
                {
                    int nb = serialPort1.Read(Buf_temp, index_buf, (TBUF - index_buf));
                    index_buf += nb;
                }
                catch
                {
                }
            }
            else
            {
                serialPort1.DiscardInBuffer();  // jette les données hors taille buffer
            }
            compteur = 0;
            bTimeOutInterCar = true;
        }
        //--------------------------------------------------------------
        // Timer: Détection des Time Out
        //--------------------------------------------------------------
        private static void TimerEventProcessor(Object myObject,
                                                EventArgs myEventArgs)
        {
            switch (MasterStatus){
                case MASTER_SEND:
                    try
                    {   
                        if (Locos[iActiveLocoIndex].m_bESD)
                        {
                            int nb = pForm.Modbus_s.trame_writeESD(Locos[iActiveLocoIndex].m_iAddress, iStationSlaveAddress, ref Buf_emi);
                            pForm.Serial_Send(ref Buf_emi, (byte)nb);
                            //Locos[iActiveLocoIndex].m_bESD = false;
                        }
                        else
                        if (Locos[iActiveLocoIndex].m_iSpeed != Locos[iActiveLocoIndex].m_iNewSpeed)
                        {
                            //Locos[iActiveLocoIndex].m_iSpeed = Locos[iActiveLocoIndex].m_iNewSpeed;
                            int nb = pForm.Modbus_s.trame_writeLoco(Locos[iActiveLocoIndex].m_iAddress, Locos[iActiveLocoIndex].m_iDirection, Math.Abs(Locos[iActiveLocoIndex].m_iNewSpeed), iStationSlaveAddress, ref Buf_emi);
                            pForm.Serial_Send(ref Buf_emi, (byte)nb);
                        }
                        else
                            if (bUpdateLocoFunction)
                            {
                                int nb = pForm.Modbus_s.trame_writeLocoFunction(Locos[iActiveLocoIndex].m_iAddress, UpdateFunctionNb, Locos[iActiveLocoIndex].m_FunctionsArray[UpdateFunctionNb], iStationSlaveAddress, ref Buf_emi);
                                pForm.Serial_Send(ref Buf_emi, (byte)nb);
                                bUpdateLocoFunction = false;
                            }
                            else
                            {
                                // Just for test purpose, to read some data, when there is no write request
                                int nb = pForm.Modbus_s.trame_readT4(0, 2, iStationSlaveAddress, ref Buf_emi);
                                pForm.Serial_Send(ref Buf_emi, (byte)nb);
                            }
                                              
                    }

                    catch (Exception Er)
                    {
                        pForm.richTextBox1.AppendText("\nErreur " + Er.Message);
                        return; ;
                    }
                    // Reset time out counter
                    TimeOutCounter = 0;
                    MasterStatus = MASTER_WAIT_ANSWER;
                    break;
                case MASTER_WAIT_ANSWER:
                    compteur ++;
                    if ((compteur > timeOutIC) && index_buf>0) // si 10 => 100 ms de timeout inter caractère
                    {
                        for (int i = 0; i < index_buf; i++)
                        {
                            Buf_rec[i] = Buf_temp[i];
                        }
                        pForm.SetText("Recu: " + pForm.ToHexa(Buf_rec, index_buf));

                        if (index_buf > 0)
                        {
                            int nbemi = pForm.traite_req(ref Buf_emi, ref Buf_rec, index_buf);
                        }
                        FrameInNb++;
                        bTimeOutInterCar = false;
                        index_buf = 0;
                        MasterStatus = MASTER_IDLE;
                    }
                    if ((compteur > timeOutIC) && index_buf == 0) // si 10 => 100 ms de timeout inter caractère
                    {
                        //manage timeout when non response
                        TimeOutCounter++;
                        
                        if (TimeOutCounter > TimeOutMax)
                        {
                            pForm.SetText("Time out : " + TimeOutCounter.ToString());
                            MasterStatus = MASTER_SEND;
                            TimeOutCounter = 0;
                        }
                    }
                    break;

                case MASTER_IDLE:
                    IdleCounter++;
                    if ((IdleCounter > IdleTimeMax) && (!bPause))
                    {
                        IdleCounter = 0;
                        MasterStatus = MASTER_SEND;
                    }
                    if ((Locos[iActiveLocoIndex].m_iSpeed != Locos[iActiveLocoIndex].m_iNewSpeed) || (bUpdateLocoFunction) || (Locos[iActiveLocoIndex].m_bESD))
                    {
                        IdleCounter = 0;
                        MasterStatus = MASTER_SEND;
                    }
                    break;
            }

        }

        //--------------------------------------------------------------
        private int traite_req(ref byte[] buf_emi, ref byte[] buf_rec, int nb)
        {
            int nb2;
            switch (buf_rec[1])
            {
                case 1:
                case 2:
                    nb2 = Modbus_s.extrait_readbit(ref TabBits, ref buf_rec);
                    //affichBits(nb2);
                    break;
                case 3:
                case 4:
                    nb2 = Modbus_s.extrait_readT(ref TabMots, ref buf_rec);
                    //affichMots(nb2);
                    break;
                case 15:
                case 16: nb2 = 0; break;
                case 0x50: Locos[iActiveLocoIndex].m_iSpeed = Locos[iActiveLocoIndex].m_iNewSpeed; nb2 = 0; break;
                case 0x51: nb2 = 0; break;
                case 0x52: Locos[iActiveLocoIndex].m_bESD = false; nb2 = 0;  break;
                default: SetText("\nErreur: trame reçue non reconnue!"); nb2 = 0; break;
            }
            return (nb2);
        }

        //--------------------------------------------------------------
        private void affichMots(int nb)
        {
            string msg = "";

            for (int i = 0; ((i < nb) && (i < 10)); i++)
            {
                msg += TabMots[i].ToString() + " ";
                if ((i & 7) == 3) msg += " ";
                if ((i & 7) == 7) msg += "  ";
            }
            //textValMW.Text = msg;
        }
        //--------------------------------------------------------------
        private void affichBits(int nb)
        {
            string msg = "";
            for (int i = 0; ((i < nb) && (i < 20)); i++)
            {
                if (TabBits[i]) msg += "1 ";
                else msg += "0 ";
                if ((i & 7) == 3) msg += " ";
                if ((i & 7) == 7) msg += "  ";
            }
            //textValBi.Text = msg;
        }
        //--------------------------------------------------------------
        private void b_lec_mots_Click(object sender, EventArgs e)
        {
            try
            {
                int nb = Modbus_s.trame_readT4(1,1,iStationSlaveAddress,ref Buf_emi);
                Serial_Send(ref Buf_emi, (byte)nb);
            }

            catch (Exception Er)
            {
                richTextBox1.AppendText("\nErreur " + Er.Message);
                return; ;
            }
        }

        //--------------------------------------------------------------
        private void b_lec_bits_Click(object sender, EventArgs e)
        {
            try
            {
               /* int nb = Modbus_s.trame_readbit1(System.Convert.ToInt32(textAdLecBi.Text),
                    System.Convert.ToInt32(textNbLecBi.Text),
                   iStationSlaveAddress,
                    ref Buf_emi);
                serialPort1.Write(Buf_emi, 0, nb);*/
            }

            catch (Exception Er)
            {
                richTextBox1.AppendText("\nErreur " + Er.Message);
                return; ;
            }
        }

        //--------------------------------------------------------------
        private void b_ecri_mots_Click(object sender, EventArgs e)
        {
            try
            {
                /*TabMots[0] = System.Convert.ToUInt16(textValEcriMW.Text);
                int nb = Modbus_s.trame_writeT(System.Convert.ToInt32(textAdEcriMW.Text),
                    System.Convert.ToInt32(textNbEcriMW.Text),
                    iStationSlaveAddress,
                    ref TabMots,
                    ref Buf_emi);
                serialPort1.Write(Buf_emi, 0, nb);*/
            }

            catch (Exception Er)
            {
                richTextBox1.AppendText("\nErreur " + Er.Message);
                return; ;
            }
        }

        //--------------------------------------------------------------
        private void b_ecri_bits_Click(object sender, EventArgs e)
        {   /* ------------
            try
            {
                TabBits[0] = System.Convert.ToBoolean(textValEcriBi.Text);
                int nb = Modbus_s.trame_writebit(System.Convert.ToInt32(textAdEcriBi.Text),
                    System.Convert.ToInt32(textNbEcriBi.Text),
                    System.Convert.ToInt32(textNoEsclave.Text),
                    ref TabBits,
                    ref Buf_emi);
                serialPort1.Write(Buf_emi, 0, nb);
            }

            catch (Exception Er)
            {
                richTextBox1.AppendText("\nErreur " + Er.Message);
                return; ;
            }   ------------*/
        }

        private void txtLocoSpeed_TextChanged(object sender, EventArgs e)
        {
            Locos[iActiveLocoIndex].m_iNewSpeed = Convert.ToInt16(txtLocoSpeed.Text);
            if (Locos[iActiveLocoIndex].m_iNewSpeed < 0) { Locos[iActiveLocoIndex].m_iDirection = LOCO_DIR_REVERSE; } else { Locos[iActiveLocoIndex].m_iDirection = LOCO_DIR_FRONT; }
        }

        private void btIncrease_Click(object sender, EventArgs e)
        {
            if (Locos[iActiveLocoIndex].m_iSpeed < LOCO_MAX_SPEED - 5)
            {
                int iSpeed = Locos[iActiveLocoIndex].m_iSpeed + 5;
                slideSpeed.Value = iSpeed;
            }
        }

        private void btDecrease_Click(object sender, EventArgs e)
        {
            if (Locos[iActiveLocoIndex].m_iSpeed > -LOCO_MAX_SPEED + 5)
            {
                int iSpeed = Locos[iActiveLocoIndex].m_iSpeed - 5;
                slideSpeed.Value = iSpeed;
            }
        }

        private void btLocoStop_Click(object sender, EventArgs e)
        {
            if (Locos[iActiveLocoIndex].m_iSpeed < 0) slideSpeed.Value = -1; else slideSpeed.Value = 1;
        }

        private void slideSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (slideSpeed.Value == 0)
            {
                if (Locos[iActiveLocoIndex].m_iSpeed < 0) slideSpeed.Value = -1; else slideSpeed.Value = 1;
            }
            txtLocoSpeed.Text = slideSpeed.Value.ToString();
        }

        private void LocoFunctionUpdate(byte FunctionNb)
        {
            bUpdateLocoFunction = true;
            UpdateFunctionNb = FunctionNb;
        }
        public void ClickHandler(Object sender, System.EventArgs e)
        {
            byte FunctionNb = Convert.ToByte(((System.Windows.Forms.CheckBox)sender).Tag);
            FunctionNb--;
            //System.Windows.Forms.MessageBox.Show("Vous avez cliqué sur le bouton " + FunctionNb.ToString());
            Locos[iActiveLocoIndex].m_FunctionsArray[FunctionNb] = ((System.Windows.Forms.CheckBox)sender).Checked; //!Locos[iActiveLocoIndex].m_FunctionsArray[FunctionNb];
            LocoFunctionUpdate(FunctionNb);
        }

        private void btAdressDec_Click(object sender, EventArgs e)
        {
            if (iActiveLocoIndex > 0)
            {
                iActiveLocoIndex--;
                LocoInfo_Update();
            }
        }

        private void btAdressInc_Click(object sender, EventArgs e)
        {
            if (iActiveLocoIndex < iLocosNumber)
            {
                iActiveLocoIndex++;
                LocoInfo_Update();
            }
        }

        private void btEmergencyStop_Click(object sender, EventArgs e)
        {
            Locos[iActiveLocoIndex].m_bESD = true;
            slideSpeed.Value = 0;
           // Locos[iActiveLocoIndex].m_iNewSpeed = 0;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            bPause = !bPause;
        }


        //--------------------------------------------------------------

    }
}
