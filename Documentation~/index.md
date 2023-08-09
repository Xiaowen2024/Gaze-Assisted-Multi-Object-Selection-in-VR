# XRC Students su2023-in06-yuan

## Overview
XRC Students su2023-in06-yuan is a package that enables gaze assisted multiple object selection and manipulation. The package is based on XR Interaction Toolkit. Main functionalities include gaze hover for selection and manipulation, distant object scaling, and occluded objects expansion. 

## Package contents	
Runtime

Scripts:

Scripts attached to SpawnManager: scripts that allow you to generate cubes to test normal object selection, distant object selection, and occluded object selection
Scripts attached to MultiSelect Manager: scripts that implement select and maniputate functionality, assist distant and occluded objects selection

Sample

Prefabs: Interactable sample prefab, Play Area sample prefab, Coordinate Marker prefab, and MultiSelect Manager Prefab
Scenes: Sample Scenes to test object selection and manipulation, distant object scaling, and occlusion expansion
Model: Cross model for the 
Input: Custom MultiSelect Input Action Group
Material: Material for Object Hover State

## Installation instructions
To install this package, follow these steps:
1. In the Unity Editor, click on Window > Package Manager
2. Click the + button and choose Add package from git URL option
3. Paste this URL https://github.com/xrc-students/xrc-students-su2023-in06-yuan.git in the prompt and click on Add
4. You might have to authenticate as this is a private repository
5. The package should be installed into your project
6. You can download MultiSelect Samples from under Samples

To use eye tracking on Quest Pro, please import the Meta Gaze Adapter in the XR Interaction Toolkit and following the instructions here: https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.4/manual/samples.html#meta-gaze-adapter. 

To implement the functionality of this package, please attach the Initial Transform Script to every XR Interactable in the scene. For the XR Interactables, please enable gaze hover and disable gaze selection option in the gaze configuration section. In addition, please put the manager prefab into your scene. 

## Requirements	
Unity Editor 2021.3 and later

## Dependencies
XRC Students su2023-in06-yusn package has the following dependencies which are automatically added to your project when installing:

Input System (com.unity.inputsystem)
XR Interaction Toolkit (com.unity.xr.interaction.toolkit)
TextMeshPro (com.unity.textmeshpro)

## Limitations	
1. Visual feedback for object selection is different the first time you select the object and the following times you select it. 
2. Expansion for occluded objects does not gurantee the elimintation of occlusion for all objects in the eye gaze direction. Expansion may create new occlusion. 
3. Object selection and manipulation is limited to input from the right controller only. 


## Reference
### MultiSelect Input Actions

| Action Map | Action Name          | Action Mapping                                                                         |
|------------|----------------------|----------------------------------------------------------------------------------------|
| MultiSelect | Grip | Select the first object in multiselect  (Right Controller Grip Pressed)                                            |
| MultiSelect | Trigger      | Add another object to the selection (Right Controller Trigger Pressed)                                                         |
| MultiSelect | Expand       | Expand Occluded objects  (Right Controller Primary Button Pressed)  |

## Samples
The MultiSelect Samples are a great resource to explore the package. 

###Basic Selection and Manipulation  
Link to video: https://drive.google.com/file/d/1dzGX3bjdSr7n6wGwB7whaJYwuUHXG1yX/view?usp=sharing

###Scaling Distant Objects
Link to video: https://drive.google.com/file/d/1Q6kHq_09l1jqTYOvZyGGww-KPUa_ACX4/view?usp=sharing

###Expanding Occluded Objects
Link to video: https://drive.google.com/file/d/1EcjAvR_zX6xmE3Qd2M4-volNY1cH6uJE/view?usp=sharing

## Tutorials
1. Set your gameObjects in the scene as XR Grab interactables and attach the Initial Transform script to them. 
![Interactable](images/Interactable.png)

2. Drag the MultiSelect Manager component into the scene. 
![MultiSelect Manager](images/MultiSelectManager.png)
