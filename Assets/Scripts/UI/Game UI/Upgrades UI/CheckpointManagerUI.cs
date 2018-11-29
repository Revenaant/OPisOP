using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CheckpointManagerUI : MonoBehaviour
{
    [SerializeField] private Text _upgradeAmount = null;
    [SerializeField] private UpgradeLineUI _solarPaneUpgradeLine = null;
    [SerializeField] private UpgradeLineUI _storageUpgradeLine = null;
    [SerializeField] private UpgradeLineUI _cactiUpgradeLine = null;

    [SerializeField] private GameObjectActivator _activator = null;

    public UnityEvent OnSuccessUpgrade;
    public UnityEvent OnFailUpgrade;

    private void OnEnable()
    {

        CheckpointManager.onUpgradesChange += UpdateUpgradeAmount;
        CheckpointManager.onSuccessUpgradeSolarPanel += _solarPaneUpgradeLine.Upgrade;
        CheckpointManager.onSuccessUpgradeStorage += _storageUpgradeLine.Upgrade;
        CheckpointManager.onSuccessUpgradeCacti += _cactiUpgradeLine.Upgrade;

        CheckpointManager.onSuccessUpgradeSolarPanel += OnSuccessInvoke;
        CheckpointManager.onSuccessUpgradeStorage += OnSuccessInvoke;
        CheckpointManager.onSuccessUpgradeCacti += OnSuccessInvoke;

        CheckpointManager.onFailUpgradeSolarPanel += OnFailInvoke;
        CheckpointManager.onFailUpgradeStorage += OnFailInvoke;
        CheckpointManager.onFailUpgradeCacti += OnFailInvoke;

        if (_activator == null) _activator = GetComponent<GameObjectActivator>();

        if (_activator == null) return;

        CheckpointManager.onCheckpointStart += _activator.Deactivate;
        CheckpointManager.onSuccessUpgradeSolarPanel += _activator.Activate;
        CheckpointManager.onSuccessUpgradeStorage += _activator.Activate;
        CheckpointManager.onSuccessUpgradeCacti += _activator.Activate;
    }

    private void OnSuccessInvoke()
    {
        OnSuccessUpgrade.Invoke();
    }

    private void OnFailInvoke()
    {
        OnFailUpgrade.Invoke();
    }

    private void Start()
    {

        StartCoroutine(MyCoroutines.WaitForEndOfFrame(() =>
            {
                //_solarPaneUpgradeLine.UpgradeTo(-GameManager.Instance.SolarPanel.CurrentUpgradeLevel
                //                                + 3 - GameManager.Instance.SolarPanel.MaxUpgradeLevel);
                //_storageUpgradeLine.UpgradeTo(-GameManager.Instance.Storage.CurrentUpgradeLevel
                //                              + 3 - GameManager.Instance.Storage.MaxUpgradeLevel);
                _solarPaneUpgradeLine.UpgradeTo(
                    Mathf.Clamp(+GameManager.Instance.SolarPanel.CurrentUpgradeLevel
                                + 3 - GameManager.Instance.SolarPanel.MaxUpgradeLevel, 0, 3));
                _storageUpgradeLine.UpgradeTo(
                    Mathf.Clamp(+GameManager.Instance.Storage.CurrentUpgradeLevel
                                + 3 - GameManager.Instance.Storage.MaxUpgradeLevel, 0, 3));
                _cactiUpgradeLine.UpgradeTo(
                    Mathf.Clamp(+GameManager.Instance.PlantBed.CurrentUpgradeLevel
                                + 3 - GameManager.Instance.PlantBed.MaxUpgradeLevel, 0, 3));
            }));
    }

    private void OnDisable()
    {
        if (CheckpointManager.onUpgradesChange != null)
            CheckpointManager.onUpgradesChange -= UpdateUpgradeAmount;

        if (CheckpointManager.onCheckpointStart != null && _activator != null)
            CheckpointManager.onCheckpointStart -= _activator.Deactivate;

        if (CheckpointManager.onSuccessUpgradeSolarPanel != null)
        {
            CheckpointManager.onSuccessUpgradeSolarPanel -= _solarPaneUpgradeLine.Upgrade;
            CheckpointManager.onSuccessUpgradeSolarPanel -= OnSuccessInvoke;
            if (_activator != null) CheckpointManager.onSuccessUpgradeSolarPanel -= _activator.Activate;
        }

        if (CheckpointManager.onSuccessUpgradeStorage != null)
        {
            CheckpointManager.onSuccessUpgradeStorage -= _solarPaneUpgradeLine.Upgrade;
            CheckpointManager.onSuccessUpgradeStorage -= OnSuccessInvoke;
            if (_activator != null) CheckpointManager.onSuccessUpgradeStorage -= _activator.Activate;
        }

        if (CheckpointManager.onSuccessUpgradeCacti != null)
        {
            CheckpointManager.onSuccessUpgradeCacti -= _solarPaneUpgradeLine.Upgrade;
            CheckpointManager.onSuccessUpgradeCacti -= OnSuccessInvoke;
            if (_activator != null) CheckpointManager.onSuccessUpgradeCacti -= _activator.Activate;
        }

        if (CheckpointManager.onFailUpgradeSolarPanel != null)
        {
            CheckpointManager.onFailUpgradeSolarPanel -= OnFailInvoke;
        }

        if (CheckpointManager.onFailUpgradeStorage != null)
        {
            CheckpointManager.onFailUpgradeStorage -= OnFailInvoke;
        }

        if (CheckpointManager.onFailUpgradeCacti != null)
        {
            CheckpointManager.onFailUpgradeCacti -= OnFailInvoke;
        }
    }

    private void UpdateUpgradeAmount(int upgrades)
    {
        if (_upgradeAmount != null)
            _upgradeAmount.text = upgrades.ToString() + "\n";
    }

    public void UpgradeSolarPanel()
    {
        if (CheckpointManager.onUpgradeSolarPanel != null)
            CheckpointManager.onUpgradeSolarPanel();
    }

    public void UpgradeStorage()
    {
        if (CheckpointManager.onUpgradeStorage != null)
            CheckpointManager.onUpgradeStorage();
    }

    public void UpgradeCacti()
    {
        if (CheckpointManager.onUpgradeCacti != null)
            CheckpointManager.onUpgradeCacti();
    }
}
