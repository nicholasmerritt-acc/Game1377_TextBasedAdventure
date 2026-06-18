using System.Collections.Generic;
using UnityEngine;

public class TextBasedAdventure : MonoBehaviour
{
    [System.Serializable]
    public struct Room
    {
        public string Name;
        public TileType Type;
        public string Description;
        public bool WasVisited;
        public int TeleportToRow;
        public int TeleportToColumn;
    }

    public enum TileType
    {
        Invalid,
        Empty,
        Item,
        Enemy,
        Exit,
        Blockade,
        Teleporter
    }

    private List<Room[,]> dungeon = new();

    /// <summary>
    /// can be reused wherever needed by the procedural generation
    /// </summary>
    private Room[] reusableRooms =
    {
        new Room { Name = "Dark Cave",
                   Type = TileType.Empty,
                   Description = "A mysterious, spooky cave. You hear the chittering of insects and the rattling of bones coming from the depths.",
                 },
        new Room { Name = "Mossy Tunnel",
                   Type = TileType.Item,
                   Description = "A long, narrow tunnel stretches out before you. There is springy green moss all along the floor. If you were barefoot, this room would be very comfy. Probably best to leave your shoes on, though.",
                 },
        new Room { Name = "Crystal Room",
                   Type = TileType.Empty,
                   Description = "Pink emanates from this room almost aggressively. There are gemstones lining the walls that glow and pulse with an intense energy.",
                 },
        new Room { Name = "Bone Chamber",
                   Type = TileType.Enemy,
                   Description = "This chamber is enormous. The stalactites and stalagmites look jagged and bone-like, and you hear the rattling of Skeletons and their makeshift bone weapons around the corner.",
                 },
        new Room { Name = "Flooded Hall",
                   Type = TileType.Empty,
                   Description = "Deep pools of water fill this once-marvelous hall. Ragged banners and rusty shields hang awkwardly on the walls.",
                 },
        new Room { Name = "Very Empty Room",
                   Type = TileType.Empty,
                   Description = "There is absolutely nothing here. Staring into the void stings your eyes and eats at your soul. Better move on quickly.",
                 },
        new Room { Name = "Goblin Den",
                   Type = TileType.Empty,
                   Description =  "Tattered goblin clothing, scraps of fur, and half-eaten food lay scattered around this room. Looks like the goblins left in a hurry. Better get out of here before they come back.",
                 },
        new Room { Name = "Armory",
                   Type = TileType.Enemy,
                   Description = "Racks of well-organized weapons fill this room. Obviously the goblins are preparing for a fight against a powerful force.",
                 },
        new Room { Name = "Throne Room",
                   Type = TileType.Item,
                   Description = "A large, empty throne looms towards the back of this room. There are several treasure chests flanking it, which must belong to some absent royalty. Better get them quickly and escape before they return.",
                 },
        new Room { Name = "Fred's Room",
                   Type = TileType.Empty,
                   Description = "Scattered children's toys make it apparent that this was once a child's room. The child is nowhere to be seen, though. You hear a very faint giggling from somewhere behind you.",
                 },
        new Room { Name = "Goblin Hovel",
                   Type = TileType.Empty,
                   Description = "A filthy goblin hut stands before you. It smells so bad, you don't even consider going inside.",
                 },
        new Room { Name = "Jewel Room",
                   Type = TileType.Item,
                   Description = "There is a large clear case in the center of the room. Seems like it might contain a precious stone...",
                 },
        new Room { Name = "Very Large, Empty Room",
                   Type = TileType.Empty,
                   Description = "There is absolutely nothing here. Lots and lots of nothing. Kind of impressive, really, just how much nothing is in this room.",
                 },
        new Room { Name = "Goblin Berserker",
                   Type = TileType.Enemy,
                   Description = "A goblin wearing a hat with two horns and holding an ax in each hand sprints at you, screaming! AHHHHH!",
                 },
        new Room { Name = "Very Strange Room",
                   Type = TileType.Enemy,
                   Description = "The air in this room is tinged with evil. You feel your life force being drained by some malevolent unseen entity. Better run.",
                 },
    };

    /// <summary>
    /// can be used once per dungeon. if empty, we fall back to reusableRooms
    /// </summary>
    private List<Room> uniqueRooms = new()
    {
        new Room { Name = "Raven's Hoard",
                   Type = TileType.Item,
                   Description = "Shiny items litter the floor. So many they hurt your eyes a bit. Better pick just one and move on before the raven returns.",
                 },
        new Room { Name = "Fountain of Youth",
                   Type = TileType.Item,
                   Description = "A rejuvenating spring makes you feel ten years younger. Ahhhhhh. Those bubbles are nice.",
                 },
        new Room { Name = "Fountain of Decrepitude",
                   Type = TileType.Enemy,
                   Description = "The brackish water looks and smells terrible. You are not sure why, but you touch a finger to it and immediately regret it. Ow.",
                 },
    };

    private Room teleporterStartRoom = new Room
    {
        Name = "Red Teleporter #1",
        Type = TileType.Teleporter,
        Description = "A large steel teleporter glows and hums and crackles with red bolts of electricity.",
        TeleportToRow = 0,
        TeleportToColumn = 0
    };

    private Room teleporterEndRoom = new Room
    {
        Name = "Red Teleporter #2",
        Type = TileType.Teleporter,
        Description = "A large steel teleporter glistens and whirs and crackles with red sparks of electricity.",
        TeleportToRow = 0,
        TeleportToColumn = 0
    };

    private Room exitRoom = new Room
    {
        Name = "Iron Gate",
        Type = TileType.Exit,
        Description = "There is an imposing steel gate up ahead that is slightly ajar. It looks like it leads downward, deeper into the dungeon...",
    };

    private Room startRoom = new Room
    {
        Name = "Worn Dungeon Path",
        Type = TileType.Empty,
        Description = "You enter the dungeon floor and breathe for a second to get your bearings. Level 30 awaits. Onward and downward.",
    };

    /// <summary>
    /// use one of these randomly whenever we need an impassable tile
    /// </summary>
    private Room[] blockadeRooms =
    {
        new Room
        {
            Name = "Impassable Wall",
            Type = TileType.Blockade,
            Description = "A sturdy wall blocks your path.",
        },
        new Room
        {
            Name = "Impassable Sheet of Ice",
            Type = TileType.Blockade,
            Description = "A huge impenetrable wall of ice stands between you and your destination. No getting through here.",
        },
        new Room
        {
            Name = "Impassable Iron Wall",
            Type = TileType.Blockade,
            Description = "This iron wall looks like it was constructed by very serious dwarven blacksmiths. There is absolutely no way you are getting past.",
        },
    };

    private int DUNGEON_LOWER_ROW_BOUND = 0;
    private int DUNGEON_LOWER_COLUMN_BOUND = 0;
    private int FAILED_TELEPORT_COORDINATE = -1;

    /// <summary>
    /// which row the player is currently in
    /// </summary>
    private int playerRow = 0;

    /// <summary>
    /// which column the player is currently in
    /// </summary>
    private int playerColumn = 0;

    /// <summary>
    /// which floor the player is currently traversing
    /// </summary>
    private int playerFloor = 0;

    private int playerHealth = 10;
    private int enemyDamage = 1;
    private int itemHealAmount = 2;

    public bool playerDead = false;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        dungeon = new();
        SetupPlayerVariables();
        StartingMonologue();
        SetupNextFloor();
        VerifyTeleportersMatch();
        OutputTileInformation();
    }

    private void SetupPlayerVariables()
    {
        playerDead = false;
        playerHealth = 10;
        playerRow = 0;
        playerColumn = 0;
        playerFloor = 0;
    }

    private void StartingMonologue()
    {
        Debug.Log("A gnome told you long ago of a treasure, guarded by a powerful dragon, on floor 30 of this dungeon. You have decided to make your way, slowly but surely, down the 30 floors. Good luck.");
        Debug.Log("Press Q for a list of commands.");
    }

    /// <summary>
    /// procedurally generate the dungeon when we enter the next floor
    /// </summary>
    private void SetupNextFloor()
    {
        //create dimensions. for now, 4x4
        int rows = 4;
        int cols = 4;
        dungeon.Add(new Room[rows, cols]);

        List<Room> roomsToUse = new()
        {
            teleporterStartRoom,
            teleporterEndRoom
        };

        //add rooms to fill roomsToUse until we have rows * cols rooms. -1 for starting tile
        // -2 is for start tile which must be a simple tile and exit tile which must be in last col
        // -roomsToUse.Count is for other presets already in the list
        for (int i = 0; i < ((rows * cols) - 4); i++)
        {
            roomsToUse.Add(GetReusableRoom());
        }

        int exitRowIndex = Random.Range(0, rows);
        int firstTeleporterRow = FAILED_TELEPORT_COORDINATE;
        int firstTeleporterCol = FAILED_TELEPORT_COORDINATE;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (row == 0 && col == 0)
                {
                    dungeon[playerFloor][row, col] = startRoom;
                }
                else if (row == exitRowIndex && col == cols - 1)
                {
                    dungeon[playerFloor][row, col] = exitRoom;
                } 
                else
                {
                    int placeMeIndex = Random.Range(0, roomsToUse.Count);
                    Room placeMe = roomsToUse[placeMeIndex];

                    if (placeMe.Type == TileType.Teleporter)
                    {
                        //we haven't place a teleporter yet. set the coords
                        if (firstTeleporterRow == FAILED_TELEPORT_COORDINATE && firstTeleporterCol == FAILED_TELEPORT_COORDINATE)
                        {
                            firstTeleporterRow = row;
                            firstTeleporterCol = col;
                        } else
                        {
                            dungeon[playerFloor][firstTeleporterRow, firstTeleporterCol].TeleportToRow = row;
                            dungeon[playerFloor][firstTeleporterRow, firstTeleporterCol].TeleportToColumn = col;
                            placeMe.TeleportToRow = firstTeleporterRow;
                            placeMe.TeleportToColumn = firstTeleporterCol;
                        }
                    }

                    dungeon[playerFloor][row, col] = placeMe;
                    roomsToUse.RemoveAt(placeMeIndex);
                }
            }
        }
    }

    private Room GetReusableRoom()
    {
        return reusableRooms[Random.Range(0, reusableRooms.Length)];
    }

    private int GetDungeonUpperRowBound()
    {
        return dungeon[playerFloor].GetLength(0);
    }

    private int GetDungeonUpperColumnBound()
    {
        return dungeon[playerFloor].GetLength(1);
    }

    /// <summary>
    /// Guarantee that there are an even number of teleporters in the level
    /// </summary>
    private void VerifyTeleportersMatch()
    {
        int numberOfTeleporters = 0;
        foreach (Room room in dungeon[playerFloor])
        {
            if (room.Type == TileType.Teleporter)
            {
                numberOfTeleporters++;
            }
        }
        if (numberOfTeleporters % 2 != 0)
        {
            Debug.LogError("Invalid number of teleporters! Expected dungeon grid to have an even number to function properly. Found: " + numberOfTeleporters);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDead)
        {
            //only respond to new game request
            if (Input.GetButtonDown("Respawn"))
            {
                NewGame();
            }
            return;
        }

        bool wasKeyPressed = HandleInput(out int newRow, out int newColumn);
        if (!wasKeyPressed)
        {
            return;
        }

        SetPlayerPosition(newRow, newColumn);
        OutputTileInformation();
    }

    /// <summary>
    /// player has entered a tile where there is an enemy. this should happen every time the player enters the tile
    /// </summary>
    private void EncounterEnemy()
    {
        PlayerTakeDamage(enemyDamage);
    }

    /// <summary>
    /// player takes some amount of damage and potentially dies
    /// </summary>
    /// <param name="damage"></param>
    private void PlayerTakeDamage(int damage)
    {
        playerHealth -= damage;
        Debug.Log("you got hit. your health is now " + playerHealth);
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("you are dead!");
            Debug.Log("GAME OVER");
            Debug.Log("PRESS N FOR NEW GAME");
        }
    }

    /// <summary>
    /// player has entered a tile where there is an item. this should happen once, the first time we visit the tile
    /// </summary>
    private void PickupItem()
    {
        PlayerHeal(itemHealAmount);
    }

    /// <summary>
    /// there is no max health, so this will add health to the player no matter what.
    /// </summary>
    /// <param name="healAmount"></param>
    private void PlayerHeal(int healAmount)
    {
        playerHealth += healAmount;
        Debug.Log("you got healed. health is now " + playerHealth);
    }

    /// <summary>
    /// print which tile we are in, as well as long description if we have not visited before
    /// </summary>
    private void OutputTileInformation()
    {
        Debug.Log("You are in: " + GetCurrentLocation().Name);

        switch (GetCurrentLocation().Type)
        {
            case TileType.Empty:
                Debug.Log("There is nothing here.");
                break;
            case TileType.Enemy:
                Debug.Log("Oooo a spooky ghost");
                EncounterEnemy();
                break;
            case TileType.Item:
                if (GetCurrentLocation().WasVisited)
                {
                    Debug.Log("You have already collected the item that was here.");
                } 
                else
                {
                    Debug.Log("You see a shiny object");
                    PickupItem();
                }
                break;
            case TileType.Exit:
                Debug.Log("You see a way out");
                break;
            case TileType.Blockade:
                Debug.LogError("Player should not be able to access this Blockade TileType");
                break;
            case TileType.Teleporter:
                Debug.Log("You see a teleporter");
                break;
            default:
                Debug.LogError("Invalid TileType");
                break;
        }

        if (!GetCurrentLocation().WasVisited)
        {
            Look();
            GetCurrentLocation().WasVisited = true;
        }
    }

    /// <summary>
    /// set's a player's grid position in our 2d dungeon, making sure the new position is within the correct bounds
    /// </summary>
    /// <param name="newRow"></param>
    /// <param name="newColumn"></param>
    private void SetPlayerPosition(int newRow, int newColumn)
    {
        if (IsInBounds(newRow, newColumn, out string errorMessage))
        {
            playerRow = newRow;
            playerColumn = newColumn;
        }
        else
        {
            Debug.Log(errorMessage);
        }
    }

    /// <summary>
    /// is this tile in bounds of our 2d dungeon grid?
    /// </summary>
    /// <param name="newRow">attempted row position</param>
    /// <param name="newColumn">attempted column position</param>
    /// <param name="errorMessage">message to display if we are attempting to travel out of bounds</param>
    /// <returns>true if we are allowed to travel to that new tile</returns>
    private bool IsInBounds(int newRow, int newColumn, out string errorMessage)
    {
        if (newRow == FAILED_TELEPORT_COORDINATE && newColumn == FAILED_TELEPORT_COORDINATE)
        {
            errorMessage = "Can't teleport here! Find a teleporter!";
            return false;
        }
        else if (newRow < DUNGEON_LOWER_ROW_BOUND || newRow >= GetDungeonUpperRowBound() || newColumn < DUNGEON_LOWER_COLUMN_BOUND || newColumn >= GetDungeonUpperColumnBound())
        {
            errorMessage = "Can't go that way!";
            return false;
        } 
        else if (dungeon[playerFloor][newRow, newColumn].Type == TileType.Blockade)
        {
            //we never are allowed to access a Blockade tile, so let's display its description when we attempt to travel to it and fail
            errorMessage = dungeon[playerFloor][newRow, newColumn].Description;
            return false;
        }
        else
        {
            errorMessage = "";
            return true;
        }

    }

    /// <summary>
    /// Handles player's input and sets potential new position in the dungeon array
    /// </summary>
    /// <param name="newRow">new row position</param>
    /// <param name="newColumn">new column position</param>
    /// <returns>true if a key (that we care about) was pressed this frame</returns>
    private bool HandleInput(out int newRow, out int newColumn)
    {
        bool hasPressedKey = true;
        newRow = playerRow;
        newColumn = playerColumn;

        if (Input.GetButtonDown("Down"))
        {
            newRow++;
        }
        else if (Input.GetButtonDown("Up"))
        {
            newRow--;
        }
        else if (Input.GetButtonDown("Right"))
        {
            newColumn++;
        }
        else if (Input.GetButtonDown("Left"))
        {
            newColumn--;
        }
        else if (Input.GetButtonDown("Look"))
        {
            Look();
        }
        else if (Input.GetButtonDown("Help"))
        {
            OutputHelp();
        }
        else if (Input.GetButtonDown("Teleport"))
        {
            Teleport(out newRow, out newColumn);
        }
        else if (Input.GetButtonDown("Ascend"))
        {
            Ascend(out newRow, out newColumn);
        }
        else if (Input.GetButtonDown("Descend"))
        {
            Descend(out newRow, out newColumn);
        }
        else
        {
            hasPressedKey = false;
        }

        return hasPressedKey;
    }

    /// <summary>
    /// move up 1 floor in the dungeon
    /// </summary>
    private void Ascend(out int newRow, out int newColumn)
    {
        if (playerFloor > 0)
        {
            playerFloor--;
        }
        newRow = 0;
        newColumn = 0;
    }

    /// <summary>
    /// move down 1 floor in the dungeon
    /// </summary>
    private void Descend(out int newRow, out int newColumn)
    {
        //must be in tile
        if (GetCurrentLocation().Type == TileType.Exit)
        {
            playerFloor++;
            if (dungeon.Count <= playerFloor)
            {
                SetupNextFloor();
            }
            newRow = 0;
            newColumn = 0;
        } 
        else
        {
            Debug.Log("Can only descend on an exit!");
            newRow = playerRow;
            newColumn = playerColumn;
        }
    }

    /// <summary>
    /// output the long-form description of the room we are currently in
    /// </summary>
    private void Look()
    {
        Debug.Log($"(currently on floor {playerFloor}, row {playerRow}, column {playerColumn})");
        Debug.Log(GetCurrentLocation().Description);
    }

    /// <summary>
    /// display a help message with all available commands / button presses
    /// </summary>
    private void OutputHelp()
    {
        Debug.Log("Use WASD or arrow keys to move.");
        Debug.Log("Use E to Examine your current room.");
        Debug.Log("Use T to Teleport (when on a teleporter only).");
        Debug.Log("Use Z to Descend (when on an exit)");
        Debug.Log("Use X to Ascend anytime.");
        Debug.Log("Use Q to display this help message again.");
        Debug.Log("Onwards, adventurer!");
    }

    /// <summary>
    /// return a reference to the room the player is currently located in
    /// </summary>
    /// <returns></returns>
    private ref Room GetCurrentLocation()
    {
        return ref dungeon[playerFloor][playerRow, playerColumn];
    }

    /// <summary>
    /// attempt to teleport. if this square is not a teleporter, we should not move.
    /// </summary>
    /// <param name="newRow"></param>
    /// <param name="newColumn"></param>
    private void Teleport(out int newRow, out int newColumn)
    {
        if (GetCurrentLocation().Type == TileType.Teleporter)
        {
            newRow = GetCurrentLocation().TeleportToRow;
            newColumn = GetCurrentLocation().TeleportToColumn;
        } else
        {
            newRow = FAILED_TELEPORT_COORDINATE;
            newColumn = FAILED_TELEPORT_COORDINATE;
        }
    }
}
