using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgottenLandRandomizer.Util;

namespace ForgottenLandRandomizer.Types
{
    public class Yaml
    {
        private List<uint> stringOffsets;
        private List<string> strings;
        private List<uint> valuePointers;
        private List<uint> valueOffsets;

        public XData XData { get; private set; }
        public byte[] Version { get; private set; }

        public Data Root { get; private set; }

        public Yaml(Data root, Endianness endianness)
        {
            XData = new XData(endianness, new byte[] { 4, 0 });
            Version = new byte[] { 2, 0, 0, 0 };
            Root = root;
        }

        public Yaml(string path)
        {
            using (EndianBinaryReader reader = new EndianBinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                Read(reader);
            }
        }

        public Yaml(EndianBinaryReader reader)
        {
            Read(reader);
        }

        public enum Type : int
        {
            Invalid,
            Int,
            Float,
            Bool,
            String,
            Hash,
            Array
        }

        void Read(EndianBinaryReader reader)
        {
            XData = new XData(reader);

            if (Encoding.UTF8.GetString(reader.ReadBytes(4)) != "YAML")
                throw new InvalidDataException("Invalid Yaml file");

            Version = reader.ReadBytes(4);
            Root = new Data(reader);
        }

        public void Write(string path)
        {
            using (EndianBinaryWriter writer = new EndianBinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
                Write(writer);
        }

        public void Write(EndianBinaryWriter writer)
        {
            if (Version[0] == 4)
            {
                writer.Write(new byte[] {
                0x58, 0x42, 0x49, 0x4E, 0x34, 0x12, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0xFD, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x59, 0x41, 0x4D, 0x4C, 0x02, 0x00, 0x00, 0x00 });
            }
            else if (Version[0] == 2)
            {
                writer.Write(new byte[] {
                0x58, 0x42, 0x49, 0x4E, 0x34, 0x12, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE9, 0xFD, 0x00, 0x00,
                0x59, 0x41, 0x4D, 0x4C, 0x02, 0x00, 0x00, 0x00 });
            }

            writer.Write((int)Root.Type);

            stringOffsets = new List<uint>();
            strings = new List<string>();
            valuePointers = new List<uint>();
            valueOffsets = new List<uint>();

            //Console.WriteLine("Reading nodes");
            if (Root.Type == Type.Hash)
            {
                writer.Write(Root.Length);
                uint pos = (uint)writer.BaseStream.Position;
                for (int i = 0; i < Root.Length; i++)
                {
                    writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                }
                for (int i = 0; i < Root.Length; i++)
                {
                    writer.BaseStream.Seek(pos, SeekOrigin.Begin);
                    strings.Add(Root.Key(i));
                    stringOffsets.Add((uint)writer.BaseStream.Position);
                    writer.BaseStream.Seek(0x4, SeekOrigin.Current);
                    valuePointers.Add((uint)writer.BaseStream.Position);
                    writer.BaseStream.Seek(0x4, SeekOrigin.Current);
                    pos = (uint)writer.BaseStream.Position;
                    WriteNode(writer, Root[Root.Key(i)]);
                }
            }
            else if (Root.Type == Type.Array)
            {
                writer.Write(Root[0].Length);
                uint pos = (uint)writer.BaseStream.Position;
                for (int i = 0; i < Root[0].Length; i++)
                {
                    writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                }
                for (int i = 0; i < Root[0].Length; i++)
                {
                    writer.BaseStream.Seek(pos, SeekOrigin.Begin);
                    valuePointers.Add((uint)writer.BaseStream.Position);
                    writer.BaseStream.Seek(0x4, SeekOrigin.Current);
                    pos = (uint)writer.BaseStream.Position;
                    WriteNode(writer, Root[0][Root[0].Key(i)]);
                }
            }

            //Console.WriteLine("Inserting string offsets");
            //Console.WriteLine($"strings: {strings.Count} - offsets: {stringOffsets.Count}");
            for (int i = 0; i < strings.Count; i++)
            {
                //Console.WriteLine($"{strings[i]} - 0x{stringOffsets[i].ToString("X8")}");
                writer.BaseStream.Seek(0, SeekOrigin.End);
                uint pos = (uint)writer.BaseStream.Position;
                writer.Write(strings[i].Length);
                writer.Write(strings[i].ToCharArray());
                writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                while ((writer.BaseStream.Length).ToString("X").Last() != '0' && (writer.BaseStream.Length).ToString("X").Last() != '4' && (writer.BaseStream.Length).ToString("X").Last() != '8' && (writer.BaseStream.Length).ToString("X").Last() != 'C')
                {
                    writer.Write((byte)0);
                }
                writer.BaseStream.Seek(stringOffsets[i], SeekOrigin.Begin);
                writer.Write(pos);
            }

            //Console.WriteLine("Insetings value offsets");
            //Console.WriteLine($"values: {valueOffsets.Count} - offsets: {valuePointers.Count}");
            for (int i = 0; i < valueOffsets.Count; i++)
            {
                //Console.WriteLine($"offset to value: 0x{valueOffsets[i].ToString("X8")} - offset to offset: 0x{valuePointers[i].ToString("X8")}");
                writer.BaseStream.Seek(valuePointers[i], SeekOrigin.Begin);
                writer.Write(valueOffsets[i]);
            }
            writer.BaseStream.Seek(0, SeekOrigin.End);
            uint rlocPos = (uint)writer.BaseStream.Position;
            if (Version[0] == 4)
            {
                writer.Write(Encoding.UTF8.GetBytes("RLOC".ToCharArray()));
                writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                writer.BaseStream.Seek(0x10, SeekOrigin.Begin);
                writer.Write(rlocPos);
            }
            writer.BaseStream.Seek(0x8, SeekOrigin.Begin);
            writer.Write(rlocPos);

            writer.Flush();
            writer.Dispose();
            writer.Close();
        }

        public void WriteNode(EndianBinaryWriter writer, Data node)
        {
            writer.BaseStream.Seek(0, SeekOrigin.End);
            //Console.WriteLine($"Reading node {node.Text} : {node.Name} - 0x{writer.BaseStream.Position.ToString("X8")}");
            valueOffsets.Add((uint)writer.BaseStream.Position);
            switch (node.Type)
            {
                case Type.Int:
                    {
                        writer.Write(1);
                        writer.Write(node.ToInt());
                        break;
                    }
                case Type.Float:
                    {
                        writer.Write(2);
                        writer.Write(node.ToFloat());
                        break;
                    }
                case Type.Bool:
                    {
                        writer.Write(3);
                        if (node.ToBool())
                        {
                            writer.Write(1);
                        }
                        else
                        {
                            writer.Write(0);
                        }
                        break;
                    }
                case Type.String:
                    {
                        writer.Write(4);
                        strings.Add(node.ToString());
                        stringOffsets.Add((uint)writer.BaseStream.Position);
                        writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        break;
                    }
                case Type.Hash:
                    {
                        writer.Write(5);
                        writer.Write(node.Length);
                        uint pos = (uint)writer.BaseStream.Position;
                        for (int i = 0; i < node.Length; i++)
                        {
                            writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
                        }
                        for (int i = 0; i < node.Length; i++)
                        {
                            writer.BaseStream.Seek(pos, SeekOrigin.Begin);
                            strings.Add(node.Key(i));
                            stringOffsets.Add((uint)writer.BaseStream.Position);
                            writer.BaseStream.Seek(0x4, SeekOrigin.Current);
                            valuePointers.Add((uint)writer.BaseStream.Position);
                            writer.BaseStream.Seek(0x4, SeekOrigin.Current);
                            pos = (uint)writer.BaseStream.Position;
                            WriteNode(writer, node[node.Key(i)]);
                        }
                        break;
                    }
                case Type.Array:
                    {
                        writer.Write(6);
                        writer.Write(node[0].Length);
                        uint pos = (uint)writer.BaseStream.Position;
                        for (int i = 0; i < node[0].Length; i++)
                        {
                            writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00 });
                        }
                        for (int i = 0; i < node[0].Length; i++)
                        {
                            writer.BaseStream.Seek(pos, SeekOrigin.Begin);
                            valuePointers.Add((uint)writer.BaseStream.Position);
                            writer.BaseStream.Seek(0x4, SeekOrigin.Current);
                            pos = (uint)writer.BaseStream.Position;
                            WriteNode(writer, node[0][node[0].Key(i)]);
                        }
                        break;
                    }
            }
        }

        public class Data
        {
            public Type Type { get; private set; }

            private byte[] data;
            private Dictionary<string, Data> children;

            public Data(Type type, object value)
            {
                Type = type;
                Set(value);
            }

            public Data(EndianBinaryReader reader)
            {
                Type = (Type)reader.ReadInt32();
                switch (Type)
                {
                    default:
                    case Type.Invalid:
                        break;
                    case Type.Int:
                    case Type.Float:
                    case Type.Bool:
                        {
                            data = reader.ReadBytes(4);
                            break;
                        }
                    case Type.String:
                        {
                            uint offs = reader.ReadUInt32();
                            long pos = reader.BaseStream.Position;
                            reader.BaseStream.Seek(offs, SeekOrigin.Begin);
                            data = reader.ReadBytes(reader.ReadInt32());
                            reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                            break;
                        }
                    case Type.Hash:
                        {
                            children = new Dictionary<string, Data>();
                            int count = reader.ReadInt32();
                            long pos = reader.BaseStream.Position;
                            for (int i = 0; i < count; i++)
                            {
                                reader.BaseStream.Seek(pos + i * 8, SeekOrigin.Begin);
                                string key = ReadUtil.ReadString(reader);
                                reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);

                                children.Add(key, new Data(reader));
                            }
                            break;
                        }
                    case Type.Array:
                        {
                            children = new Dictionary<string, Data>();
                            int count = reader.ReadInt32();
                            long pos = reader.BaseStream.Position;
                            for (int i = 0; i < count; i++)
                            {
                                reader.BaseStream.Seek(pos + i * 4, SeekOrigin.Begin);
                                reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);

                                children.Add(i.ToString(), new Data(reader));
                            }
                            break;
                        }
                }
            }

            public void Set(object value)
            {
                children = null;
                switch (Type)
                {
                    case Type.Invalid:
                        {
                            data = new byte[] { };
                            break;
                        }
                    case Type.Int:
                        {
                            data = BitConverter.GetBytes((int)value);
                            break;
                        }
                    case Type.Float:
                        {
                            data = BitConverter.GetBytes((float)value);
                            break;
                        }
                    case Type.Bool:
                        {
                            data = BitConverter.GetBytes(Convert.ToInt32((bool)value));
                            break;
                        }
                    case Type.String:
                        {
                            data = Encoding.UTF8.GetBytes((string)value);
                            break;
                        }
                    case Type.Hash:
                        {
                            children = new Dictionary<string, Data>();
                            Dictionary<string, Data> newChildren = (Dictionary<string, Data>)value;
                            foreach (KeyValuePair<string, Data> pair in newChildren)
                                children.Add(pair.Key, pair.Value);
                            break;
                        }
                    case Type.Array:
                        {
                            children = new Dictionary<string, Data>();
                            Dictionary<string, Data> newChildren = (Dictionary<string, Data>)value;
                            for (int i = 0; i < newChildren.Count; i++)
                                children.Add(i.ToString(), newChildren.Values.ElementAt(i));
                            break;
                        }
                }
            }

            public int ToInt()
            {
                if (Type == Type.Int)
                    return BitConverter.ToInt32(data, 0);
                return 0;
            }

            public float ToFloat()
            {
                if (Type == Type.Float)
                    return BitConverter.ToSingle(data, 0);
                return 0;
            }

            public bool ToBool()
            {
                if (Type == Type.Bool)
                    return BitConverter.ToBoolean(data, 0);
                return false;
            }

            public override string ToString()
            {
                if (Type == Type.String)
                    return Encoding.UTF8.GetString(data);
                return string.Empty;
            }

            public int Length
            {
                get
                {
                    if (Type == Type.Hash || Type == Type.Array)
                        return children.Count;
                    return 0;
                }
            }

            public string Key(int index)
            {
                if (index >= 0 && Type == Type.Hash && index < Length)
                    return children.Keys.ElementAt(index);
                return string.Empty;
            }

            public bool HasKey(string key)
            {
                if (Type == Type.Hash && Length > 0)
                    return children.Keys.Contains(key);
                return false;
            }

            public void Add(string key, Data value)
            {
                if (Type != Type.Hash)
                    return;

                children.Add(key, value);
            }

            public void Add(Data value)
            {
                if (Type != Type.Array)
                    return;

                children.Add(Length.ToString(), value);
            }

            public Data this[int i]
            {
                get
                {
                    if (Type == Type.Array && i > -1 && i < Length)
                        return children[i.ToString()];
                    return null;
                }
                set
                {
                    if (Type == Type.Array && i > -1 && i < Length)
                        children[i.ToString()] = value;
                }
            }

            public Data this[string i]
            {
                get
                {
                    if (Type == Type.Hash && HasKey(i))
                        return children[i];
                    return null;
                }
                set
                {
                    if (Type == Type.Hash && HasKey(i))
                        children[i.ToString()] = value;
                }
            }
        }
    }
}
