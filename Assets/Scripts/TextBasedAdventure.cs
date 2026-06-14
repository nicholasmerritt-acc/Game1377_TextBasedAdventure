using UnityEngine;

public class TextBasedAdventure : MonoBehaviour
{
    [System.Serializable]
    public struct Room
    {
        public string Name;
        public TileType Type;
        public string Description;
        public bool Visited;
    }

    //dont need to do this if we don't want to see in inspector
    //public struct RoomRow
    //{
    //    public Room[] rooms;
    //}

    //[Tooltip("Rooms the player can explore")]
    //public RoomRow[] dungeon;

    public enum TileType
    {
        Invalid,
        Empty,
        Item,
        Enemy,
        Exit,
    }

    //string[,] tileNames = { { "Dark Cave",      "Mossy Tunnel",     "Crystal Room" },
    //                        { "Bone Chamber",   "Flooded Hall",     "Iron Gate"    },
    //                        { "Goblin Den",     "Armory",           "Throne Room"  }
    //                        };

    //TileType[,] tileTypes = {{ TileType.Empty, TileType.Item,  TileType.Empty},
    //                         { TileType.Enemy, TileType.Empty, TileType.Exit},
    //                         { TileType.Empty, TileType.Enemy, TileType.Item}
    //                        };

    //string[,] tileDescriptions = { { /*dark cave*/      "A mysterious, spooky cave. You hear the chittering of insects and the rattling of bones coming from the depths.", 
    //                                 /*mossy tunnel*/   "A long, narrow tunnel stretches out before you. There is springy green moss all along the floor. If you were barefoot, this room would be very comfy. Probably best to leave your shoes on, though.",
    //                                 /*crystal room*/   "Pink emanates from this room almost aggressively. There are gemstones lining the walls that glow and pulse with an intense energy."
    //                               },
    //                               { /*bone chamber*/   "The chamber is enormous. The stalactites and stalagmites look jagged and bone-like, and one drop of putrid water coming from the ceiling hits you in the eye. Ew.",
    //                                 /*flooded hall*/   "Deep pools of water fill this once-marvelous hall. Ragged banners and rusty shields hang awkwardly on the walls.",
    //                                 /*iron gate*/      "There is an imposing steel gate up ahead that is slightly ajar. It looks incredibly heavy, but you might be able to squeeze through the opening."
    //                               },
    //                               { /*goblin den*/     "Tattered goblin clothing, scraps of fur, and half-eaten food lay scattered around this room. Looks like the goblins left in a hurry. Better get out of here before they come back.",
    //                                 /*armory*/         "Racks of well-organized weapons fill this room. Obviously the goblins are preparing for a fight against a powerful force.",           
    //                                 /*throne room*/    "A large, empty throne looms towards the back of this room. There are several treasure chests flanking it, which must belong to some absent royalty. Better get them quickly and escape before they return."
    //                               }
    //                             };

    private Room[,] dungeon =
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
        },

    };

    bool[,] visited;

    private int playerRow = 0;
    private int playerCol = 0;
    private int playerHealth = 10;
    private int enemyDamage = 1;
    private int itemHealAmount = 2;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OutputTileInformation();
    }

    // Update is called once per frame
    void Update()
    {
        bool keyPressed = HandleInput(out int newRow, out int newCol);
        if (!keyPressed)
        {
            return;
        }

        SetPlayerPosition(newRow, newCol);
        OutputTileInformation();
    }

    //private void SetupGrid()
    //{
    //    visited = new bool[tileTypes.GetLength(0), tileTypes.GetLength(1)];
    //}

    private void EncounterEnemy()
    {
        PlayerTakeDamage(enemyDamage);
    }

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

    private void ItemPickup()
    {
        PlayerHeal(itemHealAmount);
    }

    private void PlayerHeal(int healAmount)
    {
        playerHealth += healAmount;
        Debug.Log("you got healed. health is now " + playerHealth);
    }

    /// <summary>
    /// print which tile we are in
    /// </summary>
    private void OutputTileInformation()
    {
        Debug.Log("You are in: " + dungeon[playerRow,playerCol].Name);

        switch (dungeon[playerRow, playerCol].Type)
        {
            case TileType.Empty:
                Debug.Log("There is nothing here.");
                break;
            case TileType.Enemy:
                Debug.Log("Oooo a spooky ghost");
                EncounterEnemy();
                break;
            case TileType.Item:
                Debug.Log("You see a shiny object");
                ItemPickup();
                break;
            case TileType.Exit:
                Debug.Log("You see a way out");
                break;
            default:
                Debug.LogError("Invalid TileType");
                break;
        }

        if (!dungeon[playerRow, playerCol].Visited)
        {
            Look();
            dungeon[playerRow, playerCol].Visited = true;
        }
    }

    private void SetPlayerPosition(int newRow, int newCol)
    {
        if (IsInBounds(newRow, newCol))
        {
            playerRow = newRow;
            playerCol = newCol;
        }
        else
        {
            Debug.Log("Can't go that way");
        }
    }

    /// <summary>
    /// is this tile in bounds of our 2d dungeon array?
    /// </summary>
    /// <param name="newRow"></param>
    /// <param name="newCol"></param>
    /// <returns></returns>
    private bool IsInBounds(int newRow, int newCol)
    {
        return newRow >= 0 && newRow < dungeon.GetLength(0) && newCol >= 0 && newCol < dungeon.GetLength(1);
    }

    /// <summary>
    /// Handles player's input and sets potential new position in the filenames array
    /// </summary>
    /// <param name="newRow1">new row position</param>
    /// <param name="newCol1">new col position</param>
    /// <returns>true if a key (that we care about) was pressed this frame</returns>
    private bool HandleInput(out int newRow1, out int newCol1)
    {
        bool hasPressedKey = true;
        newRow1 = playerRow;
        newCol1 = playerCol;

        if (Input.GetKeyDown(KeyCode.S))
        {
            newRow1++;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            newRow1--;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            newCol1++;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            newCol1--;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Look();
        }
        else
        {
            hasPressedKey = false;
        }

        return hasPressedKey;
    }


    private void Look()
    {
        Debug.Log(dungeon[playerRow, playerCol].Description);
    }
}
