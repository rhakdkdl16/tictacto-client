using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class TicTacToeManager : MonoBehaviour
{    
    [SerializeField] GameObject cellsObject;
    [SerializeField] Button readyButton;
    [SerializeField] Button connectButton;
    [SerializeField] Text logText;
    [SerializeField] GameOverPanelManager gameOverPanelManager;
    //화면에있는 cell의정보
    //public Cell[] cells;
    public List<Cell> cells;
    // 플레이어의 마커타입
    private MarkerType playerMakerType;

    // 현재 게임의 상태
    private enum GameState { None, PlayerTurn, OpponentTurn, GameOver }
    private GameState currentState;
    private GameState CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            switch(value)
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
    public enum Winner {Player , Oppoenet , Tie ,None}
    Winner winner;

    //gird 행과열의수
    public const int rowColNum = 3;
    //SocketIO
    SocketIOComponent socket;
    //room ID 와 Client ID 
    ClientInfo client;

    private void Start() 
    {
      

        //소켓초기화
        InitSocket();
        //Cell숨기기
        cellsObject.SetActive(false);
        readyButton.gameObject.SetActive(false);
    }

    private void Update() 
    {
        if (CurrentState == GameState.PlayerTurn )
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);

                if (hitInfo && hitInfo.transform.tag == "Cell")
                {
                    Cell cell = hitInfo.transform.GetComponent<Cell>();

                        cell.MarkerType = playerMakerType;
                        Winner winner = CheckWinner();
                        Debug.Log("Winner" + winner);
                    //currentState = (currentState == GameState.PlayerTurn) ? GameState.OpponentTurn : GameState.PlayerTurn;

                    //선택된 셀의 정보전달
                    int index = cells.IndexOf(cell);
                    JSONObject data = new JSONObject();
                    data.AddField("index", index);
                    data.AddField("roomId", client.roomId);
                    switch (winner)
                    {
                        case Winner.None:
                            {
                               
                                //TODO : 서버에게 게임을진행할수있도록 메세지 전달
                                CurrentState = GameState.OpponentTurn;                                                
                                socket.Emit("select", data);
                                break;
                            }
                        case Winner.Player:
                            {
                                // TODO: 승리 팝업창표시
                                // 서버에게 player 가 승리했음을 알림
                                CurrentState = GameState.GameOver;                                
                                socket.Emit("win",data);
                                SetLog("승리");
                                gameOverPanelManager.SetMessage("승리 하였습니다!");
                                gameOverPanelManager.Show();
                            break;
                            }
                        case Winner.Tie:
                            {
                                // TODO : 무승부 팝업창 표시
                                // 서버에게 무승부를 알림
                                CurrentState = GameState.GameOver;
                                socket.Emit("tie", data);
                                SetLog("무승부");
                                gameOverPanelManager.SetMessage("무승부");
                                gameOverPanelManager.Show();
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

        //SocketIO 이벤트등록
        socket.On("connect", EventConnect);         //서버연결
        socket.On("join", EventJoin);               //방입장
        socket.On("play", EventPlay);               //게임시작
        socket.On("selected", EventSelected);       //상대방의 플레이
        socket.On("lose", EventLose);               //졌을때
        socket.On("tie", EventTie);                 //비겼을때
        //socket.On("test", Test);
    }
    #region socket.io Events

    //서버에 연결되었을때
    void EventConnect(SocketIOEvent e)
    {
        SetLog("Connect Server");

        //connect버튼 숨김
        connectButton.gameObject.SetActive(false);
    }
    //방에들어갔을때
    void EventJoin(SocketIOEvent e)
    {        
        readyButton.gameObject.SetActive(true);

        //roomID 가져오기
        string roomID = e.data.GetField("roomId").str;
        string clientId = e.data.GetField("clientId").str;

        client = new ClientInfo(roomID, clientId);

        SetLog("방 입장");
        SetLog("방에입장:" + roomID);
        SetLog("Client ID" + clientId);
    }
    // 게임시작
    void EventPlay(SocketIOEvent e)
    {
        SetLog("게임시작");

        //ready button 숨기기
        readyButton.gameObject.SetActive(false);

        bool isFirst = false;
        e.data.GetField(ref isFirst, "first");

        // 턴과 마커 지정
        if (isFirst)
        {
            SetLog("선입니다.");

            // 임시 코드
            playerMakerType = MarkerType.Circle;
            CurrentState = GameState.PlayerTurn;
        }
        else
        {
            playerMakerType = MarkerType.Cross;
            CurrentState = GameState.OpponentTurn;
        }
        cellsObject.SetActive(true);
    }
  
    //상대방이 selec 했을경우
    void EventSelected(SocketIOEvent e)
    {
        int index = -1;
        e.data.GetField(ref index, "index");

        MarkerType markerType = (playerMakerType == MarkerType.Circle) ? MarkerType.Cross : MarkerType.Circle;
        cells[index].GetComponent<Cell>().MarkerType = markerType;

        CurrentState = GameState.PlayerTurn;
    }
    void EventLose(SocketIOEvent e)
    {
        int index = 01;
        e.data.GetField(ref index, "index");

        MarkerType markerType = (playerMakerType == MarkerType.Circle) ? MarkerType.Cross : MarkerType.Circle;
        cells[index].GetComponent<Cell>().MarkerType = markerType;

        currentState = GameState.GameOver;
        SetLog("패배");
        gameOverPanelManager.Show();
        gameOverPanelManager.SetMessage("패배");
    }
    void EventTie(SocketIOEvent e)
    {
        int index = 01;
        e.data.GetField(ref index, "index");

        MarkerType markerType = (playerMakerType == MarkerType.Circle) ? MarkerType.Cross : MarkerType.Circle;
        cells[index].GetComponent<Cell>().MarkerType = markerType;

        currentState = GameState.GameOver;
        SetLog("무승부");
        gameOverPanelManager.Show();
        gameOverPanelManager.SetMessage("무승부");
    }
    #endregion

    //void Test(SocketIOEvent e)
    //{
    //    string msg = e.data.GetField("message").str;
    //    Debug.Log(msg);

    //    //클라이언트  -> 서버로 전달
    //    JSONObject data = new JSONObject();
    //    data.AddField("msg", "저도 반갑습니다");
    //    socket.Emit("hello", data);
    //}

    void SetActiveTouchCells(bool active)
    {
        foreach (Cell cell in cells)
        {
            cell.SetActiveTouch(active);
        }
    }
 

    Winner CheckWinner()
    {
        //1 가로체크
        for(int i = 0; i < rowColNum; i++)
        {
            //비교를위한 첫번째 Cell
            Cell cell = cells[rowColNum * i];
            int num = 0;
            //첫번째 cell이 player Marker type 과다르면 for loop 빠져나옴
            if (cell.MarkerType != playerMakerType) continue;

            for(int j = 1; j < rowColNum; j++)
            {
                int index = i * rowColNum + j;
                if (cell.MarkerType == cells[index].MarkerType)
                {
                    ++num;
                }
            }
            if(cell.MarkerType != MarkerType.None && num == rowColNum -1)
            {
                return Winner.Player;
            }
        }
        // 2 세로체크
        for(int i = 0; i < rowColNum; i ++)
        {
            Cell cell = cells[i];
            int num = 0;

            //첫번째 cell이 player Marker type 과다르면 for loop 빠져나옴
            if (cell.MarkerType != playerMakerType) continue;

            for (int j = 1; j < rowColNum; j++)
            {
                int index = j * rowColNum + i;
                if(cell.MarkerType == cells[index].MarkerType)
                {
                    ++num;
                }
            }
            if (cell.MarkerType != MarkerType.None && num == rowColNum - 1)
            {
                return Winner.Player;
            }
        }
        //3.왼쪽 대각선 체크
           // 첫
            if (cells[0].MarkerType == playerMakerType)
            {
                Cell cell = cells[0];
                int num = 0;
            for (int i = 1; i < rowColNum; i++)
            {
                int index = i * rowColNum + i;
                if (cell.MarkerType == cells[index].MarkerType)
                {
                    num++;
                }
            }
            if (cell.MarkerType != MarkerType.None && num == rowColNum - 1)
            {
             return Winner.Player;
            }
                
            }
        //오른쪽 대각선 체크
        if (cells[rowColNum -1 ].MarkerType == playerMakerType)
        {
            Cell cell = cells[rowColNum - 1];
            int num = 0;
            for (int i = 1; i < rowColNum; i++)
            {
                int index = i * rowColNum + rowColNum - i - 1;
                if (cell.MarkerType == cells[index].MarkerType)
                {
                    num ++;
                }
            }
            if (cell.MarkerType != MarkerType.None && num == rowColNum - 1)
            {
                return Winner.Player;
            }

        }
        //5.비겼는지 체크
        {
            int num = 0;
            foreach(Cell cell in cells)
            {
                if (cell.MarkerType == MarkerType.None)
                    ++num;
            }
            if(num == 0)
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

        //서버에접속
        if (socket)
        {
            socket.Connect();
            //cellsObject.SetActive(true);
        }
    }

    //ready 버튼클릭시 호출되는함수
    public void OnClickReady()
    {
        readyButton.interactable = false;

        JSONObject data = new JSONObject();
        data.AddField("roomId", client.roomId);
        data.AddField("clientId", client.clientId);
        socket.Emit("ready",data);

        SetLog("준비완료");
    }

    #endregion

    void SetLog(string message)
    {
        logText.text += message + "\n";
    }
}
