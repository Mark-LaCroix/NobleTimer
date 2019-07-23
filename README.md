# NobleTimerCS
A Timer class for C#, designed with a Unity workflow in mind. Meant to emulate the native Timer class found in ActionScript 3.0. 

Use-cases for this would include places where you might otherwise use Coroutines or Update loops.

This implimentation includes a `Timer.TimerEvent` delegate you can use as a "local function" in order to keep all your code together, although you may feed the Timer normal methods as well.

## API

### Create new Timer

```C#
Timer timer = new Timer(
	float _tickDurationInMilliseconds,
	TimerEvent _onCompleteMethod,
	int _totalTickCount,
	TimerEvent _onTickMethod,
	bool startUponCreation
);
```

### Public Methods (sorry C# purists, I don't use TitleCase for methods)

`start()`: Begin your timer.

`pause()`: Pauses timer, resume from where you left off with `start()`.

`stop()`: Stops and resets timer. Can be run again with `start()`.

`destroy()`: stops, destroys, and sets to `null` all internal objects. Only call this if you also intend to set the timer object to `null`.

### Public Properties

`float tickDuration` (in milliseconds)

`double durationRemaining` (in milliseconds)

`int totalTickCount`

`int currentTickCount`

`bool isRunning`

### Usage

Simple usage (Runs "complete" code after 500ms):
```C#
Timer.TimerEvent timerCompleteHandler = delegate { /* Your code here */ };
timer = new Timer(500, timerCompleteHandler);
timer.start();
```
You can also use a lambda function.
```C#
timer = new Timer(500, ()=>{ /* Your code here */ });
timer.start();
```
The `startUponCreation` bool allows you to start the timer immediately, without having to seperately call `start()` on it:
```C#
timer = new Timer(500, ()=>{ /* Your code here */ }, 1, null, true);
````

Advanced usage (Runs "tick" code every 500ms, and "complete" code at the end of 10 ticks):
```C#
Timer.TimerEvent timerTickHandler = delegate { /* Your code here */ };
Timer.TimerEvent timerCompleteHandler = delegate { /* Your code here */ };
timer = new Timer(500, timerCompleteHandler, 10, timerTickHandler);
timer.start();
```

Only the "onComplete" code is run when the timer completes, if you want to run the "onTick" code when the timer completes, use the same method for both arguments:
```C#
Timer.TimerEvent timerHandler = delegate { /* Your code here */ };
timer = new Timer(500, timerHandler, 10, timerHandler);
timer.start();
```
