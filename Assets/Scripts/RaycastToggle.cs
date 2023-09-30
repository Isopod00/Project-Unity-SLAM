using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class RaycastToggle : MonoBehaviour
{
    public InputActionReference triggerAction = null;
    public XRRayInteractor rayInteractor = null;

    private void OnEnable()
    {
        if (triggerAction != null)
        {
            triggerAction.action.performed += TriggerPressed;
        }
    }

    private void OnDisable()
    {
        if (triggerAction != null)
        {
            triggerAction.action.performed -= TriggerPressed;
        }
    }

    private void TriggerPressed(InputAction.CallbackContext context)
    {
        rayInteractor.enabled = !rayInteractor.enabled;
    }
}
