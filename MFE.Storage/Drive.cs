using GHI.Premium.IO;
using GHI.Premium.System;
using Microsoft.SPOT.IO;
using System;

namespace MFE.Storage
{
    /// <summary>
    /// SD card or USB Hard Drive
    /// </summary>
    [Serializable]
    public struct Drive
    {
        public PersistentStorage ps;
        public bool IsFormatted;
        public string VolumeName;
        public string RootName;
        public VolumeInfo VolumeInfo;
        public USBH_Device Device;
    }
}
