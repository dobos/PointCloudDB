using Elte.PointCloudDB.CodeGen;
using Elte.PointCloudDB.Schema;
using Elte.PointCloudDB.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Utilities
{
    public class TupleToColumnsChunkConverter
    {
        private int chunkSize;
        private ColumnHelperBase columnHelper;

        public int ChunkSize
        {
            get { return chunkSize; }
            set { chunkSize = value; }
        }

        public TupleToColumnsChunkConverter(SchemaObjectCollection<Column> columns, int chunkSize)
        {
            InitializeMembers(columns, chunkSize);
        }

        private void InitializeMembers(SchemaObjectCollection<Column> columns, int chunkSize)
        {
            this.chunkSize = chunkSize;
            this.columnHelper = ColumnFactory.Instance.GetColumnHelper(columns, chunkSize);
        }

        public StorageOfColumnChunksBase ConvertChunk(TupleChunkBase tupleChunk)
        {
            StorageOfColumnChunksBase columnsChunk = null;

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
