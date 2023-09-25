using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using System.Collections.Generic;

public class PointCloudUpdater : MonoBehaviour
{
    public string pointCloudTopic = "/zed2i/zed_node/point_cloud/cloud_registered"; // Configure the point cloud topic name here
    private MeshRenderer pointCloudRenderer; // Reference to the mesh renderer for the point cloud
    private Texture2D pointCloudTexture; // Texture to update with the point cloud data

    void Start()
    {
        pointCloudRenderer = GetComponent<MeshRenderer>(); // Assumes the point cloud is displayed on the same GameObject as this script

        pointCloudTexture = new Texture2D(1, 1); // Initialize a blank texture
        pointCloudRenderer.material.mainTexture = pointCloudTexture; // Assign the texture to the material

        ROSConnection.GetOrCreateInstance().Subscribe<PointCloud2Msg>(pointCloudTopic, PointCloudCallback); // Subscribe to the point cloud topic
    }

    private void PointCloudCallback(PointCloud2Msg message)
    {
        Debug.Log("PointCloudCallback called");

        int width = (int)message.width;
        int height = (int)message.height;

        // Ensure the point cloud texture size matches the incoming data
        if (pointCloudTexture.width != width || pointCloudTexture.height != height)
        {
            pointCloudTexture.Reinitialize(width, height);
        }

        Color[] colors = new Color[width * height];

        // Iterate through point cloud data and convert it to colors
        for (int i = 0; i < width * height; i++)
        {
            float x = message.data[i * message.point_step + message.fields[0].offset];
            float y = message.data[i * message.point_step + message.fields[1].offset];
            float z = message.data[i * message.point_step + message.fields[2].offset];

            // Modify color based on point cloud data (example: map Z values to grayscale)
            Color pixelColor = new Color(z, z, z);
            colors[i] = pixelColor;
        }

        pointCloudTexture.SetPixels(colors);
        pointCloudTexture.Apply(); // Apply changes to the texture

        Debug.Log("PointCloudCallback finished");
    }
}