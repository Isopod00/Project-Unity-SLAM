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

    // These are for manually rotating the markerGameObject Singleton
    public InputActionReference rotateRightAction = null;
    public InputActionReference rotateLeftAction = null;
    // Smoothly rotates the markerGameObject Singleton
    private Coroutine rotateCoroutine = null;

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
        if (rotateLeftAction != null)
        {
            rotateLeftAction.action.performed += LeftGripPressed;
        }
        if (rotateRightAction != null)
        {
            rotateRightAction.action.performed += RightGripPressed;
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
        if (rotateLeftAction != null)
        {
            rotateLeftAction.action.performed -= LeftGripPressed;
        }
        if (rotateRightAction != null)
        {
            rotateRightAction.action.performed -= RightGripPressed;
        }
    }

    private void TriggerPressed(InputAction.CallbackContext context)
    {
        rayInteractor.enabled = !rayInteractor.enabled;
    }

    private void LeftGripPressed(InputAction.CallbackContext context)
    {
        if (rotateCoroutine == null)
        {
            rotateCoroutine = StartCoroutine(RotateMarker(-1));
        }
        else
        {
            StopCoroutine(rotateCoroutine);
            rotateCoroutine = null;

            // Publish the markerGameObject's position and orientation to /goal_pose if the markerGameObject is enabled
            if (markerGameObject.activeSelf == true)
            {
                ros.Publish(topicName, new PoseStampedMsg
                {
                    header = new HeaderMsg
                    {
                        stamp = new TimeMsg((int)(Time.timeSinceLevelLoad), 0), // Set the timestamp
                        frame_id = "map" // Set the frame id
                    },
                    pose = new PoseMsg
                    {
                        position = new PointMsg { x = markerGameObject.transform.position.z, y = -markerGameObject.transform.position.x, z = markerGameObject.transform.position.y }, // Set the position (note the axis conversion from Unity to ROS)
                        orientation = new QuaternionMsg { x = markerGameObject.transform.rotation.z, y = -markerGameObject.transform.rotation.x, z = markerGameObject.transform.rotation.y, w = markerGameObject.transform.rotation.w } // Set the orientation (note the axis conversion from Unity to ROS)
                    }
                });
            }
        }
    }

    private void RightGripPressed(InputAction.CallbackContext context)
    {
        if (rotateCoroutine == null)
        {
            rotateCoroutine = StartCoroutine(RotateMarker(1));
        }
        else
        {
            StopCoroutine(rotateCoroutine);
            rotateCoroutine = null;

            // Publish the markerGameObject's position and orientation to /goal_pose if the markerGameObject is enabled
            if (markerGameObject.activeSelf == true)
            {
                ros.Publish(topicName, new PoseStampedMsg
                {
                    header = new HeaderMsg
                    {
                        stamp = new TimeMsg((int)(Time.timeSinceLevelLoad), 0), // Set the timestamp
                        frame_id = "map" // Set the frame id
                    },
                    pose = new PoseMsg
                    {
                        position = new PointMsg { x = markerGameObject.transform.position.z, y = -markerGameObject.transform.position.x, z = markerGameObject.transform.position.y }, // Set the position (note the axis conversion from Unity to ROS)
                        orientation = new QuaternionMsg { x = markerGameObject.transform.rotation.z, y = -markerGameObject.transform.rotation.x, z = markerGameObject.transform.rotation.y, w = markerGameObject.transform.rotation.w } // Set the orientation (note the axis conversion from Unity to ROS)
                    }
                });
            }
        }
    }

    private IEnumerator RotateMarker(int direction)
    {
        while (true)
        {
            // Create a rotation that rotates an angle around the markerGameObject's normal direction
            Quaternion rotation = Quaternion.AngleAxis(direction * Time.deltaTime * 25, markerGameObject.transform.up); // Adjust the speed of rotation as needed

            // Apply the rotation to the markerGameObject
            markerGameObject.transform.rotation = rotation * markerGameObject.transform.rotation;

            yield return null;
        }
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
                // Rotate the markerGameObject Singleton to point in the same direction as the raycast (but project it onto the hit surface)
                markerGameObject.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(rayInteractor.transform.forward, hit.normal));

                // If markerGameObject isn't visible yet, enable it now
                if (markerGameObject.activeSelf == false)
                {
                    markerGameObject.SetActive(true);
                }

                // Publish the markerGameObject's position and orientation to /goal_pose
                ros.Publish(topicName, new PoseStampedMsg
                {
                    header = new HeaderMsg
                    {
                        stamp = new TimeMsg((int)(Time.timeSinceLevelLoad), 0), // Set the timestamp
                        frame_id = "map" // Set the frame id
                    },
                    pose = new PoseMsg
                    {
                        position = new PointMsg {x = markerGameObject.transform.position.z, y = -markerGameObject.transform.position.x, z = markerGameObject.transform.position.y}, // Set the position (note the axis conversion from Unity to ROS)
                        orientation = new QuaternionMsg {x = markerGameObject.transform.rotation.z, y = -markerGameObject.transform.rotation.x, z = markerGameObject.transform.rotation.y, w = markerGameObject.transform.rotation.w} // Set the orientation (note the axis conversion from Unity to ROS)
                    }
                });
            }
        }
    }
}
