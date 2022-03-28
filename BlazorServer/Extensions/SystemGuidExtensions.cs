using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Security.Cryptography;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace BlazorServer.Extensions;

/// <summary>
/// Generates a Guid based on the current computer hardware
/// Example: C384B159-8E36-6C85-8ED8-6897486500FF
/// </summary>
public class SystemGuid
{

    private static string _systemGuid = string.Empty;

    public string DecodeValue(string val)
    {
        return val.DecryptString();
    }

    public string ValueAsync()
    {
        var lVolSl = GetRunningOSDriveSerialNumber();
        //var lCompMod = GetComputerModel();
        var lCpuId = GetCpuId();
        var lBiosId = GetBiosId();
        var lMainboard = GetMainboardId();
        var lGpuId = GetGpuId();
        var lMac = GetMac();
        var lConcatStr = $"CPU: {lCpuId}\nBIOS:{lBiosId}\nMainboard: {lMainboard}\nGPU: {lGpuId}\nMAC: {lMac}\nVolume: {lVolSl}";
        _systemGuid = GetHash(lConcatStr);
        return _systemGuid;
    }

    //private static string GetHash(string s)
    //{
    //    MD5 sec = new MD5CryptoServiceProvider();
    //    ASCIIEncoding enc = new ASCIIEncoding();
    //    byte[] bt = enc.GetBytes(s);
    //    return GetHexString(sec.ComputeHash(bt));
    //}
    //private static string GetHexString(byte[] bt)
    //{
    //    string s = string.Empty;
    //    for (int i = 0; i < bt.Length; i++)
    //    {
    //        byte b = bt[i];
    //        int n, n1, n2;
    //        n = (int)b;
    //        n1 = n & 15;
    //        n2 = (n >> 4) & 15;
    //        if (n2 > 9)
    //            s += ((char)(n2 - 10 + (int)'A')).ToString();
    //        else
    //            s += n2.ToString();
    //        if (n1 > 9)
    //            s += ((char)(n1 - 10 + (int)'A')).ToString();
    //        else
    //            s += n1.ToString();
    //        if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
    //    }
    //    return s;
    //}


    private string GetHash(string s)
    {
        try
        {
            return s.EncryptString();
        }
        catch (Exception lEx)
        {
            return lEx.Message;
        }
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
        "ReleaseDate",
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
