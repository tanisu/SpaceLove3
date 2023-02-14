using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallAlien : MonoBehaviour
{
    public enum DIRECTION
    {
        TOP,
        RIGHT,
        DOWN,
        LEFT,
        DIRECTION_MAX
    }

    public DIRECTION direction;
    public Vector2Int currentPos, nextPos, startPos;

    int[,] move = {
      { 0, -1 },
      { 1, 0 },
      { 0, 1 },
      { -1, 0 }
    };
    List<DIRECTION> directions = new List<DIRECTION>();

    private void Start()
    {
        startPos = currentPos;
        InvokeRepeating("Move", 2f, 0.25f);
    }



    public MapGenerator mapGenerator;
    public void Move()
    {
        if (GameManager.I.isStop) return;
        if (GameManager.I.state != GameManager.GAME_STATE.PLAY) return;
        int d = ((int)direction + 2) % (int)DIRECTION.DIRECTION_MAX;
        DIRECTION oppsit = (DIRECTION)Enum.ToObject(typeof(DIRECTION), d);
        for (int i = 0; i < (int)DIRECTION.DIRECTION_MAX; i++)
        {
            if (i == d)
            {
                continue;
            }
            Vector2Int tmpPos = currentPos + new Vector2Int(move[i, 0], move[i, 1]);
            if (mapGenerator.GetNextMapType(tmpPos) == MapGenerator.MAP_TYPE.GROUND)
            {
                directions.Add((DIRECTION)Enum.ToObject(typeof(DIRECTION), i));
            }
        }

        if (directions.Count == 0)
        {
            nextPos = currentPos + new Vector2Int(move[(int)oppsit, 0], move[(int)oppsit, 1]);
            direction = oppsit;
        }
        else
        {
            int idx = UnityEngine.Random.Range(0, directions.Count - 1);
            direction = directions[idx];

            nextPos = currentPos + new Vector2Int(move[(int)direction, 0], move[(int)direction, 1]);
        }
        if (nextPos.x < 0 || nextPos.y < 0 || nextPos.y > mapGenerator.h - 1 || nextPos.x > mapGenerator.w - 1) return;
        if (mapGenerator.GetNextMapType(nextPos) == MapGenerator.MAP_TYPE.WALL)
        {
            nextPos = currentPos;
        }
        transform.localPosition = mapGenerator.ScreenPos(nextPos);
        currentPos = nextPos;
        directions.Clear();
        mapGenerator.CheckAliens(currentPos);
    }

}
