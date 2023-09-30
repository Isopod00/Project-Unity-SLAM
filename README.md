# Project-Unity-SLAM
A Unity Engine project for visualizing the map created by a stereo depth camera in VR

## Modifications made to packages:

1) In the scene hierarchy change DefaultVisualizationSuite -> sensor_msgs -> PointCloud2 -> Color mode to "Combined RGB"

2) Modify the shaders in Packages -> Unity Robotics Visualizations -> Runtime -> Drawing3d -> Shaders to match [these](https://github.com/Isopod00/Project-Unity-SLAM/tree/main/Modified%20Shaders)
