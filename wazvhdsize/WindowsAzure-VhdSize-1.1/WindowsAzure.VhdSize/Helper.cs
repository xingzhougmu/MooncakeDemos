using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsAzure.VhdSize
{
    public static class Helper
    {
        public struct SETTING
        {
            public string accountName;
            public string storagekey;
            public string uri;
            public string pageRangeSize;
            public string environment;
        };
        public static bool  parse_arg(int argc, string[] argv, out SETTING g_setting)
        {
            g_setting = new SETTING();
            g_setting.pageRangeSize = "0";
            g_setting.environment = "Mooncake";

            if (argc < 6)
            {
                return false;
            }
            int i = 0;
            while (i < argc)
            {
                string arg = argv[i];
                if (arg == "-name")
                {
                    if (i + 1 < argc)
                    {
                        g_setting.accountName = argv[++i];
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (arg == "-key")
                {
                    if (i + 1 < argc)
                    {
                        g_setting.storagekey = argv[++i];
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (arg == "-uri")
                {
                    if (i + 1 < argc)
                    {
                        g_setting.uri = argv[++i];
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (arg == "-pageRangeSize")
                {
                    if (i + 1 < argc)
                    {
                        g_setting.pageRangeSize = argv[++i];
                    }
                    else
                    {
                        g_setting.pageRangeSize = "5";
                    }
                }
                else if (arg == "-env")
                {
                    if (i + 1 < argc)
                    {
                        g_setting.environment = argv[++i];
                    }
                    else
                    {
                        g_setting.environment = "Mooncake";
                    }
                }
                ++i;
            }

            return true;
        }

        public static void print_usage(int argc, string[] argv)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("Info: ");
            str.AppendLine("    For highly fragment page blobs, please specify -pageRangeSize parameter expliclty, default size is 5GB");
            str.AppendLine("usage: ");
            str.AppendLine("    wazvhdsize.exe " + "-name <accountName> " + "-key <key> " + "-uri <vhdUri or containerName>");
            str.AppendLine("    wazvhdsize.exe " + "-name <accountName> " + "-key <key> " + "-uri <vhdUri or containerName> " + "-pageRangeSize 5");
            str.AppendLine("Optional Paramters: ");
            str.AppendLine("    -pageRangeSize <5> " + "// Default Value: 5GB.");
            str.AppendLine("    -env <Mooncake | Global> " + "// Default Env: Mooncake");
            str.AppendLine("Example: ");
            str.AppendLine(@" .\wazvhdsize.exe -name appendstore  -key y2YkfsNZbBnWGfPzB4baOCHRKw0DiXNFuyg0KZSovh0wLpcnEi4oWIqKh56qndFf9Tm3w== -uri https://appendstore.blob.core.chinacloudapi.cn/vhds/geocenos01-geoc.vhd");
            Console.WriteLine(str);
        }

    }
}
