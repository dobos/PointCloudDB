using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Schema
{
    /// <summary>
    /// Represents a database table
    /// </summary>
    public class Table : SchemaObject
    {
        private SchemaObjectCollection<Column> columns;
        private SchemaObjectCollection<Projection> projections;

        public SchemaObjectCollection<Column> Columns
        {
            get { return columns; }
        }

        public SchemaObjectCollection<Projection> Projections
        {
            get { return projections; }
        }

        public Table()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.columns = new SchemaObjectCollection<Column>();
            this.projections = new SchemaObjectCollection<Projection>();
        }
    }
}
