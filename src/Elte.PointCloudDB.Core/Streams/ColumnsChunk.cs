using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elte.PointCloudDB.CodeGen;

namespace Elte.PointCloudDB.Streams
{
    class ColumnsChunk<C, T> : ColumnsChunkBase
        where C : class, new()
        where T : struct
    {
        private C data;
        private ColumnHelper<C, T> helper;

        public ColumnsChunk(int chunkSize, ColumnHelper<C, T> helper)
            : base(chunkSize)
        {
            this.data = new C();
            this.helper = helper;
        }

        protected override Object GetDataImpl()
        {
            return data;
        }

        public override void AllocateColumnFields()
        {
            helper.Allocate(data, ChunkSize);
        }

        public override void AssignColumnValues(TupleChunkBase tupleChunk)
        {
            helper.Assign((T[])tupleChunk.Data, data, ChunkSize);
        }
    }
}
