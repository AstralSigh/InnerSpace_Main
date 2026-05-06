using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNG {
    public class SteeringWheel_Custom : GrabbableEvents
    {

        [Header("Rotation Limits")]
        //[Tooltip("Maximum Z value in Local Euler Angles. Can be < -360. Ex : -450")]
        //public float MinAngle = -360f;

        //[Tooltip("Maximum Z value in Local Euler Angles. Can be > 360. Ex : 450")]
        //public float MaxAngle = 360f;

        [Header("Rotation Object")]
        [Tooltip("The Transform to rotate on its Z axis.")]
        public Transform RotatorObject;

        [Header("Rotation Speed")]
        [Tooltip("How fast to move the wheel towards the target angle. 0 = Instant.")]
        public float RotationSpeed = 0f;

        [Header("Two-Handed Option")]
        [Tooltip("IF true both hands will effect the rotation of the steering wheel while grabbed with both hands. Set to false if you only want one hand to control the rotation.")]
        public bool AllowTwoHanded = false;

        [Header("Return to Center")]
        public bool ReturnToCenter = false;
        public float ReturnToCenterSpeed = 45;

        [Header("Debug Options")]
        public Text DebugText;

        [Header("Events")]
        [Tooltip("Called if the SteeringWheel changes angle. Returns the current angle in degrees, clamped between MinAngle / MaxAngle")]
        public FloatEvent onAngleChange;

        [Tooltip("Called every frame. Returns the current current rotation between -1, 1")]
        public FloatEvent onValueChange;

        [Header("Editor Option")]
        [Tooltip("If true will show an angle helper in editor mode (Gizmos must be enabled)")]
        public bool ShowEditorGizmos = true;

        /// <summary>
        /// Returns the angle of the rotation, taking RotationSpeed into account
        /// </summary>
        public float Angle
        {
            get
            {
                //return Mathf.Clamp(smoothedAngle, MinAngle, MaxAngle);
                return smoothedAngle;
            }
        }

        /// <summary>
        /// Always returns the target angle, not taking RotationSpeed into account
        /// </summary>
        public float RawAngle
        {
            get
            {
                return targetAngle;
            }
        }

        public float AngleInverted
        {
            get
            {
                return Angle * -1;
            }
        }

        public Grabber PrimaryGrabber
        {
            get
            {
                return GetPrimaryGrabber();
            }
        }

        protected Vector3 rotatePosition;
        protected Vector3 previousPrimaryPosition;
        protected Vector3 previousSecondaryPosition;

        protected float targetAngle;
        protected float previousTargetAngle;

        /// <summary>
        /// This angle is smoothed towards target angle in Update using RotationSpeed
        /// </summary>
        protected float smoothedAngle;

        void Update()
        {

            // Calculate rotation if being held or returning to center
            if (grab.BeingHeld)
            {
                UpdateAngleCalculations();
            }
            else
            {
                //Play
            }

            // Apply the new angle
            ApplyAngleToSteeringWheel(Angle);

            // Call any events
            CallEvents();

            //UpdatePreviewText();

            // Update the angle so we can compare it next frame
            UpdatePreviousAngle(targetAngle);
        }

        public virtual void UpdateAngleCalculations()
        {

            float angleAdjustment = 0f;

            // Add first Grabber
            if (PrimaryGrabber)
            {
                rotatePosition = transform.InverseTransformPoint(PrimaryGrabber.transform.position);
                rotatePosition = new Vector3(rotatePosition.x, rotatePosition.y, 0);

                // Add in the angles to turn
                angleAdjustment += GetRelativeAngle(rotatePosition, previousPrimaryPosition);

                previousPrimaryPosition = rotatePosition;
            }

            // Apply the angle adjustment
            targetAngle -= angleAdjustment;

            // Update Smooth Angle
            // Instant Rotation
            if (RotationSpeed == 0)
            {
                smoothedAngle = targetAngle;
            }
            // Apply smoothing based on RotationSpeed
            else
            {
                smoothedAngle = Mathf.Lerp(smoothedAngle, targetAngle, Time.deltaTime * RotationSpeed);
            }
        }

        public float GetRelativeAngle(Vector3 position1, Vector3 position2)
        {

            // Are we turning left or right?
            if (Vector3.Cross(position1, position2).z < 0)
            {
                return -Vector3.Angle(position1, position2);
            }

            return Vector3.Angle(position1, position2);
        }

        public virtual void ApplyAngleToSteeringWheel(float angle)
        {
            RotatorObject.localEulerAngles = new Vector3(0, 0, angle);
        }

        /*
        public virtual void UpdatePreviewText()
        {
            if (DebugText)
            {
                // Invert the values for display. Inverted values are easier to read (i.e 5 = clockwise rotation of 5 degrees). 
                DebugText.text = String.Format("{0}\n{1}", (int)AngleInverted, (ScaleValueInverted).ToString("F2"));
            }
        }
        */

        public virtual void CallEvents()
        {
            // Call events
            if (targetAngle != previousTargetAngle)
            {
                onAngleChange.Invoke(targetAngle);
            }
        }

        public override void OnGrab(Grabber grabber)
        {
                previousPrimaryPosition = transform.InverseTransformPoint(PrimaryGrabber.transform.position);

                // Discard the Z value
                previousPrimaryPosition = new Vector3(previousPrimaryPosition.x, previousPrimaryPosition.y, 0);   
        }

        public Grabber GetPrimaryGrabber()
        {
            if (grab.HeldByGrabbers != null)
            {
                for (int x = 0; x < grab.HeldByGrabbers.Count; x++)
                {
                    Grabber g = grab.HeldByGrabbers[x];
                    if (g.HandSide == ControllerHand.Right)
                    {
                        return g;
                    }
                }
            }
            return null;
        }

        public virtual void UpdatePreviousAngle(float angle)
        {
            previousTargetAngle = angle;
        }

        /// <summary>
        /// Returns a value between -1 and 1
        /// </summary>
        /// <param name="value">Current value to compute against</param>
        /// <param name="min">Minimum value of range used for conversion. </param>
        /// <param name="max">Maximum value of range used for conversion. Must be greater then min</param>
        /// <returns>Value between -1 and 1</returns>
        public virtual float GetScaledValue(float value, float min, float max)
        {
            float range = (max - min) / 2f;
            float returnValue = ((value - min) / range) - 1;

            return returnValue;
        }
    }
}

