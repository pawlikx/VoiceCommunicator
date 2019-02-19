using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ozeki.Media;
using Ozeki.VoIP;
using System.Security.Permissions;
using System.IO;
using System.Net.Sockets;
using System.Data.SqlClient;
using System.Diagnostics;

namespace PineVoice
{
    public partial class Home : Form
    {
       /* private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }*/

        static public string globalIPStarts = "25.";
        static ISoftPhone softphone;   // softphone object
        static IPhoneLine phoneLine;   // phoneline object
        static IPhoneCall call;
        static string caller;
        static Microphone microphone;
        static Speaker speaker;
        static PhoneCallAudioSender mediaSender;
        static PhoneCallAudioReceiver mediaReceiver;
        static MediaConnector connector;
        private Thread Caller;
        public int counter = 0;
        //Ogolne
        //static string local_ip = null;
        static string local_ip;// = "192.168.1.65";
        static string destination_ip;
        static string destination_user = "mietek";
        //db
        SqlConnection connection = new SqlConnection(@"Data source=den1.mssql3.gear.host;
                                                             database=tip1;
                                                             User id=tip1;
                                                             Password=On1Yt!RAN7-0;");
        //Server 
        private TcpListener _server;
        private Boolean _isRunning;
        //private IPAddress ipAd = IPAddress.Parse(GetLocalIPAddress());
        private IPAddress ipAd;// = IPAddress.Parse("192.168.1.65");
        private int port = 8003;
        public int myID;
        private static int port2 = 8003;
        private static Boolean busy { get; set; }

        //Klient 
        private static TcpClient _client;
        private static StreamReader _sReader;
        private static StreamWriter _sWriter;
        private static Boolean _isConnected;
        public static string data;
        public static String sDataIncomming;

        public static int globalConnected;
        public static int odbiorcaDostajeBYE;
        public static int odbiorcaTCP;
        public static int nadawcaDostajeBYE;
        public static int lastCallMaker;

        private Talk t { get; set; }
        public bool stopCall;


        bool bChanging = false;

        private void refreshAll()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            allContactsBox.Items.Clear();
            foreach (IPInfo ip in IPInfo.GetIPInfo())
            {
                //localIP = ip.IpAddress.ToString();
                Console.WriteLine(ip.IPAddress.ToString());
                if (ip.IPAddress.StartsWith(globalIPStarts)) // 192. !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                {
                    SqlCommand command1 = connection.CreateCommand();
                    //listBox2.Items.Add(ip.IPAddress);
                    command1.CommandText = "SELECT name FROM users WHERE ip=@0 AND online='online'";
                    command1.Parameters.AddWithValue("@0", ip.IPAddress);
                    string login = "";
                    try
                    {
                        login = command1.ExecuteScalar().ToString();
                    }
                    catch (Exception ex)
                    {

                    }
                    if (login != "")
                    {
                        allContactsBox.Items.Add(login);
                    }
                }
            }
            connection.Close();
        }

        private void refreshFav()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            favContactsBox.Items.Clear();
            foreach (IPInfo ip in IPInfo.GetIPInfo())
            {
                //localIP = ip.IpAddress.ToString();
                Console.WriteLine(ip.IPAddress.ToString());
                if (ip.IPAddress.StartsWith(globalIPStarts)) // 192. !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                {
                    SqlCommand command1 = connection.CreateCommand();
                    //listBox2.Items.Add(ip.IPAddress);
                    command1.CommandText = "SELECT name FROM users,friends WHERE ip=@0 AND online='online'";
                    command1.Parameters.AddWithValue("@0", ip.IPAddress);
                    string login = "";
                    try
                    {
                        login = command1.ExecuteScalar().ToString();
                    }
                    catch (Exception ex)
                    {

                    }
                    if (login != "")
                    {
                        favContactsBox.Items.Add(login);
                    }
                }
            }
            connection.Close();
        }

        public Home(int uID)
        {
            myID = uID;
            InitializeComponent();
            this.CenterToScreen();
            stopCall = false;
            OzekiInit();
            // POBRANIE ADRESU IP
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                    if (localIP.StartsWith(globalIPStarts)) // globalIPStarts !!!!!!!!!!!!
                    {
                        local_ip = localIP;
                        ipAd = IPAddress.Parse(localIP.ToString());

                        busy = false;
                        _server = new TcpListener(ipAd, port);
                        _server.Start();
                        _isRunning = true;
                        Thread t = new Thread(LoopClients);
                        t.Start();

                        //local_ip = listBox1.SelectedItem.ToString();
                        // listBox1.SelectedItem.ToString();
                        //OzekiInit(local_ip);
                        //phoneLineInit(local_ip);
                        //phoneLineInit(local_ip);

                        //TODO: WYWALA PRZEZ OZEKI BYC MOZE PRZEZ TO ZE W WATKU
                        // TRZEBA SPRAWDZIC CZY NIE PRZEZ WATEK ZADZIALA
                        Caller = new Thread(Ozeki);
                        Caller.Start();

                        //MessageBox.Show("Rejestracja pomyślna", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
            }
            //#########################

            globalConnected = -1;
            odbiorcaDostajeBYE = 0;
            odbiorcaTCP = 0;
            nadawcaDostajeBYE = 0;
            lastCallMaker = -1;
            refreshAll();
            refreshFav();
        }


        private void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand updateIP = connection.CreateCommand();
            updateIP.CommandText = "UPDATE users SET online='offline' WHERE userID = @0";
            updateIP.Parameters.AddWithValue("@0", myID);
            updateIP.ExecuteNonQuery();
            HangUp();
            connection.Close();
            Application.Exit();
            Process.GetCurrentProcess().Kill();
        }

        #region Ozeki

        static void OzekiInit()
        {
            //softphone = SoftPhoneFactory.CreateSoftPhone(6000, 6200);
            softphone = SoftPhoneFactory.CreateSoftPhone(5000, 10000);
            //Console.WriteLine("OZEKI: CreatedSoftPhone");
            microphone = Microphone.GetDefaultDevice();
            speaker = Speaker.GetDefaultDevice();
            mediaSender = new PhoneCallAudioSender();
            mediaReceiver = new PhoneCallAudioReceiver();
            connector = new MediaConnector();
        }
        void Ozeki()
        {
            Console.WriteLine("@@@@@@@@@@@@@@ OZEKI: counter = " + counter);
            var config = new DirectIPPhoneLineConfig(local_ip, 5060 + counter);
            phoneLine = softphone.CreateDirectIPPhoneLine(config);
            //Console.WriteLine("OZEKI: IPPhoneLine Created");
            phoneLine.RegistrationStateChanged += line_RegStateChanged;
            //Console.WriteLine("OZEKI: Po StateChanged, przed IncomingCall");
            softphone.IncomingCall += softphone_IncomingCall;
            //Console.WriteLine("OZEKI: Po IncomingCall");
            softphone.RegisterPhoneLine(phoneLine);
            //Console.WriteLine("OZEKI: po registerPhoneLine");
            counter++;

            /*
            try
            {
                while (!stopCall)
                {
                    
                }
            }
            catch (ThreadInterruptedException exception)
            {
                // Clean up. 
            }
           */
        }

        private static void line_RegStateChanged(object sender, RegistrationStateChangedArgs e)
        {
            if (e.State == RegState.NotRegistered || e.State == RegState.Error)
                Console.WriteLine("Rejestracja nieudana!");

            if (e.State == RegState.RegistrationSucceeded)
            {
                Console.WriteLine("Rejestracja pomyslna!");
                //Console.WriteLine("Podaj adres IP osoby z ktora chcesz sie polaczyc: ");

                /*string ipToDial = Console.ReadLine();
                StartCall(ipToDial);*/
            }
        }

        private void StartCall(string numberToDial)
        {
            //phoneLineInit(myNumber);
            Caller = new Thread(Ozeki);
            Caller.Start();
            Thread.Sleep(1000);
            if (call == null)
            {
                call = softphone.CreateDirectIPCallObject(phoneLine, new DirectIPDialParameters("5060"), numberToDial);
                call.CallStateChanged += call_CallStateChanged;

                call.Start();
            }
        }

        static void softphone_IncomingCall(object sender, VoIPEventArgs<IPhoneCall> e)
        {
            call = e.Item;
            caller = call.DialInfo.CallerID;
            call.CallStateChanged += call_CallStateChanged;
            call.Answer();
        }

        static void call_CallStateChanged(object sender, CallStateChangedArgs e)
        {
            Console.WriteLine("Call state: {0}.", e.State);

            if (e.State == CallState.Completed)
                MessageBox.Show("Zakończono rozmowę");

            if (e.State == CallState.Answered)
                SetupDevices();

            if (e.State.IsCallEnded())
                CloseDevices();
        }
        static int x = 0;
        static void SetupDevices()
        {
            if (x > 0)
            {
                OzekiInit();
                Console.WriteLine("setupDevices With OZEKI ####################################");
            }


            x++;
            microphone.Start();
            connector.Connect(microphone, mediaSender);

            speaker.Start();
            connector.Connect(mediaReceiver, speaker);

            mediaSender.AttachToCall(call);
            mediaReceiver.AttachToCall(call);
        }

        static void CloseDevices()
        {
            Console.WriteLine("CloseDevices()");
            //phoneLine.Dispose(); // nowo dodane

            microphone.Dispose();
            speaker.Dispose();

            mediaReceiver.Detach();
            mediaSender.Detach();
            connector.Dispose();
        }

        public static void HangUp()
        {
            if (call != null)
            {

                call.HangUp();
                call.PhoneLine.Dispose();
                call.CallStateChanged += call_CallStateChanged;
                call = null;
            }
            else
            {
                Console.WriteLine("call = null");
            }
        }


        //Połączenie
        private void TalkWindow(string login)
        {
            t = new Talk();
            t.Invoke(new MethodInvoker(delegate
            {
                t.user = login;
                t.ShowDialog();
                checker();
            }));

        }
        private void checker()
        {
            if (t.OnCall == false)
            {
                Console.WriteLine("Checker: Wykryta zmiana OnCall==false");
                if (_client != null)
                {
                    Console.WriteLine("_client != null ");
                    //string answer = simpleSend("BYE");
                    //send("BYE");

                    //Status.Invoke(new MethodInvoker(delegate { Status.Text = "Rozłączono"; }));
                    Console.WriteLine("Checker: after send BYE");


                    connect(); //tutaj connect zbedne bo juz polaczone

                    string answer = send("BYE");
                    if (answer == "OK")
                    {
                        Console.WriteLine("Checker: Otrzymano OK");

                        //sWriter.WriteLine("OK");
                        //sWriter.Flush();
                        //if (odbiorcaTCP == 1)
                        if (lastCallMaker == 1)
                            HangUp();
                        busy = false;
                        globalConnected = 0;
                        Console.WriteLine("globalConnected = 0");
                        //bClientConnected = false;
                        close();

                        Console.WriteLine("Okno rozmowy ZAMKNIETE");
                    }

                }
                else
                {
                    Console.WriteLine("_client == null ");
                    /*  CHUJ WIE CZEMU TO   */
                    connect();

                    Console.WriteLine("Checker: after send BYE");
                   
                    string answer = send("BYE");
                    if (answer == "OK")
                    {
                        Console.WriteLine("Checker: Otrzymano OK");

                        //sWriter.WriteLine("OK");
                        //sWriter.Flush();
                        if (lastCallMaker == 1)
                            HangUp();
                        busy = false;
                        //bClientConnected = false;
                        globalConnected = 0;
                        Console.WriteLine("globalConnected = 0");
                        close();

                        Console.WriteLine("Okno rozmowy ZAMKNIETE");

                    }

                }
            }
        }
        //Serwer
        public void LoopClients()
        {
            while (_isRunning)
            {
                // wait for client connection
                TcpClient newClient = _server.AcceptTcpClient();
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
            }
        }
        public void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;

            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            // you could use the NetworkStream to read and write, 
            // but there is no forcing flush, even when requested

            Boolean bClientConnected = true;
            globalConnected = 1;
            Console.WriteLine("globalConnected = 1");
            String sData = null;
            Console.WriteLine("Przed WHILE");

            while (bClientConnected)
            {

                Console.WriteLine("Wewnatrz WHILE");
                Thread.Sleep(100);
                //Console.WriteLine("Wewnatrz WHILE2");

                // reads from stream
                if (globalConnected == 1)
                {
                    Console.WriteLine("Wewnatrz WHILE(CZYTAM KOLEJNY KOMUNIKAT)");
                    sData = sReader.ReadLine();
                }
                else
                {
                    sData = null;
                    Console.WriteLine("Wewnatrz WHILE(gdy globalConnected == false)");
                }


                if (sData == "INV")
                {
                    IPEndPoint remoteIpEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                    destination_ip = remoteIpEndPoint.Address.ToString();
                    //destination_user = Program.Friends.Where(p => p.ip == destination_ip).Select(p => p.login).FirstOrDefault();
                    
                    try
                    {
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                    }
                    catch (Exception ex) { }
                    SqlCommand command2 = connection.CreateCommand();
                    command2.CommandText = "SELECT name FROM users WHERE ip = @0 AND online='online'";
                    command2.Parameters.AddWithValue("@0", destination_ip);
                    string login = command2.ExecuteScalar().ToString();

                    if (busy == false)
                    {
                        //ring.Play();
                        DialogResult dialogResult = MessageBox.Show("Czy chcesz odebrać połączenie od: " + login + "?", "Połączenie", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            //ring.Stop();
                            //connect();
                            busy = true;
                            sWriter.WriteLine("ACK");
                            sWriter.Flush();
                            odbiorcaTCP = 1;

                            if(lastCallMaker == -1 || lastCallMaker == 1)
                            {
                                StartCall(destination_ip);
                                lastCallMaker = 1;
                            }
                                

                            Console.WriteLine("Okno rozmowy OTWARTE");
                            TalkWindow(login);
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            //ring.Stop();
                            sWriter.WriteLine("REF");
                            sWriter.Flush();
                            bClientConnected = false;
                            MessageBox.Show("Polaczenie od: " + login + " odrzucone");
                        }
                    }
                    if (busy == true)
                    {
                        sWriter.WriteLine("BUS");
                        sWriter.Flush();
                        bClientConnected = false;
                    }

                }

                else if (sData == "BYE")
                {
                    Console.WriteLine("HandleClient: Otrzymano BYE");

                    sWriter.WriteLine("OK");
                    sWriter.Flush();
                    if (odbiorcaTCP == 1)
                    {
                        //HangUp();
                        if (lastCallMaker == 1)
                        {
                            HangUp();
                        }
                        odbiorcaDostajeBYE = 1;
                    }

                    else if (odbiorcaTCP == 0)
                    {
                        if (lastCallMaker == 1)
                        {
                            HangUp();
                        }
                        nadawcaDostajeBYE = 1;
                    }
                       
                    busy = false;
                    bClientConnected = false;
                    //HangUp();
                    //close();
                    _isConnected = false;
                    Console.WriteLine("_isConnected == false");

                    /* PRZYGOTOWANIE DO KOLEJNEGO POLACZENIA */
                    CloseDevices();
                    //Console.WriteLine("HandleClient: closeDevices");

                    //Caller.Interrupt();
                    //Caller.Abort();

                    stopCall = true;

                    //Console.WriteLine("HandleClient: Caller Abort");

                    //Thread.Sleep(100);
                    stopCall = false;
                    //Caller = new Thread(Ozeki);
                    //Caller.Start();
                    //Console.WriteLine("HandleClient: Caller new Start");
                    busy = false;
                    //globalConnected = 0;
                    /* ------------------------------------- */

                    Console.WriteLine("Okno rozmowy ZAMKNIETE");
                    t.Invoke(new MethodInvoker(delegate
                    {
                        t.Hide();
                    }));
                }
                if (globalConnected == 0)
                //if (_isConnected == false)
                {
                    Console.WriteLine("HandleClient: Otrzymano OK ZAMYKAM POLACZENIE");
                    sWriter.Flush();
                    //busy = false;
                    bClientConnected = false;
                    //HangUp();

                    /* PRZYGOTOWANIE DO KOLEJNEGO POLACZENIA */
                    CloseDevices();
                    //Console.WriteLine("HandleClient: closeDevices");
                    //Caller.Interrupt();
                    //Caller.Abort();
                    stopCall = true;

                    //Console.WriteLine("HandleClient: Caller Abort");

                    //Thread.Sleep(100);
                    stopCall = false;
                    //Caller = new Thread(Ozeki);
                    //Caller.Start();
                    //Console.WriteLine("HandleClient: Caller new Start");
                    //busy = false;
                    /* ------------------------------------- */
                    //close();
                }
                if (odbiorcaDostajeBYE == 1)
                {
                    if (lastCallMaker == 1)
                    {
                        //HangUp();
                        Caller.Interrupt();
                        Caller.Abort();
                    }
                    //Caller.Interrupt();
                    //Caller.Abort();
                    odbiorcaTCP = -1;
                    globalConnected = 0;
                    odbiorcaDostajeBYE = 0;
                }
                if (nadawcaDostajeBYE == 1)
                {
                    if (lastCallMaker == 1)
                    {
                        //HangUp();
                        Caller.Interrupt();
                        Caller.Abort();
                    }
                    odbiorcaTCP = -1;
                    nadawcaDostajeBYE = 0;
                }
            }
        }
        //Klient
        public static Boolean connect()
        {
            try
            {
                IPAddress ip = IPAddress.Parse(destination_ip);
                _client = new TcpClient();
                _client.Connect(ip, port2);
                HandleCommunication();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static void HandleCommunication()
        {
            _sReader = new StreamReader(_client.GetStream(), Encoding.ASCII);
            _sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);

            _isConnected = true;
            Console.WriteLine("_isConnected == true");

        }

        public static string simpleSend(string text)
        {
            //_sReader = new StreamReader(_client.GetStream(), Encoding.ASCII);
            _sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);

            if (_isConnected)
            {
                // write data and make sure to flush, or the buffer will continue to 
                // grow, and your data might not be sent when you want it, and will
                // only be sent once the buffer is filled.
                _sWriter.WriteLine(text);
                _sWriter.Flush();

                //sDataIncomming = _sReader.ReadLine();
                return "true";
            }

            return "false";
        }
        public static string send(string text)
        {
            _sReader = new StreamReader(_client.GetStream(), Encoding.ASCII);
            _sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);

            if (_isConnected)
            {
                // write data and make sure to flush, or the buffer will continue to 
                // grow, and your data might not be sent when you want it, and will
                // only be sent once the buffer is filled.
                _sWriter.WriteLine(text);
                _sWriter.Flush();

                sDataIncomming = _sReader.ReadLine();
                return sDataIncomming;
            }

            return "false";
        }
        public static void close()
        {
            //send("BYE");
            _isConnected = false;
            Console.WriteLine("_isConnected == false");
            _sReader.Close();
            _sWriter.Close();
            _client.Client.Close();
            _client.Close();
        }

        #endregion
        private void button1_Click(object sender, EventArgs e)
        {
            //Console.WriteLine("Please enter the IP address of your machine: ");
            //var ipAddress = Console.ReadLine();
            if (favContactsBox.SelectedIndex == -1)
            {
                //Console.WriteLine("Podaj adres IP");
                MessageBox.Show("Wybierz swój adres IP do rejestracji", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                local_ip = favContactsBox.SelectedItem.ToString();
                ipAd = IPAddress.Parse(favContactsBox.SelectedItem.ToString());

                busy = false;
                _server = new TcpListener(ipAd, port);
                _server.Start();
                _isRunning = true;
                Thread t = new Thread(LoopClients);
                t.Start();

                //local_ip = listBox1.SelectedItem.ToString();
                // listBox1.SelectedItem.ToString();
                //OzekiInit(local_ip);
                //phoneLineInit(local_ip);
                //phoneLineInit(local_ip);

                //TODO: WYWALA PRZEZ OZEKI BYC MOZE PRZEZ TO ZE W WATKU
                // TRZEBA SPRAWDZIC CZY NIE PRZEZ WATEK ZADZIALA
                Caller = new Thread(Ozeki);
                Caller.Start();

                MessageBox.Show("Rejestracja pomyślna", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (allContactsBox.SelectedIndex == -1 && favContactsBox.SelectedIndex == -1)
            {
                //Console.WriteLine("Podaj adres IP");
                MessageBox.Show("Wybierz do kogo chcesz zadzwonić", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //destination_ip
                string login = "";
                try
                {
                    login = allContactsBox.SelectedItem.ToString();
                } catch (Exception ex) { }
                try
                {
                    login = favContactsBox.SelectedItem.ToString();
                }
                catch (Exception ex) { }
                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                } catch(Exception ex) { }
                SqlCommand command2 = connection.CreateCommand();
                command2.CommandText = "SELECT ip FROM users WHERE name = @0 and online='online'";
                command2.Parameters.AddWithValue("@0", login);
                try
                {
                    destination_ip = command2.ExecuteScalar().ToString();
                } catch(Exception ex) { }
                connect();

                string answer = send("INV");
                //string answer = "ACK";
                if (answer == "ACK")
                {
                    //destination_user = Program.Friends.Where(p => p.ip == destination_ip).Select(p => p.login).FirstOrDefault();
                    odbiorcaTCP = 0;
                    if (lastCallMaker == 1)
                    {
                        StartCall(destination_ip);
                    }
                    else 
                        lastCallMaker = 0;
                    busy = true;
                    Console.WriteLine("Okno rozmowy OTWARTE");
                    TalkWindow(login);

                }
                if (answer == "REF")
                {
                    //destination_user = Program.Friends.Where(p => p.ip == destination_ip).Select(p => p.login).FirstOrDefault();
                    MessageBox.Show("Polaczenie od: " + login + " zostało odrzucone.");
                    //MessageBox.Show("REFUSE");
                }
                if (answer == "BUS")
                {
                    //destination_user = Program.Friends.Where(p => p.ip == destination_ip).Select(p => p.login).FirstOrDefault();
                    MessageBox.Show("Użytkownik jest zajęty.");

                }


                //string ipMy = listBox1.SelectedItem.ToString();
                //string ipToDial = listBox2.SelectedItem.ToString();
                //MessageBox.Show(ipToDial, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //StartCall(ipToDial);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            favContactsBox.Items.Clear();
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                    favContactsBox.Items.Add(localIP);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //HangUp();
            CloseDevices();
            Caller.Abort();
            Caller = new Thread(Ozeki);
            Caller.Start();
            busy = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            refreshAll();
            refreshFav();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            connect();

            string answer = send("BYE");
            if (answer == "OK")
                Console.WriteLine("OD PRZYCISKU OTRZYMANO BYE OK");
            //simpleSend("BYE");
        }

        public void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bChanging) return;
            bChanging = true;
            favContactsBox.ClearSelected();
            bChanging = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bChanging) return;
            bChanging = true;
            allContactsBox.ClearSelected();
            bChanging = false;
        }

        private void addFavBtn_Click(object sender, EventArgs e)
        {
            if (allContactsBox.SelectedIndex == -1)
            {
                //Console.WriteLine("Podaj adres IP");
                MessageBox.Show("Wybierz, który kontakt chcesz dodać do ulubionych.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //destination_ip
                string login = allContactsBox.SelectedItem.ToString();
                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                }
                catch (Exception ex) { }
                SqlCommand command2 = connection.CreateCommand();
                command2.CommandText = "SELECT userID FROM users WHERE name = @0";
                command2.Parameters.AddWithValue("@0", login);
                int id = Convert.ToInt32(command2.ExecuteScalar());
                SqlCommand command4 = connection.CreateCommand();
                command4.CommandText = "SELECT COUNT(contactID) FROM friends WHERE userID=@0 AND personID=@1";
                command4.Parameters.AddWithValue("@0", myID);
                command4.Parameters.AddWithValue("@1", id);
                int count = Convert.ToInt32(command4.ExecuteScalar());
                if(count == 0)
                {
                    SqlCommand command3 = connection.CreateCommand();
                    command3.CommandText = "INSERT INTO friends(userID, personID) VALUES(@0, @1)";
                    command3.Parameters.AddWithValue("@0", myID);
                    command3.Parameters.AddWithValue("@1", id);
                    command3.ExecuteNonQuery();
                    refreshFav();
                }
                else
                {
                    MessageBox.Show("Użytkownik już widnieje na Twojej liście ulubionych.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                

                connection.Close();
            }
        }

        private void RmFavBtn_Click(object sender, EventArgs e)
        {
            if (favContactsBox.SelectedIndex == -1)
            {
                //Console.WriteLine("Podaj adres IP");
                MessageBox.Show("Wybierz, który kontakt chcesz usunąć z ulubionych.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string login = favContactsBox.SelectedItem.ToString();
                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                }
                catch (Exception ex) { }
                SqlCommand command1 = connection.CreateCommand();
                command1.CommandText = "SELECT userID FROM users WHERE name=@0";
                command1.Parameters.AddWithValue("@0", login);
                int id = Convert.ToInt32(command1.ExecuteScalar());
                SqlCommand command2 = connection.CreateCommand();
                command2.CommandText = "DELETE FROM friends WHERE userID=@0 AND personID=@1";
                command2.Parameters.AddWithValue("@0", myID);
                command2.Parameters.AddWithValue("@1", id);
                command2.ExecuteNonQuery();
                refreshFav();
            }
            connection.Close();
        }

        private void refreshFavBtn_Click(object sender, EventArgs e)
        {
            refreshFav();
        }

        private void settingsBtn_Click(object sender, EventArgs e)
        {
            Settings settings_form = new Settings(myID);
            settings_form.Show();
        }
    }
}
