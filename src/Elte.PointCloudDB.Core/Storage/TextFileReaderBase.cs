using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Elte.PointCloudDB.Storage
{
    public abstract class TextFileReaderBase : BulkFileReaderBase, IDisposable
    {
        private StreamReader inputReader;

        protected StreamReader InputReader
        {
            get { return inputReader; }
        }

        protected TextFileReaderBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        public override void Dispose()
        {
            Close();

            base.Dispose();
        }

        public override void Open()
        {
            base.Open();

            if (inputReader != null)
            {
                throw new InvalidOperationException();
            }

            inputReader = new StreamReader(InputStream);
        }

        public override void Close()
        {
            if (inputReader != null)
            {
                inputReader.Close();
                inputReader.Dispose();
                inputReader = null;
            }

            base.Close();
        }
    }
}
