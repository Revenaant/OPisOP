using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
#pragma warning disable 0414
    [SerializeField] private GameObject _resolutionScreen = null;
    [SerializeField] private GameObject _pauseScreen = null;
    [SerializeField] private GameObject _upgradeScreen = null;

    [SerializeField] private GameObject _bottomUI = null;
    [SerializeField] private GameObject _topUI = null;
    [SerializeField] private GameObject _minigameUI = null;
#pragma warning restore 0414
    // Use this for initialization
    private void Start()
    {
        GameManager.Instance.GameUIManager = this;
    }

    private void OnEnable()
    {
        CheckpointManager.onCheckpointStart += UpgradeScreenActivate;
        GameRules.onEnd += ResolutionScreenActivate;

        StartCoroutine(MyCoroutines.WaitXFrames(2, GameManager.Instance.ForceEvents));
    }

    private void OnDisable()
    {
        if (CheckpointManager.onCheckpointStart != null)
            CheckpointManager.onCheckpointStart -= UpgradeScreenActivate;
        if (GameRules.onEnd != null)
            GameRules.onEnd -= ResolutionScreenActivate;

    }

    private void UpgradeScreenActivate()
    {
        _upgradeScreen.SetActive(true);
    }

    private void ResolutionScreenActivate()
    {
        _resolutionScreen.SetActive(true);
    }
}
