using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitUI : MonoBehaviour
{
    public enum Growth : int { Small = 0, Medium = 1, Big = 2 }
    public enum Emotion : int { Sad = 0, Neutral = 1, Happy = 2 }
    private Sprite[,] _sprites = new Sprite[3, 3];

    [SerializeField, ReadOnly] private bool _dirty = true;

    [SerializeField] private bool _isOn = true;
    [SerializeField] private Image _image = null;

    [SerializeField] private Growth _growth = Growth.Medium;
    [SerializeField] private Emotion _emotion = Emotion.Neutral;
    //[SerializeField, ReadOnly] private int _growth = 2;
    //[SerializeField, ReadOnly] private int _emotion = 2;

    [Tooltip("Min is minimum happiness needed for Neutral, Max is minimum happiness for Happy")]
    [SerializeField, MinMax(0, 1)] private MinMaxPair _happinessRange = new MinMaxPair(0.33f, 0.66f);

    [Header("Growth Stage 1")]
    [SerializeField] private Sprite _spriteSmallSad = null;
    [SerializeField] private Sprite _spriteSmallNeutral = null;
    [SerializeField] private Sprite _spriteSmallHappy = null;
    [Header("Growth Stage 2")]
    [SerializeField] private Sprite _spriteMediumSad = null;
    [SerializeField] private Sprite _spriteMediumNeutral = null;
    [SerializeField] private Sprite _spriteMediumHappy = null;
    [Header("Growth Stage 3")]
    [SerializeField] private Sprite _spriteBigSad = null;
    [SerializeField] private Sprite _spriteBigNeutral = null;
    [SerializeField] private Sprite _spriteBigHappy = null;

    public bool IsOn
    {
        get { return _isOn; }
        set { _isOn = value; }
    }

    /// <summary>
    /// <see cref="Growth"/> Stage (Small, Medium and Big). Changes portrait.
    /// </summary>
    public Growth GrowthStage
    {
        get { return _growth; }
        set
        {
            _growth = value;

            if (_isOn) _dirty = true;
            else UpdateSprite();
        }
    }

    /// <summary>
    /// <see cref="Emotion"/>al State (Sad, Neutral and Happy). Changes portrait.
    /// </summary>
    public Emotion EmotionalState
    {
        get { return _emotion; }
        set
        {
            _emotion = value;

            if (_isOn) _dirty = true;
            else UpdateSprite();
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            _dirty = true;
    }

    private void OnEnable()
    {
        Pet.onHappinessChanged += OnHappinessChanged;
        CheckpointManager.onCheckpointStart += GrowthHappens;
    }

    private void Start()
    {
        AssignSprites();
        StartCoroutine(MyCoroutines.WaitOneFrame(() =>
            GrowthStage = (Growth)Mathf.Clamp(GameManager.Instance.CheckpointManager.CheckpointsPassed, 0, 2)));
    }

    private void OnDisable()
    {
        if (CheckpointManager.onCheckpointStart != null)
            CheckpointManager.onCheckpointStart -= GrowthHappens;
        if (Pet.onHappinessChanged != null)
            Pet.onHappinessChanged -= OnHappinessChanged;
    }

    private void AssignSprites()
    {
        //Row 0 is Small
        _sprites[0, 0] = _spriteSmallSad;
        _sprites[0, 1] = _spriteSmallNeutral;
        _sprites[0, 2] = _spriteSmallHappy;

        //Row 1 is Medium
        _sprites[1, 0] = _spriteMediumSad;
        _sprites[1, 1] = _spriteMediumNeutral;
        _sprites[1, 2] = _spriteMediumHappy;

        //Row 2 is Big
        _sprites[2, 0] = _spriteBigSad;
        _sprites[2, 1] = _spriteBigNeutral;
        _sprites[2, 2] = _spriteBigHappy;
    }

    private void OnHappinessChanged100(float happiness)
    {
        if (!_isOn) return;
        OnHappinessChanged(happiness / 100);
    }

    private void OnHappinessChanged(float happiness)
    {
        MinMaxPair.Position positionInRange = _happinessRange.Evaluate(happiness);

        switch (positionInRange)
        {
            default:
            case MinMaxPair.Position.Invalid:
                Debug.Log("Invalid Portrait Happiness!");
                break;
            case MinMaxPair.Position.Under:
                EmotionalState = Emotion.Sad;
                break;
            case MinMaxPair.Position.InRange:
                EmotionalState = Emotion.Neutral;
                break;
            case MinMaxPair.Position.Over:
                EmotionalState = Emotion.Happy;
                break;
        }
    }

    private void GrowthHappens()
    {
        int i = (int)GrowthStage;
        i = Mathf.Clamp(i, 0, 2);
        GrowthStage = (Growth)i;
    }

    private void Update()
    {
        if (!_dirty) return;

        UpdateSprite();
        _dirty = false;
    }

    private void UpdateSprite()
    {
        _image.sprite = _sprites[(int)_growth, (int)_emotion];
    }
}
