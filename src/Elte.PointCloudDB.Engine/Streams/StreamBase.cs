using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Engine.Streams
{
    private abstract class StreamBase
    {
        private Schema.SchemaObjectCollection<Schema.Column> columns;

        public Schema.SchemaObjectCollection<Schema.Column> Columns
        {
            get { return columns; }
        }

        public StreamBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.columns = new Schema.SchemaObjectCollection<Schema.Column>();
        }
    }
}
