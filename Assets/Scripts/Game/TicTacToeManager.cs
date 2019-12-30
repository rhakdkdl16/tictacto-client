using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD

public class TicTacToeManager : MonoBehaviour
{
    // 플레이어의 마커타입
    MarkerType playerMarkerType;
    //현재게임의 상태
    enum GameState {None,PlayerTurn,OpponentTurn,GameOver } 
    GameState currentState;
    int point;
    Cell[] cell;

    private void Start()
    {
        //임시 코드
        playerMarkerType = MarkerType.Circle;
        currentState = GameState.PlayerTurn;
    }

    private void Update()
    {
        

        if(currentState == GameState.PlayerTurn || currentState == GameState.OpponentTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);     //마우스를누른시점의 마우스위치
=======
using UnityEngine.UI;
using SocketIO;

public class TicTacToeManager : MonoBehaviour
{
    [SerializeField] Button connectButton;
    [SerializeField] Button readyButton;
    [SerializeField] Text logText;

    [SerializeField] GameObject cellsObject;

    // 화면에 있는 Cell의 정보
    //public Cell[] cells;
    public List<Cell> cells;

    // 플레이어의 마커타입
    private MarkerType playerMakerType;

    // 현재 게임의 상태
    private enum GameState { None, PlayerTurn, OpponentTurn, GameOver }
    // 현재 게임 상태
    private GameState currentState;
    private GameState CurrentState
    {
        get 
        {
            return currentState;
        }
        set
        {
            switch (value)
            {
                case GameState.None:
                case GameState.OpponentTurn:
                case GameState.GameOver:
                    SetActiveTouchCells(false);
                    break;
                case GameState.PlayerTurn:
                    SetActiveTouchCells(true);
                    break;
            }
            currentState = value;
        }
    }

    // 승리 판정
    private enum Winner { None, Player, Opponent, Tie }

    // Grid의 행과 열의 수
    private const int rowColNum = 3;

    // SocketIO
    private SocketIOComponent socket;

    // Room ID와 Client ID
    ClientInfo clientInfo;

    private void Start() 
    {
        // 소켓 초기화
        InitSocket();

        // Cells 오브젝트 숨기기
        cellsObject.SetActive(false);

        // Ready 버튼 숨기기
        readyButton.gameObject.SetActive(false);
    }

    private void Update() 
    {
        if (CurrentState == GameState.PlayerTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
>>>>>>> 60dc553554f433e6d2519f1b46bfee1d2f3c71c5
                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);

                if (hitInfo && hitInfo.transform.tag == "Cell")
                {
                    Cell cell = hitInfo.transform.GetComponent<Cell>();
<<<<<<< HEAD
                    if (currentState == GameState.PlayerTurn)
                    {
                        cell.MarkerType = playerMarkerType;
                        cell.tag = "User";

                    }
                    else
                    {
                        cell.MarkerType = (playerMarkerType == MarkerType.Circle) ? MarkerType.Cross : MarkerType.Circle;
                        cell.tag = "Enemy";
                    }             
                    currentState = (currentState == GameState.PlayerTurn) ? GameState.OpponentTurn : GameState.PlayerTurn;
                    FCell();
                }                
            }
        }        
    }
    public void FindUserCell(Cell cell)
    {
       RaycastHit2D rightCell  = Physics2D.Raycast(cell.rightPos.position, Vector2.right,1);
       RaycastHit2D leftCell = Physics2D.Raycast(cell.leftPos.position, Vector2.left,1);
       RaycastHit2D upCell = Physics2D.Raycast(cell.upPos.position, Vector2.up,1);
       RaycastHit2D downCell = Physics2D.Raycast(cell.downPos.position, Vector2.down,1);
       RaycastHit2D righUpCell = Physics2D.Raycast(cell.rightUpPos.position ,new Vector2(1,1));
        
        if (rightCell != null && leftCell != null )
        {
            if (rightCell.transform.tag == "User" && leftCell.transform.tag == "User")
            {
                GameOver();
            }
        }
        if(upCell != null && downCell != null)
        {
            if (downCell.transform.tag == "User" && upCell.transform.tag == "User")
            {
                GameOver();
            }
        }
        
       
    }
    void FCell()
    {
        GameObject[] gm = GameObject.FindGameObjectsWithTag("User");
        Debug.Log(gm.Length);
        for (int i=0; i< gm.Length; i++)
        {
            Cell findCell = gm[i].GetComponent<Cell>();
            FindUserCell(findCell);      
        }
    }

    void GameOver()
    {
        Debug.Log("UserWin");
=======

                    cell.MarkerType = playerMakerType;
                    Winner winner = CheckWinner();

                    // 선택된 셀의 정보 전달
                    int index = cells.IndexOf(cell);

                    JSONObject data = new JSONObject();
                    data.AddField("index", index);
                    data.AddField("roomId", clientInfo.roomId);

                    switch (winner)
                    {
                        case Winner.None:
                            {
                                // TODO: currentState를 상대턴으로 변경
                                // 서버에게 상대가 게임을 진행할 수 있도록 메시지 전달
                                CurrentState = GameState.OpponentTurn;
                                socket.Emit("select", data);
                                break;
                            }
                        case Winner.Player:
                            {
                                // TODO: 승리 팝업창 표시
                                // 서버에게 Player가 승리했음을 알림
                                CurrentState = GameState.GameOver;
                                socket.Emit("win", data);
                                SetLog("이겼음");
                                break;
                            }
                        case Winner.Tie:
                            {
                                // TODO: 비겼음을 팝업창으로 표시
                                // 서버에게 비겼음을 알림
                                CurrentState = GameState.GameOver;
                                socket.Emit("tie", data);
                                SetLog("비겼음");
                                break;
                            }
                    }
                }
            }
        }
    }

    private void InitSocket()
    {
        GameObject socketObject = GameObject.Find("SocketIO");
        socket = socketObject.GetComponent<SocketIOComponent>();

        // Socket.io 이벤트 등록
        socket.On("connect", EventConnect);     // 서버 연결
        socket.On("join", EventJoin);           // 방 입장
        socket.On("play", EventPlay);           // 게임시작
        socket.On("selected", EventSelected);   // 상대방의 플레이
        socket.On("lose", EventLose);           // 졌을때
        socket.On("tie", EventTie);             // 비겼을때
    }

    #region Socket.io Events

    // 서버에 연결 되었을때
    void EventConnect(SocketIOEvent e)
    {
        SetLog("서버에 접속함");

        // connect button은 숨김
        connectButton.gameObject.SetActive(false);
    }

    // 방에 들어갔을때
    void EventJoin(SocketIOEvent e)
    {
        // Ready 버튼 활성화
        readyButton.gameObject.SetActive(true);

        // roomId 가져오기
        string roomId = e.data.GetField("roomId").str;
        string clientId = e.data.GetField("clientId").str;

        clientInfo = new ClientInfo(roomId, clientId);

        SetLog("방 입장");
        SetLog("Room ID: " + roomId);
        SetLog("Client ID: " + clientId);
    }

    // 게임 시작
    void EventPlay(SocketIOEvent e)
    {
        SetLog("게임 시작");

        // Ready button 숨기기
        readyButton.gameObject.SetActive(false);

        bool isFirst = false;
        e.data.GetField(ref isFirst, "first");

        // 턴과 Marker 지정
        if (isFirst)
        {
            playerMakerType = MarkerType.Circle;
            CurrentState = GameState.PlayerTurn;
        }
        else
        {
            playerMakerType = MarkerType.Cross;
            CurrentState = GameState.OpponentTurn;
        }

        // Cells 오브젝트 표시
        cellsObject.SetActive(true);
    }

    // 상대방이 Cell을 선택했을 경우
    void EventSelected(SocketIOEvent e)
    {
        int index = -1;
        e.data.GetField(ref index, "index");

        MarkerType markerType = (playerMakerType == MarkerType.Circle) ? MarkerType.Cross : MarkerType.Circle;
        cells[index].GetComponent<Cell>().MarkerType = markerType;

        CurrentState = GameState.PlayerTurn;
    }

    // 게임에서 졌을때
    void EventLose(SocketIOEvent e)
    {
        int index = -1;
        e.data.GetField(ref index, "index");

        MarkerType markerType = (playerMakerType == MarkerType.Circle) ? MarkerType.Cross : MarkerType.Circle;
        cells[index].GetComponent<Cell>().MarkerType = markerType;

        CurrentState = GameState.GameOver;
        SetLog("졌음");
    }

    // 게임에서 비겼을때
    void EventTie(SocketIOEvent e)
    {
        int index = -1;
        e.data.GetField(ref index, "index");

        MarkerType markerType = (playerMakerType == MarkerType.Circle) ? MarkerType.Cross : MarkerType.Circle;
        cells[index].GetComponent<Cell>().MarkerType = markerType;

        CurrentState = GameState.GameOver;
        SetLog("비겼음");
    }

    #endregion

    void SetActiveTouchCells(bool active)
    {
        foreach (Cell cell in cells)
        {
            cell.SetActiveTouch(active);
        }
    }

    Winner CheckWinner()
    {
        // 1. 가로체크
        for (int i = 0; i < rowColNum; i++)
        {
            // 비교를 위한 첫 번째 Cell
            Cell cell = cells[rowColNum * i];
            int num = 0;

            // 첫 번째 Cell이 Player Marker type과 다르면 for loop 빠져나옴
            if (cell.MarkerType != playerMakerType) continue;

            for (int j = 1; j < rowColNum; j++)
            {
                int index = i * rowColNum + j;
                if (cell.MarkerType == cells[index].MarkerType)
                {
                    ++num;
                }
            }

            if (cell.MarkerType != MarkerType.None && num == rowColNum - 1)
            {
                return Winner.Player;
            }
        }

        // 2. 세로체크
        for (int i = 0; i < rowColNum; i++)
        {
            Cell cell = cells[i];
            int num = 0;
            if (cell.MarkerType != playerMakerType) continue;
            for (int j = 1; j < rowColNum; j++)
            {
                int index = j * rowColNum + i;
                if (cell.MarkerType == cells[index].MarkerType)
                {
                    ++num;
                }
            }
            if (cell.MarkerType != MarkerType.None && num == rowColNum - 1)
            {
                return Winner.Player;
            }
        }

        // 3. 왼쪽 대각선 체크
        if (cells[0].MarkerType == playerMakerType) {
            Cell cell = cells[0];
            int num = 0;
            for (int i = 1; i < rowColNum; i++)
            {
                int index = i * rowColNum + i;
                if (cell.MarkerType == cells[index].MarkerType)
                {
                    ++num;
                }
            }
            if (cell.MarkerType != MarkerType.None && num == rowColNum - 1)
            {
                return Winner.Player;
            }
        }

        // 4. 오른쪽 대각선 체크
        if (cells[rowColNum - 1].MarkerType == playerMakerType) {
            Cell cell = cells[rowColNum - 1];
            int num = 0;
            for (int i = 1; i < rowColNum; i++)
            {
                int index = i * rowColNum + rowColNum - i - 1;
                if (cell.MarkerType == cells[index].MarkerType)
                {
                    ++num;
                }
            }
            if (cell.MarkerType != MarkerType.None && num == rowColNum - 1)
            {
                return Winner.Player;
            }
        }

        // 5. 비겼는지 체크
        {
            int num = 0;
            foreach (Cell cell in cells)
            {
                if (cell.MarkerType == MarkerType.None)
                    ++num;
            }
            if (num == 0)
            {
                return Winner.Tie;
            }
        }

        return Winner.None;
    }

    #region UI Events

    public void OnClickConnect()
    {
        connectButton.interactable = false;

        // 서버에 접속
        if (socket)
           socket.Connect();
    }

    // Ready 버튼 클릭시 호출되는 함수
    public void OnClickReady()
    {
        readyButton.interactable = false;

        JSONObject data = new JSONObject();
        data.AddField("roomId", clientInfo.roomId);
        data.AddField("clientId", clientInfo.clientId);

        socket.Emit("ready", data);

        SetLog("준비완료");
    }

    #endregion

    void SetLog(string message)
    {
        logText.text += message + "\n";
>>>>>>> 60dc553554f433e6d2519f1b46bfee1d2f3c71c5
    }
}
