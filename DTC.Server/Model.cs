using MFE.Net.Http;
using MFE.Net.Messaging;
using MFE.Net.Tcp;
using MFE.Net.WebSocket;
using MFE.Storage;
using Microsoft.SPOT;
using System.Collections;
using System.Net;
using System.Reflection;

namespace DTC.Server
{
    class Model
    {
        #region Fields
        private static Options options;
        private const uint optionsID = 0;

        //private static string root = @"\SD"; // for device
        private static string root = @"\WINFS"; // for emulator

        //private static DBManager dbManager;

        //private static Booster mainBooster;
        //private static Booster progBooster;
        //private static Timer timerBoostersCurrent;

        //private static INetworkManager networkManager;

        private static NetworkMessageFormat msgFormat = NetworkMessageFormat.Text;
        //private static DiscoveryListener discoveryListener;
        //private static TCPServer tcpServer;
        private static WSServer wsServer;

        private static HttpServer httpServer;

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

            ////ackDetector = new AcknowledgementDetector(HardwareConfiguration.PinAcknowledgementSense);

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


            //WebServer.StartLocalServer(""/*eth.NetworkInterface.IPAddress*/, 80);
            //WebServer.DefaultEvent.WebEventReceived += DefaultEvent_WebEventReceived;
            //WebServer.SetupWebEvent("test/").WebEventReceived += webEvent_WebEventReceived;

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

            httpServer.Start("http", 81); // for emulator only!!!
            wsServer.Start(); // for emulator only!!!
        }
        private static void InitDB()
        {
            //dbManager = new DBManager();
            ////root = @"\NAND";
            //dbManager.Open(root + "\\Layout.dat");

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

        //private static void Response404(Responder responder)
        //{
        //    string strResp = "<!DOCTYPE html PUBLIC \" -//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">";
        //    strResp += "<html><head></head><body>.Net Micro Framework Example HTTP Server";
        //    // Print requested verb, URL and version.. Adds information from the request.
        //    //strResp += "HTTP Method: " + responder.HttpMethod + "<br> Requested URL: \"" + request.RawUrl + "<br> HTTP Version: " + responder.ProtocolVersion + "\"<p>";
        //    //// Information about the path that we access.
        //    //strResp += "File to access " + strFilePath + "<p>";
        //    //if (Directory.Exists(strFilePath)) // If directory present - iterate over it and 
        //    //    strResp += FillDirectoryContext(strFilePath, request);
        //    //else // Neither File or directory exists, prints that directory is not found.
        //    //    strResp += "Directory: \"" + strFilePath + "\" Does not exists";
        //    strResp += "</body></html>";
        //    //responder.Respond(strResp);
        //    //responder.Respond(UTF8Encoding.UTF8.GetBytes(strResp), "text/html");

        //    strResp = "HTTP/1.1 404 File Not Found\r\n";
        //    //responder.ContentType = "text/html";
        //    //response.StatusCode = (int)HttpStatusCode.NotFound;
        //    byte[] messageBody = Encoding.UTF8.GetBytes(strResp);
        //    responder.Respond(messageBody, "text/html");
        //    //response.OutputStream.Write(messageBody, 0, messageBody.Length);
        //}
        //private static string GetContentType(string ext)
        //{
        //    switch (ext)
        //    {
        //        case ".htm":
        //        case ".html":
        //        case ".htmls":
        //            return "text/html";
        //        case ".js":
        //            return "text/javascript";
        //        //return "application/javascript";
        //        case ".jpe":
        //        case ".jpg":
        //        case ".jpeg":
        //            return "image/jpeg";
        //        case ".gif":
        //            return "image/gif";
        //        case ".png":
        //            return "image/png";
        //        case ".ico":
        //            return "image/x-icon";
        //        case ".pdf":
        //            return "application/pdf";
        //        case ".svg":
        //            return "image/svg+xml";
        //        case ".css":
        //            return "text/css";
        //        case ".xml":
        //            return "text/xml";
        //        case ".json":
        //            return "application/json";
        //        case ".arj":
        //        case ".lzh":
        //        case ".exe":
        //        case ".rar":
        //        case ".tar":
        //        case ".zip":
        //            return "application/octet-stream";
        //        case ".mid":
        //        case ".midi":
        //            return "application/x-midi";
        //        case ".mp3":
        //            return "audio/mpeg";
        //        case ".swf":
        //            return "application/x-shockwave-flash";
        //        default:
        //            return "text/plain";
        //    }
        //}
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
            ////tcpServer.Start();

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




        // Any URL requests that do not match registered WebEvent paths will end up here.
        //private static void DefaultEvent_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        //{
        //    path = Utils.StringReplace(path, "/", "\\");
        //    path = root + "\\" + path;

        //    if (!File.Exists(path))
        //        Response404(responder);
        //    else
        //    {
        //        string ext = Path.GetExtension(path).ToLower();
        //        string ct = GetContentType(ext);

        //        //int bufferSize = 1024 * 1024; // 4
        //        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        //        {
        //            long fileLength = fs.Length;
        //            //response.ContentLength64 = fileLength;
        //            //Debug.Print("File: " + path + "; length = " + fileLength);

        //            byte[] buf = new byte[fileLength];//bufferSize];

        //            fs.Read(buf, 0, (int)fileLength);
        //            responder.Respond(buf, ct);

        //            //for (long bytesSent = 0; bytesSent < fileLength; )
        //            //{
        //            //    long bytesToRead = fileLength - bytesSent;
        //            //    bytesToRead = bytesToRead < bufferSize ? bytesToRead : bufferSize;

        //            //    byte[] buf = new byte[bytesToRead];

        //            //    int bytesRead = fs.Read(buf, 0, (int)bytesToRead);
        //            //    responder.OutputStream.Write(buf, 0, bytesRead);
        //            //    bytesSent += bytesRead;

        //            //    Thread.Sleep(0);
        //            //}
        //            fs.Close();
        //        }
        //    }
        //}
        // This method will only be called to handle a web request for http://xxx/test/
        //private static void webEvent_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        //{
        //    responder.Respond("Hello World, specific handler");
        //}
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
