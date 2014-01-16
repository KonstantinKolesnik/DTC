using System;

namespace DTC.API.Decoders
{
    public class LocomotiveDecoder : Decoder
    {
        private int speed = 0;
        private bool direction = true;



        public LocomotiveDecoder(Guid id)
            : base(id, DecoderType.Locomotive)
        {
        }
        public LocomotiveDecoder(Guid id, string name)
            : base(id, DecoderType.Locomotive, name)
        {
        }







    }
}
