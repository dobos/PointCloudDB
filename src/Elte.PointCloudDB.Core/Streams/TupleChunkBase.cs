using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Streams
{
    public abstract class TupleChunkBase
    {
        int chunkSize;
        int chunkPos;

        public int ChunkSize
        {
            get { return chunkSize; }
        }

        public int ChunkPos
        {
            get { return chunkPos; }
            protected set { chunkPos = value; }
        }

        public bool IsFull
        {
            get { return chunkPos == chunkSize; }
        }

        protected TupleChunkBase(int chunkSize)
        {
            this.chunkSize = chunkSize;
            this.chunkPos = 0;
        }

        public abstract void AppendTuple(string[] parts);
    }
}
