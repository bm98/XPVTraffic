using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// End Command; signals end of script
  /// </summary>
  class CmdE : CmdBase
  {
    /// <summary>
    /// cTor:
    /// </summary>
    public CmdE()
    {
      Cmd = Cmd.E;
    }

    /// <summary>
    /// Write the Command to the stream
    /// </summary>
    /// <param name="stream">The output stream</param>
    public override void WriteToStream( StreamWriter stream )
    {
      ; // we don't write CmdE as it is appended by the reader
    }

  }
}
