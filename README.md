# X-Plane 11 Virtual Traffic V 0.8 Build 6

A library and GUI to create virtual traffic in X-Plane 11.  
Interacts with the LiveTraffic plugin. Acts as 'RealTraffic' data provider

![LiveTraffic on Map](https://raw.githubusercontent.com/bm98/XPVTraffic/master/Doc/LiveTraffic-Map.png "LiveTraffic on Map")



Program Files:  
* XPTsim.exe  - a small GUI  
* libXPVTgen_netstd.dll - the library

Projects included:
* libXPVTgen_netstd
* XPTsim

Built with Microsoft Visual Studio 2017 (free edition)  

More screenshots here: https://github.com/bm98/XPVTraffic/tree/master/Doc  

Note: readily available only as Win program. It connects via network i.e. should be able to run in a WinBox.  
The library is built against .NET Standard 2.0 so you may create the GUI in any env. and recreate the application for Mac or Linux

### Quick Guide

First time use:
* Put the library and the GUI file into any folder (best to create a new one)
* Start the GUI (XPTsim.exe)
* Set the X-Plane 11 Base Path - use the (...) button
* Create the application database from custom or default data files (CrateDB button), 
this creates my_awy.dat in the application folder
* __
* Add the LiveTraffic plugin to X-Plane 11 (according to the authors guidance)
* Start X-Plane 11 
* Set the LiveTraffic plugin to accept 'RealTraffic' (again use the authors guidance)

Regular use:  
* Start X-Plane 11
* Start the GUI (XPTsim.exe)
* Hit EstablishLink (button should turn green - else see the message below the button)
* Wait until the plugin reports traffic (about after 90 sec as default)
* Use the Map to see the traffic, or chase a plane.. 

---

#### LiveTraffic

see: https://forums.x-plane.org/index.php?/files/file/49749-livetraffic/

---

### Library features & function

The library uses its own airway and runway file derived from either the X-Plane 11 custom or default earth_fix.dat, 
earth_nav.dat, earth_awy.dat and CIFP\*.dat files.  
While creating the file it combines the nav, fixes and airways into one file and does some housekeeping to 
keep only valid airways.

Once running it will establish the connection with the LiveTraffic plugin and 
communicates with the 'RealTraffic' application protocol.

#### IFR Traffic

All virtual IFR aircrafts have a designation of **VAC-nnnn**, where nnnn is the numbered sequence since start of the program.
The ICAO hex code is the nnnn number in hex for easy tracking (and therefore not valid at all..)  
e.g. VAC-0234  -> HexCode: 000234

The program maintains 100 virtual aircrafts at any given time (`VAcftPool.cs `).  
The aircraft type is selected from a variety of jets, props at random (`VAcft.cs`).  
The program selects airways for aircraft routes where the midpoint is within a range of 100nm of the users aircraft position (`TrafficHandler.cs`).  
The route displayed in LiveTraffic are the start and end Fix names.  
The aircrafts use Low and High enroute segments to fly along.  
Aircrafts maintain the segments direction restriction.  
Aircrafts maintain the segments top and bottom FL restriction.   
The altitude of an aircraft is created at random between bottom and top FL.     
For now the aircrafts will not change altitude while enroute.  
The aircrafts get an assigned TAS depending on High or Low IFR route, which is random for each aircraft but maintained all the time.  
Aircrafts are preferring named routes as direction when crossing fixes.  
If there is no further named airway segment they will divert into another airway at an angle <70°.  
If no further segment can be used an aircraft is considered out of bound and is removed.  
For any removed aircraft a new one is created along the rules above.  
LiveTraffic shows removed aircrafts for some time but also drops them if no longer reported.  
Note: there is a buffer delay in LiveTraffic of 90sec but not applied to the route shown as label 
i.e. the label changes before the aircraft hits the next Fix.  

#### VFR Traffic

All virtual VFR aircrafts have a designation of **VGA-nnnn**, where nnnn is the numbered sequence since start of the program.
The ICAO hex code is the nnnn number in hex for easy tracking (and therefore not valid at all..)  
e.g. VAC-0016  -> HexCode: 000016

VFR Traffic is created from script files found in the application folder 'vfrScripts' it subfolders.  
Scripts describe a path the aircraft flies along. Turns are always standard turns.

The script language can be found in the README.md of in 'vfrScripts'.

For VFR traffic a simulation is provided which visualizes the track as KML file. Load it with Google Earth or a similar program that displays KML files.


---

#### Credits

LiveTraffic: https://forums.x-plane.org/index.php?/files/file/49749-livetraffic/  
The Plugin ... thanks a lot !!!

---

Latitude/longitude spherical geodesy tools    
(c) Chris Veness 2002-2017  MIT Licence   
www.movable-type.co.uk/scripts/latlong.html  
www.movable-type.co.uk/scripts/geodesy/docs/module-latlon-spherical.html

---
RealTraffic:  https://rtweb.flyrealtraffic.com/  
Communication specification 1.4 used.


