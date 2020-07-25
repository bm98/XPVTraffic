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

    private int m_acftIndex = 0;

    /// <summary>
    /// Number of virtual Acft in the Pool
    /// </summary>
    public uint NumAcft { get; set; } = 100;
    /// <summary>
    /// Number of virtual VFR aircrafts
    /// </summary>
    public uint NumVFRcraft { get; set; } = 20;

    /// <summary>
    /// The selection range
    /// </summary>
    public double Range_nm { get; private set; } = 100;

    /// <summary>
    /// The Step Length to update Virt Aircrafts for LiveTraffic [sec]
    /// </summary>
    public uint StepLen_sec { get; private set; } = 10; // default

    // Get one route script as random selection or null 
    private CmdList GetRandomRouteScript()
    {
      if ( m_vfrScripts.Count <= 0 ) return null; // no script in table 

      int item = m_random.Next( m_vfrScripts.Count );
      return m_vfrScripts.ElementAt( item );
    }

    // Get one runway as random selection or null 
    // preferrence will be used if found
    private rwyRec GetRandomRwy( string pref_rwy_id, bool strict )
    {
      if ( m_rwysSelected.Count <= 0 ) return null; // no Rwy in table 
      // if defined look for the preferred one
      if ( !string.IsNullOrEmpty( pref_rwy_id ) ) {
        if ( m_rwysSelected.ContainsKey( pref_rwy_id ) ) {
          return m_rwysSelected[pref_rwy_id];
        }
        if ( strict )
          return null; // cannot use a random one
      }

      // preferred not defined or not found, return a random one
      int item = m_random.Next( m_rwysSelected.Count );
      return m_rwysSelected.ElementAt( item ).Value;
    }

    // returns true for an IFR to be created
    private bool IsRandomAcftIFR()
    {
      if ( m_numVFR >= NumVFRcraft ) return true; // VFRs are full
      if ( m_numIFR >= ( NumAcft - NumVFRcraft ) ) return false; // IFRs are full

      int item = m_random.Next( 4 ); // create 1/4 VFRs
      return item > 0;
    }

    /// <summary>
    /// cTor: Init the pool
    /// </summary>
    /// <param name="stepLen_sec">Use stepLen [sec] as update pace</param>
    public VAcftPool( double range_nm, uint stepLen_sec )
    {
      Range_nm = range_nm;
      StepLen_sec = stepLen_sec;
      m_lastTime = DateTime.Now;
    }

    /// <summary>
    /// Returns the pool
    /// </summary>
    public List<VAcft> AircraftPoolRef { get => m_pool; }

    /// <summary>
    /// Create the selection of airways/runways to choose from
    /// </summary>
    /// <param name="awysRef">All Airways</param>
    /// <param name="rwysRef">All Runways</param>
    /// <param name="radius_nm">Selection radius [nm]</param>
    /// <param name="acftPos">Position to select Airways from</param>
    public void CreateAwySelection( awyTable awysRef, rwyTable rwysRef, LatLon acftPos )
    {
      m_userPos = new LatLon( acftPos );
      m_awysSelected = awysRef.GetSubtable( Range_nm, m_userPos.Lat, m_userPos.Lon );
      m_rwysSelected = rwysRef.GetSubtable( Range_nm / 2.0, m_userPos.Lat, m_userPos.Lon ); // runways only around half the radius
    }

    /// <summary>
    /// Update the selection of airways to choose from
    /// 1/4 from last target
    /// </summary>
    /// <param name="awysRef">All Airways</param>
    /// <param name="rwysRef">All Runways</param>
    /// <param name="radius_nm">Selection radius [nm]</param>
    /// <param name="acftPos">Position to select Airways from</param>
    public void UpdateAwySelection( awyTable awysRef, rwyTable rwysRef, LatLon acftPos )
    {
      // update if further away than 1/4 radius
      if ( m_userPos.DistanceTo( acftPos, ConvConsts.EarthRadiusNm ) > ( Range_nm / 4 ) ) {
        m_userPos = new LatLon( acftPos );
        m_awysSelected = awysRef.GetSubtable( Range_nm, m_userPos.Lat, m_userPos.Lon );
        m_rwysSelected = rwysRef.GetSubtable( Range_nm / 2.0, m_userPos.Lat, m_userPos.Lon ); // runways only around half the radius
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
      for ( int i = m_pool.Count - 1; i >= 0; i-- ) { // start from end, else we messup when deleting records
        var vac = m_pool.ElementAt( i );
        if ( vac.Out ) {
          // This aircraft run out of it's current plan
          m_pool.RemoveAt( i ); // remove old
          Logger.Instance.Log( $"VAcftPool-Removed: {vac.ID} - {vac.AcftFrom} - {vac.AcftTo}" );

          if ( vac is IFRvAcft ) {
            m_numIFR--;
          }
          else if ( vac is VFRvAcft ) {
            m_numVFR--;
          }
        }
      }

      // create inbounds
      if ( m_pool.Count < NumAcft ) {

        if ( IsRandomAcftIFR( ) ) {
          // IFR - get a supported aircraft type and operator
          var acftTypeOp = AircraftSelection.GetRandomAcftTypeOp( );
          // get a route
          var route = IFRroute.GetRandomFlight( m_awysSelected, m_acftIndex++, acftTypeOp.AcftType, acftTypeOp.AcftOperator );
          if ( route == null ) return; // no airways available ??
          // and create a flight
          var vac = new IFRvAcft( route );
          m_pool.Add( vac );
          m_numIFR++;
          Logger.Instance.Log( $"VAcftPool-Airway:     {vac.ID} - {vac.AcftFrom} - {vac.AcftTo}" );
        }
        else {
          // VFR - get a random VFR route script
          var route = GetRandomRouteScript( );

          if ( route.Descriptor.FlightType == CmdA.FlightT.Runway ) {
            // get the preferred RWY if possible, else it is a random one in range
            var rwy = GetRandomRwy( route.Descriptor.RunwayPreference, route.Descriptor.RunwayPrefStrict );
            if ( rwy == null ) return; // no runways available ??
            // the simulated aircraft
            route.Descriptor.InitFromRunway( m_acftIndex++, rwy ); // Complete the script
            var vac = new VFRvAcft( route );
            m_pool.Add( vac );
            m_numVFR++;
            Logger.Instance.Log( $"VAcftPool-Runway:     {vac.ID} - {vac.AcftFrom} - {vac.AcftTo}" );
          }
          else if ( route.Descriptor.FlightType == CmdA.FlightT.Airway ) {
            return; // not supported - use the one above...
          }
          else if ( route.Descriptor.FlightType == CmdA.FlightT.MsgRelative ) {
            // get the preferred RWY if possible, else it is a random one in range
            var rwy = GetRandomRwy( "", false );
            if ( rwy == null ) return; // no runways available ??
            // the simulated aircraft
            route.Descriptor.InitFromMsgRelative( m_acftIndex++, rwy, "YYY" ); // Complete the script
            var vac = new VFRvAcft( route );
            m_pool.Add( vac );
            m_numVFR++;
            Logger.Instance.Log( $"VAcftPool-MsgRel:     {vac.ID} - {vac.AcftFrom} - {vac.AcftTo}" );
          }
          else if ( route.Descriptor.FlightType == CmdA.FlightT.MsgAbsolute ) {
            if ( m_userPos.DistanceTo( route.Descriptor.StartPos_latlon, ConvConsts.EarthRadiusNm ) > Range_nm ) return; // the starting point is out of range
            // the simulated aircraft is in range
            route.Descriptor.InitFromMsgAbsolute( m_acftIndex++, "YYY" ); // Complete the script
            var vac = new VFRvAcft( route );
            m_pool.Add( vac );
            m_numVFR++;
            Logger.Instance.Log( $"VAcftPool-MsgAbs:     {vac.ID} - {vac.AcftFrom} - {vac.AcftTo}" );

          }
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
