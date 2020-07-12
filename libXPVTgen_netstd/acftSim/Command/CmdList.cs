using System;
using System.Collections.Generic;
using System.Text;

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
      for (int i=0; i<array.Length; i++ ) {
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
    public bool IsEmpty { get => this.Count < 1; }

  
    /// <summary>
    /// Return a Runway_ID if available, else an empty string
    /// </summary>
    public string Runway_ID
    {
      get {
        if ( this.Peek( ) is CmdA ) {
          return ( this.Peek( ) as CmdA ).RwyID;
        }
        return "";
      }
    }

    /// <summary>
    /// Return a Runway_ID if available, else an empty string
    /// </summary>
    public string AircraftType
    {
      get {
        if ( this.Peek( ) is CmdA ) {
          return ( this.Peek( ) as CmdA ).AcftType;
        }
        return "";
      }
    }
  }
}
