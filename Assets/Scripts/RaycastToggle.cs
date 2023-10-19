using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class RaycastToggle : MonoBehaviour
{
    public InputActionReference triggerAction = null;
    public XRRayInteractor rayInteractor = null;

    public InputActionReference placeMarkerAction = null;
    public GameObject markerGameObject = null; // Global reference to the markerGameObject Singleton

    private void OnEnable()
    {
        if (triggerAction != null)
        {
            triggerAction.action.performed += TriggerPressed;
        }
        if (placeMarkerAction != null)
        {
            placeMarkerAction.action.performed += PlaceMarker;
        }
    }

    private void OnDisable()
    {
        if (triggerAction != null)
        {
            triggerAction.action.performed -= TriggerPressed;
        }
        if (placeMarkerAction != null)
        {
            placeMarkerAction.action.performed -= PlaceMarker;
        }
    }

    private void TriggerPressed(InputAction.CallbackContext context)
    {
        rayInteractor.enabled = !rayInteractor.enabled;
    }

    private void PlaceMarker(InputAction.CallbackContext context)
    {
        if (rayInteractor.enabled)
        {
            RaycastHit hit;
            if (Physics.Raycast(rayInteractor.transform.position, rayInteractor.transform.forward, out hit))
            {
                // Move the markerGameObject Singleton to the hit point
                markerGameObject.transform.position = hit.point;

                // If markerGameObject isn't visible yet, enable it now
                if (markerGameObject.activeSelf == false)
                {
                    markerGameObject.SetActive(true);
                }

                // TODO: Publish the markerGameObject's position to /goal_pose
            }
        }
    }
}
