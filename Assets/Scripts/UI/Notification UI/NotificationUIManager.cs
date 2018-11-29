using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationUIManager : MonoBehaviour
{
    private List<NotificationUI> _notificationUIs = new List<NotificationUI>();
    private Dictionary<NotificationUI, Notification> _conditionDictionary = new Dictionary<NotificationUI, Notification>();
    private Dictionary<Notification, Action> _expressionsTrigger = new Dictionary<Notification, Action>();
    private Dictionary<Notification, Action> _expressionsEnd = new Dictionary<Notification, Action>();

    //private NotificationPanel _notificationPanel = null;
    //[SerializeField] private CameraControls _camera = null;

    [Header("References")]
    [SerializeField] private RectTransform _content = null;
    [SerializeField] private SimpleObjectPool _panelPrefabPool = null;
    //[SerializeField] private GameObject _panelPrefab = null;

    [SerializeField] private NotificationsConditionManager _notificationsConditions = null;

    // Use this for initialization
    private void Start()
    {
        //_notificationsConditions.BatteryOverchargeNotification.onTrigger += () =>
        //    InitializeNotification(_notificationsConditions.BatteryOverchargeNotification.prefabUI);

        StartCoroutine(MyCoroutines.Wait(1, SetupNotificationEvents));
    }

    private void OnDestroy()
    {
        foreach (KeyValuePair<Notification, Action> kvp in _expressionsTrigger)
            if (kvp.Key != null && kvp.Value != null && kvp.Key.onTrigger != null)
                kvp.Key.onTrigger -= kvp.Value;

        _expressionsTrigger.Clear();

        foreach (KeyValuePair<Notification, Action> kvp in _expressionsEnd)
            if (kvp.Key != null && kvp.Value != null && kvp.Key.onEnd != null)
                kvp.Key.onEnd -= kvp.Value;

        _expressionsEnd.Clear();
    }

    private void SetupNotificationEvents()
    {
        Debug.Assert(_notificationsConditions != null, "Notification Conditions Manager MUST be assigned.");

        if (_notificationsConditions == null) return;

        //return;
        List<Notification> notifications
            //= new List<Notification>();
            = _notificationsConditions.GetAllConditions();
        foreach (Notification notification in notifications)
        {
            Notification copy = notification;
            var trigger = NotificationOnTriggerAction(copy);
            var end = NotificationOnEnd(copy);
            _expressionsTrigger[copy] = trigger;
            _expressionsEnd[copy] = end;

            notification.onTrigger += trigger;
            notification.onEnd += end;
        }
    }

    private Action NotificationOnTriggerAction(Notification copy)
    {
        return () => NotificationOnTrigger(copy);
    }
    
    private void NotificationOnTrigger(Notification copy)
    {
        if (_conditionDictionary.ContainsValue(copy)) return;

        GameObject notificationGO = InitializeNotification(copy.prefabUI);
        if (notificationGO != null)
        {
            NotificationUI component = notificationGO.GetComponent<NotificationUI>();
            //_notificationUICache = component;
            if (component == null)
            {
                Debug.Log("Y U NULL!?");
                return;
            }

            GetComponent<PlayAfterDead>().PlayClipVoid();
            _conditionDictionary[component] = copy;
            component.Focus = copy.focus;
            Activate(component);

            component.onFinishMoveBack += () =>
            {
                //Debug.Log(component.gameObject);
                Destroy(component.gameObject);
                if (_panelPrefabPool != null) _panelPrefabPool.ReturnObject(component.Panel.gameObject);
                //Destroy(component.Panel.gameObject);
            };
        }
    }

    private Action NotificationOnEnd(Notification copy)
    {
        //Debug.Log("Initial: " + (nui != null));
        return () =>
        {
            var nui = GetKeyClass(_conditionDictionary, copy);
            if (nui != null)
            {
                //Debug.Log("Action: " + nui.ToString());
                Dismiss(nui);

            }
            //else Debug.Log("Action is null");
        };
    }

    private TKey GetKeyClass<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TValue searchFor)
        where TKey : class
        where TValue : class
    {
        foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
            if (kvp.Value == searchFor)
                return kvp.Key;

        return null;
        //if (Key is class)
        //return default(Key);
    }

    private GameObject InitializeNotification(GameObject prefab)
    {
        if (_content == null || _panelPrefabPool == null || prefab == null) return null;

        //Instantiate Notification
        GameObject notification = Instantiate(prefab);
        Transform notificationTransform = notification.transform;

        //Instantiate Panel prefab which will hold the element, provide start 
        //and end points and panel will act as an element in a scrollable list
        GameObject panel = _panelPrefabPool.GetObject();
        panel.transform.SetParent(_content, false);
        NotificationPanelUI holder = panel.GetComponent<NotificationPanelUI>();
        notificationTransform.SetParent(holder.transform, false);
        notificationTransform.localPosition = holder.start.localPosition;

        //Set convenience values
        NotificationUI ui = notification.GetComponent<NotificationUI>();

        if (ui == null) return notification;

        _notificationUIs.Add(ui);
        ui.Panel = holder;
        ui.NotificationManager = this;
        ui.GoToStart();

        return notification;
    }

    public void Activate(NotificationUI notification)
    {
        if (notification.InView)
        {
            notification.Jiggle();
            return;
        }
        notification.InView = true;

        notification.MoveTowards();
    }

    public void Trigger(NotificationUI notification)
    {
        if (/*_camera != null && */notification.Focus != null)
        {
            notification.Focus.Focus();
            notification.Focus.ActivateActionPanel();

            //_camera.MoveFocus(notification.Focus);
            //if (_shouldFocus) _camera.SetZoom(40);
        }

        Dismiss(notification);
    }

    public void Dismiss(NotificationUI notification)
    {
        if (!notification.InView) return;
        notification.InView = false;

        notification.MoveBack();

        _conditionDictionary[notification].TriggerCooldown();
        _conditionDictionary.Remove(notification);

        //Destroy(notification.Panel.gameObject);
    }

    /**/
}
