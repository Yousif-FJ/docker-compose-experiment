using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace service_1;

public static class Utils
{
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
                                                        freeBytes.ToString());
        return systemInformation;
    }
}

public record SystemInformation(List<string> Ips, List<string> Processes, string Uptime, string FreeSpace);
