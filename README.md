Non-parametric A* Algorithm by Ethan Hannen

----------------
Overview
----------------

This program is written in C# using XAML. It was created for an Intro to AI course at the University of North Florida in April 2020. 

The main pathfinding section utilizes an algorithm published by Stern R., et al in Artificial Intelligence (2014).

Reference: https://goldberg.berkeley.edu/pubs/Anytime-Nonparametric-A-star-AI-Journal-Sept-2014.pdf

This particular algorithm is suited for finding fast solutions to a pathfinding problem and then working toward refining that solution until the shortest path is found.

----------------
Instructions
----------------

1. Choose your desired number of obstacles (shapes) using the provided slider in the left pane.
2. Choose shape color, if desired, by dragging the sliders for RGB color values.
3. Drag goals to desired location throughout state space.
4. Click generate. The algorithm will find multiple paths. The path in green is the shortest, and final, solution.

----------------
Notes
----------------

Number of nodes and edges are reported in the right pane.
Below that, the time it took to find each solution will be reported after the algorith is finished.
You may slow down the pathfinding by clicking the 'Slow Solution' checkbox. This allows you to see which solutions are found in a visually-comprehensible timescale.
You can see which edges have been created by clicking the 'Show Edges' checkbox.

----------------
Issues & To Do's
----------------

1. There is an issue with dragging the goal (green) node. It doesn't always reconnect the appropriate edges.
2. The reset button is not working properly at this time.
3. The Linear Deviation algorithm is still in development. It will use Non-Parametric A* as the default in all cases.
4. On initial start, it will sometimes find a suboptimal path when the default obstacle count is set to 10. 

These issues shall be fixed in the future and a finalized revision will be committed once I have some free time.
