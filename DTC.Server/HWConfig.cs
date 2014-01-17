using Gadgeteer.Modules.GHIElectronics;
using GHIElectronics.Gadgeteer;

namespace DTC.Server
{
    static class HWConfig
    {
        public static FEZRaptor Mainboard;
        public static int LEDNetwork = 3;

        // modules:
        public static LED_Strip Indicators;
        public static SDCard SDCard;
        public static WiFi_RS21 WiFi;
    }
}
