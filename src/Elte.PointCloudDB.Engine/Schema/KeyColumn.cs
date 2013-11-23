using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Engine.Schema
{
    public class KeyColumn : Column
    {
        private SortOrder sortOrder;

        public SortOrder SortOrder
        {
            get { return sortOrder; }
        }
    }
}
