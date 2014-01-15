using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace MFE
{
    public static class Utils
    {
        public static bool IsEmulator
        {
            get { return SystemInfo.SystemID.SKU == 3; }
        }



        public static string StringReplace(string Source, string ToFind, string ReplaceWith)
        {
            int i;
            int iStart = 0;

            if (ToFind == string.Empty)
                return Source;

            while (true)
            {
                i = Source.IndexOf(ToFind, iStart);
                if (i < 0) break;

                if (i > 0)
                {
                    Source = Source.Substring(0, i) + ReplaceWith + Source.Substring(i + ToFind.Length);
                }
                else
                {
                    Source = ReplaceWith + Source.Substring(i + ToFind.Length);
                }

                iStart = i + ReplaceWith.Length;
            }
            return Source;
        }







    }
}
