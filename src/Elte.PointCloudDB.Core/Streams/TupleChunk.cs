using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elte.PointCloudDB.CodeGen;

namespace Elte.PointCloudDB.Streams
{
    class TupleChunk<T> : TupleChunkBase
        where T : struct
    {
        private TupleHelper<T> helper;
        private T[] data;

        public TupleChunk(int chunkSize, TupleHelper<T> helper)
            :base(chunkSize)
        {
            this.helper = helper;
            this.data = new T[chunkSize];
        }

        public override void AppendTuple(string[] values)
        {
            helper.Parse(ref data[ChunkPos++], values);
        }
    }
}
