using System;
using System.Collections.Generic;
using System.Text;

namespace libXPVTgen.xp11_awylib
{
  /// <summary>
  /// One XPlane 11 Airway database record
  /// </summary>
  public class awyRec
  {
    // fields in db
    public string ident = ""; // Key ?! start_icao_id + "_" + start_icao_region + "_" + end_icao_id + "_" + end_icao_region

    public string start_icao_id = "";     //01 Identifier of enroute fix or navaid at the beginning of this segment
    public string start_icao_region = ""; //02 ICAO region code of enroute NDB or terminal area airport Must be region code according to ICAO document No. 7910 
    public short start_navaid = 0;      //03 Type of Fix or Navaid 11 = Fix, 2 = enroute NDB, 3 = VHF navaid
    public string end_icao_id = "";       //04 Identifier of enroute fix or navaid at the beginning of this segment
    public string end_icao_region = "";   //05 ICAO region code of enroute NDB or terminal area airport Must be region code according to ICAO document No. 7910 
    public short end_navaid = 0;        //06 Type of Fix or Navaid 11 = Fix, 2 = enroute NDB, 3 = VHF navaid
    public string restriction = "";       //07 N = “None”, F = ”Forward”, B = “Backward”
                                          //   If the directional restriction is F = “Forward”, the airway segment is authorized to be flown in 
                                          //    the direction from the first fix to the second fix.
                                          //   If the directional restriction is B = “Backward”, the segment is only to be flown in the
                                          //    direction from second fix to first fix.
    public short layer = 0;             //08 This is a "High" airway (1 = "low", 2 = "high"). 
                                        //   If an airway segment is both High and Low, then it should be listed twice( once in each category ). 
                                        //   This determines if the airway is shown on X-Plane's "High Enroute" or "Low Enroute" charts
    public short baselevel = 0;         //09 Base of airway in hundreds of feet (18000 ft in this example) Integer between 0 and 600
    public int Base_ft { get => baselevel * 100; }
    public short toplevel = 0;          //10 Top of airways in hundreds of feet (45000 ft in this example) Integer between 0 and 600
    public int Top_ft { get => toplevel * 100; }

    public string name = "";              //11 Airway segment name. Up to five characters per name, names separated by hyphens
                                          //  If multiple airways share this segment, then all names will be included separated by a hyphen( eg. "J13-J14-J15")

    public string startID = "";
    public string endID = "";

    /// <summary>
    /// cTor: populate the record
    /// </summary>
    /// <param name="id">ident (UPPERCASE)</param>
    public awyRec( string sid, string sreg, string styp,
                   string eid, string ereg, string etyp,
                   string rest, string lay, string blevel, string tlevel,
                   string nam )
    {
#if !DEBUG
      try {
#endif
      start_icao_id = sid.ToUpperInvariant( );
      start_icao_region = sreg.ToUpperInvariant( );
      start_navaid = short.Parse( styp );
      end_icao_id = eid.ToUpperInvariant( );
      end_icao_region = ereg.ToUpperInvariant( );
      end_navaid = short.Parse( etyp );
      restriction = rest;
      layer = short.Parse( lay );
      baselevel = short.Parse( blevel );
      toplevel = short.Parse( tlevel );
      name = nam.ToUpperInvariant( );
#if !DEBUG
      }
      catch {
        return; // invalid, leave ident empty
      }
#endif

      startID = $"{sid}_{sreg}";
      endID = $"{eid}_{ereg}";
      ident = $"{startID}_{endID}";
    }


    /// <summary>
    /// returns true if the record is valid
    /// </summary>
    public bool IsValid
    {
      get => !string.IsNullOrEmpty( start_icao_id ) && !string.IsNullOrEmpty( end_icao_id ) && !string.IsNullOrEmpty( ident );
    }


  }
}
