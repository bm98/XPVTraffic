using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libXPVTgen.coordlib;

using libXPVTgen.my_awlib;

namespace libXPVTgen
{
  /// <summary>
  /// The pool of virtual aircrafts
  /// </summary>
  internal class VAcftPool
  {
    private static Random m_random = new Random( );

    private List<VAcft> m_pool = new List<VAcft>( );
    private DateTime m_lastTime;

    private awyTable m_awysSelected = new awyTable( ); // contains the selected Airways for a region
    private LatLon m_userPos = new LatLon( );         // last known user position

    /// <summary>
    /// Number of virtual Acft in the Pool
    /// </summary>
    public uint NumAcft { get; set; } = 100;
    /// <summary>
    /// The Step Length to update Virt Aircrafts for LiveTraffic [sec]
    /// </summary>
    public uint StepLen_sec { get; private set; } = 10; // default

    private uint m_acCount = 0;

    // Get one airway as random selection or null 
    private awyRec GetRandomAwy()
    {
      if ( m_awysSelected.Count <= 0 ) return null; // no Awy in table 

      int item = m_random.Next( m_awysSelected.Count );
      return m_awysSelected.ElementAt( item ).Value;
    }


    /// <summary>
    /// cTor: Init the pool
    /// </summary>
    public VAcftPool( uint stepLen_sec )
    {
      StepLen_sec = stepLen_sec;
      m_lastTime = DateTime.Now;
    }

    public List<VAcft> AircraftPool { get => m_pool; }

    /// <summary>
    /// (Re)Create the selection of airways to choose from
    /// </summary>
    /// <param name="awysRef">All Airways</param>
    /// <param name="radius_nm">Selection radius [nm]</param>
    /// <param name="acftPos">Position to select Airways from</param>
    public void CreateAwySelection( awyTable awysRef, double radius_nm, LatLon acftPos )
    {
      m_userPos = new LatLon( acftPos );
      m_awysSelected = awysRef.GetSubtable( radius_nm, m_userPos.Lat, m_userPos.Lon );
    }

    /// <summary>
    /// Update the selection of airways to choose from
    /// 1/4 from last target
    /// </summary>
    /// <param name="awysRef">All Airways</param>
    /// <param name="radius_nm">Selection radius [nm]</param>
    /// <param name="acftPos">Position to select Airways from</param>
    public void UpdateAwySelection( awyTable awysRef, double radius_nm, LatLon acftPos )
    {
      // updat if further away than 1/4 radius
      if ( m_userPos.DistanceTo(acftPos, ConvConsts.EarthRadiusNm) > ( radius_nm / 4 ) ) {
        m_userPos = new LatLon( acftPos );
        m_awysSelected = awysRef.GetSubtable( radius_nm, m_userPos.Lat, m_userPos.Lon );
      }
    }

    /// <summary>
    /// Re-Generate the Pool 
    /// i.e. throw outbounds out, create new inbounds until the pool is full again
    /// </summary>
    public void ReGenerate()
    {
      // check outbounds
      for ( int i = m_pool.Count; i > 0; i-- ) {
        var vac = m_pool.ElementAt( i - 1 );
        if ( vac.Out ) {
          // do we have an airway to go from here?
          VAcft newVac = null;
          var newleg = m_awysSelected.GetNextSegment( vac.Route );
          if ( newleg != null ) {
            newVac = new VAcft( vac, newleg );
          }
          m_pool.RemoveAt( i - 1 ); // remove old
          if ( newVac != null ) {
            Logger.Instance.Log( $"VAcftPool-NewLeg: {newVac.ID} - {newVac.Route.ident}" );
            m_pool.Add( newVac ); // add same with new leg
          }
          else {
            Logger.Instance.Log( $"VAcftPool-Removed: {vac.ID} - {vac.Route.ident}" );
          }
        }
      }

      // create inbounds
      while ( m_pool.Count < NumAcft ) {
        var awy = GetRandomAwy( );
        if ( awy == null ) break; // no airways available

        string actype = VAcft.GetAcftType;
        if ( awy.layer == 3 ) {
          // our VFR layer
          actype = VAcft.GetGAAcftType;
        }
        var alt = m_random.Next( awy.baseFt, awy.topFt );
        alt = (int)Math.Round( alt / 100.0 ) * 100; // get 100 ft increments
        var tas = 100;
        if ( awy.layer==1) { // low
          tas = m_random.Next( 140, 210 ); // some variations is tas..
        }
        else if ( awy.layer == 2 ) { // high
          tas = m_random.Next( 190, 320 ); // some variations is tas..
        }
        else {// vfr
          tas = m_random.Next( 100, 150 ); // some variations is tas..
        }
        var vac = new VAcft( $"VAC-{++m_acCount:0000}", awy, tas, StepLen_sec ) {
          Alt_ft = alt, AcftType = actype, AcftHex = $"00{m_acCount:0000}", // TODO alt, speed...
        };
        m_pool.Add( vac );
        Logger.Instance.Log( $"VAcftPool-Add:     {vac.ID} - {vac.Route.ident}" );
      }
    }

    /// <summary>
    /// Increment the Acfts in the pool
    /// </summary>
    public void Update()
    {
      Logger.Instance.Log( $"VAcftPool-Update now" );
      foreach ( var vac in m_pool ) {
        vac.IncPos( );
      }
    }

  }
}
