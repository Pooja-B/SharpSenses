﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpSenses.Gestures {
    public class GestureSensor : IGestureSensor {

        public event Action SwipeLeft;
        public event Action SwipeRight;
        public event Action SwipeUp;
        public event Action SwipeDown;
        public event Action MoveForward;

        public GestureSensor(ICamera camera) {
            CreateSwipeGesture(camera.LeftHand, Direction.Left).GestureDetected += OnGestureSwipeLeft;
            CreateSwipeGesture(camera.RightHand, Direction.Left).GestureDetected += OnGestureSwipeLeft;

            CreateSwipeGesture(camera.LeftHand, Direction.Right).GestureDetected += OnGestureSwipeRight;
            CreateSwipeGesture(camera.RightHand, Direction.Right).GestureDetected += OnGestureSwipeRight;

            CreateSwipeGesture(camera.LeftHand, Direction.Up).GestureDetected += OnGestureSwipeUp;
            CreateSwipeGesture(camera.RightHand, Direction.Up).GestureDetected += OnGestureSwipeUp;

            CreateSwipeGesture(camera.LeftHand, Direction.Down).GestureDetected += OnGestureSwipeDown;
            CreateSwipeGesture(camera.RightHand, Direction.Down).GestureDetected += OnGestureSwipeDown;

            CreateSwipeGesture(camera.LeftHand, Direction.Forward).GestureDetected += OnMoveForward;
            CreateSwipeGesture(camera.RightHand, Direction.Forward).GestureDetected += OnMoveForward;
            
        }

        private Gesture CreateSwipeGesture(Hand hand, Direction direction) {
            var swipe = new Gesture("hand"+hand.Side + "_" + direction);
            swipe.AddStep(800, Movement.CreateMovement(direction, hand, 18));
            swipe.Activate();
            return swipe;
        }

        public void OnGestureSwipeRight() {
            Action handler = SwipeRight;
            if (handler != null) handler();
        }

        public void OnGestureSwipeLeft() {
            Action handler = SwipeLeft;
            if (handler != null) handler();
        }

        public virtual void OnGestureSwipeUp() {
            Action handler = SwipeUp;
            if (handler != null) handler();
        }
        public virtual void OnGestureSwipeDown() {
            Action handler = SwipeDown;
            if (handler != null) handler();
        }

        protected virtual void OnMoveForward() {
            Action handler = MoveForward;
            if (handler != null) handler();
        }

    }
}