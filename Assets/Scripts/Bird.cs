using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : Alien
{

    int[,] move = {
      { 0, -1 },
      { 1, 0 },
      { 0, 1 },
      { -1, 0 }
    };
    
    
    void Start()
    {
        startPos = currentPos;
    }

    new public void Move()
    {
        nextPos = currentPos + new Vector2Int(move[(int)direction, 0], move[(int)direction, 1]);
        if (nextPos.x < 0 || nextPos.y < 0 || nextPos.y > mapGenerator.h - 1 || nextPos.x > mapGenerator.w - 1) {
            gameObject.SetActive(false);
            nextPos = currentPos;
            return;
        }
        transform.localPosition = mapGenerator.ScreenPos(nextPos);
        if(mapGenerator.GetNextMapType(nextPos) == MapGenerator.MAP_TYPE.GROUND && !mapGenerator.player.isDead)
        {
            mapGenerator.TracePlayer(nextPos);
        }
        
        currentPos = nextPos;
        mapGenerator.CheckAliens(currentPos);
    }
    
}
