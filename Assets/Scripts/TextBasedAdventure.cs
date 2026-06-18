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

    private static Room[,] dungeon =
    {
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
            new Room { Name = "Red Teleporter #1",
                       Type = TileType.Teleporter,
                       Description = "A large steel teleporter glows and hums and crackles with red electricity.",
                       TeleportToRow = 3,
                       TeleportToColumn = 0
                     },
            new Room { Name = "Very Empty Room",
                       Type = TileType.Empty,
                       Description = "There is absolutely nothing here. Staring into the void stings your eyes and eats at your soul. Better move on quickly.",
                     },
        },

        {
            new Room { Name = "Bone Chamber",
                       Type = TileType.Enemy,
                       Description = "This chamber is enormous. The stalactites and stalagmites look jagged and bone-like, and you hear the rattling of Skeletons and their makeshift bone weapons around the corner.",
                     },
            new Room { Name = "Flooded Hall",
                       Type = TileType.Empty,
                       Description = "Deep pools of water fill this once-marvelous hall. Ragged banners and rusty shields hang awkwardly on the walls.",
                     },
            new Room { Name = "Iron Gate",
                       Type = TileType.Exit,
                       Description = "There is an imposing steel gate up ahead that is slightly ajar. It looks incredibly heavy, but you might be able to squeeze through the opening.",
                     },
            new Room { Name = "Very Empty Room",
                       Type = TileType.Empty,
                       Description = "There is absolutely nothing here. Staring into the void stings your eyes and eats at your soul. Better move on quickly.",
                     },
            new Room { Name = "Raven's Hoard",
                       Type = TileType.Item,
                       Description = "Shiny items litter the floor. So many they hurt your eyes a bit. Better pick just one and move on before the raven returns.",
                     },
        },

        {
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
        },


        {
            new Room { Name = "Red Teleporter #2",
                       Type = TileType.Teleporter,
                       Description = "A large steel teleporter glows and hums and crackles with red electricity.",
                       TeleportToRow = 0,
                       TeleportToColumn = 3
                     },
            new Room { Name = "Impassable Wall",
                       Type = TileType.Blockade,
                       Description = "A sturdy wall blocks your path.",
                     },
            new Room { Name = "Fountain of Youth",
                       Type = TileType.Item,
                       Description = "A rejuvenating spring makes you feel ten years younger. Ahhhhhh. Those bubbles are nice.",
                     },
            new Room { Name = "Fountain of Decrepitude",
                       Type = TileType.Enemy,
                       Description = "The brackish water looks and smells terrible. You are not sure why, but you touch a finger to it and immediately regret it. Ow.",
                     },
            new Room { Name = "Impassable Sheet of Ice",
                       Type = TileType.Blockade,
                       Description = "A huge impenetrable wall of ice stands between you and your destination. No getting through here.",
                     },
        },

                {
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
            new Room { Name = "Impassable Iron Wall",
                       Type = TileType.Blockade,
                       Description = "This iron wall looks like it was constructed by very serious dwarven blacksmiths. There is absolutely no way you are getting past.",
                     },
        },

    };

    private int DUNGEON_LOWER_ROW_BOUND = 0;
    private int DUNGEON_UPPER_ROW_BOUND = dungeon.GetLength(0);
    private int DUNGEON_LOWER_COLUMN_BOUND = 0;
    private int DUNGEON_UPPER_COLUMN_BOUND = dungeon.GetLength(1);
    private int FAILED_TELEPORT_COORDINATE = -1;

    private int playerRow = 0;
    private int playerColumn = 0;
    private int playerHealth = 10;
    private int enemyDamage = 1;
    private int itemHealAmount = 2;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VerifyTeleportersMatch();
        WelcomeMonologue();
        OutputHelp();
        OutputTileInformation();
    }

    private void WelcomeMonologue()
    {
        Debug.Log("A gnome told you long ago of a treasure, guarded by a powerful dragon, on floor 30 of this dungeon.");
        Debug.Log("You have decided to make your way, slowly but surely, down the 30 floors. Good luck.");
        Debug.Log("Welcome to the Dungeon.");
    }

    /// <summary>
    /// Guarantee that there are an even number of teleporters in the level
    /// </summary>
    private void VerifyTeleportersMatch()
    {
        int numberOfTeleporters = 0;
        foreach (Room room in dungeon)
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
            Debug.Log("you are dead");
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
        else if (newRow < DUNGEON_LOWER_ROW_BOUND || newRow >= DUNGEON_UPPER_ROW_BOUND || newColumn < DUNGEON_LOWER_COLUMN_BOUND || newColumn >= DUNGEON_UPPER_COLUMN_BOUND)
        {
            errorMessage = "Can't go that way!";
            return false;
        } 
        else if (dungeon[newRow, newColumn].Type == TileType.Blockade)
        {
            //we never are allowed to access a Blockade tile, so let's display its description when we attempt to travel to it and fail
            errorMessage = dungeon[newRow, newColumn].Description;
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
        else
        {
            hasPressedKey = false;
        }

        return hasPressedKey;
    }

    /// <summary>
    /// output the long-form description of the room we are currently in
    /// </summary>
    private void Look()
    {
        Debug.Log(GetCurrentLocation().Description);
    }

    /// <summary>
    /// display a help message with all available commands / button presses
    /// </summary>
    private void OutputHelp()
    {
        Debug.Log("Use WASD or arrow keys to move, E to look around your current room, T to Teleport, and Q to display this help message again.");
        Debug.Log("Onwards, adventurer!");
    }

    /// <summary>
    /// return a reference to the room the player is currently located in
    /// </summary>
    /// <returns></returns>
    private ref Room GetCurrentLocation()
    {
        return ref dungeon[playerRow, playerColumn];
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
