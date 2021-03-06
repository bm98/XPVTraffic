## VFR Traffic Script Language

VFR Scripts are textfiles ending with .vsc (e.g. AirfieldRoundL.vsc).   
Use only Notepad (or a similar code editor) to edit!

Scripts accept  **# comments** at the end of a command or as comment lines

NOTE: There is not much error handling or the like - make sure the script follows the rules, else it may stop working...

--- 

**First command is an A command, then add further commands one per line.**  
An aircraft wants to start at the beginning of the designated or chosen runway, craft the script appropriately!

#### A(ircraft) Command:  
Creates a new aircraft and must be the first command in a file. There can only be one A command in a file.  

There are 4 type of scripts where only one tpye is used for user scripts (the other 3 are for internal purposes of conversion of AIT files and virtual IFR traffic).  
The type is the second argument of an A command - for user scripts it is 'Runway' i.e. Runway based.

`A=AircraftType;ScriptType[;Apt_RunwayID[;StrictFlag]]`  where the ones in [] are optional.  

An AircraftType is mandatory and should be a supported type e.g. C172, BE20 see LiveTraffic plugin for supported types. 
If a type is not found LiveTraffic just uses one.  

ScriptType is mandatory and always 'Runway' for user scripts.  
Such scripts assume to start from a Runway and take V commands with relative altitude to the Runway elevation.  

If no runway is defined the program always uses a random runway in a 50nm range to start from.  
`A=C172;Runway # Use a Cessna 172`  

Runway_ID is optional composed from ICAO Airport ID _ Runway e.g. LSZH_RW14 or NZWN_RW16 etc.
The aircraft starts at this runway if the runway is found within 50nm of the current user position, otherwise a random airport and runway is chosen.  
`A=C172;Runway;LSZH_RW14 # Use a Cessna 172 and the preferred runway is apt LSZH runway 14`  

To locate a script strictly on the Apt/Runway use the "S" parameter, if the runway is not found in range the script is ignored.  
`A=C172;Runway;LSZH_RW16;S # Use a Cessna 172 and the mandatory runway is apt LSZH runway 16`  

Note: GA Aircraft Types supported by the Bluebird CSL library are: "C150", "C172", "C421", "BE20", "PC9"
If you use other ones LiveTraffic either finds them in the libraray or just tries to get you one of its choice (see LiveTraffic docs).  

#### D(istance or Direct) Command:  
Advises to fly straight for a distance of given nautical miles  
`D=2.3  # fly straight for 2.3 nm`  

#### T(urn) Command:  
Advise to make a turn for the given angle (+-360.0 deg) optional use a given turnrate (0.5..9.0 deg/sec) - defaults to a standard turn at 3�/sec.   
`T=123      # fly a right turn of 123 deg with default turnrate`  
`T=-30;3.5  # fly a left turn of 30 deg with optional turnrate 3.5 deg/sec`  

#### H(ead towards) Command:  
Advise to fly a turn until the given track direction is reached (0..360 degm) optional use a given turnrate (0.5..9.0 deg/sec).  
`H=90       # turn to heading 90 deg with default turnrate`  
`H=120;3.5   # turn to heading 120 deg with a turnrate of 3.5 deg/sec`  

---   
  
#### S(peed) Command:  
Increase or decrease ground speed to the given value in kt (1..180) optional use given acceleration.  
`S=95       # accel or decel and maintain 95 kt with default accel`   
`S=120;5     # accel or decel and maintain 120 kt with accel of 5 kt/sec`   
The aircraft accelerates or decelerates from current ground speed to the given speed.  

#### V(ertical to Altitude) Command:  
Climb or descend at given VS from the current to the new altitude.  
`V=800;4000  # climb or descend at 800ft/sec to 4000ft`   
Altitudes are default based on the runway the plane started from (see below M command).  

S and V commands are immediate and maintained during straight, turn and heading segments. 
You may fly upwards, downwards in circles therefore.  

With some caution in scripting aircrafts can land as well.

#### M(SL altitudes) Command:
At startup of a script all V command altitudes are relative to the starting runway elevation.  
To switch to MSL based altitudes use:   
`M=1  # MSL based, V altitudes are taken as absolute AMSL from now on`   
To switch back to relative, above runway, elevation based altitudes use   
`M=0  # MSL based off, V altitudes are taken as relative to rwy elevation from now on`   

--- 

### Example

**#AirfieldRound.vsc**  
`# Go around on LSZH Runway 14 if in reach`  
`A=C172;Runway;LSZH_RW14  # a new C172, start from LSZH_RW14 (if it is found in range)`   
`S=95        # speed-up to GS 95 kts`   
`V=500;1000  # climb with 500ft/Min to 1000 ft AGL`   
`D=2         # go straight for 2nm`  
`T=-180      # turn left -180�`  
`D=4         # go straight for 4nm`  
`T=-180      # turn left -180�`  
`S=60        # slow to GS 60 kts`  
`V=400;0     # descend at 400ft/Min to 0 ft AGL`  
`D=2.2       # go straight for 2.2nm / land ????`  
`S=20        # slow to GS 20 kts`   
`D=0.5       # go straight for 0.5nm`  
`# End of the script`  

---

### Scripting Limits and Defaults

For all models:
* Min accel = 0.5 kt/sec
* Max accel = 10 kt/sec
* Min turnrate = 0.5 deg/sec
* Max turnrate = 9 deg/sec

For GA aircrafts the following limits and defaults apply:  
* Max GS = 180 kt
* Max Vertical Rate = 1500 ft/min
* Max Altitude = 10'000 ft AMSL
* Default acceleration = 2 kt/sec
* Default turnrate = 3 deg/sec

For all other aircrafts the following limits and defaults apply:  
* Max GS = 500 kt
* Max Vertical Rate = 2500 ft/min
* Max Altitude = 45'000 ft AMSL
* Default acceleration = 5 kt/sec
* Default turnrate = 3 deg/sec

---


### Simulation   

The program can create a KML file from a script file. It either uses the scripted Runway or the fallback runway from the GUI.  
Use the Simulation Button and select one of the VSC scripts.  
You may then visualize the created KML file with Google Earth or any website/program which accepts KML files as input.  
If all is well a `scriptname.vsc.kml` file is created in the same folder as the script.

---

### Conversion of AITraffic files
Specification see https://rtweb.flyrealtraffic.com/RTdev1.4.pdf

There is a conversion for captured AIT files to scripts in order to create virtual traffic from such records.  
Note: There is little checking for such files built in and some may create AIT conversion errors.     
AIT record files **must have the extension '.csv'** else they are not recognised by the converter.  
AIT message records must start with the 'AITFC=' tag and have all 14 arguments included.  
**Numbers must exists** but can be 0, strings e.g. from, to, cs, tail may be left empty.  
**Lat/Lon/Alt/Timestamp are mandatory**, GS is recommended to create a somehow proper flight profile. Very slow GS are to be avoided.       
The conversion will try to estimate GS is zero but those guesses are not considering turns i.e. speed is ususally too slow.  
By default only records with the Airborne flag =1 are recorded but you may ignore this flag in the GUI but again then the conversion may fail.    
A conversion can be absolute or relative (checkbox).  
* Default is relative i.e. the virtual traffic created from such a script starts from a random location in range it is composed from turns and straight segments at the recorded Altitude (MSL).  
* Absolute conversion will locate and fly the recorded coordinates if the starting point is in range else the script is ignored.    

Successfully created scripts are created in the same folder as the AIT file and get a .vsc' extension.  

---

