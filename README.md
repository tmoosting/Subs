# Subs

Final Assignment for Masters course: [Modern Game AI Algorithms](https://studiegids.universiteitleiden.nl/courses/98799/modern-game-ai-algorithms)

Unity version: 2020.3.f1
MLAgents version: 1.9.1


### WebGL

### Project Report
[Link](https://drive.google.com/file/d/1ayQ4Lkly29q4ofP5zjz57u9UhYBNetSD/view?usp=sharing)

### Install
Unzip the Unity Source file and open the folder through Unity Hub.
Find and open the three different Scenes in the Assets/Scenes folder.


### Play
The game has a battle-ready scenario with player input and deadly Uboats.
..* Click on any ship, or their icon at the top of the screen, to select it
..* Use the Engine selector to set the ship’s engine
..* Use the bearing-field, or middle mouse button, to set the ship’s target bearing
..* Note the controls as indicated in the movable Ship Window:
○ Middle Mouse: Toggle target target location
○ Rightmouse: Pan camera, Scroll: zoom in/out
○ Left/Right arrows: Select next ship
○ F: Follow selected ship
○ P: Toggle physics correction (default: ON)
○ T: Fire torpedo (Uboat selected)
○ X: Throw depth charges (Destroyer selected)
○ C: Toggle strategy view


### PCG

● The Perlin noise generation is tunable by altering the Width, Height and Scale
parameters in the _Generation object.
● The Locator object in each ship finds the dynamic greyscale on the map which can
be seen in the “Gs” variable.
● This variable updates the linear drag of the ship depending on its location.
Training
Environment for training one or multiple agents with the MLAgents toolkit
Open the ChaserParent object in Hierarchy, and its first ChaserBox:
