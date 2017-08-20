using System;
using System.Collections.Generic;
using UnityEngine;

namespace MooCooEngine.Input
{
    public class CommandInputMonitor : MonoBehaviour, IInputCommand
    {
        private static CommandInputMonitor instance;
        public static CommandInputMonitor Instance
        {
            get { return instance; }
        }

        public static void PostInputCommand(InputCommand command)
        {
            if (CommandInputMonitor.Instance != null)
                CommandInputMonitor.Instance.PostInputCommandEvent(command);
        }

        public static readonly Dictionary<SupportedKey, InputCommand> SupportedCommands = new Dictionary<SupportedKey, InputCommand> {

            // Target selection keys (Space and the Keypad's Enter key)
            //{ new SupportedKey(KeyCode.Space, KeyCode.None, KeyEventRespondMode.KeyDown), InputCommand.SelectionKeyPressed },
            { new SupportedKey(KeyCode.Space, KeyCode.None, KeyEventRespondMode.KeyUp), InputCommand.SelectionKeyReleased },

            { new SupportedKey(KeyCode.Return, KeyCode.None, KeyEventRespondMode.KeyDown), InputCommand.SelectionKeyPressed },
            { new SupportedKey(KeyCode.Return, KeyCode.None, KeyEventRespondMode.KeyUp), InputCommand.SelectionKeyReleased },

            { new SupportedKey(KeyCode.R, KeyCode.None, KeyEventRespondMode.KeyDown), InputCommand.ReadyStateStart },
            { new SupportedKey(KeyCode.R, KeyCode.None, KeyEventRespondMode.KeyUp), InputCommand.ReadyStateStop },

            { new SupportedKey(KeyCode.R, KeyCode.None, KeyEventRespondMode.KeyDown), InputCommand.RestartLevel },
            // UI related keys
            { new SupportedKey(KeyCode.U, KeyCode.None, KeyEventRespondMode.KeyDown), InputCommand.ShowCursor },
            { new SupportedKey(KeyCode.H, KeyCode.None, KeyEventRespondMode.KeyDown), InputCommand.HideCursor },
            { new SupportedKey(KeyCode.Alpha1, KeyCode.None, KeyEventRespondMode.KeyDown), InputCommand.Cursor_Mouse },
            { new SupportedKey(KeyCode.Alpha2, KeyCode.None, KeyEventRespondMode.KeyDown), InputCommand.Cursor_CenteredCam },
            { new SupportedKey(KeyCode.Alpha3, KeyCode.None, KeyEventRespondMode.KeyDown), InputCommand.Cursor_Manual },
        };

        public event EventHandler<InputCommandArgs> InputCommandArrived;
        public event EventHandler<InputDeltaArgs> InputDeltaArrived;

        public void detectPressedKeyDown()
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (UnityEngine.Input.GetKeyDown(kcode))
                {
                    //pressedKeys.Add(kcode);
                    Debug.Log(">> Key DOWN: " + kcode);
                }

                if (UnityEngine.Input.GetKeyUp(kcode))
                {
                    //pressedKeys.Add(kcode);
                    Debug.Log(">> Key UP: " + kcode);
                }
            }
        }

        void Start()
        {
            if (instance == null)
                instance = this;
        }

        void Update()
        {
            detectPressedKeyDown();

            foreach (var ent in SupportedCommands)
            {
                if(ent.Key.Modifier != KeyCode.None && !UnityEngine.Input.GetKey(ent.Key.Modifier))
                {
                    continue;
                }

                if (ent.Key.RespondMode == KeyEventRespondMode.KeyUp && UnityEngine.Input.GetKeyUp(ent.Key.Code))
                {
                    PostKeyboardInputEvent(ent);
                    // One event at a time.
                    break;
                }
                else if(ent.Key.RespondMode == KeyEventRespondMode.KeyDown && UnityEngine.Input.GetKeyDown(ent.Key.Code))
                {
                    PostKeyboardInputEvent(ent);
                    // One event at a time.
                    break;
                }
            }
        }

        protected virtual void PostKeyboardInputEvent(KeyValuePair<SupportedKey, InputCommand> ent)
        {
            var handler = this.InputCommandArrived;
            if (handler != null)
            {
                handler(this, new InputCommandArgs
                {
                    Timestamp = System.Diagnostics.Stopwatch.GetTimestamp(),
                    Action = ent.Value
                });
            }
        }

        public virtual void PostInputCommandEvent(InputCommand command)
        {
            var handler = this.InputCommandArrived;
            if (handler != null)
            {
                handler(this, new InputCommandArgs
                {
                    Timestamp = System.Diagnostics.Stopwatch.GetTimestamp(),
                    Action = command
                });
            }
        }

        public virtual void PostInputDeltaEvent(InputCommand command, Vector3 delta)
        {
            var handler = this.InputDeltaArrived;
            if (handler != null)
            {
                handler(this, new InputDeltaArgs
                {
                    Timestamp = System.Diagnostics.Stopwatch.GetTimestamp(),
                    Action = command,
                    Delta = delta
                });
            }
        }

        public InputProviderType InputType
        {
            get { return InputProviderType.Keyboard; }
        }
    }

    public enum KeyEventRespondMode
    {
        Never = 0,
        KeyDown,
        KeyUp,
    }

    public class SupportedKey
    {
        public KeyCode Code { get; set; }
        public KeyCode Modifier { get; set; }
        public KeyEventRespondMode RespondMode { get; set; }

        public SupportedKey(KeyCode code, KeyCode modifier = KeyCode.None, KeyEventRespondMode mode = KeyEventRespondMode.KeyDown)
        {
            this.Code = code;
            this.Modifier = modifier;
            this.RespondMode = mode;
        }

        public override int GetHashCode()
        {
            return this.Code.GetHashCode() ^
                this.Modifier.GetHashCode() ^
                this.RespondMode.GetHashCode();
        }
    }
}
