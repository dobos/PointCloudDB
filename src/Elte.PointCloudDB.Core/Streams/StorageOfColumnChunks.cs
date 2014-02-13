using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elte.PointCloudDB.CodeGen;

namespace Elte.PointCloudDB.Streams
{
    class StorageOfColumnChunks<C, T> : StorageOfColumnChunksBase
        where C : class, new()
        where T : struct
    {
        private C columnChunks;
        private ColumnHelper<C, T> helper;

        public StorageOfColumnChunks(int chunkSize, ColumnHelper<C, T> helper)
            : base(chunkSize)
        {
            this.columnChunks = new C();
            this.helper = helper;
        }

        protected override Object GetColumnChunksImpl()
        {
            return columnChunks;
        }

        public override void AssignColumnValues(TupleChunkBase tupleChunk)
        {
            helper.Assign((T[])tupleChunk.Data, columnChunks, ChunkSize);
        }
    }
}
