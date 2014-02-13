using Elte.PointCloudDB.Schema;
using Elte.PointCloudDB.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.CodeGen
{
    public abstract class ColumnHelperBase
    {
        private SchemaObjectCollection<Column> columns;

        public SchemaObjectCollection<Column> Columns
        {
            get { return columns; }
        }

        public ColumnHelperBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.columns = new SchemaObjectCollection<Column>();
        }

        public abstract Type GetColumnsStructType();

        public abstract void SetColumns(SchemaObjectCollection<Column> columns);

        public abstract void SetColumnValuesAssigner(int i, Delegate assigner);

        public abstract StorageOfColumnChunksBase CreateChunk(int chunkSize);
    }
}
