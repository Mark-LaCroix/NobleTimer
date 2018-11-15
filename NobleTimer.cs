using System;
using System.Timers;
using System.Diagnostics;
using System.Threading;

public class Timer {

	private System.Timers.Timer systemTimer;
	private Stopwatch stopwatch;
	
	public int totalTickCount;
	public int currentTickCount;
	
	public float tickDuration;
	public double durationRemaining;
	
	public bool isRunning;

	public delegate void TimerEvent();
	public event TimerEvent onTimerTick;
	public event TimerEvent onTimerComplete;

	private SynchronizationContext syncContext;

	public Timer(float _tickDurationInMilliseconds, TimerEvent _onCompleteMethod = null, int _totalTickCount = 1, TimerEvent _onTickMethod = null, bool startUponCreation = false){
		
		systemTimer = new System.Timers.Timer();
		stopwatch = new Stopwatch();

		currentTickCount = 0;
		totalTickCount = _totalTickCount;
		systemTimer.Interval = durationRemaining = tickDuration = _tickDurationInMilliseconds;

		systemTimer.Elapsed += onTick;

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
		currentTickCount = 0;
		systemTimer.Interval = durationRemaining = tickDuration;
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
