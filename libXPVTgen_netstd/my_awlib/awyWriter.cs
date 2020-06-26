using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libXPVTgen.my_awlib
{
  public class awyWriter
  {
    private static string WriteDbFile( awyDatabase db, string fName )
    {
      string ret = "";
      using ( var sr = new StreamWriter( fName ) ) {
        foreach(var awrec in db.GetTable() ) {
          sr.WriteLine( awrec.Value.ToString( ) );
        }
        sr.WriteLine( "99 " );
      }
      return ret;
    }

    /// <summary>
    /// Writes the given DB to a file
    /// NOTE does not overwrite but complains
    /// </summary>
    /// <param name="db">The DB to write</param>
    /// <param name="fName">The file to write to</param>
    /// <returns>An empty string or error message</returns>
    public static string WriteDb( awyDatabase db, string fName )
    {
      if ( File.Exists( fName ) ) return $"File does exist\n";

      return WriteDbFile( db, fName );
    }

  }
}
