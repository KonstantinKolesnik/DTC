using Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using MFE.Data;
using MFE.Net.Http;
using MFE.Net.Managers;
using MFE.Net.Messaging;
using MFE.Net.Tcp;
using MFE.Net.WebSocket;
using Microsoft.SPOT;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Reflection;
using System.Threading;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;

namespace DTC.Server
{
    class Model
    {
        #region Fields
        private Options options;
        private string root = @"\WINFS"; // for emulator

        private string dbFileName = @"\DTC\Layout.dat";
        private DBManager dbManager = new DBManager();

        //private Booster mainBooster;
        //private Booster progBooster;
        //private Timer timerBoostersCurrent;

        private INetworkManager networkManager;

        private NetworkMessageFormat msgFormat = NetworkMessageFormat.Text;
        //private DiscoveryListener discoveryListener;
        private TcpServer tcpServer;
        private WSServer wsServer;

        private HttpServer httpServer;

        private SerialPort uart;

        //private Buttons btns;

        private GT.Timer timerNetworkConnect;
        #endregion

        #region Properties
        public static string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }
        #endregion

        #region Constructor
        public Model()
        {
        }
        #endregion

        public void Start()
        {
            InitHardware();
            //InitDB();
            InitNetwork();
        }

        #region Private methods
        private void InitHardware()
        {
            HWConfig.Indicators.TurnAllLedsOff();

            timerNetworkConnect = new GT.Timer(500);
            timerNetworkConnect.Tick += delegate(GT.Timer t) { HWConfig.Indicators[HWConfig.LEDNetwork] = !HWConfig.Indicators[HWConfig.LEDNetwork]; };

            HWConfig.SDCard.SDCardMounted += sdCard_Mounted;
            HWConfig.SDCard.SDCardUnmounted += sdCard_Unmounted;
            if (HWConfig.SDCard.IsCardInserted)
            {
                HWConfig.SDCard.MountSDCard();
                Thread.Sleep(500);

                options = Options.LoadFromSD(HWConfig.SDCard.GetStorageDevice().RootDirectory);
                //ApplyOptions();
            }

            // UART on socket 4:
            string portName = GT.Socket.GetSocket(4, true, null, null).SerialPortName;
            uart = new SerialPort(portName, 115200);
            uart.DataReceived += uart_DataReceived;
            uart.ErrorReceived += uart_ErrorReceived;
            uart.Open();


            //mainBooster = new Booster(
            //    true,
            //    HardwareConfiguration.PinMainBoosterEnable,
            //    HardwareConfiguration.PinMainBoosterEnableLED,
            //    HardwareConfiguration.PinMainBoosterSense,
            //    HardwareConfiguration.PinMainBoosterOverloadLED,
            //    HardwareConfiguration.SenseResistor,
            //    options.MainBridgeCurrentThreshould,
            //    HardwareConfiguration.PinMainOutputGenerator,
            //    DCCCommand.Idle().ToTimings()
            //    );
            //mainBooster.PropertyChanged += new PropertyChangedEventHandler(Booster_PropertyChanged);

            //progBooster = new Booster(
            //    true,
            //    HardwareConfiguration.PinProgBoosterEnable,
            //    HardwareConfiguration.PinProgBoosterEnableLED,
            //    HardwareConfiguration.PinProgBoosterSense,
            //    HardwareConfiguration.PinProgBoosterOverloadLED,
            //    HardwareConfiguration.SenseResistor,
            //    options.ProgBridgeCurrentThreshould,
            //    HardwareConfiguration.PinProgOutputGenerator,
            //    DCCCommand.Idle().ToTimings(true)
            //    );
            //progBooster.PropertyChanged += new PropertyChangedEventHandler(Booster_PropertyChanged);

            //if (options.BroadcastBoostersCurrent)
            //    timerBoostersCurrent = new Timer(TimerBustersCurrent_Tick, null, 0, 1000);

            //btns = new Buttons();
        }
        private void InitDB()
        {
            bool dbExists = File.Exists(HWConfig.SDCard.GetStorageDevice().RootDirectory + dbFileName);
            // for test!!!
            //if (dbExists)
            //    File.Delete(dbFileName);

            if (dbManager.Open(HWConfig.SDCard.GetStorageDevice().RootDirectory + dbFileName))
            {
                if (!dbExists)
                    CreateDB();

                ////dbManager.Add(new Consist() { Name = "Consist 1", });
                ////dbManager.Add(new Consist() { Name = "Consist 2", });
                ////dbManager.Add(new Consist() { Name = "Consist 3", });

                ////dbManager.Add(new Locomotive() { Name = "Loco 1", Protocol = ProtocolType.DCC14 });
                ////dbManager.Add(new Locomotive() { Name = "Loco 2", Protocol = ProtocolType.DCC28 });
                ////dbManager.Add(new Locomotive() { Name = "Loco 3", Protocol = ProtocolType.DCC128 });

                //Debug.Print(Utils.FreeRAM(true));

                //for (int i = 0; i < 100; i++)
                //    dbManager.Add(new Locomotive() { Name = "Loco " + i, Protocol = ProtocolType.DCC28 });

                //Debug.Print("DB size: " + Utils.FormatSize(dbManager.Size));



                //Debug.Print(Utils.FreeRAM(true));
                //ArrayList locos = dbManager.GetLocomotives();
                //Debug.Print(Utils.FreeRAM(true));

                ////Locomotive loc = (Locomotive)locos[0];
                ////Locomotive loc2 = dbManager.GetLocomotive(loc.ID);
                ////Debug.Print(loc2.Name);

                ////ArrayList consists = dbManager.GetConsists();
                ////Consist consist = (Consist)consists[1];
                ////Consist consist2 = dbManager.GetConsist(consist.ID);
                ////Debug.Print(consist2.Name);

                dbManager.Close();
            }
        }
        private void CreateDB()
        {
            //db.ExecuteNonQuery("CREATE Table Locomotives(ID TEXT, Name TEXT, Description TEXT, Protocol INTEGER, ConsistID TEXT, ConsistForward INTEGER)");
            //db.ExecuteNonQuery("CREATE Table Consists(ID TEXT, Name TEXT, Description TEXT)");
        }
        private void InitNetwork()
        {
            //discoveryListener = new DiscoveryListener();

            tcpServer = new TcpServer(Options.IPPort);
            tcpServer.SessionConnected += Session_Connected;
            tcpServer.SessionDataReceived += Session_DataReceived;
            tcpServer.SessionDisconnected += Session_Disconnected;

            wsServer = new WSServer(Options.WSPort);
            wsServer.SessionConnected += Session_Connected;
            wsServer.SessionDataReceived += Session_DataReceived;
            wsServer.SessionDisconnected += Session_Disconnected;

            httpServer = new HttpServer();
            httpServer.OnGetRequest += httpServer_OnGetRequest;
            httpServer.OnResponse += httpServer_OnResponse;

            //if (options.UseWiFi)
            //networkManager = new GadgeteerWiFiManager(HWConfig.WiFi, options.WiFiSSID, options.WiFiPassword);//, GT.Socket.GetSocket(18, true, null, null).PWM9);
            //else
            networkManager = new GadgeteerEthernetManager(HWConfig.Ethernet);//HWConfig.PinNetworkLED);

            networkManager.Started += new EventHandler(Network_Started);
            networkManager.Stopped += new EventHandler(Network_Stopped);

            StartNetwork();
        }
        private void StartNetwork()
        {
            timerNetworkConnect.Start();
            new Thread(delegate
            {
                networkManager.Start();
            }).Start();
        }

        private void ApplyOptions()
        {
            //if (mainBooster != null)
            //    mainBooster.CurrentThreshould = options.MainBridgeCurrentThreshould;
            //if (progBooster != null)
            //    progBooster.CurrentThreshould = options.ProgBridgeCurrentThreshould;
            //if (options.BroadcastBoostersCurrent)
            //{
            //    if (timerBoostersCurrent == null)
            //        timerBoostersCurrent = new Timer(TimerBustersCurrent_Tick, null, 0, 1000);
            //}
            //else
            //{
            //    timerBoostersCurrent.Dispose();
            //    timerBoostersCurrent = null;
            //}
        }
        private void BlinkLED(int led)
        {
            //HWConfig.Indicators[led] = false;
            //Thread.Sleep(20);
            //HWConfig.Indicators[led] = true;

            new Thread(delegate
            {
                HWConfig.Indicators[led] = false;
                Thread.Sleep(20);
                HWConfig.Indicators[led] = true;
            }).Start();
        }
        #endregion

        #region Event handlers
        private void sdCard_Mounted(SDCard sender, StorageDevice SDCard)
        {
            Debug.Print("SD card mounted. Unmount before removing");
        }
        private void sdCard_Unmounted(SDCard sender)
        {
            Debug.Print("SD card unmounted. Don't try to access it without mounting it again first.");
        }

        private void Network_Started(object sender, EventArgs e)
        {
            //HWConfig.WiFi.NetworkSettings.IPAddress
            //HWConfig.Ethernet.Interface.NetworkInterface.IPAddress

            timerNetworkConnect.Stop();
            HWConfig.Indicators[HWConfig.LEDNetwork] = true;

            httpServer.Start("http", 80);
            wsServer.Start();
            tcpServer.Start();
            
            //discoveryListener.Start(Options.UDPPort, "TyphoonCentralStation");

            //NameService ns = new NameService();
            //ns.AddName("TYPHOON", NameService.NameType.Unique, NameService.MsSuffix.Default);
        }
        private void Network_Stopped(object sender, EventArgs e)
        {
            httpServer.Stop();
            wsServer.Stop();
            tcpServer.Stop();

            Thread.Sleep(1000);

            StartNetwork();
        }

        private void httpServer_OnGetRequest(string path, Hashtable parameters, HttpListenerResponse response)
        {
            BlinkLED(HWConfig.LEDNetwork);

            if (HWConfig.SDCard.IsCardMounted)
            {
                if (path.ToLower() == "\\admin") // There is one particular URL that we process differently
                {
                    //httpServer.ProcessPasswordProtectedArea(request, response);
                }
                else
                    httpServer.SendFile(HWConfig.SDCard.GetStorageDevice().RootDirectory + "\\DTC" + path, response);
                
                BlinkLED(HWConfig.LEDNetwork);
            }
        }
        private void httpServer_OnResponse(HttpListenerResponse response)
        {
            BlinkLED(HWConfig.LEDNetwork);
        }

        private void Session_Connected(TcpSession session)
        {
            session.Tag = new NetworkMessageReceiver(msgFormat);
        }
        private bool Session_DataReceived(TcpSession session, byte[] data)
        {
            BlinkLED(HWConfig.LEDNetwork);

            NetworkMessageReceiver nmr = session.Tag as NetworkMessageReceiver;
            NetworkMessage[] msgs = nmr.Process(data);
            if (msgs != null)
                foreach (NetworkMessage msg in msgs)
                {
                    NetworkMessage response = ProcessNetworkMessage(msg);
                    if (response != null)
                    {
                        session.Send(WSDataFrame.WrapString(response.PackToString(nmr.MessageFormat)));
                        BlinkLED(HWConfig.LEDNetwork);
                    }
                }

            return false; // don't disconnect
        }
        private void Session_Disconnected(TcpSession session)
        {
            // TODO: release locos and accessories
        }

        private void uart_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
        }
        private void uart_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Network message processing
        private NetworkMessage ProcessNetworkMessage(NetworkMessage msg)
        {
            NetworkMessage response = null;

            if (msg != null)
            {
                switch (msg.ID)
                {

                    #region Version
                    case "Version": response = GetVersionMessage(); break;
                    #endregion



                    default: break;
                }
            }

            return response;
        }


        private NetworkMessage GetVersionMessage()
        {
            NetworkMessage msg = new NetworkMessage("Version");
            msg["Version"] = Version;
            return msg;
        }

        #endregion
    }
}
