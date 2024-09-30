using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace service_1;

public static class Utils
{
    public static string BytesToString(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0)
            return "0" + suf[0];
        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(byteCount) * num).ToString() + suf[place];
    }

    public static SystemInformation ExtractSystemInfo()
    {
        var ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                    .Select(ip => ip.ToString()).ToList();

        var processes = Process.GetProcesses().Select(p => p.ProcessName).ToList();
        long freeBytes;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            freeBytes = new DriveInfo("/").AvailableFreeSpace;
        }
        else
        {
            throw new NotImplementedException("None Linux OS cases were not implemented");
        }
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);

        var systemInformation = new SystemInformation(ip, processes, uptime.ToString(),
                            BytesToString(freeBytes));
        return systemInformation;
    }
}

public record SystemInformation(List<string> Ips, List<string> Processes, string Uptime, string FreeSpace);
