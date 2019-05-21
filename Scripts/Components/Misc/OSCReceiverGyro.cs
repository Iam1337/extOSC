/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Components.Misc
{
    [AddComponentMenu("extOSC/Components/Misc/Gyro")]
    public class OSCReceiverGyro : OSCReceiverComponent
    {
        #region Extensions

        public enum GyroMode
        {
            Other,

            TouchOSC
        }

        #endregion

        #region Public Vars

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public GyroMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        #endregion

        #region Protected Vars

        [SerializeField]
        protected float speed = 1;

        [SerializeField]
        protected GyroMode mode = GyroMode.TouchOSC;

        protected Quaternion defaultRotation;

        #endregion

        #region Unity Methods

        protected virtual void Start()
        {
            defaultRotation = transform.localRotation;
        }

        #endregion

        #region Protected Methods

        protected override void Invoke(OSCMessage message)
        {
            Vector3 value;

            if (message.ToVector3(out value))
            {
                var rotation = mode == GyroMode.Other ?
                                               OtherProcess(value) :
                                               TouchOSCProcess(value);

                rotation *= defaultRotation;

                transform.localRotation = Quaternion.Slerp(transform.localRotation, rotation, Time.deltaTime * Speed);
            }
        }

        protected Quaternion TouchOSCProcess(Vector3 value)
        {
            return Quaternion.Euler(value.y * 90, 0, -value.x * 90);
        }

        protected Quaternion OtherProcess(Vector3 value)
        {
            return Quaternion.Euler(value.x, value.y, value.z);
        }

        #endregion
    }
}