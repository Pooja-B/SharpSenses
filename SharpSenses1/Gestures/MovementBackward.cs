﻿using System;

namespace SharpSenses.Gestures {
    public class MovementBackward : MovementForward {
        public MovementBackward(Item item, double distance) : base(item, distance) { }

        protected override bool IsGoingRightDirection(Point3D currentLocation) {
            return currentLocation.Z >= LastPosition.Z &&
                   Math.Abs(StartPosition.Y - currentLocation.Y) < ToleranceForWrongDirection &&
                   Math.Abs(StartPosition.X - currentLocation.X) < ToleranceForWrongDirection;

        }
    }
}