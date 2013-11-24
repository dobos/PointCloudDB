using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elte.PointCloudDB.CodeGen;

namespace Elte.PointCloudDB.Streams
{
    class TupleBlock<T> : TupleBlockBase
        where T : struct
    {
        private TupleHelper<T> helper;
        private T[] data;

        public TupleBlock(int blockSize, TupleHelper<T> helper)
            :base(blockSize)
        {
            this.helper = helper;
            this.data = new T[blockSize];
        }

        public override void AppendTuple(string[] values)
        {
            helper.Parse(ref data[ItemCount++], values);
        }
    }
}
