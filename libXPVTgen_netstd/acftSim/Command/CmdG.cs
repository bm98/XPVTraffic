using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using libXPVTgen.coordlib;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// Goto Lat Lon Command
  /// e.g. G=47.2;8.5[;3.5]  # head towards pt 47.2;8.5 and turn at a rate of 3.5 deg/sec
  /// </summary>
  class CmdG : CmdBase
  {
    /// <summary>
    /// Create a CmdG from a string array 
    /// </summary>
    /// <param name="script">An array of strings </param>
    /// <returns>A valid Cmd or null</returns>
    public static CmdG CreateFromStrings( string[] script )
    {
      // must be 3 at least
      if ( script.Length < 3 ) return null; // ERROR exit

      if ( double.TryParse( script[1], out double lat ) && double.TryParse( script[2], out double lon ) ) {
        var cmd = new CmdG(new LatLon(lat,lon) );
        if ( script.Length > 3 ) {
          // may have turnrate
          if ( double.TryParse( script[3], out double tRate ) ) {
            cmd.TurnRate = tRate;
          }
        }
        return cmd;
      }
      return null; // ERROR number parse
    }


    // CLASS

    public LatLon Destination { get; private set; } = new LatLon( ); // dest point Lat/Lon
    public double TurnRate { get; set; } = 3.0;   // Turnrate deg/sec (optional)

    /// <summary>
    /// cTor: 
    /// </summary>
    /// <param name="destination">Dest to goto [lat/lon]</param>
    public CmdG( LatLon destination )
    {
      Cmd = Cmd.G;
      Destination = new LatLon( destination );
    }

    /// <summary>
    /// Write the Command to the stream
    /// </summary>
    /// <param name="stream">The output stream</param>
    public override void WriteToStream( StreamWriter stream )
    {
      stream.WriteLine( $"G={Destination.Lat:#0.000000};{Destination.Lon:#0.000000};{TurnRate:#0.0}" );
    }

  }
}
