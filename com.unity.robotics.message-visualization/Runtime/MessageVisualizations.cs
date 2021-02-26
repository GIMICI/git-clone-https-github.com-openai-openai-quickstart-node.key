using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using System;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;

namespace Unity.Robotics.MessageVisualizers
{
    public interface IVisualizer
    {
        object CreateDrawing(Message message, MessageMetadata meta);
        void DeleteDrawing(object drawing);
        Action CreateGUI(Message message, MessageMetadata meta, object drawing);
    }

    public struct MessageMetadata
    {
        public readonly string topic;
        public readonly DateTime timestamp;

        public MessageMetadata(string topic, DateTime timestamp)
        {
            this.topic = topic;
            this.timestamp = timestamp;
        }
    }

    public static class MessageVisualizations
    {
        private static Dictionary<string, Tuple<IVisualizer, int>> TopicVisualizers = new Dictionary<string, Tuple<IVisualizer, int>>();
        private static Dictionary<Type, Tuple<IVisualizer, int>> TypeVisualizers = new Dictionary<Type, Tuple<IVisualizer, int>>();

        public static void RegisterVisualizer<MsgType>(IVisualizer config, int priority = 0)
        {
            RegisterVisualizer(typeof(MsgType), config, priority);
        }

        public static void RegisterVisualizer(Type MsgType, IVisualizer config, int priority = 0)
        {
            Tuple<IVisualizer, int> currentEntry;
            if (!TypeVisualizers.TryGetValue(MsgType, out currentEntry) || currentEntry.Item2 <= priority)
            {
                TypeVisualizers[MsgType] = new Tuple<IVisualizer, int>(config, priority);
            }
        }

        public static void RegisterVisualizer(string topic, IVisualizer config, int priority = 0)
        {
            Tuple<IVisualizer, int> currentEntry;
            if (!TopicVisualizers.TryGetValue(topic, out currentEntry) || currentEntry.Item2 <= priority)
            {
                TopicVisualizers[topic] = new Tuple<IVisualizer, int>(config, priority);
            }
        }

        public static IVisualizer GetVisualizer(Message message, MessageMetadata meta)
        {
            Tuple<IVisualizer, int> result;
            TopicVisualizers.TryGetValue(meta.topic, out result);
            if (result != null)
                return result.Item1;

            TypeVisualizers.TryGetValue(message.GetType(), out result);
            if (result != null)
                return result.Item1;

            return defaultVisualizer;
        }

        class DefaultVisualizer : IVisualizer
        {
            // If you're trying to register the default visualizer, something has gone extremely wrong...
            public void Register(int priority) { throw new NotImplementedException(); }

            public object CreateDrawing(Message message, MessageMetadata meta) => null;

            public void DeleteDrawing(object drawing) { }

            public Action CreateGUI(Message message, MessageMetadata meta, object drawing) => CreateDefaultGUI(message, meta);
        }

        static DefaultVisualizer defaultVisualizer = new DefaultVisualizer();

        public static Action CreateDefaultGUI(Message message, MessageMetadata meta)
        {
            string text = message.ToString();
            return () => { GUILayout.Label(text); };
        }

        public static void Draw<C>(DebugDraw.Drawing drawing, MPoint message, Color color, string label, float size = 0.01f) where C : ICoordinateSpace, new()
        {
            drawing.DrawPoint(message.From<C>(), color, size);
            drawing.DrawLabel(label, message.From<C>(), color, size * 1.5f);
        }

        public static void GUI(MHeader message)
        {
            GUILayout.Label($"<{message.seq} {message.frame_id} {TimeToString(message.stamp)}>");
        }

        public static void GUI(MTime message)
        {
            GUILayout.Label(TimeToString(message));
        }

        public static string TimeToString(MTime message)
        {
            // TODO: display a friendly date/time?
            return $"{message.secs}/{message.nsecs}";
        }

        public static void GUI(string name, MPoint message)
        {
            string body = $"[{message.x:F2}, {message.y:F2}, {message.z:F2}]";
            if (name == null || name == "")
                GUILayout.Label(body);
            else
                GUILayout.Label($"{name}: {body}");
        }

        public static void GUI(MPoint message)
        {
            GUILayout.Label($"[{message.x:F2}, {message.y:F2}, {message.z:F2}]");
        }

        public static void Draw<C>(DebugDraw.Drawing drawing, MPoint32 message, Color color, string label, float size = 0.01f) where C : ICoordinateSpace, new()
        {
            drawing.DrawPoint(message.From<C>(), color, size);
            drawing.DrawLabel(label, message.From<C>(), color, size * 1.5f);
        }

        public static void GUI(string name, MPoint32 message)
        {
            string body = $"[{message.x:F2}, {message.y:F2}, {message.z:F2}]";
            if (name == null || name == "")
                GUILayout.Label(body);
            else
                GUILayout.Label($"{name}: {body}");
        }

        public static void GUI(MPoint32 message)
        {
            GUILayout.Label($"[{message.x:F2}, {message.y:F2}, {message.z:F2}]");
        }

        public static void Draw<C>(DebugDraw.Drawing drawing, MVector3 message, Color color, string label, float size = 0.01f) where C : ICoordinateSpace, new()
        {
            drawing.DrawPoint(message.From<C>(), color, size);
            drawing.DrawLabel(label, message.From<C>(), color, size * 1.5f);
        }

        public static void GUI(string name, MVector3 message)
        {
            string body = $"[{message.x:F2}, {message.y:F2}, {message.z:F2}]";
            if (name == null || name == "")
                GUILayout.Label(body);
            else
                GUILayout.Label($"{name}: {body}");
        }

        public static void GUI(MVector3 message)
        {
            GUILayout.Label($"[{message.x:F2}, {message.y:F2}, {message.z:F2}]");
        }

        public static void Draw<C>(DebugDraw.Drawing drawing, MPose message, Color color, string label, float size = 0.01f) where C : ICoordinateSpace, new()
        {
            Draw<C>(drawing, message.position, color, label, size);
            UnityEngine.Vector3 point = message.position.From<C>();
            UnityEngine.Vector3 facing = message.orientation.From<C>() * UnityEngine.Vector3.forward;
            drawing.DrawLine(point, point + facing, color, size * 0.5f);
        }

        public static void GUI(MPose message)
        {
            GUI("Position", message.position);
            GUI("Orientation", message.orientation);
        }

        public static void Draw<C>(DebugDraw.Drawing drawing, MQuaternion message, GameObject drawAtPosition = null, float size = 0.01f) where C : ICoordinateSpace, new()
        {
            Vector3 position = drawAtPosition != null ? drawAtPosition.transform.position : Vector3.zero;
            Draw<C>(drawing, message, position, size);
        }

        public static void Draw<C>(DebugDraw.Drawing drawing, MQuaternion message, UnityEngine.Vector3 position, float size = 0.01f) where C : ICoordinateSpace, new()
        {
            UnityEngine.Quaternion quaternion = message.From<C>();
            UnityEngine.Vector3 right = quaternion * UnityEngine.Vector3.right * size;
            UnityEngine.Vector3 up = quaternion * UnityEngine.Vector3.up * size;
            UnityEngine.Vector3 forward = quaternion * UnityEngine.Vector3.forward * size;
            drawing.DrawLine(position, position + right, Color.red, size * 0.1f);
            drawing.DrawLine(position, position + up, Color.green, size * 0.1f);
            drawing.DrawLine(position, position + forward, Color.blue, size * 0.1f);
        }

        public static void GUI(string label, MQuaternion message)
        {
            if (label != "" && label != null)
                label += ": ";
            GUILayout.Label($"{label}[{message.x:F2}, {message.y:F2}, {message.z:F2}, {message.w:F2}]");
        }

        public static void GUI(MQuaternion message)
        {
            GUILayout.Label($"[{message.x:F2}, {message.y:F2}, {message.z:F2}, {message.w:F2}]");
        }

        public static void Draw<C>(DebugDraw.Drawing drawing, MTransform transform, float size = 0.01f) where C : ICoordinateSpace, new()
        {
            Draw<C>(drawing, transform.rotation, transform.translation.From<C>(), size);
        }

        public static void GUI(MTransform message)
        {
            GUI("Translation", message.translation);
            GUI("Rotation", message.rotation);
        }

        public static void Draw<C>(DebugDraw.Drawing drawing, MPolygon message, Color color, float thickness = 0.01f) where C : ICoordinateSpace, new()
        {
            Vector3 prevPos = message.points[message.points.Length - 1].From<FLU>();
            foreach (MPoint32 p in message.points)
            {
                Vector3 curPos = p.From<C>();
                drawing.DrawLine(prevPos, curPos, color, thickness);
                prevPos = curPos;
            }
        }

        public static void GUI(MPolygon message)
        {
            GUILayout.Label($"({message.points.Length} points):");
            foreach(MPoint32 p in message.points)
                GUI(p);
        }

        public static Color32 PickColorForTopic(string topic)
        {
            if (topic == "")
                return Color.black;

            byte[] bytes = BitConverter.GetBytes(topic.GetHashCode());
            return new Color32(bytes[0], bytes[1], bytes[2], 255);
        }
    }
}