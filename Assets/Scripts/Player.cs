using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
public class Player : MonoBehaviour
{
    public enum DIRECTION
    {
        TOP,
        RIGHT,
        DOWN,
        LEFT
    }

    public DIRECTION direction;
    public Vector2Int currentPos,nextPos,startPos;
    int[,] move = {
      { 0, -1 },
      { 1, 0 }, 
      { 0, 1 }, 
      { -1, 0 } 
    };
    public MapGenerator mapGenerator;
    [SerializeField]Cursor[] cursors;
    public bool isDead,canPut;
    public UnityAction<string> GetItem ;
    
    void Start()
    {
        startPos = currentPos;
        direction = DIRECTION.DOWN;
    }



    public void Move(int _idx)
    {

        direction = (DIRECTION)Enum.ToObject(typeof(DIRECTION), _idx);
        nextPos = currentPos + new Vector2Int(move[(int)direction, 0], move[(int)direction, 1]);
        if (nextPos.x < 0 || nextPos.y < 0 || nextPos.y > mapGenerator.h - 1 || nextPos.x > mapGenerator.w - 1) return;

        transform.localPosition = mapGenerator.ScreenPos(nextPos);
        mapGenerator.TracePlayer(currentPos);
        if (mapGenerator.GetNextMapType(nextPos) == MapGenerator.MAP_TYPE.WALL)
        {
            mapGenerator.InTheWall();
        }
        else
        {
            SoundManager.I.PlaySE(SESoundData.SE.WALK);
        }
        currentPos = nextPos;

        mapGenerator.CheckPlayerMoved(currentPos);
        if (!isDead)
        {
            mapGenerator.MoveAliens();
        }
        
    }

    public void ResetPlayer()
    {
        
        currentPos = startPos;
        transform.localPosition = mapGenerator.ScreenPos(currentPos);
    }

    public void ShowCursor()
    {
        bool cursorActive = false;
        foreach(DIRECTION d in Enum.GetValues(typeof(DIRECTION)))
        {
            Vector2Int targetPos = currentPos + new Vector2Int(move[(int)d, 0], move[(int)d, 1]);
            if (targetPos.x < 0 || targetPos.y < 0 || targetPos.y > mapGenerator.h - 1 || targetPos.x > mapGenerator.w -1) continue;
            if (mapGenerator.GetNextMapType(targetPos) == MapGenerator.MAP_TYPE.WALL)
            {
                cursors[(int)d].gameObject.SetActive(true);
                cursorActive = true;
            }
        }
        if (!cursorActive)
        {
            GameManager.I.CancelMode();
        }
    }

    public void HideCursor()
    {
        foreach(Cursor c in cursors)
        {
            c.gameObject.SetActive(false);
        }
    }

    public void PutGround(int _idx)
    {
        direction = (DIRECTION)Enum.ToObject(typeof(DIRECTION), _idx);
        Vector2Int pos = currentPos + new Vector2Int(move[(int)direction, 0], move[(int)direction, 1]);
        if (pos.x < 0 || pos.y < 0 || pos.y > mapGenerator.h - 1 || pos.x > mapGenerator.w - 1) return;
        if (mapGenerator.GetNextMapType(pos) == MapGenerator.MAP_TYPE.WALL)
        {
            mapGenerator.PutGround(pos);
            canPut = true;
        }
 
        
        HideCursor();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Life"))
        {
            collision.gameObject.SetActive(false);
            GetItem?.Invoke(collision.tag);
        }
        if (collision.CompareTag("Block"))
        {
            collision.gameObject.SetActive(false);
            GetItem?.Invoke(collision.tag);
        }

    }
}
