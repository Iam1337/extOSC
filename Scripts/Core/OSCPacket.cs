/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System;
using System.Net;

namespace extOSC.Core
{
	public abstract class OSCPacket
	{
		#region Static Public Methods

		public static string ToBase64String(OSCPacket packet)
		{
		    var length = 0;
		    var buffer = OSCConverter.Pack(packet, out length);

		    return Convert.ToBase64String(buffer, 0, length);
		}

		public static OSCPacket FromBase64String(string base64String)
		{
			return OSCConverter.Unpack(Convert.FromBase64String(base64String));
		}

		#endregion

		#region Public Vars

		public virtual int Port
		{
			get { return port; }
			set { port = value; }
		}

		public virtual IPAddress Ip
		{
			get { return ip; }
			set { ip = value; }
		}

		public virtual string Address
		{
			get { return address; }
			set { address = value; }
		}

		#endregion

		#region Protected Vars

		protected int port;

		protected IPAddress ip;

		protected string address;

		#endregion

		#region Public Methods

		public bool IsBundle()
		{
			return  OSCUtilities.IsSubclassOf(GetType(), typeof(OSCBundle));
		}

		public OSCPacket Copy()
		{
			var size = 0;
			return OSCConverter.Unpack(OSCConverter.Pack(this, out size), size);
		}

		#endregion
	}
}