using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace WindowsAzure.VhdSize
{
    public static class PageBlobExtensions
    {
        // public static long pageBlobSize = 0;
        /// <summary>
        /// Based on this script: http://gallery.technet.microsoft.com/scriptcenter/Get-Billable-Size-of-32175802
        /// </summary>
        /// <returns></returns>
        public static long GetActualDiskSize(this CloudPageBlob pageBlob, bool PageRange = false)
        {
            pageBlob.FetchAttributes();

            if (PageRange)
            {
                long pageBlobSize = 0;
                long startOffset = 0;
                long rangeSize = 1024 * 1024 * 1024; //1 GB
                Console.Write("Calculating...");
                while (startOffset < pageBlob.Properties.Length)
                {
                    pageBlobSize += pageBlob.GetPageRanges(startOffset, rangeSize).Sum(r => 12 + (r.EndOffset - r.StartOffset));
                    startOffset += rangeSize;
                    Console.Write("...");
                }

                return 124 + pageBlob.Name.Length * 2 + pageBlob.Metadata.Sum(m => m.Key.Length + m.Value.Length + 3) + pageBlobSize;
            }
            else
                return 124 + pageBlob.Name.Length * 2 + pageBlob.Metadata.Sum(m => m.Key.Length + m.Value.Length + 3) + pageBlob.GetPageRanges().Sum(r => 12 + (r.EndOffset - r.StartOffset));
        }

        public static long GetActualDiskSizeAsync(this CloudPageBlob pageBlob, long range = 5 /* default is 5 GB */)
        {
            pageBlob.FetchAttributes();
            long pageBlobSize = 0;
            Object _Lock = new Object();

            long rangeSize = range * 1024 * 1024 * 1024;

            List<long> offsetList = new List<long>();
            long startOffset = 0;

            while (startOffset < pageBlob.Properties.Length)
            {
                offsetList.Add(startOffset);
                startOffset += rangeSize;
            }

            // add blob name size and metadata size
            pageBlobSize += (124 + pageBlob.Name.Length * 2 + pageBlob.Metadata.Sum(m => m.Key.Length + m.Value.Length + 3));

            Parallel.ForEach(offsetList, (currentOffset) =>
                                            {
                                                long tmp = pageBlob.GetPageRanges(currentOffset, rangeSize).Sum(r => 12 + (r.EndOffset - r.StartOffset));
                                                lock (_Lock)
                                                {
                                                    pageBlobSize += tmp;
                                                }
                                                // Console.WriteLine("Process {0} on thread {1}: {2}", currentOffset, Thread.CurrentThread.ManagedThreadId, GetFormattedDiskSize(pageBlobSize));
                                            });
            Task.WaitAll();
            return pageBlobSize;
        }

        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize);

        public static string GetFormattedDiskSize(long size)
        {
            var sb = new StringBuilder(11);
            StrFormatByteSize(size, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}