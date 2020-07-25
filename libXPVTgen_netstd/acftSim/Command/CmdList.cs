using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Text;
using libXPVTgen.coordlib;
using libXPVTgen.my_rwylib;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A list of commands to execute
  /// Implemented as Queue
  /// </summary>
  class CmdList : Queue<CmdBase>
  {
    /// <summary>
    /// cTor: empty
    /// </summary>
    public CmdList()
    {
    }

    /// <summary>
    /// cTor: Copy from array of commands
    /// </summary>
    /// <param name="array"></param>
    public CmdList( CmdBase[] array )
    {
      for ( int i = 0; i < array.Length; i++ ) {
        this.Enqueue( array[i] );
      }
    }

    /// <summary>
    /// Return a deep copy of this list
    /// </summary>
    /// <returns></returns>
    public CmdList DeepCopy()
    {
      var s = this.ToArray( );
      return new CmdList( s );
    }

    /// <summary>
    /// Returns true if the list is empty
    /// </summary>
    public bool IsEmpty { get => this.Count < 2; } // must have A and E (2 elements at least)

    /// <summary>
    /// Returns true if the list is valid
    /// </summary>
    public bool IsValid
    {
      get {
        if ( IsEmpty ) return false;
        if ( !( this.First( ) is CmdA && this.Last( ) is CmdE ) ) return false;
        // more checks ??
        return true;
      }
    }

    public CmdA Descriptor
    {
      get {
        if ( ( this.Count > 0 ) && ( this.Peek( ) is CmdA ) ) return this.Peek( ) as CmdA;
        throw new NotSupportedException( "CmdList.Descriptor - CmdA element not found" ); // this is a program error..
      }
    }

    /// <summary>
    /// Write the Route to the stream
    /// </summary>
    /// <param name="stream">The output stream</param>
    public bool WriteRoute( StreamWriter stream )
    {
      if ( this.IsEmpty ) return false;
      stream.WriteLine( $"# Start of script" );
      foreach(var c in this ) { 
        c.WriteToStream( stream );
      }
      stream.WriteLine( $"# End of script" );
      return true;
    }

  }
}
