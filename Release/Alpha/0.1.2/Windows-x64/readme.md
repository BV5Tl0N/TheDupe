# THE DUPE
[logo]:https://github.com/BV5Tl0N/TheDupe/blob/main/thedupe.png
![logo]

The Dupe is a *free, multi-platform* sensor for your decoy systems or honeypots. This sensor is *easy to deploy and easy to use*, **you will have your own running decoy system in under 10 minutes!**

### What is a Honeypot?
A honeypot is a computer system that acts as a decoy for threat actors. It mimics a legitimate system that hackers could attack. Their intrusion attempts are logged for analysis thereafter. 

### Minimum Requirements
This sensor is tested to work using the default configuration on  specifications and operating systems listed below. You may need to use higher specifications if you need to monitor more ports and if your honeypot is expected to receive high volumes of traffic (like if it's placed on DMZ). If your system is not listed below, you may try, but the sensor may or may not work as intended. 
* 1 Ghz CPU
* 128 MB RAM
* 50 GB Disk Space
* .NET Core Runtime 5.0
* Microsoft 64 Bit OS: Windows 10, Server 2016
* Linux 64 Bit OS: Ubuntu 20.04

### Supported Protocols
This release currently supports the list of protocols below. The list will be expanded if needed in the future.
* IPV4
* TCP
* UDP
* ICMP

### Configuration Files

#### Log Configuration
* **log_dir** identifies the location where event and error logs is stored.
* **date_format** identifies the date format that the program will use. This must contain day, month & year and affects all date formatting.
* **time_format** identifies the time format that the program will use. This must contain hour, minute & second and affects all time formatting.

#### Network Monitor Configuration
* **interface_id** identifies the target network interface for traffic monitoring.

    How to get your Interface Device ID
    - Open the command prompt
    - Run "getmac" command without quotes
    - Refer to the output. Your device ID will be shown under Transport Name, ***inside the curly braces***.

    | Physical Address  | Transport Name                                            |
    |-------------------|-----------------------------------------------------------|
    | FF-A1-A2-A3-A4-A5 | \Device\Tcpip_{***ABCDEF12-3456-7890-ABCD-EF123456789***} |

* **ip_address** identifies the exact IP address to listen to, from the chosen network interface.

    How to get your IP Address
    - Open the command prompt
    - Run "ipconfig" command without quotes
    - Refer to the output. Your IP address will be shown under "IPv4 Address" of your target interface

    IPv4 Address. . . . . . . . . . . : 192.168.69.69

#### Home Call Configuration
These configs are inactive and will be used future releases.
* **check_update** indicates whether the program would check for updated packages online.
* **update_interval** identifies the amount of time in minutes, on when the program will check for updates. 
* **send_intel** indicates whether the program will anonymously send public IPs detected by The Dupe for threat intelligence.
* **send_interval** identifies the amount of time in minutes, on when the program send intel to the honeynet.

#### Service Monitoring Configuration
* **monitor** accepts one service to monitor per line. For multiple services, add monitor keyword every entry then its value.
Monitor keyword accepts the format [protocol]<space>[Port Number]<space>[Port Name] as value,
_eg.: monitor=TCP 20 FTP._

#### Whitelist Configuration
* **"whitelist** accepts one service and IP pair to monitor per line. For multiple pairs, add whitelist keyword every entry then its value. Whitelist keyword accepts the format [Source IP]<space>[protocol]<space>[Port Number]<space>[Port Name] as value, _eg.: "whitelist=10.0.0.1 UDP 123"_

## Development Team
#### Follow us on Github and Hack The Box!

#### BV5Tl0N
Github https://github.com/BV5Tl0N \
Hack The Box https://www.hackthebox.eu/home/users/profile/538292

#### WUNDERWEISS
Github	https://github.com/wunderweiss-el \
Hack The Box https://www.hackthebox.eu/home/users/profile/543260

## Thanks To

#### PacketDotNet / Chris Morgan
https://www.nuget.org/packages/PacketDotNet/1.2.0/
#### SharpPcap / Tamir Gal, Chris Morgan and others
https://github.com/chmorgan/sharppcap
https://www.nuget.org/packages/SharpPcap/5.4.0

## License

This software is license under [GNU GENERAL PUBLIC LICENSE](https://raw.githubusercontent.com/BV5Tl0N/TheDupe/main/LICENSE).
