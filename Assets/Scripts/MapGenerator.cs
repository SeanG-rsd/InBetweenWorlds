using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile[] tiles;
    [SerializeField] private Tile[] earthTiles;
    [SerializeField] private Tile[] moonTiles;
    [SerializeField] private GameObject player;
    [SerializeField] private int floorBuffer;

    [Header("GenerationInfo")]
    [SerializeField] private int maxHeightOfMap;
    [SerializeField] private int roomLength;
    [SerializeField] private int minPlatformHeight;
    private int gapBetweenPlatforms = 2;

    [SerializeField] private int swappersPerRoom;
    [SerializeField] private GameObject swapperPrefab;
    [SerializeField] private int keysInMoonRoom;
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private int maxEnemiesPerRoom;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private int roomGap;

    [Header("---Coin---")]
    [SerializeField] private GameObject coinPrefab;
    [Range(0, 3)]
    [SerializeField] private int minNumberOfCoins;

    [Range(3, 10)]
    [SerializeField] private int maxNumberOfCoins;


    [Header("---Rooms---")]
    [SerializeField] private int chanceOfThreeLongPlatform;
    private int nextUpdatePos;
    [SerializeField] private int roomBuffer;

    [SerializeField] private Transform objectParent;

    bool gameHasStarted;

    public int biggestX;
    public int biggestY;

    public enum World
    {
        Earth,
        Moon
    }

    private void Update()
    {
        int playerX = (int)player.transform.position.x;

        for (int i = playerX - floorBuffer; i <= playerX + floorBuffer; i++)
        {
            tilemap.SetTile(new Vector3Int(i, -1, 0), tiles[0]);
            tilemap.SetTile(new Vector3Int(i, 0, 0), tiles[1]);
            if (i > biggestX)
            {
                biggestX = i;
            }
        }

        if (gameHasStarted)
        {
            if (playerX + (roomLength * roomBuffer) > nextUpdatePos)
            {
                int offset = nextUpdatePos + (nextUpdatePos / roomLength * roomGap);
                GameObject earthSwapper = GenerateRoom(offset, World.Earth, true);
                GameObject door = Instantiate(doorPrefab, new Vector3(offset - (roomGap / 2), 4.9f, 0), Quaternion.identity);
                GameObject moonSwapper = GenerateRoom(offset, World.Moon, true);
                GameObject moonDoor = Instantiate(doorPrefab, new Vector3(offset - (roomGap / 2), -4.9f, 0), Quaternion.identity);
                moonDoor.transform.Rotate(180, 0, 0);

                earthSwapper.transform.SetParent(objectParent);
                door.transform.SetParent(objectParent);
                moonSwapper.transform.SetParent(objectParent);
                moonDoor.transform.SetParent(objectParent);

                nextUpdatePos += roomLength;

                earthSwapper.GetComponent<Swapper>().SetDestinationSwapper(moonSwapper);
                moonSwapper.GetComponent<Swapper>().SetDestinationSwapper(earthSwapper);

                door.GetComponent<Door>().SetOpposite(moonDoor);
                moonDoor.GetComponent<Door>().SetOpposite(door);
            }
        }
    }

    private void Awake()
    {
        GameManager.OnStartGame += HandleStartGame;
        GameManager.OnGenerateMap += HandleStartGame;
        RewardedAd.OnLeaveContinueScreen += HandlePlayerDeath;
    }

    private void OnDestroy()
    {
        GameManager.OnStartGame -= HandleStartGame;
        GameManager.OnGenerateMap -= HandleStartGame;
        RewardedAd.OnLeaveContinueScreen -= HandlePlayerDeath;
    }

    private void HandleStartGame()
    {
        if (!gameHasStarted)
        {
            nextUpdatePos = 0;
            gameHasStarted = true;
            biggestY = maxHeightOfMap;
            GameObject earthSwapper = GenerateRoom(nextUpdatePos / roomLength * roomGap, World.Earth, false);
            GameObject moonSwapper = GenerateRoom(nextUpdatePos / roomLength * roomGap, World.Moon, false);

            earthSwapper.transform.SetParent(objectParent);
            moonSwapper.transform.SetParent(objectParent);

            earthSwapper.GetComponent<Swapper>().SetDestinationSwapper(moonSwapper);
            moonSwapper.GetComponent<Swapper>().SetDestinationSwapper(earthSwapper);

            nextUpdatePos = roomLength;
        }
    }

    private void HandlePlayerDeath()
    {
        gameHasStarted = false;
        ResetTileMap();
    }

    private void ResetTileMap()
    {
        for (int horiztonal = 0; horiztonal < biggestX; horiztonal++)
        {
            for (int vertical = -biggestY - 1; vertical < biggestY; vertical++)
            {
                tilemap.SetTile(new Vector3Int(horiztonal, vertical, 0), null);
            }
        }

        for (int index = 0; index < objectParent.childCount; ++index)
        {
            Destroy(objectParent.GetChild(index).gameObject);
        }
    }

    private GameObject GenerateRoom(int offset, World world, bool hasEnemies)
    {
        bool[,] platforms = new bool[roomLength, maxHeightOfMap];

        List<Vector3Int> possiblePlatformPositions = new List<Vector3Int>();
        List<Vector3Int> possibleCoinPositions = new List<Vector3Int>();

        for (int columns = minPlatformHeight - 1; columns < maxHeightOfMap; columns++)
        {
            for (int rows = 0; rows < roomLength; rows++)
            {
                Vector3Int next = new Vector3Int(rows, columns, 0);
                possiblePlatformPositions.Add(next);
            }
        }

        

        Vector3Int check = new Vector3Int();
        List<Platform> platformObjects = new List<Platform>();

        while (check.y != minPlatformHeight)
        {
            possiblePlatformPositions = possiblePlatformPositions.OrderBy(x => Random.value).ToList();
            check = possiblePlatformPositions[0];
        }
        bool first = true;

        while (possiblePlatformPositions.Count > 0)
        {
            if (!first)
            {
                possiblePlatformPositions = possiblePlatformPositions.OrderBy(x => Random.value).ToList();
                check = possiblePlatformPositions[0];

            }
            else
            {
                first = false;
            }

            RemoveAroundBlock(possiblePlatformPositions, check);

            possiblePlatformPositions.Remove(check);

            if (IsWithinPlatformRange(check.x, check.y))
            {
                Platform newPlatform = new Platform();
                platforms[check.x, check.y] = true;
                newPlatform.AddTileToPlatform(new Vector3Int(check.x, check.y, 0));
                possibleCoinPositions.Add(check);
                int randIndex = Random.Range(0, 100);
                int numberMore = randIndex < chanceOfThreeLongPlatform ? 2 : 1;
                for (int index = 0; index < numberMore; ++index)
                {
                    int leftOrRight = Random.Range(0, 2) == 0 ? -1 : 1; // left is -1, right is 1
                    while (true)
                    {
                        Vector3Int checkPos = new Vector3Int(check.x + leftOrRight, check.y, 0);
                        if (IsWithinPlatformRange(checkPos.x, checkPos.y))
                        {
                            RemoveAroundBlock(possiblePlatformPositions, checkPos);
                            possibleCoinPositions.Add(checkPos);
                            platforms[checkPos.x, checkPos.y] = true;
                            newPlatform.AddTileToPlatform(new Vector3Int(checkPos.x, checkPos.y, 0));
                            break;
                        }
                        leftOrRight *= -1;
                    }
                }

                if (newPlatform.GetRightBlock().x > biggestX)
                {
                    biggestX = newPlatform.GetRightBlock().x;
                }

                platformObjects.Add(newPlatform);
            }
        }

        for (int rows = 0; rows < roomLength; rows++)
        {
            for (int columns = 0; columns < maxHeightOfMap; columns++)
            {
                Vector3Int currentCheckingPos = new Vector3Int(rows + offset, world == 0 ? columns : (columns + 1) * -1, 0);
                if (platforms[rows, columns])
                {
                    tilemap.SetTile(currentCheckingPos, world == World.Earth ? tiles[1] : tiles[0]);
                }
                else
                {
                    tilemap.SetTile(currentCheckingPos, null);
                }
            }
        }

        int numEnemies = Random.Range(1, maxEnemiesPerRoom);
        int currentEnemies = 0;

        foreach (Platform p in platformObjects) // set edge tiles of platforms
        {
            Vector3Int left = p.GetLeftBlock();
            Vector3Int right = p.GetRightBlock();
            tilemap.SetTile(new Vector3Int(left.x + offset, world == World.Earth ? left.y : (left.y + 1) * -1, 0), world == World.Earth ? earthTiles[0] : moonTiles[0]);
            tilemap.SetTile(new Vector3Int(right.x + offset, world == World.Earth ? right.y : (right.y + 1) * -1, 0), world == World.Earth ? earthTiles[1] : moonTiles[1]);
        }

        if (hasEnemies) // enemies
        {
            List<Platform> usedEnemyPlatforms = new List<Platform>();
            while (currentEnemies < numEnemies)
            {
                Platform p = platformObjects[Random.Range(0, platformObjects.Count)];

                while (usedEnemyPlatforms.Contains(p))
                {
                    p = platformObjects[Random.Range(0, platformObjects.Count)];
                }

                usedEnemyPlatforms.Add(p);

                Vector3Int position = p.GetRandomBlock();
                GameObject enemy = Instantiate(enemyPrefab, new Vector3(position.x + offset + 0.5f, (position.y + 1) * -1 - 0.5f, 0), Quaternion.identity);
                enemy.transform.SetParent(objectParent);
                enemy.transform.Rotate(0, 0, 180);

                currentEnemies++;
            }
        }

        Vector3Int keyPos = new Vector3Int();
        if (world == World.Moon)
        {
            Platform p = platformObjects[Random.Range(0, platformObjects.Count)];
            keyPos = p.GetRandomBlock();
            GameObject key = Instantiate(keyPrefab, new Vector3(keyPos.x + offset + 0.5f, (keyPos.y + 1) * -1 - 0.5f, 0), Quaternion.identity);
            key.transform.SetParent(objectParent);
        }

        Vector3Int swapperPos = keyPos;

        while (swapperPos == keyPos)
        {
            Platform swapperPlatform = platformObjects[Random.Range(0, platformObjects.Count)];
            swapperPos = swapperPlatform.GetRandomBlock();
            possibleCoinPositions.Remove(swapperPos);
        }

        GameObject swapper = Instantiate(swapperPrefab, new Vector3(swapperPos.x + offset + 0.5f, world == World.Earth ? swapperPos.y + 1.5f : (swapperPos.y + 1) * -1 - 0.5f, 0), Quaternion.identity);
        if (world == World.Moon)
        {
            swapper.transform.Rotate(0, 0, 180);
        }

        int numCoins = Random.Range(minNumberOfCoins, maxNumberOfCoins);
        int currentCoins = 0;
        possibleCoinPositions = possibleCoinPositions.OrderBy(x => Random.value).ToList();

        while (currentCoins < numCoins && possibleCoinPositions.Count >= currentCoins)
        {
            GameObject coin = Instantiate(coinPrefab, new Vector3(possibleCoinPositions[currentCoins].x + offset + 0.5f, world == World.Earth ? possibleCoinPositions[currentCoins].y + 1.5f : (possibleCoinPositions[currentCoins].y + 1.5f) * -1, possibleCoinPositions[currentCoins].z), Quaternion.identity);
            coin.transform.SetParent(objectParent);

            currentCoins++;
        }

        return swapper;
    }

    private void RemoveAroundBlock(List<Vector3Int> possiblePlatformPositions, Vector3Int check)
    {
        for (int columns = check.y - gapBetweenPlatforms; columns <= check.y + gapBetweenPlatforms; ++columns)
        {
            for (int rows = check.x - gapBetweenPlatforms; rows <= check.x + gapBetweenPlatforms; ++rows)
            {
                possiblePlatformPositions.Remove(new Vector3Int(rows, columns, 0));
            }
        }
    }

    private bool IsWithinPlatformRange(int column, int row)
    {
        return row >= minPlatformHeight && row < maxHeightOfMap && column >= 0 && column < roomLength;
    }
}
