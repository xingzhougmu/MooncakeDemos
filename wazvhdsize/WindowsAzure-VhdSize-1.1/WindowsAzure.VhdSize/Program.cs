using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;

namespace WindowsAzure.VhdSize
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parse Arguments
            Helper.SETTING g_setting = new Helper.SETTING();
            if (!Helper.parse_arg(args.Length, args, out g_setting))
            {
                Helper.print_usage(args.Length, args);
                // Console.ReadKey();
                return;
            }
            Console.WriteLine();

            // Get the uri.
            var uri = g_setting.uri;
            Console.WriteLine(" Processing: {0}", uri);
            Console.WriteLine("");

            try
            {
                // Init client and blob list. Endpoint is set to Mooncake by default.                                
                var client = new CloudStorageAccount(new StorageCredentials(g_setting.accountName, g_setting.storagekey), ConfigurationManager.AppSettings[g_setting.environment.ToLower()], false).CreateCloudBlobClient();

                var isBlobUri = false;
                var blobs = new List<CloudPageBlob>();

                // It's an uri.
                if (uri.StartsWith("http://") || uri.StartsWith("https://"))
                {
                    var blob = client.GetBlobReferenceFromServer(new Uri(uri)) as CloudPageBlob;
                    if (blob == null)
                        throw new FileNotFoundException("Unable to find the Page Blob.");
                    blobs.Add(blob);
                    isBlobUri = true;
                }
                else
                {
                    // It's a container.
                    var container = client.GetContainerReference(uri);
                    if (!container.Exists())
                        throw new InvalidOperationException("Container does not exist: " + uri);
                    blobs.AddRange(container.ListBlobs().OfType<CloudPageBlob>());
                }

                // Show blob sizes.
                foreach (var blob in blobs)
                {
                    if (!isBlobUri)
                        Console.WriteLine(" Blob: {0}", blob.Uri);

                    // Display length.
                    Console.WriteLine(" > Size: {0} ({1} bytes)", PageBlobExtensions.GetFormattedDiskSize(blob.Properties.Length), blob.Properties.Length);

                    // Calculate size.
                    if (Int64.Parse(g_setting.pageRangeSize) > 0)
                    {
                        var size = blob.GetActualDiskSizeAsync(Int64.Parse(g_setting.pageRangeSize));
                        Console.WriteLine(" > Actual/Billable Size: {0} ({1} bytes)", PageBlobExtensions.GetFormattedDiskSize(size), size);
                        Console.WriteLine("");
                    }
                    else
                    {
                        var size = blob.GetActualDiskSize();
                        Console.WriteLine(" > Actual/Billable Size: {0} ({1} bytes)", PageBlobExtensions.GetFormattedDiskSize(size), size);
                        Console.WriteLine("");
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Calulation completed, press any key to continue...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error:");
                Console.WriteLine(" {0}", ex);
                Console.ReadLine();
            }
        }
    }
}
