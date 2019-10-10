 /* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

using extOSC.Core.Events;

 namespace extOSC.Components.Events
 {
	 [AddComponentMenu("extOSC/Components/Receiver/TimeTag Event")]
	 public class OSCReceiverEventTimeTag : OSCReceiverEvent<OSCEventDateTime>
	 {
		 #region Protected Methods

		 protected override void Invoke(OSCMessage message)
		 {
			 if (onReceive != null && message.ToTimeTag(out var timeTag))
			 {
				 onReceive.Invoke(timeTag);
			 }
		 }

		 #endregion
	 }
 }