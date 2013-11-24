using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elte.PointCloudDB.Schema;
using Elte.PointCloudDB.Streams;

namespace Elte.PointCloudDB.CodeGen
{
    /// <summary>
    /// Defines the function signatures for a non-type tuple helper class.
    /// </summary>
    public abstract class TupleHelperBase
    {
        private SchemaObjectCollection<Column> columns;

        public SchemaObjectCollection<Column> Columns
        {
            get { return columns; }
        }

        public TupleHelperBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.columns = new SchemaObjectCollection<Column>();
        }
        
        public abstract Type GetTupleType();

        public abstract void SetColumns(SchemaObjectCollection<Column> columns);

        public abstract void SetParseColumnValueDelegate(int i, Delegate parser);

        public abstract TupleChunkBase CreateBlock(int blockSize);
    }
}
