using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FridayTutorial : MonoBehaviour
{
    [SerializeField]
    private int _count = -1;
    private Tweener _previousTweener = null;
    private Tweener _nextTweener = null;

    [SerializeField] private GameObject _tutorial = null;

    [SerializeField] private List<Text> _texts = null;


    // Use this for initialization
    private void Start()
    {
        GameManager.Instance.Pause();
    }

    public void Next()
    {
        if (_texts == null || _texts.Count == 0) return;

        if (_count >= _texts.Count)
        {
            GameManager.Instance.Unpause();
            _tutorial.SetActive(false);
            return;
        }

        //Debug.Log("Next");
        Text previous, next;

        if (_count >= 0)
        {
            if (_previousTweener != null)
                _previousTweener.Complete(true);
            
            previous = _texts[_count];
            _previousTweener = previous.DOFade(0, 0.2f).SetAutoKill(true)
                .SetRecyclable().OnComplete(() =>
                {
                    previous.enabled = false;
                    _previousTweener = null;
                });
        }

        if (_count + 1 < _texts.Count)
        {
            if (_nextTweener != null)
                _nextTweener.Complete(true);

            next = _texts[_count + 1];
            next.enabled = true;
            _nextTweener = next.DOFade(1, 0.2f).SetAutoKill(true)
                .SetRecyclable().OnComplete(() => _nextTweener = null);
        }

        _count++;
    }
}
