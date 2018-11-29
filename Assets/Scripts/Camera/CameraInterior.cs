using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

public enum State { Minigame, Default };

[RequireComponent(typeof(ScreenTransformGesture))]
public class CameraInterior : MonoBehaviour
{
#pragma warning disable 0414
    [Header("Transforms")]
    [SerializeField] private Transform _pivot = null;
    [SerializeField] private Transform _camera = null;

    [Header("Gestures")]
    [SerializeField] private ScreenTransformGesture _panGesture = null;

    [Header("Movement")]
    public Vector2 sensitivityXY = new Vector2(0.5f, 0.5f);
    public Vector2 xMinMax = new Vector2(-60f, 60f);
    public Vector2 yMinMax = new Vector2(-30f, 30f);

    public Transform positionToBe;

    private float rotationX = 0F;
    private float rotationY = 0F;

    private float rotAverageX = 0F;
    private float rotAverageY = 0F;

    public float lerpFactor = 4;
    public float resetLerpFactor = 2;

    private Quaternion xQuaternion;
    private Quaternion yQuaternion;

    public float resetTimer = 1f;
    private float currentResetTime;
    private bool isManipulating = false;

    bool updatePos = true;
    public Vector3 inCameraPosTarget { get; set; }
    public State _state;

    public Transform CurrentMount;
    public Transform DefaultMount;

    private float speedFactor = 10f;
    private float zoomFactor = 1.0f;
    private bool _backFromMinigames = true;

    [SerializeField] private GameObject _postProcesing;
    [SerializeField] private GameObject _jumpRopeButton;

#pragma warning restore 0414

    private void OnEnable()
    {
        _panGesture.Transformed += PanTransformHandler;
        _panGesture.TransformStarted += PanStartHandler;
        _panGesture.TransformCompleted += PanEndHandler;

        xQuaternion = Quaternion.identity;
        yQuaternion = Quaternion.identity;
        currentResetTime = resetTimer;
        _state = State.Default;
        if (GameManager.Instance.MainCamera)
        {
            StartCoroutine(MyCoroutines.DoUntil(() => { return GameManager.Instance.MainCamera.fieldOfView == 20; },
                () =>
                {
                    float f = GameManager.Instance.MainCamera.fieldOfView;
                    f = Mathf.Lerp(f, 20, 4 * Time.deltaTime);
                    if (f <= 20.01f) f = 20f;

                    GameManager.Instance.MainCamera.fieldOfView = f;
                }));
        }
            //GameManager.Instance.MainCamera.fieldOfView = 20;
    }

    private void OnDisable()
    {
        _panGesture.Transformed -= PanTransformHandler;
        _panGesture.TransformStarted -= PanStartHandler;
        _panGesture.TransformCompleted -= PanEndHandler;

        if (GameManager.Instance.MainCamera)
            GameManager.Instance.MainCamera.fieldOfView = 53;
    }

    private void LateUpdate()
    {
        switch (_state)
        {
            case (State.Minigame):
                {
                    _pivot.position = CurrentMount.localPosition;
                    _camera.rotation = CurrentMount.localRotation;

                    if (!_jumpRopeButton.activeSelf) _jumpRopeButton.SetActive(true);

                    if ((CurrentMount.localPosition - _pivot.position).sqrMagnitude <= float.Epsilon)
                        _pivot.position = CurrentMount.localPosition;

                    _backFromMinigames = false;
                    if(_postProcesing.activeSelf) _postProcesing.SetActive(false);
                    break;
                }
            case (State.Default):

                if (_jumpRopeButton.activeSelf) _jumpRopeButton.SetActive(false);

                if (!_backFromMinigames) StartCoroutine(delay());
                if (!_postProcesing.activeSelf) _postProcesing.SetActive(true);

                rotAverageY = 0f;
                rotAverageX = 0f;

                rotAverageY = ClampAngle(rotationY, yMinMax.x, yMinMax.y);
                rotAverageX = ClampAngle(rotationX, xMinMax.x, yMinMax.y);

                Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
                Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

                Quaternion newRot = positionToBe.rotation * xQuaternion * yQuaternion;
                float lerpie = lerpFactor;

                if (!isManipulating)
                {

                    currentResetTime -= 1 * Time.deltaTime;
                    if (currentResetTime <= 0)
                    {
                        lerpie = resetLerpFactor;
                        rotationX = Mathf.Lerp(rotationX, 0, lerpFactor);
                        rotationY = Mathf.Lerp(rotationY, 0, lerpFactor);
                        //_camera.localRotation = Quaternion.Slerp(_camera.localRotation, originalRotation, lerpFactor * 0.5f * Time.deltaTime);
                    }
                }

                //if (Input.GetKeyDown(KeyCode.S)) GetComponent<CameraShake>().Shake();

                _camera.rotation = Quaternion.Slerp(_camera.rotation, newRot, lerpie * Time.deltaTime);

                if (!updatePos) return;

                // Update the position of the camera
                _pivot.position = Vector3.Lerp(_pivot.position, positionToBe.position, 10 * Time.deltaTime);

                // Snap when close enough to prevent annoying slerp behavior
                if ((positionToBe.position - _pivot.position).sqrMagnitude <= float.Epsilon)
                    _pivot.position = positionToBe.position;
                break;
        }

    }

    private void PanTransformHandler(object sender, EventArgs e)
    {
        isManipulating = true;
        rotationY -= _panGesture.DeltaPosition.y * sensitivityXY.y;
        rotationX -= _panGesture.DeltaPosition.x * sensitivityXY.x;
    }

    private void PanStartHandler(object sender, EventArgs e)
    {
        //rotationY = 0;
        //rotationX = 0;
    }

    private void PanEndHandler(object sender, EventArgs e)
    {
        isManipulating = false;
        currentResetTime = resetTimer;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }

    private IEnumerator delay()
    {
        yield return new WaitForSeconds(2);
        if (_pivot.position != DefaultMount.localPosition)
            _pivot.position = DefaultMount.localPosition;

        if (_camera.rotation != DefaultMount.localRotation)
            _camera.rotation = DefaultMount.localRotation;

        _backFromMinigames = true;
    }
}