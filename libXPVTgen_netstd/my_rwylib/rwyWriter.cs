using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libXPVTgen.my_rwylib
{
  public class rwyWriter
  {
    private static string WriteDbFile( rwyDatabase db, string fName )
    {
      string ret = "";
      using ( var sr = new StreamWriter( fName ) ) {
        foreach(var awrec in db.GetTable() ) {
          sr.WriteLine( awrec.Value.ToString( ) );
        }
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
    public static string WriteDb( rwyDatabase db, string fName )
    {
      if ( File.Exists( fName ) ) return $"File does exist\n";

      return WriteDbFile( db, fName );
    }

  }
}
