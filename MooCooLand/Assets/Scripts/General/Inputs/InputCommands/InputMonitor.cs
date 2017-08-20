using System;
using System.Collections.Generic;
using UnityEngine;

namespace MooCooEngine.Input
{
    [System.Flags]
    public enum InputProviderType
    {
        None = 0,

        Athelas = (1 << 0),
        Retrofit = (1 << 1),
        SimulatedETWithMouse = (1 << 2),
        Keyboard = (1 << 3),
        Clicker = (1 << 4),
        AirTap = (1 << 5),

        KeyboardAll = Keyboard,
    }

    public interface IInputProvider
    {
        InputProviderType InputType { get; }
    }

    public interface IInputCommandListener
    {
        void InputArrived(object sender, InputCommandArgs args);
    }

    /// <summary>
    /// Monitor all kinds of all kinds of inputs, listeners  should listen to this monitor only.
    /// </summary>
    public class InputMonitor : MonoBehaviour
    {
        class KeyboardListenerWrapper
        {
            public InputProviderType Type { get; set; }
            public IInputCommandListener Listener { get; set; }
            public bool IsActivated { get; set; }
        }

        class EyeTrackingListenerWrapper
        {
            public InputProviderType Type { get; set; }
            public bool IsActivated { get; set; }
        }
        
        private Dictionary<InputProviderType, IInputCommand> keyboardProviders = new Dictionary<InputProviderType, IInputCommand>();
        private List<KeyboardListenerWrapper> keyboardListeners = new List<KeyboardListenerWrapper> ();
        private List<EyeTrackingListenerWrapper> eyeTrackingListeners = new List<EyeTrackingListenerWrapper>();

        private static InputMonitor instance;
        private bool isAthelas = false;

        public static InputMonitor Instance
        {
            get { return instance; }
        }

        protected void Awake()
        {
            Debug.LogWarning("InputMonitor has awaken");
            if(instance == null)
                instance = this;
            isAthelas = (IntPtr.Size == 8);

            Debug.LogWarning("InputMonitor >> "+InputMonitor.Instance);
        }

        protected void Start()
        {
            this.keyboardProviders.Clear();

            // Find all available Keyboard input providers
            if (CommandInputMonitor.Instance != null)
            {
                var kbProvider = GameObject.FindObjectOfType<CommandInputMonitor>() as IInputCommand;
                if (kbProvider != null)
                {
                    kbProvider.InputCommandArrived += this.OnKeyboardEventArrived;
                    this.keyboardProviders.Add(InputProviderType.Keyboard, kbProvider);
                }
            }
        }

        protected void OnDestroy()
        {
            foreach (var provider in this.keyboardProviders)
            {
                provider.Value.InputCommandArrived -= this.OnKeyboardEventArrived;
            }

            keyboardProviders.Clear();
        }
        
        public void Register(IInputCommandListener listener, InputProviderType inputTypes)
        {
            if (((inputTypes & InputProviderType.KeyboardAll) != InputProviderType.None))
            {
                //# Check if a ref already exists
                KeyboardListenerWrapper klw = GetListenerRef(listener);

                if (listener.ToString() != "null")
                {
                    if (klw == null) // Do not add duplicates
                    {
                        this.keyboardListeners.Add(new KeyboardListenerWrapper
                        {
                            Type = inputTypes,
                            Listener = listener,
                            IsActivated = true
                        });
                    }
                    else
                    {
                        this.keyboardListeners.Remove(klw);
                        this.keyboardListeners.Add(new KeyboardListenerWrapper
                        {
                            Type = inputTypes,
                            Listener = listener,
                            IsActivated = true
                        });
                    }
                }
                else
                {
                    Debug.LogError("Tried to add NULL LISTENER!");
                }
            }
        }

  
        public bool IsActive(IInputCommandListener listener)
        {
            foreach (var lw in this.keyboardListeners)
            {
                if (lw.Listener == listener)
                {
                    return lw.IsActivated;
                }
            }
            return false;
        }

        private KeyboardListenerWrapper GetListenerRef(IInputCommandListener listener)
        {
            foreach (var lw in this.keyboardListeners)
            {
                if (lw.Listener == listener)
                {
                    return lw;
                }
            }
            return null;
        }

        public void ListOfListeners()
        {
            //foreach (var lw in this.keyboardListeners)
            //{
            //    Debug.LogWarning("\t>>> " + lw.Listener + " IS ACTIVATED: \t\t" + lw.IsActivated + " \t "+lw.Type);
            //}
        }

        public void DeregisterAll()
        {
            Debug.LogWarning("Deregister ALL ");

            foreach (var lw in this.keyboardListeners)
            {
                // lw.IsActivated = false;
            }
            InputMonitor.Instance.keyboardListeners.RemoveRange(0, this.keyboardListeners.Count);
        }

        public void Deregister(IInputCommandListener listener)
        {
            //Debug.LogWarning("Deregister: \t " + listener);
            foreach (var lw in this.keyboardListeners)
            {
                if (lw.Listener == listener)
                {
                    lw.IsActivated = false;
                }
            }
        }
        
        protected virtual void OnKeyboardEventArrived(object sender, InputCommandArgs args)
        {
            var provider = sender as IInputProvider;

            if (provider != null)
            {
                foreach (var lw in keyboardListeners)
                { 
                    if (lw.IsActivated && ((provider.InputType & lw.Type) != InputProviderType.None))
                    {                        
                        lw.Listener.InputArrived(sender, args);
                    }
                }
            }
        }

        /// <summary>
        /// Register for manual input events
        /// </summary>
        public static void RegisterManualInputListener(IInputCommandListener listener)
        {
            try
            {
                InputMonitor.Instance.Register(listener, InputProviderType.KeyboardAll);
            }
            catch (System.NullReferenceException)
            {
                Debug.LogError("NullReferenceException when trying to register input listener.");
            }
        }

        //protected virtual void OnKeyboardEventArrived(object sender, InputDeltaArgs args)
        //{
        //    var provider = sender as IInputProvider;

        //    if (provider != null)
        //    {
        //        foreach (var lw in keyboardListeners)
        //        {
        //            if (lw.IsActivated && ((provider.InputType & lw.Type) != InputProviderType.None))
        //            {
        //                lw.Listener.InputArrived(sender, args);
        //            }
        //        }
        //    }
        //}
    }
}
