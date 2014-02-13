using Elte.PointCloudDB.Streams;
using Elte.PointCloudDB.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Reflection;
using Elte.PointCloudDB.Storage;
using Elte.PointCloudDB.Utilities;

namespace Elte.PointCloudDB.Operators
{
    public class Untangle : OperatorBase
    {
        private TupleToColumnsChunkConverter converter;
        private TransformBlock<TupleChunkBase, StorageOfColumnChunksBase> columnSeparator;
        private int maxDegreeOfParallelism;

        public Untangle(TupleToColumnsChunkConverter converter, int maxDegreeOfParallelism)
        {
            this.converter = converter;
            this.columnSeparator = null;
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }
        
        public override void Initialize()
        {
            columnSeparator = new TransformBlock<TupleChunkBase, StorageOfColumnChunksBase>(tupleChunk => converter.ConvertChunk(tupleChunk));
        }

        public override void Uninitialize()
        {
            columnSeparator = null;
        }

        public void LinkFromWorkerBlockTo(ITargetBlock<StorageOfColumnChunksBase> targetBlock)
        {
            columnSeparator.LinkTo(targetBlock);
        }
    }
}
