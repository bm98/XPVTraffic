using System;
using System.Collections.Generic;
using System.Text;

using libXPVTgen.coordlib;

namespace libXPVTgen.acftSim
{
  /// <summary>
  /// A simulation for a Virtual VFR type aircraft
  /// </summary>
  class VFRModel
  {
    private const double c_accel = 3.0; // default acceleration kt/s ?? Value ??
    private const double c_tRate = 3.0; // turnrate deg/sec ??

    private DateTimeOffset m_tick = DateTimeOffset.Now;  // actual sim time
    private object m_lock = new object( );

    // set vars
    private double m_setTas = 0;         // kt (nm/h) target speed
    private double m_setAltAgl = 0;      // ft        target alt
    private double m_setTrk = 0.0;    // deg       target Trk
    // dynamic vars
    private double m_accel = 0.0f;    // kt/s      applied accel
    private double m_turn = 0.0;      // deg       applied turn
    private double m_leg = 0.0;       // nm        applied leglength

    // state vars
    private LatLon m_pos = new LatLon( );
    private double m_altMslInit = 0;
    private double m_altAgl = 0;
    private double m_trk = 0;
    private double m_tas = 0;
    private double m_vs = 0;

    private string m_acftID = "";

    private CmdList m_route = new CmdList( );
    private CmdBase m_currentSeg = null;

    /// <summary>
    /// Current Pos [lat,lon]
    /// </summary>
    public LatLon Pos
    {
      get => m_pos;
      private set { lock ( m_lock ) { m_pos = new LatLon( value ); } }
    }

    /// <summary>
    /// Current Alt [ft]
    /// </summary>
    public double AltAgl
    {
      get => m_altAgl;
      private set { lock ( m_lock ) { m_altAgl = value; } }
    }

    /// <summary>
    /// Current Alt [ft]
    /// </summary>
    public double AltMsl
    {
      get => m_altAgl + m_altMslInit;
    }

    /// <summary>
    /// Current Track [°]
    /// </summary>
    public double Trk
    {
      get => m_trk;
      private set { lock ( m_lock ) { m_trk = value; } }
    }

    /// <summary>
    /// Current TAS [kt]
    /// </summary>
    public double TAS
    {
      get => m_tas;
      private set { lock ( m_lock ) { m_tas = value; } }
    }

    /// <summary>
    /// Current VSI [ft/Min]
    /// </summary>
    public double VSI
    {
      get => m_vs;
      private set { lock ( m_lock ) { m_vs = value; } }
    }

    /// <summary>
    /// Current LegLength to travel straight
    /// </summary>
    public double Leg
    {
      get => m_leg;
      private set { lock ( m_lock ) { m_leg = value; } }
    }

    /// <summary>
    /// True if a turn/heading change has finished
    /// </summary>
    public bool TurnFinished { get => m_turn == 0; } // double specs that 0.0 exists..

    /// <summary>
    /// True if a leg is passed
    /// </summary>
    public bool LegFinished { get => m_leg == 0; } // double specs that 0.0 exists..

    /// <summary>
    /// Returns the UNIX 'Epoch' Timestamp of this position
    /// </summary>
    public long TimeStamp { get => m_tick.ToUnixTimeSeconds( ); }

    public bool Out { get => ( m_currentSeg is CmdE ) && TurnFinished && LegFinished; }

    /// <summary>
    /// cTor: Init a new flight model
    /// </summary>
    /// <param name="pos">Initial LatLon position</param>
    /// <param name="alt">Initial Altitude MSL [ft]</param>
    /// <param name="hdg">Initial Heading [°]</param>
    public VFRModel( string acftID, LatLon pos, double alt, double hdg, CmdList route )
    {
      m_acftID = acftID;

      m_pos = new LatLon( pos );

      m_altMslInit = alt;
      m_altAgl = 0;
      m_setAltAgl = 0;
      m_vs = 0;

      m_trk = hdg.AddDegrees( 0 ); // avoid circle issues..
      m_setTrk = m_trk;
      m_turn = 0;

      m_tas = 0;
      m_setTas = m_tas;
      m_accel = 0;
      m_leg = 0;

      m_route = route.DeepCopy( );
      NextSeg( ); // load first segment

      m_tick = DateTime.Now;
      Logger.Instance.Log( $"VFRModel-Init-{m_acftID} Pos: {m_pos.ToString( )}; Alt: {alt}; Hdg: {hdg}" );
    }

    /// <summary>
    /// Set a new AGL Alt target and vs to go there 
    /// Limits are +- 1500 ft/min and 0 .. 10000 ft agl
    /// </summary>
    /// <param name="vs">vert speed [ft/min] +-1500 </param>
    /// <param name="altAgl">target alt agl [ft] 0 .. alt .. 10000 </param>
    public void SetAlt( double vs, double altAgl )
    {
      // some sanity checks
      if ( vs > 1500 ) return; // ERROR in vs
      if ( vs < -1500 ) return; // ERROR in vs
      if ( altAgl > 10_000 ) return; // ERROR in alt
      if ( altAgl < 0 ) return; // ERROR in alt

      lock ( m_lock ) {
        m_vs = vs;
        m_setAltAgl = altAgl;
      }
    }

    /// <summary>
    /// Set a new Speed target kts (>0 .. 180)
    /// </summary>
    /// <param name="tas">target spd [kt] >0 .. 180 </param>
    public void SetSpd( double tas )
    {
      // some sanity checks
      if ( tas > 180 ) return; // ERROR in spd
      if ( tas <= 0 ) return; // ERROR in spd

      lock ( m_lock ) {
        m_setTas = tas;
        if ( m_setTas > m_tas ) m_accel = c_accel;
        if ( m_setTas < m_tas ) m_accel = -c_accel;
      }
    }

    /// <summary>
    /// Set a turn 
    /// </summary>
    /// <param name="angle">turn angle [°] -360 .. angle .. 360 </param>
    public void SetTurn( double angle )
    {
      // some sanity checks
      if ( angle > 360.0 ) return; // ERROR in hdg 0..360
      if ( angle < -360.0 ) return; // ERROR in hdg 0..360

      lock ( m_lock ) {
        m_turn = angle;
        m_setTrk = m_setTrk.AddDegrees( angle );
      }
    }

    /// <summary>
    /// Set a target Hdg
    /// </summary>
    /// <param name="hdg">target hdg [°] 0..360</param>
    public void SetHdg( double hdg )
    {
      // some sanity checks
      if ( hdg < 0 ) return; // ERROR in hdg 0..360
      if ( hdg > 360 ) return; // ERROR in hdg 0..360

      lock ( m_lock ) {
        // set a turn towards the smaller angle
        double turn = hdg.SubDegrees( m_trk ); // -> positive angle
        // take the shorter one (<=180.0)
        if ( turn <= 180.0 )
          m_turn = turn;
        else
          m_turn = turn - 360.0;

        m_setTrk = hdg;
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
          ; // TODO
          Logger.Instance.Log( $"VFRModel-{m_acftID} SegA" );
          NextSeg( );
          break;
        case Cmd.D:
          m_leg = ( m_currentSeg as CmdD ).Dist;
          Logger.Instance.Log( $"VFRModel-{m_acftID} SegD" );
          break;
        case Cmd.T:
          SetTurn( ( m_currentSeg as CmdT ).TurnAngle );
          Logger.Instance.Log( $"VFRModel-{m_acftID} SegT" );
          break;
        case Cmd.H:
          SetHdg( ( m_currentSeg as CmdH ).Heading );
          Logger.Instance.Log( $"VFRModel-{m_acftID} SegH" );
          break;

        case Cmd.S:
          SetSpd( ( m_currentSeg as CmdS ).TAS );
          Logger.Instance.Log( $"VFRModel-{m_acftID} SegS" );
          NextSeg( ); // immediate command - just trigger the next one
          break;
        case Cmd.V:
          SetAlt( ( m_currentSeg as CmdV ).VSI, ( m_currentSeg as CmdV ).AltAGL );
          Logger.Instance.Log( $"VFRModel-{m_acftID} SegV" );
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
        var nTas = m_tas + m_accel * dt;
        var nAlt = m_altAgl + m_vs / 60.0 * dt;
        // check for end of change in speed and alt
        if ( m_accel > 0 && nTas > m_setTas ) { nTas = m_setTas; m_accel = 0.0; } // accel finished, maintain setTas
        if ( m_accel < 0 && nTas < m_setTas ) { nTas = m_setTas; m_accel = 0.0; } // decel finished, maintain setTas
        if ( m_vs > 0 && nAlt > m_setAltAgl ) { nAlt = m_setAltAgl; m_vs = 0; } // ascent finished, maintain setAlt
        if ( m_vs < 0 && nAlt < m_setAltAgl ) { nAlt = m_setAltAgl; m_vs = 0; } // descent finished, maintain setAlt

        double rot = 0.0; // no turning to start with
        if ( m_turn < 0 )
          rot = -c_tRate * dt;     // need to turn left
        else if ( m_turn > 0 )
          rot = c_tRate * dt; // need to turn right

        m_turn -= rot; // reduce turn for the rot amount
        if ( rot > 0 && m_turn < 0 ) {
          rot += m_turn; m_turn = 0;
        } // correct rot and set turn to finished
        if ( rot < 0 && m_turn > 0 ) {
          rot += m_turn; m_turn = 0;
        } // correct rot and set turn to finished

        double nTrack = m_trk.AddDegrees( rot );
        if ( m_turn == 0 ) nTrack = m_setTrk; // avoid rounding errors, maintain setTrk

        double dist = nTas / 3600.0 * dt;
        if ( m_leg > 0 && dist > m_leg ) dist = m_leg; // correct at end of a D leg


        // assign new values
        m_pos = m_pos.DestinationPoint( dist, m_trk + rot / 2.0, ConvConsts.EarthRadiusNm ); // use linear move 
        m_trk = nTrack;
        m_altAgl = nAlt;
        m_tas = nTas;
        m_leg -= dist;
        if ( m_leg < 0 ) m_leg = 0;
      }//lock

      // progress through segments if needed
      if ( m_currentSeg is CmdD && LegFinished ) NextSeg( );
      else if ( m_currentSeg is CmdH && TurnFinished ) NextSeg( );
      else if ( m_currentSeg is CmdT && TurnFinished ) NextSeg( );
    }

  }
}
