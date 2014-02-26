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
using Elte.PointCloudDB.CodeGen;

namespace Elte.PointCloudDB.Operators
{
    public class Untangle : OperatorBase
    {
        private TransformBlock<TupleChunkBase, ColumnsChunkBase> columnSeparator;
        private int maxDegreeOfParallelism;
        private int chunkSize;
        private ColumnHelperBase columnHelper;

        public int ChunkSize
        {
            get { return chunkSize; }
            set { chunkSize = value; }
        }

        public Untangle(SchemaObjectCollection<Column> columns, int chunkSize, int maxDegreeOfParallelism)
        {
            InitializeMembers(columns, chunkSize, maxDegreeOfParallelism);
            
        }

        private void InitializeMembers(SchemaObjectCollection<Column> columns, int chunkSize, int maxDegreeOfParallelism)
        {
            this.columnSeparator = null;
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
            this.chunkSize = chunkSize;
            this.columnHelper = ColumnFactory.Instance.GetColumnHelper(columns, chunkSize);
        }
        
        public override void Initialize()
        {
            columnSeparator = new TransformBlock<TupleChunkBase, ColumnsChunkBase>(tupleChunk => ConvertChunk(tupleChunk));
        }

        public override void Uninitialize()
        {
            columnSeparator = null;
        }

        public void LinkFromWorkerBlockTo(ITargetBlock<ColumnsChunkBase> targetBlock)
        {
            columnSeparator.LinkTo(targetBlock);
        }

        public ColumnsChunkBase ConvertChunk(TupleChunkBase tupleChunk)
        {
            ColumnsChunkBase columnsChunk = null;

            // Create a new chunk for storing the columns
            if (columnsChunk == null)
            {
                columnsChunk = columnHelper.CreateChunk(chunkSize);
            }

            columnsChunk.AssignColumnValues(tupleChunk);

            return columnsChunk;
        }
    }
}
