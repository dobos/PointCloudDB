﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Elte.PointCloudDB.Schema;
using Elte.PointCloudDB.Streams;
using Elte.PointCloudDB.Storage;

namespace Elte.PointCloudDB.Operators
{
    /// <summary>
    /// 
    /// </summary>
    public class BulkRead : OperatorBase, ITupleStreamSource
    {
        private BulkFileReaderBase[] readers;
        private TransformManyBlock<BulkFileReaderBase, TupleChunkBase> workerBlock;

        public ISourceBlock<TupleChunkBase> SourceBlock
        {
            get { return workerBlock; }
        }

        public BulkRead(BulkFileReaderBase[] readers)
        {
            this.readers = readers;
            this.workerBlock = null;
        }

        public override void Initialize()
        {
            workerBlock = new TransformManyBlock<BulkFileReaderBase, TupleChunkBase>((Func<BulkFileReaderBase, IEnumerable<TupleChunkBase>>)BulkReadWorker);
        }

        public override void Uninitialize()
        {
            workerBlock = null;
        }

        public void Execute()
        {
            for (int i = 0; i < readers.Length; i++)
            {
                workerBlock.Post(readers[i]);
            }

            workerBlock.Complete();
        }

        private IEnumerable<TupleChunkBase> BulkReadWorker(BulkFileReaderBase reader)
        {
            reader.Open();

            foreach (var tc in reader.ReadChunks())
            {
                yield return tc;
            }

            reader.Close();
            reader.Dispose();
        }

    }
}
