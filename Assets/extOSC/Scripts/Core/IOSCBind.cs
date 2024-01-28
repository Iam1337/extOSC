/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using extOSC.Core.Events;

namespace extOSC.Core
{
    public interface IOSCBind
    {
        #region Public Vars

        OSCEventMessage Callback { get; }

        string ReceiverAddress { get; }

        #endregion
    }
}