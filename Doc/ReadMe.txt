XPVTraffic V 0.9 - Build 9 BETA
(c) M.Burri - 25-Jul-2020

Files & folders:

XPTsim.exe              The program
libXPVTgen_netstd.dll   - MUST be in the same folder as the Exe file

vfrScripts\*.vsc		- Script folder and files

ReadMe.txt                   This file

XPTsim (.Net 4.7.2)
libXPVTgen_netstd(.Net Standard V2.0)

Put all files into one folder and hit XPTsim.exe to run it.
see: https://github.com/bm98/XPVTraffic/blob/master/Doc/GUI.png

For Updates and information visit:
https://github.com/bm98/XPVTraffic


Scanned for viruses before packing... 
bm98@burri-web.org

Changelog:
V 0.9Build9
- add conversion of AITraffic files to scripts
- add a number of recorded ADS-B flights converted to relative scripts
- update Refacture Scripting classes and modes for AIT conversions
- update GUI and settings for conversion of AIT files
- fix Sim Model issues with GS=0
- fix Wrong entries in Aircraft Selections

V 0.9Build8
- update Calculate proper turns for IFR scripts
- update Allow for up to 150 deg angles of IFR continuation segments and avoid loops
- fix Error in Script P command

V 0.9Build7
- add Persist user settings
- add User settings for number of aircrafts supported
- add Logging debug support
- add Readme/Guide for Script Language
- add Optional scripting for accel and turnrates, new Msl Alt mode
- update Throttle the LT sender to avoid excess CPU usage
- update Use script and model for IFR flight creation
- update Refactor aircraft, sim parts, review/update comments
- update Some misleading names
- update Use Bluebird CSL supported aircrafts and operators for IFR flights
- update Provide callsign, tail registration and airborne tag to LT
- fix Change to ground speed (GS) naming to match AITRAFFIC format

V 0.8 
- added VFR scripting and aircraft model, 
- added Runway database from XP11 data files
- added VFR Script simulation and KML file creation
- improved The aircraft hexcode matches the reg number as string now e.g. VAC-0234  -> HexCode: 000234
- improved Some GUI fixes

V 0.7 initial 
