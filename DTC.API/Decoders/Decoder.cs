using System;

namespace DTC.API.Decoders
{
    public abstract class Decoder
    {
        private Guid id;
        private DecoderType type;
        private string name;

        public Guid ID
        {
            get { return id; }
        }
        public DecoderType Type
        {
            get { return type; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Decoder(Guid id, DecoderType type)
        {
            this.id = id;
            this.type = type;
        }
        public Decoder(Guid id, DecoderType type, string name)
            : this(id, type)
        {
            this.name = name;
        }
    }
}
