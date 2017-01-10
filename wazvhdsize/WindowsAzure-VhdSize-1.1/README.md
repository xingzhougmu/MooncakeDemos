V1.1 XingZhou 2016-06-07
1. Add another function “GetActualDiskSizeAsync” to populate page ranges by segment.
2.
	usage:
		wazvhdsize.exe -name <accountName> -key <key> -uri <vhdUri or containerName>
	Optional Paramters:
		-pageRangeSize <5> // Default Value: 5GB
		-env <Mooncake | Global> // Default Env: Mooncake
	Example:
	 .\wazvhdsize.exe -name appendstore  -key y2YkfsNZbBnWGfPzB4baOCHRKw0DiXNFuyg0KZSovh0wLpcnEi4oWIqKh56qndFf9Tm3w== -uri https://appendstore.blob.core.c
	hinacloudapi.cn/vhds/geocenos01-geoc.vhd
