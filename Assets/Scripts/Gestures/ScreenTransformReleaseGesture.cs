using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Gestures.TransformGestures.Base;
using TouchScript.Layers;
using TouchScript.Pointers;
using TouchScript.Utils;
using TouchScript.Utils.Attributes;
using TouchScript.Utils.Geom;

namespace TouchScript.Custom
{
    public class ScreenTransformReleaseGesture : TwoPointTransformGestureBase
    {
        #region Constants

        /// <summary>
        /// Message name when gesture is recognized
        /// </summary>
        public const string RELEASE_MESSAGE = "OnRelease";

        #endregion

        #region Events

        /// <summary>
        /// Occurs when gesture is recognized.
        /// </summary>
        public event EventHandler<EventArgs> Released
        {
            add { releasedInvoker += value; }
            remove { releasedInvoker -= value; }
        }

        // Needed to overcome iOS AOT limitations
        private EventHandler<EventArgs> releasedInvoker;

        /// <summary>
        /// Unity event, occurs when gesture is recognized.
        /// </summary>
        public GestureEvent OnRelease = new GestureEvent();

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets a value indicating whether actions coming from children should be ingored.
        /// </summary>
        /// <value> <c>true</c> if actions from children should be ignored; otherwise, <c>false</c>. </value>
        public bool IgnoreChildren
        {
            get { return ignoreChildren; }
            set { ignoreChildren = value; }
        }

        #endregion

        #region Private variables

        [SerializeField]
        [ToggleLeft]
        private bool ignoreChildren = false;

        #endregion

        #region Unity methods

        [ContextMenu("Basic Editor")]
        private void switchToBasicEditor()
        {
            basicEditor = true;
        }

        #endregion

        #region Gesture callbacks

        /// <inheritdoc />
        public override bool ShouldReceivePointer(Pointer pointer)
        {
            if (!IgnoreChildren) return base.ShouldReceivePointer(pointer);
            if (!base.ShouldReceivePointer(pointer)) return false;

            if (pointer.GetPressData().Target != cachedTransform) return false;
            return true;
        }

        /// <inheritdoc />
        public override bool CanPreventGesture(Gesture gesture)
        {
            if (Delegate == null) return false;
            return !Delegate.ShouldRecognizeSimultaneously(this, gesture);
        }

        /// <inheritdoc />
        public override bool CanBePreventedByGesture(Gesture gesture)
        {
            if (Delegate == null) return false;
            return !Delegate.ShouldRecognizeSimultaneously(this, gesture);
        }

        /// <inheritdoc />
        protected override void pointersPressed(IList<Pointer> pointers)
        {
            base.pointersPressed(pointers);

            if (pointersNumState == PointersNumState.PassedMinThreshold)
            {
                if (State == GestureState.Idle) setState(GestureState.Possible);
                return;
            }
            if (pointersNumState == PointersNumState.PassedMaxThreshold ||
                pointersNumState == PointersNumState.PassedMinMaxThreshold)
            {
                switch (State)
                {
                    case GestureState.Began:
                    case GestureState.Changed:
                        setState(GestureState.Ended);
                        break;
                }
            }
        }

        /// <inheritdoc />
        protected override void pointersUpdated(IList<Pointer> pointers)
        {
            base.pointersUpdated(pointers);
        }

        /// <inheritdoc />
        protected override void pointersReleased(IList<Pointer> pointers)
        {
            if (!isTransforming)
            {
                if (pointersNumState == PointersNumState.PassedMinThreshold) setState(GestureState.Recognized);
            }
            else
            {
                base.pointersReleased(pointers);
            }
        }

        /// <inheritdoc />
        protected override void onRecognized()
        {
            if (!isTransforming)
            {
                if (releasedInvoker != null) releasedInvoker.InvokeHandleExceptions(this, EventArgs.Empty);
                if (UseSendMessage && SendMessageTarget != null)
                    SendMessageTarget.SendMessage(RELEASE_MESSAGE, this, SendMessageOptions.DontRequireReceiver);
                if (UseUnityEvents) OnRelease.Invoke(this);
            }
            else
            {
                base.onRecognized();
            }
        }

        #endregion

        #region Protected methods

        /// <inheritdoc />
        protected override float doRotation(Vector2 oldScreenPos1, Vector2 oldScreenPos2, Vector2 newScreenPos1,
                                            Vector2 newScreenPos2, ProjectionParams projectionParams)
        {
            var oldScreenDelta = oldScreenPos2 - oldScreenPos1;
            var newScreenDelta = newScreenPos2 - newScreenPos1;
            return (Mathf.Atan2(newScreenDelta.y, newScreenDelta.x) -
                    Mathf.Atan2(oldScreenDelta.y, oldScreenDelta.x)) * Mathf.Rad2Deg;
        }

        /// <inheritdoc />
        protected override float doScaling(Vector2 oldScreenPos1, Vector2 oldScreenPos2, Vector2 newScreenPos1,
                                           Vector2 newScreenPos2, ProjectionParams projectionParams)
        {
            return (newScreenPos2 - newScreenPos1).magnitude / (oldScreenPos2 - oldScreenPos1).magnitude;
        }

        /// <inheritdoc />
        protected override Vector3 doOnePointTranslation(Vector2 oldScreenPos, Vector2 newScreenPos,
                                                         ProjectionParams projectionParams)
        {
            if (isTransforming)
            {
                return new Vector3(newScreenPos.x - oldScreenPos.x, newScreenPos.y - oldScreenPos.y, 0);
            }

            screenPixelTranslationBuffer += newScreenPos - oldScreenPos;
            if (screenPixelTranslationBuffer.sqrMagnitude > screenTransformPixelThresholdSquared)
            {
                isTransforming = true;
                return screenPixelTranslationBuffer;
            }

            return Vector3.zero;
        }

        /// <inheritdoc />
        protected override Vector3 doTwoPointTranslation(Vector2 oldScreenPos1, Vector2 oldScreenPos2,
                                                         Vector2 newScreenPos1, Vector2 newScreenPos2, float dR, float dS, ProjectionParams projectionParams)
        {
            if (isTransforming)
            {
                var transformedPoint = scaleAndRotate(oldScreenPos1, (oldScreenPos1 + oldScreenPos2) * .5f, dR, dS);
                return new Vector3(newScreenPos1.x - transformedPoint.x, newScreenPos1.y - transformedPoint.y, 0);
            }

            screenPixelTranslationBuffer += newScreenPos1 - oldScreenPos1;

            if (screenPixelTranslationBuffer.sqrMagnitude > screenTransformPixelThresholdSquared)
            {
                isTransforming = true;
                oldScreenPos1 = newScreenPos1 - screenPixelTranslationBuffer;
                var transformedPoint = scaleAndRotate(oldScreenPos1, (oldScreenPos1 + oldScreenPos2) * .5f, dR, dS);
                return new Vector3(newScreenPos1.x - transformedPoint.x, newScreenPos1.y - transformedPoint.y, 0);
            }

            return Vector3.zero;
        }

        #endregion

        #region Private functions

        private Vector2 scaleAndRotate(Vector2 point, Vector2 center, float dR, float dS)
        {
            var delta = point - center;
            if (Math.Abs(dR) > Mathf.Epsilon) delta = TwoD.Rotate(delta, dR);
            if (Math.Abs(dS) > Mathf.Epsilon) delta = delta * dS;
            return center + delta;
        }

        #endregion
    }
}