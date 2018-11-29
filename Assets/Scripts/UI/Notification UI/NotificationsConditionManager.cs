using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationsConditionManager : MonoBehaviour
{
    //[SerializeField, HideInInspector]
    private List<Notification> _conditions = new List<Notification>();

    #region Private Declaration, Instantion and Serialization
    [SerializeField]
    private BatteryOverchargeNotification _batteryOverchargeNotification
        = null;
    //= new BatteryOverchargeNotification();
    [SerializeField]
    private SolarPanelDirtyNotification _solarPanelDirtyNotification
            = null;
    //= new SolarPanelDirtyNotification();
    [SerializeField]
    private PollutionDamageNotification _pollutionDamageNotification
            = null;
    //= new PollutionDamageNotification();
    [SerializeField]
    private HungerNotification _hungerNotification
            = null;
    //= new HungerNotification();
    [SerializeField]
    private TemperatureTooHotNotification _temperatureTooHotNotification
            = null;
    //= new TemperatureTooHotNotification();
    [SerializeField]
    private TemperatureTooColdNotification _temperatureTooColdNotification
            = null;
    //= new TemperatureTooColdNotification();
    [SerializeField]
    private GasBatteryUnderchargeNotification _gasBatteryUnderchargeNotification
            = null;
    //= new GasBatteryUnderchargeNotification();
    [SerializeField]
    private StormNotification _stormNotification
            = null;
    //= new StormNotification();
    [SerializeField]
    private LightBulbNotification _lightBulbNotification
            = null;
    //= new LightBulbNotification();
    #endregion

    #region Public Properties
    public BatteryOverchargeNotification BatteryOverchargeNotification
    {
        get { return _batteryOverchargeNotification; }
    }

    public SolarPanelDirtyNotification SolarPanelDirtyNotification
    {
        get { return _solarPanelDirtyNotification; }
    }

    public PollutionDamageNotification PollutionDamageNotification
    {
        get { return _pollutionDamageNotification; }
    }

    public HungerNotification HungerNotification
    {
        get { return _hungerNotification; }
    }

    public TemperatureTooHotNotification TemperatureTooHotNotification
    {
        get { return _temperatureTooHotNotification; }
    }

    public TemperatureTooColdNotification TemperatureTooColdNotification
    {
        get { return _temperatureTooColdNotification; }
    }

    public GasBatteryUnderchargeNotification GasBatteryUnderchargeNotification
    {
        get { return _gasBatteryUnderchargeNotification; }
    }

    public StormNotification StormNotification
    {
        get { return _stormNotification; }
    }

    public LightBulbNotification LightBulbNotification
    {
        get { return _lightBulbNotification; }
    }
    #endregion

    public List<Notification> GetAllConditions()
    {
        return _conditions;
    }

    private void Awake()
    {
        _conditions.Add(_batteryOverchargeNotification);
        _conditions.Add(_solarPanelDirtyNotification);
        _conditions.Add(_pollutionDamageNotification);
        _conditions.Add(_hungerNotification);
        _conditions.Add(_temperatureTooHotNotification);
        _conditions.Add(_temperatureTooColdNotification);
        _conditions.Add(_gasBatteryUnderchargeNotification);
        _conditions.Add(_stormNotification);
        _conditions.Add(_lightBulbNotification);
    }

    private void Start()
    {
        //_conditions.Add(_batteryOverchargeNotification);
        //_conditions.Add(_solarPanelDirtyNotification);
        //_conditions.Add(_pollutionDamageNotification);
        //_conditions.Add(_hungerNotification);
        //_conditions.Add(_temperatureTooHotNotification);
        //_conditions.Add(_temperatureTooColdNotification);
        //_conditions.Add(_gasBatteryUnderchargeNotification);
        //_conditions.Add(_stormNotification);
        //_conditions.Add(_lightBulbNotification);


        StartCoroutine(MyCoroutines.WaitOneFrame(() =>
        {
            foreach (Notification notificationCondition in _conditions)
                notificationCondition.OnValidate();
            foreach (Notification notificationCondition in _conditions)
                notificationCondition.Start();
        }));

        //if (Application.isPlaying)
    }
}
