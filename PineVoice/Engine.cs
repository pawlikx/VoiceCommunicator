using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Ozeki.Media;
using Ozeki.VoIP;


namespace PineVoice
{
    class Engine
    {

        /*
        static ISoftPhone softphone;   // softphone object
        static IPhoneLine phoneLine;   // phoneline object
        static IPhoneCall call;
        static string caller;
        static Microphone microphone;
        static Speaker speaker;
        static PhoneCallAudioSender mediaSender;
        static PhoneCallAudioReceiver mediaReceiver;
        static MediaConnector connector;

        static void Main(string[] args)
        {
            softphone = SoftPhoneFactory.CreateSoftPhone(5000, 10000);

            microphone = Microphone.GetDefaultDevice();
            speaker = Speaker.GetDefaultDevice();
            mediaSender = new PhoneCallAudioSender();
            mediaReceiver = new PhoneCallAudioReceiver();
            connector = new MediaConnector();

            Console.WriteLine("Wprowadz twój adres IP: ");
            var ipAddress = Console.ReadLine();
            var config = new DirectIPPhoneLineConfig(ipAddress, 5060);
            phoneLine = softphone.CreateDirectIPPhoneLine(config);
            phoneLine.RegistrationStateChanged += line_RegStateChanged;
            softphone.IncomingCall += softphone_IncomingCall;
            //softphone.IncomingCall += softphone_IncomingCall;
            softphone.RegisterPhoneLine(phoneLine);

            Console.ReadLine();
        }

        private static void line_RegStateChanged(object sender, RegistrationStateChangedArgs e)
        {
            if (e.State == RegState.NotRegistered || e.State == RegState.Error)
                Console.WriteLine("Rejestracja nieudana!");

            if (e.State == RegState.RegistrationSucceeded)
            {
                Console.WriteLine("Rejestracja pomyslna!");
                Console.WriteLine("Podaj adres IP osoby z ktora chcesz sie polaczyc: ");

                string ipToDial = Console.ReadLine();
                StartCall(ipToDial);
            }
        }

        private static void StartCall(string numberToDial)
        {
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

            if (e.State == CallState.Answered)
                SetupDevices();

            if (e.State.IsCallEnded())
                CloseDevices();
        }

        static void SetupDevices()
        {
            microphone.Start();
            connector.Connect(microphone, mediaSender);

            speaker.Start();
            connector.Connect(mediaReceiver, speaker);

            mediaSender.AttachToCall(call);
            mediaReceiver.AttachToCall(call);
        }

        static void CloseDevices()
        {
            microphone.Dispose();
            speaker.Dispose();

            mediaReceiver.Detach();
            mediaSender.Detach();
            connector.Dispose();
        }*/
    }
    public class IPInfo
    {
        public IPInfo(string macAddress, string ipAddress)
        {
            this.MacAddress = macAddress;
            this.IPAddress = ipAddress;
        }

        public string MacAddress { get; private set; }
        public string IPAddress { get; private set; }

        private string _HostName = string.Empty;
        public string HostName
        {
            get
            {
                if (string.IsNullOrEmpty(this._HostName))
                {
                    try
                    {
                        // Retrieve the "Host Name" for this IP Address. This is the "Name" of the machine.
                        this._HostName = Dns.GetHostEntry(this.IPAddress).HostName;
                    }
                    catch
                    {
                        this._HostName = string.Empty;
                    }
                }
                return this._HostName;
            }
        }


        #region "Static Methods"

        /// <summary>
        /// Retrieves the IPInfo for the machine on the local network with the specified MAC Address.
        /// </summary>
        /// <param name="macAddress">The MAC Address of the IPInfo to retrieve.</param>
        /// <returns></returns>
        public static IPInfo GetIPInfo(string macAddress)
        {
            var ipinfo = (from ip in IPInfo.GetIPInfo()
                          where ip.MacAddress.ToLowerInvariant() == macAddress.ToLowerInvariant()
                          select ip).FirstOrDefault();

            return ipinfo;
        }

        /// <summary>
        /// Retrieves the IPInfo for All machines on the local network.
        /// </summary>
        /// <returns></returns>
        public static List<IPInfo> GetIPInfo()
        {
            try
            {
                var list = new List<IPInfo>();

                foreach (var arp in GetARPResult().Split(new char[] { '\n', '\r' }))
                {
                    // Parse out all the MAC / IP Address combinations
                    if (!string.IsNullOrEmpty(arp))
                    {
                        var pieces = (from piece in arp.Split(new char[] { ' ', '\t' })
                                      where !string.IsNullOrEmpty(piece)
                                      select piece).ToArray();
                        if (pieces.Length == 3)
                        {
                            list.Add(new IPInfo(pieces[1], pieces[0]));
                        }
                    }
                }

                // Return list of IPInfo objects containing MAC / IP Address combinations
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("IPInfo: Error Parsing 'arp -a' results", ex);
            }
        }

        /// <summary>
        /// This runs the "arp" utility in Windows to retrieve all the MAC / IP Address entries.
        /// </summary>
        /// <returns></returns>
        private static string GetARPResult()
        {
            Process p = null;
            string output = string.Empty;

            try
            {
                p = Process.Start(new ProcessStartInfo("arp", "-a")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                });

                output = p.StandardOutput.ReadToEnd();

                p.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("IPInfo: Error Retrieving 'arp -a' Results", ex);
            }
            finally
            {
                if (p != null)
                {
                    p.Close();
                }
            }

            return output;
        }

        #endregion
    }
}