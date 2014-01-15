using MFE;
using MFE.Storage;
using Microsoft.SPOT;
using System;
using System.Ext.Xml;
using System.IO;
using System.Text;
using System.Xml;

namespace DTC.Server
{
    public class Options
    {
        #region Constants
        private const string fileName = @"\SD\options.xml";
        public const int IPPort = 8000;
        public const int UDPPort = 9000;
        public const int WSPort = 12000;
        #endregion

        #region Fields
        private static ExtendedWeakReference ewr;
        #endregion

        #region Settings
        public bool UseWiFi = true;
        public string WiFiSSID = "GothicMaestro";
        public string WiFiPassword = "kotyara75";

        public int MainBridgeCurrentThreshould = 3700; // 3.7A
        public int ProgBridgeCurrentThreshould = 600; // 0.6A
        public bool BroadcastBoostersCurrent = false;
        //public bool ShutdownOnExternalShortCircuit = false;
        #endregion

        #region Serialization / deserialization
        public static Options FromXml(string xml)
        {
            return !Utils.IsStringNullOrEmpty(xml) ? Options.FromByteArray(Encoding.UTF8.GetBytes(xml)) : null;
        }
        public static Options FromByteArray(byte[] data)
        {
            Options res = null;

            if (data != null)
            {
                using (MemoryStream xmlStream = new MemoryStream(data))
                {
                    XmlReaderSettings ss = new XmlReaderSettings();
                    ss.IgnoreWhitespace = true;
                    ss.IgnoreComments = true;
                    using (XmlReader reader = XmlReader.Create(xmlStream, ss))
                    {
                        while (!reader.EOF)
                        {
                            reader.Read();
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Options")
                            {
                                res = new Options();

                                if (!Utils.IsStringNullOrEmpty(reader.GetAttribute("MainBridgeCurrentThreshould")))
                                    res.MainBridgeCurrentThreshould = int.Parse(reader.GetAttribute("MainBridgeCurrentThreshould"));
                                if (!Utils.IsStringNullOrEmpty(reader.GetAttribute("ProgBridgeCurrentThreshould")))
                                    res.ProgBridgeCurrentThreshould = int.Parse(reader.GetAttribute("ProgBridgeCurrentThreshould"));
                                if (!Utils.IsStringNullOrEmpty(reader.GetAttribute("BroadcastBoostersCurrent")))
                                    res.BroadcastBoostersCurrent = reader.GetAttribute("BroadcastBoostersCurrent") == bool.TrueString;

                                if (!Utils.IsStringNullOrEmpty(reader.GetAttribute("UseWiFi")))
                                    res.UseWiFi = reader.GetAttribute("UseWiFi") == bool.TrueString;
                                if (!Utils.IsStringNullOrEmpty(reader.GetAttribute("WiFiSSID")))
                                    res.WiFiSSID = reader.GetAttribute("WiFiSSID");
                                if (!Utils.IsStringNullOrEmpty(reader.GetAttribute("WiFiPassword")))
                                    res.WiFiPassword = reader.GetAttribute("WiFiPassword");

                            }
                        }
                    }
                }
            }

            return res;
        }
        public string ToXml()
        {
            byte[] res = ToByteArray();
            return res != null ? new string(Encoding.UTF8.GetChars(res)) : null;
        }
        public byte[] ToByteArray()
        {
            byte[] res = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(ms))
                {
                    //<?xml version="1.0" encoding="utf-8" ?>
                    //writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\"");

                    writer.WriteStartElement("Options");

                    writer.WriteAttributeString("MainBridgeCurrentThreshould", MainBridgeCurrentThreshould.ToString());
                    writer.WriteAttributeString("ProgBridgeCurrentThreshould", ProgBridgeCurrentThreshould.ToString());
                    writer.WriteAttributeString("BroadcastBoostersCurrent", BroadcastBoostersCurrent ? bool.TrueString : bool.FalseString);

                    writer.WriteAttributeString("UseWiFi", UseWiFi ? bool.TrueString : bool.FalseString);
                    writer.WriteAttributeString("WiFiSSID", WiFiSSID);
                    writer.WriteAttributeString("WiFiPassword", WiFiPassword);

                    writer.WriteEndElement();
                    writer.Flush();
                    //writer.Close();

                    res = ms.ToArray();
                }
            }

            return res;
        }
        #endregion

        #region Load / save
        public static Options LoadFromFlash(uint id)
        {
            ewr = ExtendedWeakReference.RecoverOrCreate(typeof(string), id, ExtendedWeakReference.c_SurvivePowerdown | ExtendedWeakReference.c_SurviveBoot);

            // Indicate how important this data is. The CLR discards from first to last:
            // OkayToThrowAway items
            // NiceToHave items
            // Important items
            // Critical items
            // System items.
            // In practice, System items are virtually never discarded.
            ewr.Priority = (Int32)ExtendedWeakReference.PriorityLevel.System;

            Options res = null;
            if (ewr.Target != null)
                res = Options.FromXml((string)ewr.Target);

            return res ?? new Options();
        }
        public void SaveToFlash()
        {
            ewr.Target = ToXml();
        }
        //public static void DeleteFromFlash()
        //{
        //    // Data recovering and storing must be kept in pairs.
        //    // Calling Recover without setting Target frees the FLASH completely from this EWR.
        //    ExtendedWeakReference.Recover(typeof(Options), 0);
        //}
        //public static void RestoreLastSaved() // Use this method to rewrite current settings using last saved data.
        //{
        //    // First, mark the stored data as unrecovered so we can Recover them.
        //    optionsReference.PushBackIntoRecoverList();
        //    // Try to recover them, but do not create them if they do not exist.
        //    ExtendedWeakReference restoreReference = ExtendedWeakReference.Recover(typeof(Options), 0);
        //    if (restoreReference != null && restoreReference.Target != null)
        //    {
        //        options = (Options)restoreReference.Target; // If they do, use them to refresh current settings.
        //        restoreReference.PushBackIntoRecoverList(); // Since we found the data, we have to put them back.
        //    }
        //    else
        //        Debug.Print("Could not restore settings.");
        //}

        public static Options LoadFromSD()
        {
            Options res = new Options();

            byte[] data = DriveManager.LoadFromSD(fileName);
            if (data == null)
                res.SaveToSD();
            else
                res = Options.FromByteArray(data);

            return res ?? new Options();
        }
        public void SaveToSD()
        {
            DriveManager.SaveToSD(ToByteArray(), fileName);
        }
        #endregion
    }
}
