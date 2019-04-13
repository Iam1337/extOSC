/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System.Net;
using System.Collections.Generic;

using extOSC.Core;

namespace extOSC
{
    public class OSCBundle : OSCPacket
    {
        #region Static Public Methods

        public static OSCBundle Create(params OSCPacket[] packets)
        {
            return new OSCBundle(packets);
        }

        #endregion
		
	    #region Constants

        public const string KBundle = "#bundle";

        #endregion

        #region Public Vars

        public long TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }

        public override string Address
        {
            get { return KBundle; }
        }

        public override IPAddress Ip
        {
            get { return base.Ip; }
            set
            {
                foreach (var packet in packets)
                    packet.Ip = value;

                base.Ip = value;
            }
        }

        public List<OSCPacket> Packets
        {
            get { return packets; }
            set
            {
                if (packets == value)
                    return;

                packets = value;
            }
        }

        #endregion

        #region Protected Vars

        protected List<OSCPacket> packets = new List<OSCPacket>();

        protected long timeStamp;

        #endregion

        #region Public Methods

        public OSCBundle() : this(null) { }

        public OSCBundle(params OSCPacket[] packets)
        {
            address = KBundle;

            if (packets != null)
            {
                foreach (var value in packets)
                    AddPacket(value);
            }
        }

        public void AddPacket(OSCPacket packet)
        {
            if (packet != null)
            {
                packets.Add(packet);
            }
        }
		
	    public override string ToString()
        {
            var stringValues = string.Empty;

            if (packets.Count > 0)
            {
                foreach (var packet in packets)
                {
                    stringValues += string.Format("[{0}], ", packet);
                }

                stringValues = string.Format("({0})", stringValues.Remove(stringValues.Length - 2));
            }

            return string.Format("<{0}:\"{1}\"> : {2}", GetType().Name, address, string.IsNullOrEmpty(stringValues) ? "null" : stringValues);
        }

        #endregion
    }
}