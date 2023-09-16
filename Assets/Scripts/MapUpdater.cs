using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Nav;

public class MapUpdater : MonoBehaviour
{
    public string mapTopic = "/map"; // TODO: Update this with the map topic name
    private MeshRenderer mapRenderer; // Reference to the mesh renderer for the map
    private Texture2D mapTexture; // Texture to update with the map data

    void Start()
    {
        mapRenderer = GetComponent<MeshRenderer>(); // Assumes the map is displayed on the same GameObject as this script

        mapTexture = new Texture2D(1, 1); // Initialize a blank texture
        mapRenderer.material.mainTexture = mapTexture; // Assign the texture to the material

        ROSConnection.GetOrCreateInstance().Subscribe<OccupancyGridMsg>(mapTopic, MapCallback); // Subscribe to the map topic
    }

    private void MapCallback(OccupancyGridMsg message)
    {
        Debug.LogWarning("/map callback was triggered");

        int width = (int)message.info.width;
        int height = (int)message.info.height;

        // Ensure the map texture size matches the incoming map
        if (mapTexture.width != width || mapTexture.height != height)
        {
            mapTexture.Reinitialize(width, height);
        }

        Color[] colors = new Color[width * height];
        for (int i = 0; i < width * height; i++)
        {
            sbyte mapValue = message.data[i];
            byte convertedValue = (byte)(mapValue + 128); // Convert sbyte to byte
            Color pixelColor = (convertedValue == 0) ? Color.black : Color.white; // Modify color based on map data
            colors[i] = pixelColor;
        }

        mapTexture.SetPixels(colors);
        mapTexture.Apply(); // Apply changes to the texture
    }

}