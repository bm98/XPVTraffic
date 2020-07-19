using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.kmllib
{
  /// <summary>
  /// A KML file
  /// </summary>
  class KmlFile
  {
    public List<placemark> Placemarks = new List<placemark>( );
    public List<line> Lines = new List<line>( );

    /// <summary>
    /// Write the complete KML File
    /// </summary>
    /// <param name="kmlfile">The filename to write</param>
    /// <returns></returns>
    public bool WriteKML ( string kmlfile )
    {
      using ( var sw = new StreamWriter( kmlfile, false ) ) {
        foreach(var l in this.AsStringList( ) ) {
          sw.WriteLine( l );
        }
      }
      return true;
    }

    /// <summary>
    /// Returns the complete file as list of KML lines
    /// </summary>
    /// <returns>List of KML lines</returns>
    public List<string> AsStringList()
    {
      List<string> ret = new List<string> {
        @"<?xml version=""1.0"" encoding=""UTF-8""?>",
        @"<kml xmlns=""http://www.opengis.net/kml/2.2"">",
        @"  <Document>",
        @"    <name>XPVTraffic KML Routes</name>",
        @"    <open>1</open>"
      };
      ret.AddRange( Styles.AsString( ) );
      foreach ( var pm in Placemarks ) {
        ret.AddRange( pm.AsStringList( ) );
      }
      foreach ( var l in Lines ) {
        ret.AddRange( l.AsStringList( ) );
      }
      ret.Add( @"  </Document>" );
      ret.Add( @"</kml>" );
      return ret;
    }
  }
}
