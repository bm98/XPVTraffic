using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libXPVTgen.Aircrafts
{
  class AcftTypes
  {
    private static Random m_random = new Random( );

    private static List<string> m_acTypes = new List<string>( )
    { "B732", "B733", "B734", "	B735", "B736", "B737", "B738",
      "B743", "B753", "B762", "B763", "B764", "	B772", "B773", "B788", "B789",
      "A320", "A332", "A333", "A342", "A343", "A345", "A346", "A388", "A20N", "A21N",
      "	MC23", "T134", "T144", "MD11", "E120", "AN26", "JS41", "C130", "SB20",
      "C525", "C550", "E50P", "LJ24", "PA47", "PC24" , "PC12" , "EA50" };
    private static List<string> m_acTypesGA = new List<string>( )
    { "PC12", "PC6T", "C172", "C162", "C182", "BE58", "BE88", "CRBN", "DHC6", "VELT", "SF50", "BE9L"};

    public static string GetAcftType { get => m_acTypes.ElementAt( m_random.Next( 0, m_acTypes.Count ) ); }
    public static string GetGAAcftType { get => m_acTypesGA.ElementAt( m_random.Next( 0, m_acTypesGA.Count ) ); }


  }
}
