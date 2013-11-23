using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Engine.Schema
{
    /// <summary>
    /// Represents a database table column.
    /// </summary>
    public class Column : SchemaObject
    {
        private DataType dataType;

        public DataType DataType
        {
            get { return dataType; }
        }
    }
}
