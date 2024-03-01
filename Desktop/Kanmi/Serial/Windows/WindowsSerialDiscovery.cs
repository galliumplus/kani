using System.IO.Ports;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Kanmi.Serial.NetCore;
using Microsoft.Win32;

namespace Kanmi.Serial.Windows;

/// <summary>
/// Une implémentation de <see cref="ISerialDiscovery"/> basée sur la classe <see cref="SerialPort"/> de .NET.
/// </summary>
[SupportedOSPlatform("Windows")]
public partial class WindowsSerialDiscovery : ISerialDiscovery
{
    private readonly Dictionary<string, NetCoreSerialPort> knownPorts = new();

    /// <inheritdoc />
    public ISerialPort[] ListNewlyAvailablePorts()
    {
        List<string> newPorts = FindAvailableComPorts();

        foreach (string portName in this.knownPorts.Keys)
        {
            if (this.knownPorts[portName].StillAvailable)
            {
                if (newPorts.Contains(portName))
                {
                    // retirer les ports déjà connus de la liste
                    newPorts.Remove(portName);
                }
                else
                {
                    // enlever les ports qui ne sont plus disponibles
                    this.knownPorts.Remove(portName);
                }
            }
            else
            {
                // une erreur est survenue avec ce port, on l'oublie
                this.knownPorts.Remove(portName);
            }
        }

        List<NetCoreSerialPort> ports = new();
        if (newPorts.Count > 0)
        {
            // si il y a des nouveaux ports ouverts, on les rajoute
            
            Dictionary<string, string> comPortToVidMap = MapPortNamesToVid();

            foreach (string name in newPorts)
            {
                ushort? vid = null;
                if (comPortToVidMap.TryGetValue(name, out string? stringVid))
                {
                    vid = Convert.ToUInt16(stringVid, 16);
                }

                ports.Add(new NetCoreSerialPort(name, vid));
            }
        }

        return ports.ToArray<ISerialPort>();
    }

    private static List<string> FindAvailableComPorts()
    {
        List<string> ports = new();

        RegistryKey? deviceMapSerialComm = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM");
        if (deviceMapSerialComm is not null)
        {
            foreach (string name in deviceMapSerialComm.GetValueNames())
            {
                if (deviceMapSerialComm.GetValue(name) is string stringValue)
                {
                    ports.Add(stringValue);
                }
            }
        }

        return ports;
    }

    [GeneratedRegex(@"^VID_([0-9A-F]{4})")]
    private static partial Regex VidKeyRegex();

    private static Dictionary<string, string> MapPortNamesToVid()
    {
        Dictionary<string, string> map = new();

        RegistryKey? enumUsb = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\USB");
        if (enumUsb is not null)
        {
            foreach (string name in enumUsb.GetSubKeyNames())
            {
                Match match = VidKeyRegex().Match(name);
                if (match.Success && enumUsb.OpenSubKey(name) is RegistryKey deviceKey)
                {
                    foreach (string portName in FindPortNamesIn(deviceKey))
                    {
                        map.Add(portName, match.Groups[1].Value);
                    }
                }
            }
        }

        return map;
    }

    private static List<string> FindPortNamesIn(RegistryKey usbDeviceKey)
    {
        List<string> names = new();

        foreach (string subDeviceKey in usbDeviceKey.GetSubKeyNames())
        {
            RegistryKey? deviceParameters = usbDeviceKey.OpenSubKey(subDeviceKey + @"\Device Parameters");
            if (deviceParameters?.GetValue("PortName") is string stringValue)
            {
                names.Add(stringValue);
            }
        }

        return names;
    }
}