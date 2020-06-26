using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using libXPVTgen.coordlib;

namespace libXPVTgen.xp11_navlib
{
  /// <summary>
  /// wraps: 
  ///   X-PLANE NAVIGATION DATA FOR NAVAIDS (USER_NAV.DAT & EARTH_NAV.DAT) FILE SPECIFICATION VERSION 1100
  /// </summary>
  public class navRec
  {
    public enum NavTypes
    {
      All = -1,
      Other = 0,
      Unknown = 99,
      NDB = 2, // NDB (Non-Directional Beacon) Includes NDB component of Locator Outer Markers( LOM)
      VOR = 3, // VOR( including VOR-DME and VORTACs) Includes VORs, VOR-DMEs, TACANs and VORTACs
      Loc_ILS = 4, // Localizer component of an ILS( Instrument Landing System)
      Loc_Aproach = 5, // Localizer component of a localizer-only approach Includes for LDAs and SDFs
      GS_ILS = 6, // Glideslope component of an ILS Frequency shown is paired frequency, not the DME channel
      OM_ILS = 7, // Outer markers( OM) for an ILS Includes outer maker component of LOMs
      MM_ILS = 8, // Middle markers( MM) for an ILS
      IM_ILS = 9, // Inner markers( IM) for an ILS
      FIX = 11, // All FIXPOINTS from fix DAT file will be added with this type
      DME_ILS = 12, // DME, including the DME component of an ILS, VORTAC or VOR-DME Paired frequency display suppressed on X-Plane’s charts
      DME = 13, // Stand-alone DME, or the DME component of an NDB-DME Paired frequency will be displayed on X-Plane’s charts
      FPAP = 14, // Final approach path alignment point of an SBAS or GBAS approach path Will not appear in X-Plane’s charts
      GLS = 15, // GBAS differential ground station of a GLS Will not appear in X-Plane’s charts
      LTP_FTP = 16, // Landing threshold point or fictitious threshold point of an SBAS/GBAS approach Will not appear in X-Plane’s charts
    }


    // fields in db
    public string ident = ""; // Key ?! NavRec.icao_id + "_" + NavRec.icao_region
    public NavTypes recType = NavTypes.Unknown;
    public LatLon LatLon = new LatLon( );
    public double lat = 0;         //02 Lat
    public double lon = 0;         //03 Lon
    public int elevation = 0;   //04 NDB,VOR: Elevation in feet above MSL
                                //   FIX: not used
    public string freq = "";        //05 NDB: Frequency in KHz
                                    //   VOR: Frequency in MHZ (multiplied by 100)
                                    //   LOC: Frequency in MHZ (multiplied by 100)
                                    //   GS: Frequency in MHZ (multiplied by 100)
                                    //   BEACON: not used
                                    //   DME: Frequency in MHZ (multiplied by 100)
                                    //   GLS: GLS GBAS channel number
                                    //   FIX: not used
    public string range = "";       //06 Maximum reception range in nautical miles
                                    //   BEACON: not used
                                    //   GLS: not used
                                    //   FIX: not used
    public string deviation = "";   //07 NDB: na; 
                                    //   VOR:Slaved variation, i.e. direction of the 0 radial measured in true degrees
                                    //   LOC: Localizer bearing in true degrees
                                    //   GS: Associated localizer bearing in true degrees prefixed by glideslope angle
                                    //   BEACON: Associated localizer bearing in true degrees (also known as “minor axis”)
                                    //   DME: DME bias in nautical miles.
                                    //   GLS: Associated final approach course in true degrees prefixed by glidepath angle
                                    //   FIX: not used
    public string icao_id = "";     //08 up to 4 char ICAO identifier
                                    //   LOC: Localizer identifier
                                    //   GS: Glideslope identifier
                                    //   BEACON: Associated approach identifier
                                    //   GLS: Approach procedure identifier
                                    //   FIX: Usually five characters. Unique within an ICAO region
    public string terminal_id = ""; //09 NDB: terminal region identifier or ENRT for enroute NDBs Airport code for terminal NDBs, ENRT otherwise
                                    //   VOR: ENRT for all VORs
                                    //   LOC: Airport ICAO code
                                    //   GS: Airport ICAO code
                                    //   BEACON: Airport ICAO code
                                    //   DME: Airport ICAO code (for DMEs associated with an ILS) ENRT for DMEs associated with VORs, VORTACs, NDBs or standalone DMEs
                                    //   GLS: Airport ICAO code
                                    //   FIX: ID of airport terminal area or “ENRT” for enroute fixes
    public string icao_region = ""; //10 ICAO region code of enroute NDB or terminal area airport Must be region code according to ICAO document No. 7910 For terminal NDBs, the region code of the airport is used
    public string name = "";        //11 item name  
                                    //   NDB: Text, suffix with “NDB” e.g. NOLLA NDB
                                    //   VOR: Text, suffix with “VOR”, “VORTAC”, “TACAN” or “VOR-DME” e.g. SEATTLE VORTAC
                                    //   LOC: Associated runway number
                                    //   GS: Associated runway number
                                    //   BEACON: Associated runway number
                                    //   DME: Associated runway number (for DMEs associated with an ILS)
                                    //   GLS: Associated runway number
                                    //   FIX:  Waypoint type as defined by the 3 columns of ARINC 424.18 field definition 5.42 This field is optional and can be left
    public string locName = "";     //   LOC: Localizer name, Use “ILS-cat-I”, “ILS-cat-II”, “ILS-cat-III”, “LOC”, “LDA” or “SDF”
                                    //   GS: 'GS'
                                    //   BEACON: “OM”, “MM” or “IM”
                                    //   DME: DME name (all DMEs) “DME-ILS” if associated with ILS, Suffix “DME” to navaid name for VOR-DMEs, VORTACs & NDB-DMEs
                                    // ( eg. “SEATTLE VORTAC DME” in example data ) For standalone DMEs just use DME name
                                    //   GLS: 'GLS'
                                    //   FIX: not used

    // NOTE : type 14 (FPAP) /16 (LTP/FTP) are not captured 

    /// <summary>
    /// cTor: populate the record
    /// </summary>
    /// <param name="id">ident (UPPERCASE)</param>
    public navRec( NavTypes rt, string la, string lo, string elev,
                   string f, string r, string dev, string i_id, string t_id, string i_reg,
                    string nam, string lnam )
    {
#if !DEBUG
      try {
#endif
      recType = rt;
      lat = double.Parse( la );
      lon = double.Parse( lo );
      if ( int.TryParse( elev, out int e ) ) {
        elevation = e;
      }
      freq = f;
      range = r;
      deviation = dev;
      icao_id = i_id;
      terminal_id = t_id;
      icao_region = i_reg;
      name = nam;
      locName = lnam;
#if !DEBUG
      }
      catch {
        recType = NavTypes.Unknown; // not a valid location
      }
#endif

      ident = $"{icao_id}_{icao_region}";
      LatLon = new LatLon( lat, lon );
    }


    /// <summary>
    /// returns true if the record is valid
    /// </summary>
    public bool IsValid { get => recType != NavTypes.Unknown; }

    /// <summary>
    /// Returns true if the record matches one of the given types
    /// </summary>
    /// <param name="navTypes">An array of NavTypes</param>
    /// <returns>True for a match, else false</returns>
    public bool IsTypeOf( NavTypes[] navTypes )
    {
      if ( navTypes.Contains( NavTypes.All ) ) return true;
      return navTypes.Contains( this.recType );
    }

  }
}
