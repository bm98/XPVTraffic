using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libXPVTgen.coordlib;
using libXPVTgen.my_awlib;

namespace libXPVTgen.Aircrafts
{
  /// <summary>
  /// A Virtual Aircraft
  /// </summary>
  internal abstract class VAcft : Acft
  {
    public abstract double VSI { get; } // ft/min
    public abstract double HDG { get; }
    public abstract double TAS { get; }
    public abstract string AcftHex { get; }  // S-Mode Code "000nnn"
    public abstract string AcftType { get; }
    public abstract string AcftReg { get; }   // XY-000
    public abstract string AcftFrom { get; }
    public abstract string AcftTo { get; }
    public abstract long TStamp { get; }

    /// <summary>
    /// True if the acft is no longer active (end of route)
    /// </summary>
    public abstract bool Out { get; }

    /// <summary>
    /// Paces the model one step
    /// </summary>
    public abstract void StepModel();

  }
}
