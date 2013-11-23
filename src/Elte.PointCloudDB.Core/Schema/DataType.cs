using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Schema
{
    /// <summary>
    /// Represents a database column type.
    /// </summary>
    public class DataType
    {
        public static DataType Byte
        {
            get
            {
                return new DataType()
                {
                    id = "ui1",
                    dotNetType = typeof(Byte),
                    size = sizeof(Byte)
                };
            }
        }

        public static DataType SByte
        {
            get
            {
                return new DataType()
                {
                    id = "i1",
                    dotNetType = typeof(SByte),
                    size = sizeof(SByte)
                };
            }
        }

        public static DataType UInt16
        {
            get
            {
                return new DataType()
                {
                    id = "ui2",
                    dotNetType = typeof(UInt16),
                    size = sizeof(UInt16)
                };
            }
        }

        public static DataType Int16
        {
            get
            {
                return new DataType()
                {
                    id = "i2",
                    dotNetType = typeof(Int16),
                    size = sizeof(Int16)
                };
            }
        }

        public static DataType UInt32
        {
            get
            {
                return new DataType()
                {
                    id = "ui4",
                    dotNetType = typeof(UInt32),
                    size = sizeof(UInt32)
                };
            }
        }

        public static DataType Int32
        {
            get
            {
                return new DataType()
                {
                    id = "i4",
                    dotNetType = typeof(Int32),
                    size = sizeof(Int32)
                };
            }
        }

        public static DataType UInt64
        {
            get
            {
                return new DataType()
                {
                    id = "ui8",
                    dotNetType = typeof(UInt64),
                    size = sizeof(UInt64)
                };
            }
        }

        public static DataType Int64
        {
            get
            {
                return new DataType()
                {
                    id = "i8",
                    dotNetType = typeof(Int64),
                    size = sizeof(Int64)
                };
            }
        }

        public static DataType Single
        {
            get
            {
                return new DataType()
                {
                    id = "f4",
                    dotNetType = typeof(Single),
                    size = sizeof(Single)
                };
            }
        }

        public static DataType Double
        {
            get
            {
                return new DataType()
                {
                    id = "f8",
                    dotNetType = typeof(Double),
                    size = sizeof(Double)
                };
            }
        }

        private string id;
        private Type dotNetType;
        private int size;

        internal string ID
        {
            get { return id; }
        }

        public Type DotNetType
        {
            get { return dotNetType; }
        }

        public int Size
        {
            get { return size; }
        }

        private DataType()
        {
        }
    }
}
