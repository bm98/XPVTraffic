using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libXPVTgen.acftSim;
using libXPVTgen.coordlib;

using libXPVTgen.my_awlib;
using libXPVTgen.my_rwylib;

namespace libXPVTgen.Aircrafts
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
    private rwyTable m_rwysSelected = new rwyTable( ); // contains the selected Runways for a region
    private List<CmdList> m_vfrScripts = new List<CmdList>( );

    private LatLon m_userPos = new LatLon( );         // last known user position

    private uint m_numIFR = 0;
    private uint m_numVFR = 0;

    /// <summary>
    /// Number of virtual Acft in the Pool
    /// </summary>
    public uint NumAcft { get; set; } = 100;
    /// <summary>
    /// Number of virtual VFR aircrafts
    /// </summary>
    public uint NumVFRcraft { get; set; } = 20;

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

    // Get one script as random selection or null 
    private CmdList GetRandomScript()
    {
      if ( m_vfrScripts.Count <= 0 ) return null; // no script in table 

      int item = m_random.Next( m_vfrScripts.Count );
      return m_vfrScripts.ElementAt( item );
    }

    // Get one runway as random selection or null 
    // preferrence will be used if found
    private rwyRec GetRandomRwy( string pref_rwy_id )
    {
      if ( m_rwysSelected.Count <= 0 ) return null; // no Rwy in table 
      // look for the preferred one
      if ( m_rwysSelected.ContainsKey( pref_rwy_id ) ) {
        return m_rwysSelected[pref_rwy_id];
      }
      // preferred not found, return a random one
      int item = m_random.Next( m_rwysSelected.Count );
      return m_rwysSelected.ElementAt( item ).Value;
    }

    // returns true for an IFR to be created
    private bool GetRandomAcftIFR()
    {
      if ( m_numVFR >= NumVFRcraft ) return true; // VFRs are full
      if ( m_numIFR >= ( NumAcft - NumVFRcraft ) ) return false; // IFRs are full

      int item = m_random.Next( 4 );
      return item > 0;
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
    public void CreateAwySelection( awyTable awysRef, rwyTable rwysRef, double radius_nm, LatLon acftPos )
    {
      m_userPos = new LatLon( acftPos );
      m_awysSelected = awysRef.GetSubtable( radius_nm, m_userPos.Lat, m_userPos.Lon );
      m_rwysSelected = rwysRef.GetSubtable( radius_nm / 2.0, m_userPos.Lat, m_userPos.Lon ); // runways only around half the radius
    }

    /// <summary>
    /// Update the selection of airways to choose from
    /// 1/4 from last target
    /// </summary>
    /// <param name="awysRef">All Airways</param>
    /// <param name="radius_nm">Selection radius [nm]</param>
    /// <param name="acftPos">Position to select Airways from</param>
    public void UpdateAwySelection( awyTable awysRef, rwyTable rwysRef, double radius_nm, LatLon acftPos )
    {
      // updat if further away than 1/4 radius
      if ( m_userPos.DistanceTo( acftPos, ConvConsts.EarthRadiusNm ) > ( radius_nm / 4 ) ) {
        m_userPos = new LatLon( acftPos );
        m_awysSelected = awysRef.GetSubtable( radius_nm, m_userPos.Lat, m_userPos.Lon );
        m_rwysSelected = rwysRef.GetSubtable( radius_nm / 2.0, m_userPos.Lat, m_userPos.Lon ); // runways only around half the radius
      }
    }

    /// <summary>
    /// Update the VFR Scripts to use
    /// </summary>
    /// <param name="scripts"></param>
    public void UpdateVFRscripts( List<CmdList> scripts )
    {
      m_vfrScripts = scripts;
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
          // This aircraft run out of it's current plan - see if we can extend or drop
          if ( vac is IFRvAcft ) {
            // IFRs may add a leg 
            var iVac = ( vac as IFRvAcft );
            IFRvAcft newVac = null;
            // do we have an airway to go from here?
            var newleg = m_awysSelected.GetNextSegment( iVac.Route );
            if ( newleg != null ) {
              newVac = new IFRvAcft( iVac, newleg );
            }
            m_pool.RemoveAt( i - 1 ); // remove old
            m_numIFR--;
            if ( newVac != null ) {
              Logger.Instance.Log( $"VAcftPool-NewLeg: {newVac.ID} - {newVac.Route.ident}" );
              m_pool.Add( newVac ); // add same with new leg
              m_numIFR++;
            }
            else {
              Logger.Instance.Log( $"VAcftPool-Removed: {vac.ID} - {iVac.Route.ident}" );
            }
          }
          else if ( vac is VFRvAcft ) {
            // VFRs are always out - no continuation element
            var iVac = ( vac as VFRvAcft );
            m_pool.RemoveAt( i - 1 ); // remove old
            m_numVFR--;
            Logger.Instance.Log( $"VAcftPool-Removed: {vac.ID} - {iVac.AcftFrom}" );
          }
        }
      }

      // create inbounds
      if ( m_pool.Count < NumAcft ) {

        if ( GetRandomAcftIFR( ) ) {
          // IFR
          string actype = AcftTypes.GetAcftType;
          var awy = GetRandomAwy( );
          if ( awy == null ) return; // no airways available

          var alt = m_random.Next( awy.baseFt, awy.topFt );
          alt = (int)Math.Round( alt / 100.0 ) * 100; // get 100 ft increments
          var tas = 100;
          if ( awy.layer == 1 ) { // low
            tas = m_random.Next( 140, 210 ); // some variations is tas..
          }
          else if ( awy.layer == 2 ) { // high
            tas = m_random.Next( 190, 320 ); // some variations is tas..
          }
          else {// vfr
            tas = m_random.Next( 100, 150 ); // some variations is tas..
          }
          var vac = new IFRvAcft( $"VAC-{++m_acCount:0000}", awy, tas, alt, StepLen_sec, actype,
                              Convert.ToInt32( m_acCount.ToString( ), 16 ).ToString( ) );
          m_pool.Add( vac );
          m_numIFR++;
          Logger.Instance.Log( $"VAcftPool-Add:     {vac.ID} - {vac.Route.ident}" );
        }
        else {
          // VFR
          string actype = AcftTypes.GetGAAcftType;
          var script = GetRandomScript( );
          var rwy = GetRandomRwy( script.Runway_ID );
          if ( rwy == null ) return; // no runways available

          var vac = new VFRvAcft( $"VGA-{++m_acCount:0000}", rwy, script, ( !string.IsNullOrEmpty( script.AircraftType ) ) ? script.AircraftType : actype,
                              Convert.ToInt32( m_acCount.ToString( ), 16 ).ToString( ) );
          m_pool.Add( vac );
          m_numVFR++;
          Logger.Instance.Log( $"VAcftPool-Add:     {vac.ID} - {vac.AcftFrom}" );
        }

      }
    }

    /// <summary>
    /// Increment the Acfts in the pool
    /// </summary>
    public void Update()
    {
      //Logger.Instance.Log( $"VAcftPool-Update now" );
      foreach ( var vac in m_pool ) {
        vac.StepModel( );
      }
    }

  }
}
