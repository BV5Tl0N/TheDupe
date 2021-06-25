using PacketDotNet;
using SharpPcap;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using TheDupe.Classes;
using TheDupe.Properties;

namespace TheDupe
{
    class Dupe
    {
        static void Main()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !File.Exists(Utility.GetFileLocation("libpcap.so")))
                    File.WriteAllBytes(Utility.GetFileLocation("libpcap.so"), Resources.libpcap);

                if (Utility.IsAdmin())
                {
                    Config.Initialize();
                    Log.Initialize();
                    StartPacketCapture();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("This program requires elevated privilege. This will close automatically.");
                    Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.InnerException.ToString(), ex.Message);
            }
        }

        private static void StartPacketCapture()
        {
            new Thread(() =>
            {
                try
                {
                    Config.PcapInterface.OnPacketArrival += new(Device_OnPacketArrival);
                    Config.PcapInterface.Open();
                    Config.PcapInterface.Filter = "(icmp[icmptype] == icmp-echo or tcp[tcpflags] == tcp-syn or udp) and dst host " + Config.IpAddress;
                    Config.PcapInterface.Capture();
                }
                catch (Exception ex)
                {
                    Log.WriteError(ex.InnerException.ToString(), ex.Message);
                }
            }).Start();
        }
        private static void Device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            try
            {
                Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data).Extract<IPv4Packet>();

                if (packet != null)
                {
                    IPPacket ipV4Packet = packet.Extract<IPv4Packet>();
                    Service service = new();
                    WhiteList whiteList = new();
                    Log log = new();

                    switch (ipV4Packet.Protocol)
                    {
                        case ProtocolType.Tcp:
                            TcpPacket tcpPacket = ipV4Packet.Extract<TcpPacket>();
                            service = Config.ServiceList.FirstOrDefault(
                                svc => svc.Protocol.Equals(ipV4Packet.Protocol) &&
                                svc.Port.Equals(tcpPacket.DestinationPort)
                            );

                            whiteList = Config.WhitelList.FirstOrDefault(
                                wl => wl.IpAddress.Equals(ipV4Packet.SourceAddress) &&
                                wl.Service.Protocol.Equals(ipV4Packet.Protocol) &&
                                wl.Service.Port.Equals(tcpPacket.DestinationPort)
                            );
                            break;
                        case ProtocolType.Udp:
                            UdpPacket udpPacket = ipV4Packet.Extract<UdpPacket>();
                            service = Config.ServiceList.FirstOrDefault(
                                svc => svc.Protocol.Equals(ipV4Packet.Protocol) &&
                                svc.Port.Equals(udpPacket.DestinationPort)
                            );

                            whiteList = Config.WhitelList.FirstOrDefault(
                                wl => wl.IpAddress.Equals(ipV4Packet.SourceAddress) &&
                                wl.Service.Protocol.Equals(ipV4Packet.Protocol) &&
                                wl.Service.Port.Equals(udpPacket.DestinationPort)
                            );
                            break;
                        case ProtocolType.Icmp:
                            IcmpV4Packet icmpV4Packet = ipV4Packet.Extract<IcmpV4Packet>();

                            if (icmpV4Packet.TypeCode == IcmpV4TypeCode.EchoRequest)
                            {
                                service = Config.ServiceList.FirstOrDefault(
                                    svc => svc.Protocol.Equals(ipV4Packet.Protocol) &&
                                    svc.Port.Equals(0)
                                );

                                whiteList = Config.WhitelList.FirstOrDefault(
                                    wl => wl.IpAddress.Equals(ipV4Packet.SourceAddress) &&
                                    wl.Service.Protocol.Equals(ipV4Packet.Protocol) &&
                                    wl.Service.Port.Equals(0)
                                );
                            }
                            break;
                    }

                    if (service != null && whiteList == null)
                    {
                        log.SourceAddress = ipV4Packet.SourceAddress;
                        log.DestinationAddress = ipV4Packet.DestinationAddress;
                        log.EventDateTime = DateTime.Now;
                        log.Protocol = ipV4Packet.Protocol;
                        log.ServiceName = service.Name;
                        log.ServiceNumber = service.Port;

                        Log.LogQueue.Enqueue(log);
                        Log.LogQueueNotif.Set();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.InnerException.ToString(), ex.Message);
            }
        }
    }
}
