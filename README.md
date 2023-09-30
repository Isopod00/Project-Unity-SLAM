# Project-Unity-SLAM
A Unity Engine project for visualizing the map created by a stereo depth camera in VR

## Modifications made to packages:

-In the scene hierarchy change DefaultVisualizationSuite -> sensor_msgs -> PointCloud2 -> Color mode to "Combined RGB"

-Modify the shaders in Packages -> Unity Robotics Visualizations -> Runtime -> Drawing3d -> Shaders to match the "Modified Shaders" in this repo