using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Schema
{
    public abstract class SchemaObject
    {
        private string name;

        public string Name 
        {
            get { return name; }
        }

        public SchemaObject()
        {
        }

        public SchemaObject(string name)
        {
            this.name = name;
        }
    }
}
