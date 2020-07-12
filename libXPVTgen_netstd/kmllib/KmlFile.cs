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


    public List<string> AsStringList()
    {
      List<string> ret = new List<string>( );

      ret.Add( @"<?xml version=""1.0"" encoding=""UTF-8""?>" );
      ret.Add( @"<kml xmlns=""http://www.opengis.net/kml/2.2"">" );
      ret.Add( @"  <Document>" );
      ret.Add( @"    <name>XPVTraffic KML Routes</name>" );
      ret.Add( @"    <open>1</open>" );
      ret.AddRange( Styles.AsString( ) );
      ret.Add( @"    <Folder>" );
      ret.Add( @"    <name>VFR Routes</name>" );
      foreach ( var pm in Placemarks ) {
        ret.AddRange( pm.AsStringList( ) );
      }
      foreach ( var l in Lines ) {
        ret.AddRange( l.AsStringList( ) );
      }
      ret.Add( @"    </Folder>" );
      ret.Add( @"  </Document>" );
      ret.Add( @"</kml>" );
      return ret;
    }
  }
}
