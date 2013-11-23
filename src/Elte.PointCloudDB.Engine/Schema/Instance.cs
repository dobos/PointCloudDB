using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Engine.Schema
{
    /// <summary>
    /// Represents a database server instance. Should run as a singleton.
    /// </summary>
    public class Instance
    {
        private SchemaObjectCollection<Database> databases;

        public SchemaObjectCollection<Database> Databases
        {
            get { return databases; }
        }

        private Instance()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            databases = new SchemaObjectCollection<Database>();
        }
    }
}
