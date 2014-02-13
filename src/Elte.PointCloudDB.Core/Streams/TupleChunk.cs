using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elte.PointCloudDB.CodeGen;
using Elte.PointCloudDB.Schema;

namespace Elte.PointCloudDB.Streams
{
    public class TupleChunk<T> : TupleChunkBase
        where T : struct
    {
        private TupleHelper<T> helper;
        private T[] data;

        // public T[] Data
        //{
        //    get { return data; }
        //}

        public TupleChunk(int chunkSize, TupleHelper<T> helper)
            :base(chunkSize)
        {
            this.helper = helper;
            this.data = new T[chunkSize];
        }

        protected override Object GetDataImpl()
        {
            return data;
        }

        public override void AppendTuple(string[] values)
        {
            helper.Parse(ref data[ChunkPos++], values);
        }
    }
}
