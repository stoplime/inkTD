# InkTD

### App and Scripting Requirements

#### Manditory Requirements
1. Create at least one backgourd sound AND one sound effect for the game. Make sure the audio clips can be managed by your scripts. (10%)
    * There is one main song that playes in the background.
    * There is also one sound effect when the tower's shoot a projectile.
    * Sound volumes are configurable through scripts.
2. Create a game setting menu to config the volume for all the audios used in the game, change resolutions, switch fullscreen/window, shadow types, etc. No less than 5 different configurable options. (10%)
    * Game setting menu has 5 options
        * Music Volume
        * Sound Effect Volume
        * Fullscreen toggle
        * Resolution Dropdown
        * Anti-Aliasing Toggle
3. Save inventory information into a file and make the game read it before running. (10%)
    * Game setting also has the option to save and load a game.
    * Save option will save the tower placement as well as game progress variables like money and income.
4. Save the game settings to a file and make the game read it before running. (10%)
    * Save option also saves the volume setting
5. Able to load the inventory information and game settings from the previous time quit when starting the game. (10%)
    * Loading the save file works

#### Semi-Manditory Requirements
1. Create different projectiles and use script to manage shooting which projectile. (5%)
    * Using different towers, they are able to shoot different types of projectiles.
2. Create at least one 3D GUI menu, menu could be a health bar, could be a button, etc. A 3D GUI menu need to face to the camera all the time. (5%)
    * All creatures and the main Tower-Castle has a floating health bar above them that always faces the camera.
3. Create 2 or more levels for the game. (10%)
    * There are 3 total levels for the game so far.
4. Create a first person or third person camera controlled by keyboard and mouse. The camera needs to be able to translate and rotate. Make sure you create a health menu for your character. (10%)
    * The camera design is a third person camera.
    * It can be controlled using "wasd" or arrow keys to pan.
    * It can also zoom with the mouse scroll wheel.
    * if the camera scrolls too close, it will rotate upwards to have a scenic view. 
5. Create at least 5 waypoints for AI characters and 12 different obstacles on the floor and bake the navigation mash. (5%)
    * AI creatures have a dynamic number of way points equal to the number of path grids to traverse.
    * Obstacles include the towers themselves as well as actual obstical objects.
6. Create at least 1 AI friend. It will always follow your character and help you attacking the enemies when the enemies are closer than 5 units from YOUR character. Create a health bar for your AI friend and make the enemies attack to your AI friend first. Your AI friend will die when the health is all gone. (15%)
    * So an AI friend doesn't work exactly like the requirements, rather the point of the game is to spawn in creatures that will attack the opponents base.
    * Technically they are friendly creatures since they will not be attacking your base but it's exactly the same code as the enemy creatures.
    * They do die when their health goes to 0.
7. Create at least 2 AI enemies. One of them will be idle when it is more than 10 units from your character. And if it is less or equal than 10 units from your character, it will start chasing you. If it is less then 5 units from your character, it will shoot a bullet to you, your health will be decreased. The other enemy will guard along the 5 waypoints when it is more than 10 units from your character. If it is less than 2 units from you it will attack your character by punching, your health will be decreased. (20%)
    * So I have many spawnable AI creatures but also a number of AI players.
    * They are able to also spawn creatures, place towers, remove towers, and upgrade towers
    * They have access to the same controls as the player and plays against the player.
8. Others but I didn't consider them to be applicable to this game.
    * The other requirements about inventory does not particularly apply to this game.

#### Optional Requirements
1. Make login menu for different players. ONLY use ONE file to store the inventory information for diferent players, and another ONE file to store the game settings for different players. (10%)
    * There is no login for different players, however, there is multiple players in the sense that the AI Opponents act like mutiple players.
    * The save is structures so that all the players(human and AI) are saved as one file.
2. Using script to create another player IN GAME at any time. Split the screen by half for two players, and allow the players to set the keyboards or mouse for the game. (10%)
    * Ummmm, no.
3. Published in Google Play. (10%)
    * So I do plan on publishing the game but probably not on Google Play.
    * There is an indie game website that lets you publish specifically for indie games called http://gamejolt.com
    * It also lets you profit any ad revenue generated by the game.
    * I do plan on revising the game before releasing it to the public. I.E. balancing the creatures and tower settings, or adding more features.
    * This is the site I'll publish it when I get around to doing it: https://gamejolt.com/games/inktd/301075