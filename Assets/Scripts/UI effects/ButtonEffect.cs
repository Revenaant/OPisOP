using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEffect : MonoBehaviour
{
    private List<Vector3> _startPositions = new List<Vector3>();

    [Header("Images To move")]
    [SerializeField] private List<Transform> _images;
    [SerializeField] private bool _useAllChildren;
    [SerializeField] private Vector2 _displacement = new Vector2(0, 2);

    // Use this for initialization
    void Start()
    {
        if(_useAllChildren) {
            for (int i = 0; i < transform.childCount; i++)
                _images.Add(transform.GetChild(i));
        }

        for (int i = 0; i < _images.Count; i++) 
            _startPositions.Add(_images[i].localPosition);
	}
	
    public void OnButtonDown()
    {
        for (int i = 0; i < _images.Count; i++)
        {
            _images[i].localPosition = new Vector3(_images[i].localPosition.x + _displacement.x,
                                                  _images[i].localPosition.y - _displacement.y,
                                                  _images[i].localPosition.z);
        }
    }

    public void OnButtonUp()
    {
        for (int i = 0; i < _images.Count; i++)
        {
            _images[i].localPosition = _startPositions[i];
        }
    }
}
