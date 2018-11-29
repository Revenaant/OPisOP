using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayAfterDead))]
public class CarrotAudio : MonoBehaviour
{
    private PlayAfterDead audio;
    [SerializeField] private AudioClip bumpCarrot;
    [SerializeField] private AudioClip bumpOther;
    [SerializeField, MinMax(0, 5)] private MinMaxPair speed = new MinMaxPair(0.1f, 3f);

    private void Start()
    {
        audio = GetComponent<PlayAfterDead>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<Supplements>() != null)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            float sqrMagn = rb.velocity.sqrMagnitude;

            var pos = speed.Evaluate(sqrMagn);
            switch (pos)
            {
                case MinMaxPair.Position.Invalid:
                    break;
                case MinMaxPair.Position.Under:
                    break;
                case MinMaxPair.Position.InRange:
                    audio.PlayClipCode(bumpCarrot, (sqrMagn - speed.Min) / (speed.Max - speed.Min), 0, 2);
                    break;
                case MinMaxPair.Position.Over:
                    audio.PlayClipCode(bumpCarrot, 1, 0, 2);
                    break;
                default:
                    break;
                    //audio.PlayClipCode(bumpCarrot, Mathf.Clamp(rb.velocity.sqrMagnitude, 0, 2);
            }
        }
    }
}