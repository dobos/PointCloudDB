using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elte.PointCloudDB.Operators
{
    public abstract class OperatorBase
    {
        public abstract void Initialize();
        public abstract void Uninitialize();
    }
}
