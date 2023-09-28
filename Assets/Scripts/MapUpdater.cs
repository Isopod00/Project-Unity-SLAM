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

        // Assuming RGB color format (3 channels per point)
        int colorOffset = 12; // Adjust this offset according to the message format

        // Iterate through point cloud data and extract RGB color information
        for (int i = 0; i < width * height; i++)
        {
            float x = message.data[i * message.point_step + message.fields[0].offset];
            float y = message.data[i * message.point_step + message.fields[1].offset];
            float z = message.data[i * message.point_step + message.fields[2].offset];

            // Extract RGB color values from the message (adjust offsets accordingly)
            byte r = (byte)message.data[i * message.point_step + colorOffset];
            byte g = (byte)message.data[i * message.point_step + colorOffset + 1];
            byte b = (byte)message.data[i * message.point_step + colorOffset + 2];

            // Normalize RGB values to [0, 1]
            Color pixelColor = new Color(r / 255.0f, g / 255.0f, b / 255.0f);

            colors[i] = pixelColor;
        }

        pointCloudTexture.SetPixels(colors);
        pointCloudTexture.Apply(); // Apply changes to the texture

        Debug.Log("PointCloudCallback finished");
    }
}
