using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargeterUi : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private static readonly int Targeting = Animator.StringToHash("Selected");

    public void IsTargeting(bool isTargeting)
    {
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            animator.SetBool(Targeting, isTargeting);
        }
    }
}
