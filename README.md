Add the following aspects to the TextBasedAdventure.cs script

    Expand the Dungeon to be at least 4 x 4.
    Add a new string array to called tileDescriptions. Add descriptions of each location.
    The description should be output the first time a player enters a room. Add code to handle this.
    Hint: What might be a good place to save information about whether the room has been visited? Where is the rest of the information about the room located?
    Add a new interaction called Look. it should output the description again. Use a key to do this interaction.
    Add a new TileType called Blockade. This should prevent movement in that direction.
    Add a new TileType called Teleporter. Using it should move you to another Teleporter tile.
    Teleporters should be connected in pairs. Validate your dungeon and be sure there is always an even number of teleporters and that they are connected properly.
    Using an input, you should be able to use the teleporter.
