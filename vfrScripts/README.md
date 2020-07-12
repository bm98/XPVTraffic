## VFR Traffic Script Language

VFR Scripts are textfiles ending with .vsc (e.g. AirfieldRoundL.vsc).   
Use only Notepad (or a similar code editor) to edit!

Scripts accept  **# comments** at the end of a command or as comment lines

NOTE: There is not much error handling or the like - make sure the script follows the rules, else it may stop working...

--- 

**First command is an A command, then add further commands one per line.**  
An aircraft wants to start at the beginning of the designated or chosen runway, craft the script appropriately!

#### A(ircraft) Command:  
Creates a new aircraft must be the first command in a file.  
`A=AcftType[; Runway_ID] # comment`  
where AcftType is one of the General Aviation types e.g. C172, C182 see LiveTraffic plugin for supported types. If a type is not found LiveTraffic just uses one.  
Runway_ID is optional composed from ICAO Airport ID _ Runway e.g. LSZH_RW14 or NZWN_RW16 etc.
The aircraft starts at this runway if the runway is found within 50nm of the current user position, otherwise a random airport and runway is chosen.
If no runway is given the program always uses a random runway in a 50nm range to start from.  

#### D(istance or Direct) Command:  
Advises to fly straight for a distance of given nautical miles  
`D=2.3  # fly straight for 2.3 nm`  

#### T(urn) Command:  
Advise to make a standard turn (3°/sec) for the given angle (+-360.0 deg)  
`T=123  # fly a right turn of 123 deg`  

#### H(eading) Command:  
Advise to fly a turn until the given heading is reached  (0..360 degm)  
`H=90   # turn to heading 90 deg`  

---   
  
#### S(peed) Command:  
Advises to increase/decrease speed to the given value in kt (1..180)  
`S=94   # accel or decel and maintain 95 kt`  
The aircraft accelerates or decelerates to the given speed at about 3kt/sec

#### V(ertical) Command:  
Advises to use the given VS to get to the given altitude (VS:+-1500 ft/Min max, Alt 0..10000 ft AGL)  
`V=800; 4000  # climb at 800ft/sec to 4000ft AGL`  
Altitudes are always AGL based on the runway the plane started from.   
Make sure to use the proper sign for the vertical speed for a new altitude .. no check by the program.

S and V commands are immediate and maintained during straight, turn and heading segments. 
You may fly upwards, downwards in circles therefore.  

With some caution in scripting aircrafts can land as well.

--- 

### Example

**#AirfieldRound.vsc**  
`# Go around on LSZH Runway 14 if in reach`  
`A=C172;LSZH_RW14  # a new C172, start from LSZH_RW14 (if it is found in range)`   
`S=95        # speed-up to TAS 95 kts`   
`V=500;1000  # climb to 1000 ft AGL @ 500ft/Min`   
`D=2         # go straight for 2nm`  
`T=-180      # turn left -180°`  
`D=4         # go straight for 4nm`  
`T=-180      # turn left -180°`  
`S=60        # slow to TAS 60 kts`  
`V=-400;0    # descend to 0 ft AGL @ -400ft/Min`  
`D=2.2       # go straight for 2.2nm / land ????`  
`S=20        # slow to TAS 20 kts`   
`D=0.5       # go straight for 0.5nm`  
`# End of the script`  


---

### Simulation   

The program can create a KML file from a script file. It either uses the scripted Runway or the fallback runway from the GUI.  
Use the Simulation Button and select one of the scripts. If all is well a kml file is created in the same folder as the script.
You may then visualize the script with Google Earth or any website/program which accepts KML files as input.

---
