using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Streams
{
    public abstract class TupleBlockBase
    {
        int blockSize;
        int itemCount;

        public int BlockSize
        {
            get { return blockSize; }
        }

        public int ItemCount
        {
            get { return itemCount; }
            protected set { itemCount = value; }
        }

        public bool HasSpace
        {
            get { return itemCount < blockSize; }
        }

        protected TupleBlockBase(int blockSize)
        {
            this.blockSize = blockSize;
            this.itemCount = 0;
        }

        public abstract void AppendTuple(string[] parts);
    }
}
