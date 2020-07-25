# X-Plane 11 Virtual Traffic V 0.9 Build 9

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

First time use (also applies to updates):
* Put the library and the GUI file into any folder (best to create a new one)
* Add VFR script files to the 'vfrScripts' folder
* Start the GUI (XPTsim.exe)
* Set the X-Plane 11 Base Path - use the (...) button
* Create the application database from custom or default data files (CrateDB button), 
this creates my_awy.dat in the application folder (and may take a while - wait for the Done message)
* __
* Add the LiveTraffic plugin to X-Plane 11 (according to the authors guidance)
* Start X-Plane 11 
* Set the LiveTraffic plugin to accept 'RealTraffic' (again use the authors guidance)

Regular use:  
* Start X-Plane 11 and start a flight at a location
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

The program maintains a total of 100 (default) virtual aircrafts at any given time. Can be changed in the GUI.  
The program maintains 20 (default) of them as 'VFR' aircrafts at any given time. Can be changed in the GUI.  

#### Traffic created from the Airway database (IFR)

All virtual IFR aircrafts get a callsign from the Bluebird CSL operators and a uniqe number assigned e.g. **JAL1234**.  
The tail registration is **VX-1234** then.
The ICAO hex code is the nnnn number in hex for easy tracking (and therefore not valid at all..)  
e.g. VX-1234  -> HexCode: 001234

The aircraft type is selected from a variety of airliners, jets, turboprops at random.  
The program selects airways for aircraft routes where the midpoint is within a range of 100nm of the users aircraft position.  
The route displayed in LiveTraffic are the start and end route Fix/Nav names.  
The aircrafts use Low and High enroute segments to fly along.  
Aircrafts maintain the segments direction restriction.  
Aircrafts maintain the segments top and bottom FL restriction.   
The altitude of an aircraft is created at random between bottom and top FL and may randomly change at the start of a new leg.     
The aircrafts get an assigned GS depending on High or Low IFR route, which is random for each aircraft it may also change at the start of a new leg.  
Aircrafts are preferring named routes as direction when crossing fixes.  
If there is no further named airway segment they will divert into another airway at an angle <150°.  
If no further segment can be used an aircraft is considered out of bound and is removed.  
For any removed aircraft a new one is created along the rules above.  
LiveTraffic shows removed aircrafts for some time but also drops them if no longer reported.  
Note: there is a buffer delay in LiveTraffic of 90sec but not applied to the route shown as label 
i.e. the label changes before the aircraft hits the next Fix.  

#### Traffic created from Scripts

All virtual aircrafts created from scripts get a callsign **YYYnnnn**, where nnnn is the numbered sequence since start of the program.
The ICAO hex code is the nnnn number in hex for easy tracking (and therefore not valid at all..)  
e.g. YYY0016  -> HexCode: 000016.  The tail registration is then  **VX-0016**.

Such traffic is created from script files found in the application folder 'vfrScripts' it subfolders.  
Scripts describe a path the aircraft flies along.  

The guide for the script language can be found in the README.md in 'vfrScripts'.

For scripted traffic a simulation is provided which visualizes the track as KML file.
Load it with Google Earth or a similar program that displays KML files.

#### Conversion of AITraffic files to scripts

There is a conversion from AITraffic file (AITFC=...) to scripts.  

Included are a number of converted ADS-B flight recordings (from my own receiver, no business terms violated). 
The KML included is not the original flight track, all are relocated starting from LSZH RWY28 (this is what relative conversion allows). 
While running such scripts as virtual traffic they will have an origin and bearing from random runways in range.

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


