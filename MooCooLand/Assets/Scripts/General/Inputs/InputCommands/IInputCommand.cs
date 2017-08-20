using System;
using UnityEngine;

namespace MooCooEngine.Input
{
    public enum InputCommand
    {
        None = 0,
        SelectionKeyPressed,
        SelectionKeyReleased,
        SelectionKeyCancel,
        RestartLevel,
        ShowCursor,
        HideCursor,
        DisconnectEyeTracker,

        // Interaction modes
        ReadyStateStart,
        ReadyStateStop,

        //# Cursor modes
        Cursor_CenteredCam,
        Cursor_Mouse,
        Cursor_Manual,
    }

    public class InputCommandArgs : EventArgs
    {
        /// <summary>
        /// Time stamp of a key event.
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// Execute user action after some delay.
        /// </summary>
        public TimeSpan Delay { get; set; }
        /// <summary>
        /// Action
        /// </summary>
        public InputCommand Action { get; set; }
        /// <summary>
        /// Meta data, can be anything.
        /// </summary>
        public object Meta { get; set; }
    }

    public class InputDeltaArgs : EventArgs
    {
        /// <summary>
        /// Time stamp of a key event.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Execute user action after some delay.
        /// </summary>
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// Action
        /// </summary>
        public InputCommand Action { get; set; }

        /// <summary>
        /// Meta data, can be anything.
        /// </summary>
        public object Meta { get; set; }

        /// <summary>
        /// Delta value to indicate the amount of change
        /// </summary>
        public Vector3 Delta { get; set; }
    }

    public interface IInputCommand : IInputProvider
    {
        event EventHandler<InputCommandArgs> InputCommandArrived;
    }
}
