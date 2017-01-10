# wazvhdsize v1.1
This version is based on Sandrino Di Mattia's [v1.0], and is intending to resolve the crash issue while working with highly fragment page blobs. More discussion on why and how to get the page ranges of a large page blob in segments can be found [here].  

### Crash analysis on Version 1.0:
    The page blob is quite fragment, which leads to the server timeout when calling “GetPageRanges()”.

### Implementation:
    Add another function “GetActualDiskSizeAsync” to populate page ranges by segment.
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


### Info:
    For highly fragment page blobs, please specify -pageRangeSize parameter expliclty, default size is 5GB. If -pageRangeSize is not specified, it just works exactly the same as v1.0.
### Usage:
    wazvhdsize.exe -name <accountName> -key <key> -uri <vhdUri or containerName> // This works exactly the same as v1.0
    wazvhdsize.exe -name <accountName> -key <key> -uri <vhdUri or containerName> -pageRangeSize 5 // For highly fragment page blob.
#### Optional Paramters:
		-pageRangeSize <5> // Default Value: 5GB.
		-env <Mooncake | Global> // Default Env: Mooncake
#### Example:
 	.\wazvhdsize.exe -name appendstore  -key y2YkfsNZbBnWGfPzB4baOCHRKw0DiXNFuyg0KZSovh0wLpcnEi4oWIqKh56qndFf9Tm3w== -uri https://appendstore.blob.core.chinacloudapi.cn/vhds/geocenos01-geoc.vhd

### Version
    v1.1
	
##### How to TRIM unused spage in Page blob is discussed in this [blog]. (Chinese Version)
[v1.0]:<https://github.com/sandrinodimattia/WindowsAzure-VhdSize/releases/tag/v1.0>
[here]:<https://blogs.msdn.microsoft.com/windowsazurestorage/2012/03/26/getting-the-page-ranges-of-a-large-page-blob-in-segments/>
[blog]:<https://www.azure.cn/documentation/articles/aog-billing-delete-unused-vhd-to-reduce-cost>
