PS8 : Kedar Bastakoti

Notes:
MinTimeToMerge: is a game world variable set to 4 seconds as default, which can be used to control 
how fast or slow players can merge.

Viruses have a very low chance of getting into world. They are green cubes with asterisk(*) in them.
If a sufficiently big cube bumps into the virus it will get exploded. They can be controlled by following parameters:
  private const int MAX_VIRUS_COUNT = 4;       // Maximum allowed at a given instance of time
  private const int INFESTATION_RATE = 1;     // Rate per 1000 updates or iteration
  private const int MIN_INFESTATION_MASS = 600;		// Represents the minimum mass of a cube to explode.

A cube with mass higher than the specified mass will be splitted to a random location by the virus.
It will take a while for those splitted cubes to merge back, as controlled by MinTimeToMerge.

Aside:
A cube can only eat another cube if that another cube is completely contained inside first cube.

Some extra features:
A player cube could randomly get stuck for a while if they consistently try to go outside the world while splitted.
If a player tries to repeatedly split the cube above the MaxSplit limit, it would randomly loose some of its
splitted cubes to be eaten by other cubes!

The world_parameters.xml is located at Resources directory by default, if it is not found the progrm will ask the user
to locate the file by opening a file open dialogue.


Previous Notes ============================================================================================

PS8
Kedar Bastakoti 
Mitchell Terry

Notes for TA:
Please, sync/pull this repo after Friday, December 4th, 2015 11:59PM.

We talked with Jim regarding extension. He agreed on one 1 extra day to work on this assignment for our team.
He asked to put this notes so that TAs can see it.

According to Jim, TAs can talk with Jim if TA's have any questions.

Implementation Notes:
Most of the implementations directly adhere to or follow the assignment specifications.
Viruses are randomly spawn and a player with a certain mass gets exploded after getting into a virus.
Smaller masses have no effect due to virus.






PS7
Mitchell Terry and Kedar Bastakoti 11/17/2015
Using an extention called Json.NET

The client starts with  controls to take in a player name and server IP. 
When the 'Connect' button is pressed, the client connets to the server and starts running the game.
If there is an issue connecting to the player, a dialog pops up to tell the user that a connection could not be made.

If there is an issue recieving server data, a dialog pops up with the exception stack.

The client centers the player in a panel that renders the world.

The player cube (or cubes) are scaled to occupy a percentage of the screen, which can be shown as the cubes shrinking
when the window is smaller and cubes growing as the window is bigger.

The user can point to a location on the panel to move the cube. When the cursor exits the panel, the cube will stop moving.
When a player is eaten, a dialog pops up to inform of player death.

User has the choice to restart the game immediately or can restart later at user's comfort time.