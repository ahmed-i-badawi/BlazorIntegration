using System.Management;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace BlazorServer.Extensions;

public static class MachineInfo
{
    public enum HardwareProfileComponents
    {
        ComputerModel,
        VolumeSerial,
        CpuId,
        //MemoryCapacity,
        VideoControllerDescription,
        MACAddress,
    }

    public static Dictionary<string, string> HardwareProfile()
    {
        var retval = new Dictionary<string, string>
                     {
                         //{HardwareProfileComponents.ComputerModel.ToString(), GetComputerModel()},
                         //{HardwareProfileComponents.VolumeSerial.ToString(), GetVolumeSerial()},
                         //{HardwareProfileComponents.CpuId.ToString(), GetCpuId()},
                         //{HardwareProfileComponents.MemoryCapacity.ToString(), GetMemoryAmount()},
                         //{HardwareProfileComponents.VideoControllerDescription.ToString(), GetVideoControllerDescription()},
                         //{HardwareProfileComponents.MACAddress.ToString(), GetMACAddress()}
                     };
        return retval;

    }

    //private static string GetMACAddress()
    //{
    //    NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
    //    String sMacAddress = string.Empty;
    //    foreach (NetworkInterface adapter in nics)
    //    {
    //        if (sMacAddress == String.Empty)// only return MAC Address from first card
    //        {
    //            IPInterfaceProperties properties = adapter.GetIPProperties();
    //            sMacAddress = adapter.GetPhysicalAddress().ToString();
    //            var myMac = sMacAddress.Replace('-', ' ');
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }
    //    return sMacAddress;
    //}

    //private static string GetVideoControllerDescription()
    //{
    //    var s1 = new ManagementObjectSearcher("select * from Win32_VideoController");
    //    foreach (ManagementObject oReturn in s1.Get())
    //    {
    //        var desc = oReturn["AdapterRam"];
    //        if (desc == null) continue;
    //        return oReturn["Description"].ToString().TrimAll();
    //    }
    //    return string.Empty;
    //}

    //private static string GetComputerModel()
    //{
    //    var s1 = new ManagementObjectSearcher("select Model from Win32_ComputerSystem");
    //    foreach (ManagementObject oReturn in s1.Get())
    //    {
    //        return oReturn["Model"].ToString().TrimAll();
    //    }
    //    return string.Empty;
    //}

    //private static string GetMemoryAmount()
    //{
    //    var s1 = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
    //    foreach (ManagementObject oReturn in s1.Get())
    //    {
    //        return oReturn["Capacity"].ToString().Trim();
    //    }
    //    return string.Empty;
    //}

    //private static string GetVolumeSerial()
    //{
    //    var disk = new ManagementObject();
    //    disk.Get();

    //    string volumeSerial = disk["VolumeSerialNumber"].ToString();
    //    disk.Dispose();

    //    return volumeSerial;
    //}

    //private static string GetCpuId()
    //{
    //    var managClass = new ManagementClass("win32_processor");
    //    var managCollec = managClass.GetInstances();
    //    List<string> res = new List<string>();

    //    foreach (ManagementObject managObj in managCollec)
    //    {
    //        res.Add(managObj.Properties["processorID"]?.Value?.ToString().TrimAll());
    //        res.Add(managObj.Properties["Name"]?.Value?.ToString().TrimAll());
    //        res.Add(managObj.Properties["PartNumber"]?.Value?.ToString().TrimAll());
    //        res.Add(managObj.Properties["ProcessorType"]?.Value?.ToString().TrimAll());
    //        res.Add(managObj.Properties["SerialNumber"]?.Value?.ToString().TrimAll());
    //        res.Add(managObj.Properties["Caption"]?.Value?.ToString().TrimAll());
    //        res.Add(managObj.Properties["CreationClassName"]?.Value?.ToString().TrimAll());
    //        res.Add(managObj.Properties["Manufacturer"]?.Value?.ToString().TrimAll());
    //        res.Add(managObj.Properties["SystemName"]?.Value?.ToString().TrimAll());

    //        return string.Join("-", res);

    //        //more possible keys to be used
    //        //AddressWidth
    //        //Architecture
    //        //AssetTag
    //        //Availability
    //        //Characteristics
    //        //CurrentClockSpeed
    //        //CurrentVoltage
    //        //DataWidth 
    //        //Description
    //        //ExtClock
    //        //Family
    //        //Level
    //        //LoadPercentage 
    //        //MaxClockSpeed
    //        //NumberOfCores
    //        //NumberOfEnabledCore
    //        //NumberOfLogicalProcessors
    //        //PowerManagementSupported
    //        //SecondLevelAddressTranslationExtensions
    //        //SerialNumber
    //        //SocketDesignation
    //        //Status
    //        //StatusInfo
    //        //SystemCreationClassName
    //        //ThreadCount
    //        //UpgradeMethod
    //        //VirtualizationFirmwareEnabled
    //        //VMMonitorModeExtensions
    //    }
    //    return string.Empty;
    //}

}
