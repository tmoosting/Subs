# Subs

Final Assignment for Masters course: [Modern Game AI Algorithms](https://studiegids.universiteitleiden.nl/courses/98799/modern-game-ai-algorithms) at Leiden University

Unity version: 2020.3.f1

[MLAgents](https://github.com/Unity-Technologies/ml-agents) version: 1.9.1

### Summary
Subs is a Unity-MLAgents project for training WWII Destroyers to protect their precious convoys from vicious, decision-tree AI Uboats.  

### Project Report
[Link](https://drive.google.com/file/d/1ayQ4Lkly29q4ofP5zjz57u9UhYBNetSD/view?usp=sharing)

### Demo (.mp4)
[Link](https://drive.google.com/file/d/1iy4SkoOeKJQPYtY_UoVT50k5dEFxXJ0T/view)

### Executable (Windows)
[Link](https://drive.google.com/file/d/1qn-W27A65L_a5ssikh2dlm2BI7_G0VKo/view?usp=sharing)



### Install
Unzip the Unity Source file and open the folder through Unity Hub. 


### Play
The game has a battle-ready scenario with player input and deadly Uboats.
<ul>
 <li>Open the Game scene in Assets/Scenes </li>
 <li>Click on any ship, or their icon at the top of the screen, to select it</li>
 <li>Use the Engine selector to set the ship’s engine</li>
 <li>Use the bearing-field, or middle mouse button, to set the ship’s target bearing</li>
 <li>Note the controls as indicated in the movable Ship Window:</li>
  <ul>
 <li>Middle Mouse: Toggle target target location</li>
<li> Rightmouse: Pan camera, Scroll: zoom in/out</li>
<li> Left/Right arrows: Select next ship</li>
<li> F: Follow selected ship</li>
<li> P: Toggle physics correction (default: ON)</li>
<li> T: Fire torpedo (Uboat selected)</li>
<li> X: Throw depth charges (Destroyer selected)</li>
<li> C: Toggle strategy view</li>
  </ul>
 </ul>
 
### Procedural Generation: Weather System
<ul>
  <li>Open the PCG scene in Assets/Scenes </li>
<li>The Perlin noise generation is tunable by altering the Width, Height and Scale
parameters in the _Generation object.</li>
<li>The Locator object in each ship finds the dynamic greyscale on the map which can
be seen in the “Gs” variable.</li>
 <li>This variable updates the linear drag of the ship depending on its location.</li>
 </ul>
 
### Training
<ul>
<li>Open the Training scene in Assets/Scenes</li>
<li>Open the ChaserParent object in Hierarchy, and its first ChaserBox</li>
 <li>Note the Behaviour Parameters script in the Inspector. It will have a pre-trained model assigned by default. </li>
 <li>To start a new training run:</li>
 <ul>
 <li> click the circle next to Model and assign ‘None’</li>
 <li>(for parallel training: activate the other 8 ChaserBoxes in the Hierarchy)</li>
 <li>Use command line, type: mlagents-learn</li>
  <li>paramaters: --run-id=NAME to name the behavior, --force to overwrite, --resume to
continue on a precious training set</li>
  <li>Press Play in Unity</li>
 </ul>
