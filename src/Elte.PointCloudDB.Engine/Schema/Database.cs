using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Schema
{
    /// <summary>
    /// Represents a database.
    /// </summary>
    public class Database : SchemaObject
    {
        private SchemaObjectCollection<Table> tables;

        public SchemaObjectCollection<Table> Tables
        {
            get { return tables; }
        }

        public Database()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.tables = new SchemaObjectCollection<Table>();
        }
    }
}
