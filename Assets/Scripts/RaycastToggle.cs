using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.BuiltinInterfaces;

public class RaycastToggle : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/goal_pose";

    public InputActionReference triggerAction = null;
    public XRRayInteractor rayInteractor = null;
    public InputActionReference placeMarkerAction = null;
    public GameObject markerGameObject = null; // Global reference to the markerGameObject Singleton

    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseStampedMsg>(topicName);
    }

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

                // Publish the markerGameObject's position to /goal_pose
                ros.Publish(topicName, new PoseStampedMsg
                {
                    header = new HeaderMsg
                    {
                        stamp = new TimeMsg((int)(Time.timeSinceLevelLoad), 0), // Set the timestamp
                        frame_id = "map" // Set the frame id
                    },
                    pose = new PoseMsg
                    {
                        position = new PointMsg { x = hit.point.z, y = -hit.point.x, z = hit.point.y }, // Set the position (note the axis conversion from Unity to ROS)
                        orientation = new QuaternionMsg { x = 0, y = 0, z = 0, w = 1 } // TODO: Implement orientation adjustment
                    }
                });
            }
        }
    }
}
