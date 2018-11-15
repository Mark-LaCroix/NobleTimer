using System;
using System.Timers;
using System.Diagnostics;
using System.Threading;

public class Timer {

	private System.Timers.Timer systemTimer;
	private Stopwatch stopwatch;
	
	public int currentTickCount;
	public float tickDuration;
	public int totalTickCount;
	public bool isRunning;

	public delegate void TimerEvent();
	public event TimerEvent onTimerTick;
	public event TimerEvent onTimerComplete;

	private double durationRemaining;

	private SynchronizationContext syncContext;

	public Timer(float _tickDurationInMilliseconds, TimerEvent _onCompleteMethod = null, int _totalTickCount = 1, TimerEvent _onTickMethod = null, bool startUponCreation = false){
		
		systemTimer = new System.Timers.Timer();		
		systemTimer.Interval = _tickDurationInMilliseconds;
		systemTimer.Elapsed += onTick;

		stopwatch = new Stopwatch();

		currentTickCount = 0;
		totalTickCount = _totalTickCount;
		tickDuration = _tickDurationInMilliseconds;
		durationRemaining = _tickDurationInMilliseconds;

		// Move operations to the main thread (required for certain Unity APIs).
		//
		syncContext = SynchronizationContext.Current;
		syncContext.Send(state =>{
			if (_onCompleteMethod != null){ onTimerComplete += _onCompleteMethod; }
			if (_onTickMethod != null){ onTimerTick += _onTickMethod; }
		}, null);

		// Saves the user a timer.start() call, if they choose.
		//
		if (startUponCreation){
			start();
		}

	}

	private void onTick(object source, ElapsedEventArgs elapsedEventArguments){
		currentTickCount++;
		if (currentTickCount < totalTickCount){
			// Calls user-set method (Internal timer resets automatically).
			syncContext.Send(state =>{ onTimerTick.Invoke(); }, null);
		} else {
			// Calls user-set method, also stops internal timer (which is normally set to repeat).
			syncContext.Send(state =>{ onTimerComplete.Invoke(); }, null);
			stop();
		}
	}

	public void start(){
		systemTimer.Start();
		stopwatch.Start();
		isRunning = true;
	}

	public void pause(){
		// Stop things.
		systemTimer.Stop();
		stopwatch.Stop();
		isRunning = false;
		// Set new interval based on remaining time.
		systemTimer.Interval = durationRemaining = tickDuration - stopwatch.Elapsed.TotalMilliseconds;
	}

	public void stop(){
		systemTimer.Stop();
		stopwatch.Stop();
		stopwatch.Reset();
		isRunning = false;
	}

	public void destroy(){
		systemTimer.Close();
		systemTimer.Dispose();
		systemTimer = null;
		stopwatch = null;
		syncContext = null;
		onTimerComplete = null;
		onTimerTick = null;
	}

}
