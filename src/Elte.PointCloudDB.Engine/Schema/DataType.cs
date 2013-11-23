using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Engine.Schema
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
                    dotNetType = typeof(Double),
                    size = sizeof(Double)
                };
            }
        }

        private Type dotNetType;
        private int size;

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
