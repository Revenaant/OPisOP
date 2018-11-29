using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtBubblesManager : MonoBehaviour
{
    [SerializeField]
    private NotificationsConditionManager _conditionManager = null;

    [Header("Thought Bubbles")]
#pragma warning disable 0414
    [SerializeField] private GameObject _hungryBubble = null;
    [SerializeField] private GameObject _tooHotBubble = null;
    [SerializeField] private GameObject _tooColdBubble = null;
    [SerializeField] private GameObject _pollutionBubble = null;
    [SerializeField] private GameObject _cactusBubble = null;
    [SerializeField] private GameObject _batteryBubble = null;
    [SerializeField] private GameObject _solarPanelBubble = null;
#pragma warning restore 0414
    // Use this for initialization
    private void Start()
    {
        if (_conditionManager == null) _conditionManager = FindObjectOfType<NotificationsConditionManager>();
        
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
