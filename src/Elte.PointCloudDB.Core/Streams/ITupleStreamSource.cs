﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Elte.PointCloudDB.Streams
{
    interface ITupleStreamSource
    {
        ISourceBlock<TupleChunkBase> SourceBlock { get; }
    }
}
