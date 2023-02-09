using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    
    [SerializeField] GameObject[] prefabs;
    [SerializeField] GameObject goal,life,addBlock;
    [SerializeField] Alien alien;
    [SerializeField] Bird bird,birdV;
    [SerializeField] BallAlien ballAlien;
    [SerializeField] Player playerPrefab;
    [SerializeField] Transform map2D;
    [SerializeField] Sprite wallSprite,groundSprite;
    
    MazeCreator maze;
    int[,] mazeDatas;
    Camera cam;
    public enum MAP_TYPE
    {
        GROUND, //0
        WALL,   //1

    }

    
    MAP_TYPE[,] mapTable;
    
    MapChip[,] mapChip;
    List<Vector2Int> tracePlayer = new List<Vector2Int>();
    List<Alien> aliens = new List<Alien>();
    List<Bird> birds = new List<Bird>();
    List<BallAlien> balls = new List<BallAlien>();
    float mapSize;
    Vector2 centerPos,goalPos;
    Vector2Int itemPos;
    public int w, h;
    int goalRandomPos;

    public Player player;

    public Vector2 currentGoalPos;
    const int MINMAP_LENGTH = 7;

    bool isClear;

    private void Awake()
    {
        GameManager.I.map = this;
    }

    public void InitMap()
    {
        cam = Camera.main;
        
        w = MINMAP_LENGTH + GameManager.I.w;
        h = MINMAP_LENGTH + GameManager.I.h;
        goalRandomPos = Random.Range(0, 4);

        if (w % 2 == 0) w++;
        if (h % 2 == 0) h++;
        _loadMapData();
        _createMap();
        _setPlayer();
        _setGoal();

        isClear = false;
        
        itemPos = Vector2Int.zero;
        _setLife();
        _setAddBlock();
        //_putBird();
        //_putBirdV();
       // PutBallAliens();
    }



    void _loadMapData()
    {
        maze = new MazeCreator(w, h);
        mazeDatas = new int[w, h];
        mazeDatas = maze.CreateMaze();
        int row = mazeDatas.GetLength(1);
        int col = mazeDatas.GetLength(0);
        mapTable = new MAP_TYPE[col, row];
        mapChip = new MapChip[col, row];
        
        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < col; x++)
            {
                if((x == col/2 && y == row / 2) || (x == col / 2 -1 && y == row / 2) || (x == col / 2 +1 && y == row / 2) || (x == col / 2 && y == row / 2 -1 ) || (x == col / 2 && y == row / 2 +1))
                {
                    mapTable[x, y] = MAP_TYPE.GROUND;
                }
                else
                {
                    mapTable[x, y] = (MAP_TYPE)mazeDatas[x, y];
                }
            }
        }

    }

    void _createMap()
    {
        mapSize = prefabs[1].GetComponent<SpriteRenderer>().bounds.size.x;

        if (mapTable.GetLength(0) % 2 == 0)
        {
            centerPos.x = mapTable.GetLength(0) / 2 * mapSize - (mapSize / 2);
        }
        else
        {
            centerPos.x = mapTable.GetLength(0) / 2 * mapSize;
        }

        if (mapTable.GetLength(1) % 2 == 0)
        {
            centerPos.y = mapTable.GetLength(1) / 2 * mapSize - (mapSize / 2);
        }
        else
        {
            centerPos.y = mapTable.GetLength(1) / 2 * mapSize;
        }

        

        for (int y = 0; y < mapTable.GetLength(1); y++)
        {
            for (int x = 0; x < mapTable.GetLength(0); x++)
            {
                
                Vector2Int pos = new Vector2Int(x, y);
                GameObject _wall = Instantiate(prefabs[(int)MAP_TYPE.WALL], map2D);
                _wall.transform.position = ScreenPos(pos);
                if (mapTable[x,y] == MAP_TYPE.GROUND)
                {
                    GameObject _map = Instantiate(prefabs[(int)mapTable[x, y]], map2D);
                    _map.transform.position = ScreenPos(pos);
                    mapChip[x, y] = _map.GetComponent<MapChip>();
                }


            }
        }
    }

    private void _setPlayer()
    {
        int harf = mapTable.GetLength(0) / 2;
        
        Vector2Int pos = new Vector2Int(harf, harf);
        
        
        player = Instantiate(playerPrefab, map2D);
        player.transform.position = ScreenPos(pos);
        cam.transform.SetParent(player.transform);
        cam.transform.localPosition = new Vector3(0, -2f, -10f);
        player.GetComponent<Player>().currentPos = pos;
        player.GetComponent<Player>().mapGenerator = this;
    }

    private void _setAddBlock()
    {
        GameObject _addBlock = Instantiate(addBlock, map2D);
        _putObj(_addBlock);
    }

    private void _setLife()
    {
        GameObject _life = Instantiate(life, map2D);
        _putObj(_life);
        
    }

    private void _putObj(GameObject _obj)
    {
        int y = Random.Range(1, h - 2);
        int x = Random.Range(1, w - 2);
     
        Vector2Int _pos = new Vector2Int(x, y);
        if (GetNextMapType(_pos) != MAP_TYPE.GROUND || player.GetComponent<Player>().currentPos == _pos || goalPos == _pos || itemPos == _pos)
        {
            _putObj(_obj);
        }
        else
        {
            itemPos = new Vector2Int(x, y);
            _obj.transform.position = ScreenPos(itemPos);
        }
    }




    private void _setGoal()
    {

        switch (goalRandomPos)
        {
            case 0:
                PutGoal(new Vector2Int(w - 2, h - 2));
                break;
            case 1:
                PutGoal(new Vector2Int(1, h - 2));
                break;
            case 2:
                PutGoal(new Vector2Int(w - 2, 1));
                break;
            case 3:
                PutGoal(new Vector2Int(1, 1));
                break;
        }
    }

    private void PutGoal(Vector2Int _pos)
    {
        GameObject _goal = Instantiate(goal, map2D);
        _goal.transform.position = ScreenPos(_pos);
        currentGoalPos = _goal.transform.localPosition;
        goalPos = _pos;
    }


    public Vector2 ScreenPos(Vector2Int _pos)
    {
        return new Vector2(
            _pos.x * mapSize - centerPos.x,
            -(_pos.y * mapSize - centerPos.y ));
    }

    public MAP_TYPE GetNextMapType(Vector2Int _pos)
    {
        return mapTable[_pos.x, _pos.y];
    }

    public void PutGround(Vector2Int _pos)
    {
        
        mapTable[_pos.x, _pos.y] = MAP_TYPE.GROUND;
        GameObject _map = Instantiate(prefabs[0], map2D);
        _map.transform.position = ScreenPos(_pos);
        mapChip[_pos.x, _pos.y] = _map.GetComponent<MapChip>();
    }

    public void TracePlayer(Vector2Int _pos)
    {
        mapTable[_pos.x, _pos.y] = MAP_TYPE.WALL;
        mapChip[_pos.x, _pos.y].LostAnim();
        tracePlayer.Add(_pos);
    }

    public void CheckPlayerMoved(Vector2Int _pos)
    {

        if (aliens.Count == 0 || birds.Count == 0)
        {
            if(_pos == goalPos)
            {
                isClear = true;
                SoundManager.I.PlaySE(SESoundData.SE.CLEAR);
                GameManager.I.StageClear();
            }
        }
        else
        {
            foreach (Alien _alien in aliens)
            {
                if (_pos == _alien.currentPos)
                {
                    InTheWall();
                    player.isDead = true;
                }
            }
            foreach(Bird _bird in birds)
            {
                if (_pos == _bird.currentPos)
                {
                    InTheWall();
                    player.isDead = true;
                }
            }
            
        }
        
        
    }

    public void CheckAliens(Vector2Int _pos)
    {
        if(_pos == player.currentPos)
        {
            InTheWall();
        }else if(player.currentPos == goalPos && !isClear)
        {
            isClear = true;
            SoundManager.I.PlaySE(SESoundData.SE.CLEAR);
            GameManager.I.StageClear();
        }
    }

    public void InTheWall()
    {
        GameManager.I.Miss();
        if(GameManager.I.life >= 0)
        {
            StartCoroutine(_resetStage());
        }
        
        
    }

    public void PutBallAliens(int _ballAliens)
    {
        for(int i = 0;i < _ballAliens; i++)
        {
            BallAlien _ball = Instantiate(ballAlien, map2D);
            _ball.mapGenerator = this;
            _getObjPos2(_ball);
            balls.Add(_ball);
        }

    }

    public void PutAlien(int _aliens)
    {
        
        for (int i = 0; i < _aliens; i++)
        {
            Alien _alien = Instantiate(alien, map2D);
            _alien.mapGenerator = this;

            _getObjPos(_alien);
            aliens.Add(_alien);
        }
    }

    public void PutBird(int _birds)
    {
        for(int i = 0;i < _birds; i++)
        {
            Bird _bird = Instantiate(bird, map2D);
            _bird.mapGenerator = this;
            int y = Random.Range(2, h - 2);
            Vector2Int _pos = new Vector2Int(0, y);
            _bird.currentPos = _pos;
            _bird.transform.position = ScreenPos(_pos);
            birds.Add(_bird);
        }

    }
    public void PutBirdV(int _brids)
    {
        for(int i = 0;i < _brids; i++)
        {
            Bird _bird = Instantiate(birdV, map2D);
            _bird.mapGenerator = this;
            int x = Random.Range(2, h - 2);
            Vector2Int _pos = new Vector2Int(x, 0);
            _bird.currentPos = _pos;
            _bird.transform.position = ScreenPos(_pos);
            birds.Add(_bird);
        }

    }

    private void  _getObjPos(Alien _alien)
    {
        int y = Random.Range(1, h - 2);
        int x = Random.Range(1, w - 2);

        Vector2Int _pos = new Vector2Int(x, y);
        if (GetNextMapType(_pos) != MAP_TYPE.GROUND || player.GetComponent<Player>().currentPos == _pos || goalPos == _pos || itemPos == _pos)
        {
            _getObjPos(_alien);
        }
        else
        {
            _alien.transform.position = ScreenPos(_pos);
            _alien.currentPos = _pos;
        }
    }

    private void _getObjPos2(BallAlien _ball)
    {
        int y = Random.Range(1, h - 2);
        int x = Random.Range(1, w - 2);

        Vector2Int _pos = new Vector2Int(x, y);
        if (GetNextMapType(_pos) != MAP_TYPE.GROUND || player.GetComponent<Player>().currentPos == _pos || goalPos == _pos || itemPos == _pos)
        {
            _getObjPos2(_ball);
        }
        else
        {
            _ball.transform.position = ScreenPos(_pos);
            _ball.currentPos = _pos;
            //_ball.startPos = _pos;
        }
    }

    public void MoveAliens()
    {
        foreach(Alien _alien in aliens)
        {
            _alien.Move();
        }
        
        foreach(Bird _bird in birds)
        {
            _bird.Move();
        }
    }


    IEnumerator _resetStage()
    {
        yield return new WaitForSeconds(1.1f);
        
        foreach (Vector2Int _pos in tracePlayer)
        {
            mapTable[_pos.x, _pos.y] = MAP_TYPE.GROUND;
            mapChip[_pos.x, _pos.y].SetDefault();
        }

        foreach(Alien _alien in aliens)
        {
            _alien.transform.localPosition = ScreenPos(_alien.startPos);
            _alien.currentPos = _alien.startPos;
        }
        foreach(Bird _bird in birds)
        {
            _bird.transform.localPosition = ScreenPos(_bird.startPos);
            _bird.currentPos = _bird.startPos;
            _bird.gameObject.SetActive(true);
        }

        foreach(BallAlien _ball in balls)
        {
            _ball.transform.localPosition = ScreenPos(_ball.startPos);
            _ball.currentPos = _ball.startPos;
            _ball.gameObject.SetActive(true);
        }
        
        
        tracePlayer.Clear();
        player.isDead = false;

        player.ResetPlayer();
        GameManager.I.Restart();
    }
}
