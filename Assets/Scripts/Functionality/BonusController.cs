using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BonusController : MonoBehaviour
{
    [SerializeField]
    private Button Spin_Button;
    [SerializeField]
    private RectTransform Wheel_Transform;
    [SerializeField]
    private BoxCollider2D[] point_colliders;
    [SerializeField]
    private TMP_Text[] Bonus_Text;
    [SerializeField]
    private GameObject Bonus_Object;
    [SerializeField]
    private SlotBehaviour slotManager;

    private Tween wheelRoutine;

    private float elasticIntensity = 5f;

    private int stopIndex = 0;


    private void Start()
    {
        if (Spin_Button) Spin_Button.onClick.RemoveAllListeners();
        if (Spin_Button) Spin_Button.onClick.AddListener(Spinbutton);
    }

    internal void StartBonus(int stop)
    {
        if (Spin_Button) Spin_Button.interactable = true;
        stopIndex = stop;
        if (Bonus_Object) Bonus_Object.SetActive(true);
    }

    private void Spinbutton()
    {
        if (Spin_Button) Spin_Button.interactable = false;
        ResetColliders();
        RotateWheel();
        DOVirtual.DelayedCall(2f, () =>
        {
            TurnCollider(stopIndex);
        });
    }

    internal void PopulateWheel(List<string> bonusdata)
    {
        for (int i = 0; i < bonusdata.Count; i++)
        {
            if (bonusdata[i] == "-1")
            {
                if (Bonus_Text[i]) Bonus_Text[i].text = "NO \nBONUS";
            }
            else
            {
                if (Bonus_Text[i]) Bonus_Text[i].text = bonusdata[i];
            }
        }
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

    private void TurnCollider(int point)
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
        DOVirtual.DelayedCall(3f, () =>
        {
            ResetColliders();
            if (Bonus_Object) Bonus_Object.SetActive(false);
            slotManager.CheckPopups = false;
        });
    }
}
