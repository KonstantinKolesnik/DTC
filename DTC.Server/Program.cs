using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace DTC.Server
{
    public partial class Program
    {
        void ProgramStarted()
        {
            HWConfig.Mainboard = Mainboard;
            HWConfig.SDCard = sdCard;
            HWConfig.Indicators = led_Strip;
            HWConfig.WiFi = wifi_RS21;

            //HWConfig.Indicators.TurnAllLedsOff();

            GT.Timer timer = new GT.Timer(1000);
            timer.Tick += delegate(GT.Timer t) { t.Stop(); new Model().Start(); };
            timer.Start();
            //new Model().Start();

            //Mainboard.SetDebugLED(true);
        }
    }
}
