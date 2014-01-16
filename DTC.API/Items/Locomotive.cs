using DTC.API.Decoders;
using System;

namespace DTC.API.Items
{
    public class Locomotive : LocomotiveDecoder
    {
        public Locomotive(Guid id)
            : base(id)
        {
        }
        public Locomotive(Guid id, string name)
            : base(id, name)
        {
        }
    }
}
