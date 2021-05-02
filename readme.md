# THE DUPE

[logo]: https://github.com/BV5Tl0N/TheDupe/blob/main/thedupe.png
![logo]

The Dupe is a *free, multi-platform* sensor for your decoy systems or honeypots. This sensor is *easy to deploy and easy to use*, **you will have your own running decoy system in under 10 minutes!**

### WHAT IS A HONEYPOT?
A honeypot is a computer system that acts as a decoy for threat actors. It mimics a legitimate system that hackers could attack. Their intrusion attempts are logged for analysis thereafter. 

### COMPATIBILITY
This sensor is tested to work on the following operating systems. If your system is not listed below, the sensor may or may not work as intended.
* Microsoft Windows 10, 64 bit 
* Microsoft Windows Server 2016, 64 bit
* Ubuntu 20.04, 64 bit

### CONFIGURATION FILES

#### EXEMPTIONS.CONF 
This configuration file can be used to list trusted connections to your honeypot. For example, if you wouldn't like your honeypot to detect a RDP from your workstation with IP 10.10.10.10, you must add below line to this configuration file.
> \<IP Address>:\<Protocol>:\<Port>:\<Name>\
> ***10.10.10.10:TCP:3389:RDP***

#### INTERFACE.CONF
**Entry to this configuration file is required** for this sensor to work. You need to determine your *network interface device ID and ip address*, where the sensor would listen for incoming connections.

How to get your interface's device ID
* In Windows, you can run the **getmac** command.
> getmac

Your device ID will be shown under Transport Name, ***inside the curly braces***.

|Physical Address  |Transport Name                                     |
|------------------|---------------------------------------------------|
|FF-A1-A2-A3-A4-A5 |\Device\Tcpip_{***ABCDEF12-3456-7890-ABCD-EF123456789***}|

* Linux
> ip addr

Your device ID will be the ***name of the network interface itself*** where your target IP is assigned.

|$ ip addr|
|-------------------------------|
|1: lo: <LOOPBACK,UP,LOWER_UP> ...|
|2: ***enp0s3***: <BROADCAST,MULTICAST,UP,LOWER_UP> ...|


#### PORTSLIB.CONF
Not used for now, other than as reference.

#### PORTSMON.CONF
**Entry to this configuration file is required** for this sensor to work. This should contain the list of protocols and services that will be monitored. A default list is provided upon installation.

> \<Protocol>:\<Port Number>:\<Service Name>\
> ***UDP:23:TELNET***

## DEVELOPMENT TEAM
#### Follow us on Github and Hack The Box!

#### BV5Tl0N
Github https://github.com/BV5Tl0N \
Hack The Box https://www.hackthebox.eu/home/users/profile/538292

#### WUNDERWEISS
Github	https://github.com/wunderweiss-el \
Hack The Box https://www.hackthebox.eu/home/users/profile/543260

## THANKS TO

#### PacketDotNet / Chris Morgan
https://www.nuget.org/packages/PacketDotNet/1.2.0/
#### SharpPcap / Tamir Gal, Chris Morgan and others
https://github.com/chmorgan/sharppcap \
https://www.nuget.org/packages/SharpPcap/5.4.0
