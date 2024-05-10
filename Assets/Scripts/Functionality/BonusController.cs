using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BonusController : MonoBehaviour
{
    [SerializeField]
    private Button Spin_Button;
    [SerializeField]
    private RectTransform Wheel_Transform;
    [SerializeField]
    private BoxCollider2D[] point_colliders;

    private Tween wheelRoutine;

    private float elasticIntensity = 5f;


    private void Start()
    {
        if (Spin_Button) Spin_Button.onClick.RemoveAllListeners();
        if (Spin_Button) Spin_Button.onClick.AddListener(Spinbutton);
    }

    private void Spinbutton()
    {
        RotateWheel();
        DOVirtual.DelayedCall(3f, () =>
        {
            TurnCollider(4);
        });
    }

    private void RotateWheel()
    {
        if (Wheel_Transform) Wheel_Transform.localEulerAngles = new Vector3(0, 0, 359);
        if (Wheel_Transform) wheelRoutine =  Wheel_Transform.DORotate(new Vector3(0, 0, 0), 1, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }

    private void ResetColliders()
    {
        foreach(BoxCollider2D col in point_colliders)
        {
            col.enabled = false;
        }
    }

    internal void TurnCollider(int point)
    {
        if (point_colliders[point]) point_colliders[point].enabled = true;
    }

    internal void StopWheel()
    {
        if (wheelRoutine != null)
        {
            wheelRoutine.Pause(); // Pause the rotation

            // Apply an elastic effect to the paused rotation
            Wheel_Transform.DORotate(Wheel_Transform.eulerAngles + Vector3.forward * Random.Range(-elasticIntensity, elasticIntensity), 1f)
                .SetEase(Ease.OutElastic);
        }
    }
}
