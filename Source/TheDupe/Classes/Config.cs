using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using TheDupe.Properties;

namespace TheDupe.Classes
{
    class Config
    {
        public static LibPcapLiveDevice PcapInterface { get => _pcapInterface; private set => _pcapInterface = value; }
        public static IPAddress IpAddress { get => _ipAddress; private set => _ipAddress = value; }
        public static string LogDir { get => _logDir; private set => _logDir = value; }
        public static string DateFormat { get => _dateFormat; private set => _dateFormat = value; }
        public static string TimeFormat { get => _timeFormat; private set => _timeFormat = value; }
        public static bool CheckUpdate { get => _checkUpdate; private set => _checkUpdate = value; }
        public static int UpdateInterval { get => _updateInterval; private set => _updateInterval = value; }
        public static bool SendIntel { get => _sendIntel; private set => _sendIntel = value; }
        public static int SendInterval { get => _sendInterval; private set => _sendInterval = value; }
        internal static List<Service> ServiceList { get => _serviceList; private set => _serviceList = value; }
        internal static List<WhiteList> WhitelList { get => _whitelList; private set => _whitelList = value; }
        public static string ConfigFile { get => _configFile; private set => _configFile = value; }

        private static LibPcapLiveDevice _pcapInterface = null;
        private static IPAddress _ipAddress = null;
        private static string _logDir = string.Empty;
        private static string _dateFormat = string.Empty;
        private static string _timeFormat = string.Empty;
        private static bool _checkUpdate = false;
        private static int _updateInterval = 0;
        private static bool _sendIntel = false;
        private static int _sendInterval = 0;
        private static List<Service> _serviceList = new();
        private static List<WhiteList> _whitelList = new();
        private static string _configFile = string.Empty;

        public static void Initialize()
        {
            try
            {
                ConfigFile = Utility.GetFileLocation("dupe.conf");
                WriteDefault();
                ReadConfig();
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.InnerException.ToString(), ex.Message);
            }
        }

        private static void WriteDefault()
        {
            try
            {
                if (!File.Exists(ConfigFile))
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        File.WriteAllBytes(ConfigFile, Resources.dupe_win);
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        File.WriteAllBytes(ConfigFile, Resources.dupe_linux);
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.InnerException.ToString(), ex.Message);
            }
        }

        private static void ReadConfig()
        {
            StreamReader sr = new(ConfigFile);

            try
            {
                while (sr.Peek() > 0)
                {
                    string config = sr.ReadLine();

                    if (!config.Contains('#') && !string.IsNullOrEmpty(config))
                    {
                        string[] _configWithValue = config.Split('=');
                        DateTime dt = DateTime.Now;

                        switch (_configWithValue[0])
                        {
                            case "log_dir":
                                if (Directory.Exists(_configWithValue[1].Trim()))
                                {
                                    _logDir = _configWithValue[1].Trim();
                                    Log.CheckDir();
                                }
                                else
                                {
                                    _logDir = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) ? "/var/log/thedupe/" : _logDir = @"C:\ProgramData\TheDupe\Logs\";
                                    Log.CheckDir();
                                    Log.WriteError("Value of 'log_dir' is not an existing directory. Default location was used to record this error.");
                                }
                                break;
                            case "date_format":
                                if ((DateTime.TryParse(dt.ToString(_configWithValue[1].Trim()), out DateTime _dt))
                                    && (dt.Month.Equals(_dt.Month) && dt.Day.Equals(_dt.Day) && dt.Year.Equals(_dt.Year)))
                                    _dateFormat = _configWithValue[1].Trim();
                                else
                                {
                                    _dateFormat = "dd-MMM-yyyy";
                                    _timeFormat = "HH:mm:ss";
                                    Log.WriteError("Value of 'date_format' is not an acceptable date format. Will use default dd-MMM-yyyy for logging purposes. Refer to the link on the latter for other formats. https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings.");
                                }
                                break;
                            case "time_format":
                                if ((DateTime.TryParse(dt.ToString(_configWithValue[1].Trim()), out DateTime _tm))
                                    && (dt.Hour.Equals(_tm.Hour) && dt.Minute.Equals(_tm.Minute) && dt.Second.Equals(_tm.Second)))
                                    _timeFormat = _configWithValue[1].Trim();
                                else
                                {
                                    _timeFormat = "HH:mm:ss";
                                    Log.WriteError("Value of 'time_format' is not an acceptable time format. Will use default HH:mm:ss for logging purposes. Refer to the link on the latter for other formats. https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings.");
                                }
                                break;
                            case "interface_id":
                                LibPcapLiveDevice _pcapInt = (LibPcapLiveDevice)CaptureDeviceList.Instance.FirstOrDefault(intId => intId.Name.Contains(_configWithValue[1].Trim()));

                                if (_pcapInt != null)
                                    _pcapInterface = _pcapInt;
                                else
                                    Log.WriteError(string.Format("Interface {0} value of 'interface_id' doesn't exist on this system.", _configWithValue[1].Trim()));
                                break;
                            case "ip_address":
                                if (IPAddress.TryParse(_configWithValue[1].Trim(), out IPAddress ipAddr))
                                    if (_pcapInterface.Addresses.FirstOrDefault(ip => ip.Addr.ToString() == ipAddr.ToString()) != null)
                                        _ipAddress = ipAddr;
                                    else
                                        Log.WriteError(string.Format("IP address '{0}' value of 'ip_address' doesn't exist on the selected interface.", ipAddr));
                                else
                                    Log.WriteError(string.Format("IP address '{0}' value of 'ip_address' is not in the correct format.", _configWithValue[1].Trim()));
                                break;
                            case "check_update":
                                if (bool.TryParse(_configWithValue[1].Trim(), out bool _chkUpd))
                                    _checkUpdate = _chkUpd;
                                else
                                    Log.WriteError("Value of 'check_update' can only be true or false.");
                                break;
                            case "update_interval":
                                if (int.TryParse(_configWithValue[1].Trim(), out int _updInt))
                                    _updateInterval = _updInt;
                                else
                                    Log.WriteError("Value of 'send_interval' is not a valid integer.");
                                break;
                            case "send_intel":
                                if (bool.TryParse(_configWithValue[1].Trim(), out bool _sndIntl))
                                    _sendIntel = _sndIntl;
                                else
                                    Log.WriteError("Value of 'send_intel' can only be true or false.");
                                break;
                            case "send_interval":
                                if (int.TryParse(_configWithValue[1].Trim(), out int _sndInt))
                                    _sendInterval = _sndInt;
                                else
                                    Log.WriteError("Value of 'send_interval' is not a valid integer.");
                                break;
                            case "monitor":
                                string svcValStr = _configWithValue[1].Trim();
                                string[] svcVal = svcValStr.Split(' ');

                                Service _svc = new()
                                {
                                    Protocol = (svcVal[0].ToUpper().Equals("TCP")) ? ProtocolType.Tcp : (svcVal[0].ToUpper().Equals("UDP")) ? ProtocolType.Udp : (svcVal[0].ToUpper().Equals("ICMP")) ? ProtocolType.Icmp : ProtocolType.Raw,
                                    Port = (int.TryParse(svcVal[1], out int _svcVal)) ? _svcVal : -1,
                                    Name = svcVal[2]
                                };

                                if (svcVal.Length == 3 && _svc.Port != -1 && _svc.Protocol != ProtocolType.Raw)
                                    _serviceList.Add(_svc);
                                else
                                    Log.WriteError(string.Format("'{0}' value of 'monitor' is not in the correct format.", svcValStr));
                                break;
                            case "whitelist":
                                string wlValStr = _configWithValue[1].Trim();
                                string[] wlVal = wlValStr.Split(' ');

                                WhiteList _wl = new()
                                {
                                    IpAddress = (IPAddress.TryParse(wlVal[0], out IPAddress _ipAdd)) ? _ipAdd : null,
                                    Service = new Service
                                    {
                                        Protocol = (wlVal[1].ToUpper().Equals("TCP")) ? ProtocolType.Tcp : (wlVal[1].ToUpper().Equals("UDP")) ? ProtocolType.Udp : (wlVal[1].ToUpper().Equals("ICMP")) ? ProtocolType.Icmp : ProtocolType.Raw,
                                        Port = (int.TryParse(wlVal[2], out int _wlVal)) ? _wlVal : -1
                                    }
                                };

                                if (wlVal.Length == 3 && _wl.Service.Port != -1 && _wl.Service.Protocol != ProtocolType.Raw)
                                    _whitelList.Add(_wl);
                                else
                                    Log.WriteError(string.Format("'{0}' value of 'whitelist' is not in the correct format.", wlValStr));
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.InnerException.ToString(), ex.Message);
            }

            sr.Dispose();
        }
    }
}
