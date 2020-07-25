using System;
using System.Collections.Generic;
using System.Text;
using libXPVTgen.acftSim;

namespace libXPVTgen.Aircrafts
{
  /// <summary>
  /// An AI Traffic read aircraft
  /// </summary>
  internal class AITvAcft : VAcft
  {

    public new double VSI { get; set; } = 0;
    public new double TRK { get; set; } = 0;
    public new double GS { get; set; } = 0;
    public new long TStamp { get; set; } = 0;
    public new bool Airborne { get; set; } = false;

    public new string AcftType { get; set; } ="";
    public new string AcftCallsign { get; set; } ="";
    public new string AcftTailReg { get; set; } ="";
    public new string AcftFrom { get; set; } ="";
    public new string AcftTo { get; set; } ="";
    public new string AcftHex { get; set; } ="";

  }
}
