// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace MooCooEngine.Input
{
    /// <summary>
    /// HandsDetected determines if the hand is currently detected or not.
    /// </summary>
    public partial class HandsManager : Singleton<HandsManager>
    {
        /// <summary>
        /// HandDetected tracks the hand detected state.
        /// Returns true if the list of tracked hands is not empty.
        /// </summary>
        public bool HandDetected
        {
            get { return trackedHands.Count > 0; }
        }

        private List<uint> trackedHands = new List<uint>();
        private GestureRecognizer gestureRecognizer;

        void Awake()
        {
            InteractionManager.SourceDetected += InteractionManager_SourceDetected;
            InteractionManager.SourceLost += InteractionManager_SourceLost;
            InteractionManager.SourceUpdated += InteractionManager_SourceUpdated;
            InteractionManager.SourcePressed += InteractionManager_SourcePressed;
            InteractionManager.SourceReleased += InteractionManager_SourceReleased;

            ////# Create a new GestureRecognizer. Sign up for tapped events.
            //gestureRecognizer = new GestureRecognizer();
            //gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
            //gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;

            ////# Start looking for gestures.
            //gestureRecognizer.StartCapturingGestures();
        }

        private bool IsPressed = false;
        private void InteractionManager_SourceReleased(InteractionSourceState state)
        {
            if (IsPressed)
            {
                IsPressed = false;
                Debug.Log(">> SOURCE RELEASED");
            }
        }

        private void InteractionManager_SourcePressed(InteractionSourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.kind != InteractionSourceKind.Hand)
            {
                return;
            }

            if (!IsPressed)
            {
                IsPressed = true;
                Debug.Log(">> SOURCE PRESSED");
                CommandInputMonitor.PostInputCommand(InputCommand.SelectionKeyPressed);
            }            
        }

        private void InteractionManager_SourceUpdated(InteractionSourceState state)
        {
            //Debug.Log(">> SOURCE Updated: "+state.properties.location);
        }

        private void InteractionManager_SourceDetected(InteractionSourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.kind != InteractionSourceKind.Hand)
            {
                return;
            }

            trackedHands.Add(state.source.id);
        }

        private void InteractionManager_SourceLost(InteractionSourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.kind != InteractionSourceKind.Hand)
            {
                return;
            }

            if (trackedHands.Contains(state.source.id))
            {
                trackedHands.Remove(state.source.id);
            }
        }

        void OnDestroy()
        {
            InteractionManager.SourceDetected -= InteractionManager_SourceDetected;
            InteractionManager.SourceLost -= InteractionManager_SourceLost;
        }
    }
}