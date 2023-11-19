using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libXPVTgen.Aircrafts;
using libXPVTgen.coordlib;

namespace libXPVTgen.LiveTraffic
{
  /// <summary>
  /// RealTraffic formatter actual - updated 20231119
  /// https://rtweb.flyrealtraffic.com/RTdev2.0.pdf
  ///
  /// legacy
  /// https://rtweb.flyrealtraffic.com/RTdev1.4.pdf
  /// </summary>
  internal class RealTraffic
  {
    /*
     LiveTraffic has to open a TCP port (10747 by default) to which RealTraffic connects. 
     Once established, LiveTraffic then periodically sends its current location via this link back to RealTraffic so that 
     RealTraffic can filter and send the traffic data of that area.
    */
    /// <summary>
    /// The LinkPort - Connect to LifeTraffic and receive messages
    /// </summary>
    public const int PortLinkTCP = 10747;

    /*
     * extended format, used by LiveTraffic (old format)
     */
    /// <summary>
    /// The Traffic port to send traffic to LifeTraffic
    /// This is the legacy AIT format - no longer supported
    /// </summary>
    public const int PortTrafficUDP_AIT = 49003;

    /*
     * RTTFC format, used by LiveTraffic 3.4.3
     */
    /// <summary>
    /// The Traffic port to send traffic to LifeTraffic
    /// </summary>
    public const int PortTrafficUDP = 49005;

    /*
     * is the port on which RealTraffic periodically sends weather data
     */
    /// <summary>
    /// The weather port to send weather to LifeTraffic
    /// </summary>
    public const int PortWeatherUDP = 49004;

    /*
     * SimPos String:
     Your simulator plugin needs to provide a TCP socket server connection. By default,
      RealTraffic expects port 10747 (but this can be changed and is configurable in the Settings
      pane of the RealTraffic User Interface). Once your plugin detects that RealTraffic has
      connected to it, you need to transmit the following parameters at between 1 - 5 Hz (i.e. 1 – 5 times per second):
      • Pitch in radians * 100000
      • Bank in radians * 100000
      • Heading (or Track) in radians
      • Altitude in feet * 1000
      • True air speed in meters per second
      • Latitude in radians
      • Longitude in radians
      • All of that preceded by the characters “Qs121=”
      An example string would look like this:
      “Qs121=6747;289;5.449771266137578;37988724;501908;0.6564195830703577;-2.1443275933742236”
     */

    /// <summary>
    /// Convert a LinkMessage from LifeTraffic to only a latLon
    /// </summary>
    /// <param name="linkString">string from LifeTraffic</param>
    /// <param name="latLon">out LatLon received</param>
    /// <returns>True if successfull</returns>
    public static bool GetFromLinkString( ref string linkString, out LatLon latLon )
    {
      latLon = new LatLon( );
      if (!linkString.Contains( "Qs121=" )) return false; // not enough

      // cheap... actually expensive CPU
      while (!linkString.StartsWith( "Qs121=" )) {
        linkString = linkString.Substring( 1 );
      }
      // Have a Qs starter now
      string[] e = linkString.Split( new char[] { ';', '=' } );
      if (e.Length < 8) return false; // not enough

      // remove from linkString 
      int len = 0;
      for (int i = 0; i < 8; i++) {
        len += e[i].Length;
      }
      len += 7; // div chars
      if (linkString.Length > len) {
        linkString = linkString.Substring( len );
      }

      // try to process
      bool retval = false;
      if (double.TryParse( e[6], out double lat ) && double.TryParse( e[7], out double lon )) {
        latLon.Lat = lat.ToDegrees( );
        latLon.Lon = lon.ToDegrees( );
        retval = true;
      }

      return retval;
    }

    /*
      RTTFC/RTDEST format
      This format contains almost all information we have available from our data sources. The
      format is:
      “RTTFC,hexid, lat, lon, baro_alt, baro_rate, gnd, track, gsp, cs_icao, ac_type, ac_tailno, from_iata, to_iata, timestamp, 
          source, cs_iata, msg_type, alt_geom, IAS, TAS, Mach,
          track_rate, roll, mag_heading, true_heading, geom_rate, emergency, category, nav_qnh,
          nav_altitude_mcp, nav_altitude_fms, nav_heading, nav_modes, seen, rssi, winddir,
          windspd, OAT, TAT, isICAOhex,augmentation_status,authentication”

     01 hexid
     02 lat = latitude
     03 lon = longitude
     04 baro_alt = barometric altitude
     05 baro_rate = barometric vertical rate
     06 gnd = ground flag
     07 track = track
     08 gsp = ground speed
     09 cs_icao = ICAO call sign
     10 ac_type = aircraft type
     11 ac_tailno = aircraft registration
     12 from_iata = origin IATA code
     13 to_iata = destination IATA code
     14 timestamp = unix epoch timestamp when data was last updated
    /+ RT new fields
     15 source = data source
     16 cs_iata = IATA call sign
     17 msg_type = type of message
     18 alt_geom = geometric altitude (WGS84 GPS altitude)
     19 IAS = indicated air speed
     20 TAS = true air speed
     21 Mach = Mach number
     22 track_rate = rate of change for track
     23 roll = roll in degrees, negative = left
     24 mag_heading = magnetic heading
     25 true_heading = true heading
     26 geom_rate = geometric vertical rate
     27 emergency = emergency status
     28 category = category of the aircraft
     29 nav_qnh = QNH setting navigation is based on
     30 nav_altitude_mcp = altitude dialled into the MCP in the flight deck
     31 nav_altitude_fms = altitude set by the flight management system (FMS)
     32 nav_heading = heading set by the MCP
     33 nav_modes = which modes the autopilot is currently in
     34 seen = seconds since any message updated this aircraft state vector
     35 rssi = signal strength of the receiver
     36 winddir = wind direction in degrees true north
     37 windspd = wind speed in kts
     38 OAT = outside air temperature / static air temperature
     39 TAT = total air temperature
     40 isICAOhex = is this hexid an ICAO assigned ID.
     41 Augmentation_status = has this record been augmented from multiple sources
     42 Authentication = authentication status of the license, safe to ignore.

      The “source” field can contain the following values:
      • adsb_icao: messages from a Mode S or ADS-B transponder.
      • adsb_icao_nt: messages from an ADS-B equipped "non-transponder" emitter e.g. a ground vehicle.
      • adsr_icao: rebroadcast of an ADS-B messages originally sent via another data link
      • tisb_icao: traffic information about a non-ADS-B target identified by a 24-bit ICAO address, e.g. a Mode S target tracked by SSR.
      • adsc: ADS-C (received by satellite downlink) – usually old positions, check tstamp.
      • mlat: MLAT, position calculated by multilateration. Usually somewhat inaccurate.
      • other: quality/source unknown. Use caution.
      • mode_s: ModeS data only, no position.
      • adsb_other: using an anonymised ICAO address. Rare.
      • adsr_other: rebroadcast of ‘adsb_other’ ADS-B messages.
      • tisb_other: traffic information about a non-ADS-B target using a non-ICAO address
      • tisb_trackfile: traffic information about a non-ADS-B target using a track/file
      identifier, typically from primary or Mode A/C radar
      A couple of example data sets:

      RTTFC,11234042,-33.9107,152.9902,26400,1248,0,90.12,490.00,AAL72,B789,N835AN,SYD,LAX,1645144774.2,
      //      X2,   callsign        msgtyp      gAlt      IAS  TAS  M      tr  rr  hdg   thdg  gvs  es cat  qnh   nalt  -   nhdg
             X2,      AA72,        adsb_icao,   27575,    320,  474,0.780,0.0,0.0,78.93,92.27,1280,none,A5,1012.8,35008,-1,71.02,
      //  modes                  seen rssi wd  ws oat t a aug
        autopilot|vnav|lnav|tcas,0.0,-21.9,223,24,-30,0,1,170124

      RTTFC,10750303,-33.7964,152.3938,20375,1376,0,66.77,484.30,UAL842,B789,
      N35953,SYD,LAX,1645144889.8,
        X2,UA842,adsb_icao,21350,343,466,0.744,-0.0,0.5,54.49,67.59,1280,none,A5,1012.8,35008,-1,54.84,
        autopilot|vnav|lnav|tcas,0.0,-20.8,227,19,-15,14,1,268697

*/

    /// <summary>
    /// Format an RT-Traffic message from a virtual aircraft
    /// </summary>
    /// <param name="vac">The V-Aircraft record</param>
    /// <returns>An RT-Traffic string to send to LiveTraffic</returns>
    public static string RTTrafficString( VAcft vac )
    {
      string airborne = (vac.Airborne) ? "1" : "0";
      //
      string rt = $"RTTFC,{vac.AcftHex},{vac.LatLon.Lat:##0.0000000},{vac.LatLon.Lon:##0.0000000},{vac.Alt_ft},{(int)vac.VSI}"
        + $",{airborne},{vac.TRK:##0.0},{vac.GS:##0.0}"
        + $",{vac.AcftCallsign},{vac.AcftType},{vac.AcftTailReg}"
        + $",{vac.AcftFrom},{vac.AcftTo},{vac.TStamp}";
      // RT extension - use mostly dummy values
      //      X2,    callsign        msgtyp      gAlt      IAS  TAS  M  tr rr     hdg            thdg             gvs         es   cat qnh   nalt  -   nhdg
      rt += $",X2,{vac.AcftTailReg},tisb_other,{vac.Alt_ft},100,100,0.2,0.0,0.0,{vac.TRK:##0.0},{vac.TRK:##0.0},{(int)vac.VSI},none,A3,1013,-1,-1,-1,";
      //      modes  seen rssi wd  ws oat t a aug
      rt += $"none,0,-10.0,0,0,15,15,0,0";
      return rt;
    }

    /// <summary>
    /// Convert an RT Traffic string to a AITvAcft moment
    /// </summary>
    /// <param name="aString">One RT Traffic message as string</param>
    /// <returns>A AITvAcft Moment of that string or null if invalid</returns>
    public static AITvAcft FromRTTrafficString( string aString )
    {
      if (!aString.StartsWith( "RTTFC," )) return null;
      string[] e = aString.Split( new char[] { ',' } );
      if (e.Length < 15) return null;

      var vac = new AITvAcft( );
      var ret = true;
      vac.AcftHex = e[1];
      ret &= double.TryParse( e[2], out double lat );
      ret &= double.TryParse( e[3], out double lon );
      ret &= double.TryParse( e[4], out double alt );
      ret &= double.TryParse( e[5], out double vsi );
      ret &= int.TryParse( e[6], out int airborne );
      ret &= double.TryParse( e[7], out double track );
      ret &= double.TryParse( e[8], out double gs );
      vac.AcftCallsign = e[9];
      vac.AcftType = e[10];
      vac.AcftTailReg = e[11];
      vac.AcftFrom = e[12];
      vac.AcftTo = e[13];
      ret &= long.TryParse( e[14], out long ts );

      if (!ret) return null; // ERROR one of the Parses failed

      // complete the Acft data
      vac.LatLon = new LatLon( lat, lon );
      vac.Alt_ft = (int)alt;
      vac.VSI = vsi;
      vac.Airborne = (airborne != 0);
      vac.TRK = track;
      vac.GS = gs;
      vac.TStamp = ts;

      // sanity checks - we need some items
      if (vac.TStamp <= 0) return null;
      if (vac.Alt_ft <= 0) return null;

      // TODO lookup acftType from hexcode in a database      
      return vac;
    }



    /*
     * “AITFC,hexid,lat,lon,alt,vs,airborne,hdg,spd,cs,type,tail,from,to,timestamp”

      • Hexid: the hexadecimal ID of the transponder of the aircraft. This is a unique ID, and
          you can use this ID to track individual aircraft.     
      • Lat: latitude in degrees
      • Lon: longitude in degrees
      • Alt: altitude in feet
      • Vs: vertical speed in ft/min
      • Airborne: 1 or 0
      • Hdg: The heading of the aircraft (it’s actually the true track, strictly speaking. )
      • Spd: The speed of the aircraft in knots
      • Cs: the ICAO callsign (Emirates 413 = UAE413 in ICAO speak, = EK413 in IATA speak)
      • Type: the ICAO type of the aircraft, e.g. A388 for Airbus 380-800. B789 for Boeing 787-9 etc.
      • Tail: The registration number of the aircraft
      • From: The origin airport where known (in IATA or ICAO code)
      • To: The destination airport where known (in IATA or ICAO code)
      • Timestamp: The UNIX epoch timestamp when this position was valid
    */

    /// <summary>
    /// Format an AI-Traffic message from a virtual aircraft
    /// </summary>
    /// <param name="vac">The V-Aircraft record</param>
    /// <returns>An AI-Traffic string to send to LiveTraffic</returns>
    public static string AITrafficString( VAcft vac )
    {
      string airborne = (vac.Airborne) ? "1" : "0";
      //
      string rt = $"AITFC,{vac.AcftHex},{vac.LatLon.Lat:##0.0000000},{vac.LatLon.Lon:##0.0000000},{vac.Alt_ft},{(int)vac.VSI}"
        + $",{airborne},{vac.TRK:##0.0},{vac.GS:##0.0}"
        + $",{vac.AcftCallsign},{vac.AcftType},{vac.AcftTailReg}"
        + $",{vac.AcftFrom},{vac.AcftTo},{vac.TStamp}";
      return rt;
    }

    /// <summary>
    /// Convert an AI Traffic string to a AITvAcft moment
    /// </summary>
    /// <param name="aString">One AI Traffic message as string</param>
    /// <returns>A AITvAcft Moment of that string or null if invalid</returns>
    public static AITvAcft FromAITrafficString( string aString )
    {
      if (!aString.StartsWith( "AITFC," )) return null;
      string[] e = aString.Split( new char[] { ',' } );
      if (e.Length < 15) return null;

      var vac = new AITvAcft( );
      var ret = true;
      vac.AcftHex = e[1];
      ret &= double.TryParse( e[2], out double lat );
      ret &= double.TryParse( e[3], out double lon );
      ret &= double.TryParse( e[4], out double alt );
      ret &= double.TryParse( e[5], out double vsi );
      ret &= int.TryParse( e[6], out int airborne );
      ret &= double.TryParse( e[7], out double track );
      ret &= double.TryParse( e[8], out double gs );
      vac.AcftCallsign = e[9];
      vac.AcftType = e[10];
      vac.AcftTailReg = e[11];
      vac.AcftFrom = e[12];
      vac.AcftTo = e[13];
      ret &= long.TryParse( e[14], out long ts );

      if (!ret) return null; // ERROR one of the Parses failed

      // complete the Acft data
      vac.LatLon = new LatLon( lat, lon );
      vac.Alt_ft = (int)alt;
      vac.VSI = vsi;
      vac.Airborne = (airborne != 0);
      vac.TRK = track;
      vac.GS = gs;
      vac.TStamp = ts;

      // sanity checks - we need some items
      if (vac.TStamp <= 0) return null;
      if (vac.Alt_ft <= 0) return null;

      // TODO lookup acftType from hexcode in a database      
      return vac;
    }


    /*
     The weather messages are broadcast as UDP packets once every 10 seconds on port 49004
      containing a JSON string with the following format:
      "{"ICAO": "XXXX","QNH": 1013, "METAR": "XXXX", "NAME": "XXXX", "IATA": "XXXX" , "DISTNM": 0}";
      The fields should be self-explanatory, in case they’re not:
      • ICAO is the ICAO code of the nearest airport,
      • QNH is the reported pressure in hPa,
      • METAR contains the full METAR received,
      • NAME shows the airport name (usually long),
      • IATA is the IATA code of the airport (YSSY = Sydney in ICAO speak, SYD = Sydney in
      IATA speak), and lastly,
      • DISTNM is the distance to said airport in nautical miles.
     */
    public static string WeatherString( )
    {
      // just a generic string reporting the standard pressure - we don't adjust heights anyway
      // METAR remains empty and the APT is Zurich... may be a long way from some...
      return $"{{\"ICAO\": \"LSZH\",\"QNH\": 1013, \"METAR\": \"\", \"NAME\": \"ZURICH\", \"IATA\": \"ZRH\" , \"DISTNM\": 10}}";
    }

  }
}
