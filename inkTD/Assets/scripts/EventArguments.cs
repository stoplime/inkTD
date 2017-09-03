using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// A delegate used during GridChange events.
/// </summary>
/// <param name="sender">The grid that sent this event.</param>
/// <param name="e">The event arguments.</param>
public delegate void OnGridChangeEventHandler(Grid sender, OnGridChangeEventArgs e);

/// <summary>
/// Event arguments for GridChange events.
/// </summary>
public class OnGridChangeEventArgs : EventArgs
{
    /// <summary>
    /// Gets the x coordinate that was changed within the grid.
    /// </summary>
    public int XChanged { get; private set; }

    /// <summary>
    /// Gets the y coordinate that was changed within the grid.
    /// </summary>
    public int YChanged { get; private set; }

    /// <summary>
    /// Gets the player ID that was assigned to the grid that was changed.
    /// </summary>
    public int PlayerID { get; private set; }

    /// <summary>
    /// Instantiates a new onGridChangeEventArgs object for use in GridChange events.
    /// </summary>
    /// <param name="playerID">The player ID of the grid that changed.</param>
    /// <param name="x">The x coordinate in the grid that changed.</param>
    /// <param name="y">The y coordinate in the grid that changed.</param>
    public OnGridChangeEventArgs(int playerID, int x, int y)
    {
        XChanged = x;
        YChanged = y;
        PlayerID = playerID;
    }
}
