using System;
using UnityEngine;

/// <summary>
/// A timer that counts during update ticks and sends an elasped event when the specified time is reached. Also it has a fantastic name.
/// </summary>
public class TaylorTimer
{

    /// <summary>
    /// If true the timer will continue to loop after an elapsed event, if false it will elapse then deactive the timer.
    /// </summary>
    public bool Loop
    {
        get { return loop; }
        set { loop = value; }
    }

    /// <summary>
    /// Gets or sets whether the timer is currently active and working toward an event call.
    /// </summary>
    public bool Active
    {
        get { return active; }
        set { active = value; }
    }

    /// <summary>
    /// Gets or sets the time between event calls, this is in milliseconds.
    /// </summary>
    public double TargetTime
    {
        get { return targetTime; }
        set { targetTime = value / 1000; }
    }

    private bool active = true;
    private bool loop = true;

    private double targetTime;
    private double currentTime;

    /// <summary>
    /// Creates a new instance of a TaylorTimer.
    /// </summary>
    /// <param name="timeTillElapsed">Time in milliseconds between event calls.</param>
    public TaylorTimer(double timeTillElapsed)
    {
        targetTime = timeTillElapsed / 1000;
    }

    /// <summary>
    /// Updates
    /// </summary>
	public void Update ()
    {
        if (active)
        {
            currentTime += Time.deltaTime;
            if (currentTime > targetTime)
            {
                currentTime = 0;
                if (!Loop)
                    active = false;

                if (Elapsed != null)
                    Elapsed(this, EventArgs.Empty);
            }
        }
	}

    /// <summary>
    /// An event called when the timer's tracked time exceeds the target time.
    /// </summary>
    public event EventHandler Elapsed;
}
