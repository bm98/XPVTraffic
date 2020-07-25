using System;
using System.Collections.Generic;
using System.Text;
using libXPVTgen.coordlib;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A simple aircraft model
  /// base for implemented Aircrafts
  /// </summary>
  abstract class AcftModel
  {
    // abstract model default consts (can be overwritten by impl. models)
    protected double c_maxGS_kt = 180;
    protected double c_minVRate_ftPmin = 100;  // +-
    protected double c_maxVRate_ftPmin = 1500; // +-
    protected double c_minTRate_degPsec = 0.5; // +-
    protected double c_maxTRate_degPsec = 9.0; // +-
    protected double c_minAccel_knPsec = 0.5;  // +-
    protected double c_maxAccel_knPsec = 10;   // +-

    protected double c_maxAlt_ftMSL = 50_000;
    protected double c_accel_ktPsec = 3.0;     // default acceleration kt/s ?? Value ??
    protected double c_tRate_degPsec = 3.0;    // turnrate deg/sec

    // Private stuff
    private object m_lock = new object( );

    // actual sim time
    private DateTimeOffset m_tick = DateTimeOffset.Now;

    // base vars
    private bool m_altBaseMsl = false;    // bool       if true V command is MSL based
    private double m_alt_ftMslInit = 0;   // ft         start altitude MSL from scripting
    private double m_altRwy_ftMsl = 0;    // ft         the runway elevation (if there is any)
    // set vars
    private double m_setGS_kt = 0;        // kt (nm/h)  target speed
    private double m_setAlt_ftMsl = 0;    // ft         target alt MSL
    private double m_setTrk_degm = 0.0;   // deg        target Trk

    // dynamic vars
    private double m_accel_ktPsec = 0.0;  // kt/s       acft acceleration
    private double m_vRate_ftPmin = 0;    // ft/Min     vertical rate
    private double m_tRate_degPsec = 0;   // deg/sec    turn rate

    // state vars
    private LatLon m_pos = new LatLon( ); // lat/lon    current position
    private double m_alt_ftMsl = 0;       // ft         current altitude MSL
    private double m_trk_degm = 0;        // degm       current track
    private double m_gs_kt = 0;           // kt         current ground speed
    private double m_turn_deg = 0.0;      // deg        remaining turn angle
    private double m_leg_nm = 0.0;        // nm         remaining leg length

    private string m_acftID = "";         // aircraft   registration (for logging only)

    private CmdList m_route = new CmdList( );   // list of commands to fly
    private CmdBase m_currentSeg = null;        // actual segment

    /// <summary>
    /// Current Pos [lat,lon]
    /// </summary>
    public LatLon Pos { get => m_pos; }

    /// <summary>
    /// Current Altitude [ft MSL]
    /// </summary>
    public double Alt_ftMsl { get => m_alt_ftMsl; }

    /// <summary>
    /// Current Track [degm]
    /// </summary>
    public double Trk_degm { get => m_trk_degm; }

    /// <summary>
    /// Current Groundspeed [kt]
    /// </summary>
    public double GS_kt { get => m_gs_kt; }

    /// <summary>
    /// Current Vertical Rate VSI [ft/Min]
    /// </summary>
    public double VRate_ftPmin { get => m_vRate_ftPmin; }

    /// <summary>
    /// True if we are above ground
    /// </summary>
    public bool Airborne { get => m_alt_ftMsl > ( m_altRwy_ftMsl + 10 ); } // use some tolerance for this 10ft

    /// <summary>
    /// Returns the UNIX 'Epoch' Timestamp of this position
    /// </summary>
    public long TimeStamp_sec { get => m_tick.ToUnixTimeSeconds( ); }

    /// <summary>
    /// True if the flight has finished and the acraft needs to be removed
    /// </summary>
    public bool Out { get => ( m_currentSeg is CmdE ) && TurnFinished && LegFinished; }

    /// <summary>
    /// Current LegLength to travel straight
    /// </summary>
    private double Leg_nm
    {
      get => m_leg_nm;
      set { lock ( m_lock ) { m_leg_nm = value; } }
    }

    /// <summary>
    /// True if a turn/heading change has finished
    /// </summary>
    private bool TurnFinished { get => m_turn_deg == 0; } // double specs that 0.0 exists..

    /// <summary>
    /// True if a leg is finished
    /// </summary>
    private bool LegFinished { get => m_leg_nm <= 0; }

    /// <summary>
    /// cTor: Init a new flight model
    /// </summary>
    /// <param name="route">Segment list to fly</param>
    public AcftModel( CmdList route )
    {
      m_acftID = route.Descriptor.AircraftTailReg;

      m_pos = new LatLon( route.Descriptor.StartPos_latlon );

      m_altBaseMsl = ( route.Descriptor.FlightType == CmdA.FlightT.Runway ) ? false : true; // relative altitudes at init for Runway scripts
      m_alt_ftMslInit = route.Descriptor.StartAlt_ftMsl; // save initial altMSL for later reference
      m_altRwy_ftMsl = route.Descriptor.RwyAlt_ftMsl;

      m_alt_ftMsl = m_alt_ftMslInit; // current alt at startup
      m_setAlt_ftMsl = m_alt_ftMsl;  // target = current
      m_vRate_ftPmin = 0;

      m_trk_degm = route.Descriptor.StartBrg_degm.AddDegrees( 0 ); // avoid circle issues..
      m_setTrk_degm = m_trk_degm;
      m_turn_deg = 0;
      m_tRate_degPsec = 0;

      m_gs_kt = route.Descriptor.StartGS_kn;
      m_setGS_kt = m_gs_kt;
      m_accel_ktPsec = 0;
      m_leg_nm = 0;

      m_route = route.DeepCopy( ); // make a local copy to manipulate
      m_tick = DateTime.Now;
      Logger.Instance.Log( $"AcftModel-Init-{m_acftID} Pos: {m_pos.ToString( )}; AltMSL: {m_alt_ftMslInit}; Trk: {m_trk_degm}" );

      NextSeg( ); // load first segment
    }

    /// <summary>
    /// Set a new Alt target and vs to go there 
    /// Limits are +- model max ft/min and 0 .. model max ft
    /// Set Alt absolute or relative to initial startup depends on the UseMSL flag (default is relative - change with M command)
    /// </summary>
    /// <param name="vs">vert speed [ft/min] +-model max </param>
    /// <param name="alt">target alt [ft] 0 .. alt .. model max </param>
    private void SetAlt( double vs, double alt )
    {
      var newAltMsl = ( m_altBaseMsl ) ? alt : m_alt_ftMslInit + alt; // set new target alt according to alt mode

      // some sanity checks - set defaults if input is out of bounds
      if ( vs > c_maxVRate_ftPmin ) vs = c_maxVRate_ftPmin;
      if ( vs < c_minVRate_ftPmin ) vs = c_minVRate_ftPmin;
      if ( newAltMsl > c_maxAlt_ftMSL ) newAltMsl = c_maxAlt_ftMSL;
      if ( newAltMsl < 0 ) newAltMsl = 0;

      lock ( m_lock ) {
        m_setAlt_ftMsl = newAltMsl;
        if ( m_setAlt_ftMsl > m_alt_ftMsl ) m_vRate_ftPmin = vs; // must climb
        if ( m_setAlt_ftMsl < m_alt_ftMsl ) m_vRate_ftPmin = -vs; // must descend
      }
    }

    /// <summary>
    /// Set a new GS Speed target kts (>0 .. max) and accel
    /// </summary>
    /// <param name="gs">target spd [kt] >0 .. max </param>
    /// <param name="accel">accel [kt/sec] >0 .. max </param>
    /// <param name="immediate">Set speed immediate [bool] </param>
    private void SetSpeed( double gs, double accel, bool immediate )
    {
      // some sanity checks - set defaults if input is out of bounds
      if ( gs > c_maxGS_kt ) gs = c_maxGS_kt;
      if ( gs <= 0 ) gs = 10;
      if ( accel > c_maxAccel_knPsec ) accel = c_maxAccel_knPsec;
      if ( accel < c_minAccel_knPsec ) accel = c_minAccel_knPsec;

      lock ( m_lock ) {
        m_setGS_kt = gs;
        if ( m_setGS_kt > m_gs_kt ) m_accel_ktPsec = accel; // must speedup
        if ( m_setGS_kt < m_gs_kt ) m_accel_ktPsec = -accel; // must speeddown
        if ( immediate ) m_gs_kt = gs; // immediate (used to intro IFR model at speed)
      }
    }

    /// <summary>
    /// Set a turn 
    /// </summary>
    /// <param name="angle">turn angle [°] -360 .. angle .. 360 </param>
    /// <param name="turnRate">turnrate [deg/sec] 0.5..9.0 </param>
    private void SetTurn( double angle, double turnRate )
    {
      // some sanity checks - set defaults if input is out of bounds
      if ( angle > 360.0 ) angle = angle.AddDegrees( 0 );
      if ( angle < -360.0 ) angle = angle.AddDegrees( 0 );
      if ( turnRate > c_maxTRate_degPsec ) turnRate = c_maxTRate_degPsec;
      if ( turnRate < c_minTRate_degPsec ) turnRate = c_minTRate_degPsec;

      lock ( m_lock ) {
        m_turn_deg = angle;
        m_setTrk_degm = m_setTrk_degm.AddDegrees( angle );
        m_tRate_degPsec = turnRate;
      }
    }

    /// <summary>
    /// Set a target Track
    /// </summary>
    /// <param name="brg">target brg [degm] 0..360</param>
    /// <param name="turnRate">turnrat [deg/sec] 0.5..9.0 </param>
    private void SetTrack( double brg, double turnRate )
    {
      // some sanity checks - set defaults if input is out of bounds
      if ( brg < 0 ) brg = 0;
      if ( brg > 360 ) brg = 360;
      if ( turnRate > c_maxTRate_degPsec ) turnRate = c_maxTRate_degPsec;
      if ( turnRate < c_minTRate_degPsec ) turnRate = c_minTRate_degPsec;

      lock ( m_lock ) {
        // set a turn towards the smaller angle
        double turn = brg.SubDegrees( m_trk_degm ); // -> positive angle
        // take the shorter one (<=180.0)
        if ( turn <= 180.0 )
          m_turn_deg = turn;
        else
          m_turn_deg = turn - 360.0;

        m_setTrk_degm = brg;
        m_tRate_degPsec = turnRate;
      }
    }

    /// <summary>
    /// Set a Track and leg to destination
    /// TODO - this is not that accurate as the turn is not immediate and the cal is assuming this
    ///        Workaround is to use a steep turnrate for now
    /// </summary>
    /// <param name="dest">target [lat/lon]</param>
    /// <param name="turnRate">turnrat [deg/sec] 0.5..9.0 </param>
    private void SetDestination( LatLon dest, double turnRate )
    {
      // some sanity checks - set defaults if input is out of bounds
      if ( turnRate > c_maxTRate_degPsec ) turnRate = c_maxTRate_degPsec;
      if ( turnRate < c_minTRate_degPsec ) turnRate = c_minTRate_degPsec;

      // from current pos (does not take the turn into account for now)
      var brg = m_pos.BearingTo( dest );
      var dist = m_pos.DistanceTo( dest, ConvConsts.EarthRadiusNm );

      SetTrack( brg, c_maxTRate_degPsec ); // TODO fix Workaround, use MaxTurnrate
      Leg_nm = dist;
    }

    /// <summary>
    /// Correct the leg for a turn radius
    /// </summary>
    /// <returns>A +- nm correction for the leg</returns>
    private void ApplyLegCorrection()
    {
      if ( m_route.Count > 0 && m_route.Peek( ) is CmdG ) {
        // we may need to turn before the next segment and hence shorten the leg accordingly
        var g = m_route.Peek( ) as CmdG;
        var midPos = m_pos.DestinationPoint( m_leg_nm, m_trk_degm ); // that would be our intermediate dest
        var endBrg = midPos.BearingTo( g.Destination ); // track to go for
        // set a turn towards the smaller angle
        double turn = endBrg.SubDegrees( m_trk_degm ); // -> positive angle
        // take the shorter one (<=180.0)
        if ( turn > 180.0 ) turn = turn - 360.0;
        double cutLen = 0; // this would be our cut of the leg
        // don't bother with small angles
        if ( Math.Abs( turn ) > 3 ) {
          var turnTime = Math.Abs( turn ) / g.TurnRate;
          cutLen = turnTime * ( m_gs_kt / 3600.0 ); // turn nm before end to make the turn
          Logger.Instance.Log( $"AcftModel-{m_acftID} SegG peeked, next turn is {turn:##0}° - cutting {cutLen} nm from current leg {m_leg_nm}" );
        }
        else {
          Logger.Instance.Log( $"AcftModel-{m_acftID} SegG peeked, next turn is {turn:##0}° - not cutting from current leg {m_leg_nm}" );
        }
        m_leg_nm -= cutLen;
      }
    }

    // Set the next segment active
    // Note: dont use this in a locked block as it locks to update items !!!!
    private void NextSeg()
    {
      if ( m_route.Count > 0 )
        m_currentSeg = m_route.Dequeue( );

      switch ( m_currentSeg.Cmd ) {
        case Cmd.A:
          ; // debug stop only, all CmdA handling was done in cTor
          Logger.Instance.Log( $"AcftModel-{m_acftID} SegA" );
          NextSeg( );
          break;
        case Cmd.D:
          Leg_nm = ( m_currentSeg as CmdD ).Dist;
          Logger.Instance.Log( $"AcftModel-{m_acftID} SegD" );
          ApplyLegCorrection( );
          break;
        case Cmd.T:
          SetTurn( ( m_currentSeg as CmdT ).TurnAngle, ( m_currentSeg as CmdT ).TurnRate );
          Logger.Instance.Log( $"AcftModel-{m_acftID} SegT" );
          ApplyLegCorrection( );
          break;
        case Cmd.H:
          SetTrack( ( m_currentSeg as CmdH ).Bearing, ( m_currentSeg as CmdH ).TurnRate );
          Logger.Instance.Log( $"AcftModel-{m_acftID} SegH" );
          ApplyLegCorrection( );
          break;
        case Cmd.G:
          SetDestination( ( m_currentSeg as CmdG ).Destination, ( m_currentSeg as CmdG ).TurnRate );
          Logger.Instance.Log( $"AcftModel-{m_acftID} SegG" );
          ApplyLegCorrection( );
          break;

        case Cmd.S:
          SetSpeed( ( m_currentSeg as CmdS ).GS, ( m_currentSeg as CmdS ).Accel, ( m_currentSeg as CmdS ).Immediate );
          Logger.Instance.Log( $"AcftModel-{m_acftID} SegS" );
          ApplyLegCorrection( );
          NextSeg( ); // immediate command - just trigger the next one
          break;
        case Cmd.V:
          SetAlt( ( m_currentSeg as CmdV ).VSI, ( m_currentSeg as CmdV ).AltAGL );
          Logger.Instance.Log( $"AcftModel-{m_acftID} SegV" );
          ApplyLegCorrection( );
          NextSeg( ); // immediate command - just trigger the next one
          break;

        case Cmd.M:
          m_altBaseMsl = ( m_currentSeg as CmdM ).MslBased;
          double alt = ( m_currentSeg as CmdM ).AltMsl;
          if ( m_altBaseMsl && alt > 0 ) {
            // set absolute altitude for the next step
            m_setAlt_ftMsl = alt;
            m_alt_ftMsl = alt;
            m_vRate_ftPmin = 0; // and no change anymore
          }
          Logger.Instance.Log( $"AcftModel-{m_acftID} SegM" );
          ApplyLegCorrection( );
          NextSeg( ); // immediate command - just trigger the next one
          break;
        default:
          break;
      }

    }


    /// <summary>
    /// Increment Model
    /// either at realtime pace or simulated pace
    /// </summary>
    /// <param name="stepSec">sim step seconds ( -1 to use the realtime clock)</param>
    public void PaceModel( int stepSec )
    {
      var now = DateTimeOffset.Now; // realtime clock
      if ( stepSec > 0 ) {
        now = m_tick.AddSeconds( stepSec ); // use simulation time if asked for
      }
      // lockout any change to conditions while updating
      lock ( m_lock ) {

        double dt = ( now - m_tick ).TotalSeconds;
        m_tick = now; // update 

        // Model Calculations
        var nGS = m_gs_kt + m_accel_ktPsec * dt;
        var nAlt = m_alt_ftMsl + m_vRate_ftPmin / 60.0 * dt;
        // check for end of change in speed and alt
        if ( m_accel_ktPsec > 0 && nGS > m_setGS_kt ) { nGS = m_setGS_kt; m_accel_ktPsec = 0.0; } // accel finished, maintain setTas
        if ( m_accel_ktPsec < 0 && nGS < m_setGS_kt ) { nGS = m_setGS_kt; m_accel_ktPsec = 0.0; } // decel finished, maintain setTas
        if ( m_vRate_ftPmin > 0 && nAlt > m_setAlt_ftMsl ) { nAlt = m_setAlt_ftMsl; m_vRate_ftPmin = 0; } // ascent finished, maintain setAlt
        if ( m_vRate_ftPmin < 0 && nAlt < m_setAlt_ftMsl ) { nAlt = m_setAlt_ftMsl; m_vRate_ftPmin = 0; } // descent finished, maintain setAlt

        double rot = 0.0; // no turning to start with
        if ( m_turn_deg < 0 )
          rot = -m_tRate_degPsec * dt; // need to turn left
        else if ( m_turn_deg > 0 )
          rot = m_tRate_degPsec * dt;  // need to turn right

        m_turn_deg -= rot; // reduce turn for the rot amount
        if ( rot > 0 && m_turn_deg < 0 ) {
          rot += m_turn_deg; m_turn_deg = 0;
        } // correct rot and set turn to finished
        if ( rot < 0 && m_turn_deg > 0 ) {
          rot += m_turn_deg; m_turn_deg = 0;
        } // correct rot and set turn to finished

        double nTrack = m_trk_degm.AddDegrees( rot );
        if ( m_turn_deg == 0 ) nTrack = m_setTrk_degm; // avoid rounding errors, maintain setTrk

        double dist = nGS / 3600.0 * dt;
        if ( m_leg_nm > 0 && dist > m_leg_nm ) dist = m_leg_nm; // correct at end of a D leg


        // assign new values
        m_pos = m_pos.DestinationPoint( dist, m_trk_degm + rot / 2.0, ConvConsts.EarthRadiusNm ); // use linear move at tanget angle
        m_trk_degm = nTrack;
        m_alt_ftMsl = nAlt;
        m_gs_kt = nGS;
        m_leg_nm -= dist;
        if ( m_leg_nm < 0 ) m_leg_nm = 0;
      }//lock

      // progress through segments if needed
      if ( m_currentSeg is CmdD && LegFinished ) NextSeg( );
      else if ( m_currentSeg is CmdG && LegFinished ) NextSeg( );
      else if ( m_currentSeg is CmdH && TurnFinished ) NextSeg( );
      else if ( m_currentSeg is CmdT && TurnFinished ) NextSeg( );
      // we don't move if GS is zero i.e. wait endlessly for the next segment
      else if ( m_gs_kt <= 0 ) NextSeg( );

    }

  }
}
