using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Security.Cryptography;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace SimpleTouchWorker.Extensions;

/// <summary>
/// Generates Info based on the current computer hardware
/// </summary>
public class SystemInfo
{

    private static string _systemInfo = string.Empty;

    public string ValueAsync()
    {
        var lVolSl = GetRunningOSDriveSerialNumber();
        var lCpuId = GetCpuId();
        var lBiosId = GetBiosId();
        var lMainboard = GetMainboardId();
        var lGpuId = GetGpuId();
        var lMac = GetMac();
        _systemInfo = $"CPU: {lCpuId}\nBIOS:{lBiosId}\nMainboard: {lMainboard}\nGPU: {lGpuId}\nMAC: {lMac}\nVolume: {lVolSl}";
        return _systemInfo;
    }


    #region Original Device ID Getting Code

    //Return a hardware identifier
    private string GetIdentifier(string pWmiClass, List<string> pProperties)
    {
        string lResult = string.Empty;
        try
        {
            ManagementClass devs = new ManagementClass(pWmiClass);

            ManagementObjectCollection moc = devs.GetInstances();


            foreach (ManagementObject lItem in moc)
            {
                foreach (var lProperty in pProperties)
                {
                    try
                    {
                        switch (lProperty)
                        {
                            case "MACAddress":
                                if (string.IsNullOrWhiteSpace(lResult) == false)
                                    return lResult; //Return just the first MAC

                                if (lItem["IPEnabled"].ToString() != "True")
                                    continue;
                                break;
                        }

                        var lItemProperty = lItem[lProperty];
                        if (lItemProperty == null)
                            continue;

                        var lValue = lItemProperty.ToString();
                        if (string.IsNullOrWhiteSpace(lValue) == false)
                            lResult += $"{lValue}; ";
                    }
                    catch { }
                }

            }
        }
        catch { }
        return lResult;
    }

    private static List<string> ListOfCpuProperties = new List<string>
    {
        "UniqueId",
        "ProcessorId",
        "Name",
        "Manufacturer",
        "PartNumber",
        "ProcessorType",
        "SerialNumber",
        "Caption",
        "CreationClassName",
        "SystemName"
    };

    private string GetCpuId()
    {
        return GetIdentifier("Win32_Processor", ListOfCpuProperties);
    }

    private static List<string> ListOfBiosProperties = new List<string>
    {
        "Manufacturer",
        "SMBIOSBIOSVersion",
        "IdentificationCode",
        "SerialNumber",
        //"ReleaseDate",
        "Version"
    };
    //BIOS Identifier
    private string GetBiosId()
    {
        return GetIdentifier("Win32_BIOS", ListOfBiosProperties);
    }

    private static List<string> ListOfMainboardProperties = new List<string>
    { "Model",
        "Manufacturer",
        "Name",
        "SerialNumber"
    };
    //Motherboard ID
    private string GetMainboardId()
    {
        return GetIdentifier("Win32_BaseBoard", ListOfMainboardProperties);
    }

    private static List<string> ListOfGpuProperties = new List<string>
    {
        "Name",
        "Description"
    };
    //Primary video controller ID
    private string GetGpuId()
    {
        return GetIdentifier("Win32_VideoController", ListOfGpuProperties);
    }

    private static List<string> ListOfNetworkProperties = new List<string>
    {
        "MACAddress"
    };
    private string GetMac()
    {
        return GetIdentifier("Win32_NetworkAdapterConfiguration", ListOfNetworkProperties);
    }

    private string GetRunningOSDriveSerialNumber()
    {
        try
        {
            string windir = Path.GetPathRoot(Environment.SystemDirectory);

            var driveSerialnumber = string.Empty;
            var pathRoot = Path.GetPathRoot(windir);
            if (pathRoot == null)
            {
                return driveSerialnumber;
            }
            var driveFixed = pathRoot.Replace("\\", "");
            if (driveFixed.Length == 1)
            {
                driveFixed = driveFixed + ":";
            }
            var wmiQuery = string.Format($"SELECT VolumeSerialNumber FROM Win32_LogicalDisk Where Name = '{driveFixed}'");
            using (var driveSearcher = new ManagementObjectSearcher(wmiQuery))
            {
                using (var driveCollection = driveSearcher.Get())
                {
                    foreach (var moItem in driveCollection.Cast<ManagementObject>())
                    {
                        driveSerialnumber = Convert.ToString(moItem["VolumeSerialNumber"]);
                    }
                }
            }
            return driveSerialnumber;
        }
        catch (Exception ex)
        {
            //handle the error your way
            return string.Empty;
        }
    }

    #endregion
}
