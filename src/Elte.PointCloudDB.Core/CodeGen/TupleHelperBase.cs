using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elte.PointCloudDB.Schema;
using Elte.PointCloudDB.Streams;

namespace Elte.PointCloudDB.CodeGen
{
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

        public abstract TupleBlockBase CreateBlock(int blockSize);
    }
}
