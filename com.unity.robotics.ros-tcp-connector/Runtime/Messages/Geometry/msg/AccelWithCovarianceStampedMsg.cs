//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Geometry
{
    [Serializable]
    public class AccelWithCovarianceStampedMsg : Message
    {
        public const string k_RosMessageName = "geometry_msgs/AccelWithCovarianceStamped";

        //  This represents an estimated accel with reference coordinate frame and timestamp.
        public Std.HeaderMsg header;
        public AccelWithCovarianceMsg accel;

        public AccelWithCovarianceStampedMsg()
        {
            this.header = new Std.HeaderMsg();
            this.accel = new AccelWithCovarianceMsg();
        }

        public AccelWithCovarianceStampedMsg(Std.HeaderMsg header, AccelWithCovarianceMsg accel)
        {
            this.header = header;
            this.accel = accel;
        }

        public static AccelWithCovarianceStampedMsg Deserialize(MessageDeserializer deserializer) => new AccelWithCovarianceStampedMsg(deserializer);

        private AccelWithCovarianceStampedMsg(MessageDeserializer deserializer)
        {
            this.header = Std.HeaderMsg.Deserialize(deserializer);
            this.accel = AccelWithCovarianceMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.accel);
        }

        public override string ToString()
        {
            return "AccelWithCovarianceStampedMsg: " +
            "\nheader: " + header.ToString() +
            "\naccel: " + accel.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
