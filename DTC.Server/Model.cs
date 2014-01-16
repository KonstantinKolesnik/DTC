using MFE.Data;
using MFE.Net.Http;
using MFE.Net.Messaging;
using MFE.Net.Tcp;
using MFE.Net.WebSocket;
using MFE.Storage;
using Microsoft.SPOT;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Reflection;

namespace DTC.Server
{
    class Model
    {
        #region Fields
        private const uint optionsID = 0;
        private static Options options;

        //private static string root = @"\SD"; // for device
        private static string root = @"\WINFS"; // for emulator

        private static string dbFileName = "Layout.dat";
        private static DBManager dbManager = new DBManager();

        //private static Booster mainBooster;
        //private static Booster progBooster;
        //private static Timer timerBoostersCurrent;

        //private static INetworkManager networkManager;

        private static NetworkMessageFormat msgFormat = NetworkMessageFormat.Text;
        //private static DiscoveryListener discoveryListener;
        private static TcpServer tcpServer;
        private static WSServer wsServer;

        private static HttpServer httpServer;

        private static SerialPort uart;

        //private static Buttons btns;
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
            DriveManager.DriveAdded += DriveManager_DriveAdded;

            InitData();
            InitHardware();
            InitDB();
            InitNetwork();

            // for test only!!!
            //IsPowerOn = true;
        }
        #endregion

        #region Private methods
        private static void InitData()
        {
            //options = Options.LoadFromFlash(optionsID);

            //options = Options.LoadFromSD();
            //ApplyOptions();
        }
        private static void InitHardware()
        {
            uart = new SerialPort("COM1", 115200);
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

        private static void InitNetwork()
        {
            //discoveryListener = new DiscoveryListener();

            //tcpServer = new TCPServer(Options.IPPort);
            //tcpServer.SessionConnected += new TCPSessionEventHandler(Session_Connected);
            //tcpServer.SessionDataReceived += new TCPSessionDataReceived(Session_DataReceived);
            //tcpServer.SessionDisconnected += new TCPSessionEventHandler(Session_Disconnected);

            wsServer = new WSServer(Options.WSPort);
            wsServer.SessionConnected += Session_Connected;
            wsServer.SessionDataReceived += Session_DataReceived;
            wsServer.SessionDisconnected += Session_Disconnected;

            tcpServer = new TcpServer(Options.IPPort);

            httpServer = new HttpServer();
            httpServer.OnGetRequest += httpServer_OnGetRequest;

            //if (options.UseWiFi)
            //    networkManager = new WiFiManager(true, HardwareConfiguration.PinNetworkLED, options.WiFiSSID, options.WiFiPassword);
            //else
            //    networkManager = new EthernetManager(HardwareConfiguration.PinNetworkLED);
            //networkManager.Started += new EventHandler(Network_Started);
            //networkManager.Stopped += new EventHandler(Network_Stopped);

            StartNetwork();
        }
        private static void StartNetwork()
        {
            //new Thread(delegate { networkManager.Start(); }).Start();

            

            //httpServer.Start("http", 81; // for emulator only!!!
            //wsServer.Start(); // for emulator only!!!
            //tcpServer.Start();
        }
        private static void InitDB()
        {
            //dbManager.OpenInMemory();







            return;


            // for test!!!
            bool dbExists = File.Exists(root + "\\" +dbFileName);
            //if (dbExists)
            //    File.Delete(dbFileName);

            dbManager.OpenOnSD(dbFileName);
            if (!dbExists)
            {
                // add tables;
            }

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

            //dbManager.Close();
        }
        private void CreateDB()
        {
            //db.ExecuteNonQuery("CREATE Table Locomotives(ID TEXT, Name TEXT, Description TEXT, Protocol INTEGER, ConsistID TEXT, ConsistForward INTEGER)");
            //db.ExecuteNonQuery("CREATE Table Consists(ID TEXT, Name TEXT, Description TEXT)");



        }

        private static void ApplyOptions()
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
        #endregion

        #region Event handlers
        private static void DriveManager_DriveAdded(string Root)
        {
            if (Root == "\\SD")
            {
                //    options = Options.LoadFromSD();
                //    ApplyOptions();
            }
        }

        private static void Network_Started(object sender, EventArgs e)
        {
            ////discoveryListener.Start(Options.UDPPort, "TyphoonCentralStation");
            //tcpServer.Start();

            //httpServer.Start("http", 80);
            //wsServer.Start();

            //NameService ns = new NameService();
            //ns.AddName("TYPHOON", NameService.NameType.Unique, NameService.MsSuffix.Default);

            //Beeper.PlaySound(Beeper.SoundID.Click);
        }
        private static void Network_Stopped(object sender, EventArgs e)
        {
            //Beeper.PlaySound(Beeper.SoundID.PowerOff);

            //httpServer.Stop();
            //wsServer.Stop();

            //Thread.Sleep(1000);

            //StartNetwork();
        }

        private static void httpServer_OnGetRequest(string path, Hashtable parameters, HttpListenerResponse response)
        {
            if (path.ToLower() == "\\admin") // There is one particular URL that we process differently
            {
                //httpServer.ProcessPasswordProtectedArea(request, response);
            }
            //else if (path.ToLower().IndexOf("json") != -1)
            //    ProcessJSONRequest(path, parameters, response);
            else
                httpServer.SendFile(root + path, response);
        }

        private static void Session_Connected(TcpSession session)
        {
            session.Tag = new NetworkMessageReceiver(msgFormat);
        }
        private static bool Session_DataReceived(TcpSession session, byte[] data)
        {
            //networkManager.OnBeforeMessage();

            NetworkMessageReceiver nmr = session.Tag as NetworkMessageReceiver;
            NetworkMessage[] msgs = nmr.Process(data);
            if (msgs != null)
                foreach (NetworkMessage msg in msgs)
                {
                    NetworkMessage response = ProcessNetworkMessage(msg);
                    if (response != null)
                        session.Send(WSDataFrame.WrapString(response.PackToString(nmr.MessageFormat)));
                }

            //networkManager.OnAfterMessage();

            return false; // don't disconnect
        }
        private static void Session_Disconnected(TcpSession session)
        {
            // TODO: release locos and accessories
        }

        private static void uart_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
        }
        private static void uart_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region Network message processing
        private static NetworkMessage ProcessNetworkMessage(NetworkMessage msg)
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


        private static NetworkMessage GetVersionMessage()
        {
            NetworkMessage msg = new NetworkMessage("Version");
            msg["Version"] = Version;
            return msg;
        }

        #endregion
    }
}
