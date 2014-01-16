using System;

namespace DTC.API.Decoders
{
    public class TrackDecoder : Decoder
    {
        private bool isOn = false;
        private int i = 0;
        private int v = 0;

        public TrackDecoder(Guid id)
            : base(id, DecoderType.Track)
        {
        }
        public TrackDecoder(Guid id, string name)
            : base(id, DecoderType.Track, name)
        {
        }


    }
}
