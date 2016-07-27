using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameController : MonoBehaviour {
  public const int FIELD_X = 10;
  public const int FIELD_Y = 10;
  public int[,,] FieldHistory = new int[65,FIELD_Y,FIELD_X];
  int FieldHistoryCount = 0;
  int[,] othelloField = new int[FIELD_Y,FIELD_X]{
    {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},
    {-1,0,0,0,0,0,0,0,0,-1},
    {-1,0,0,0,0,0,0,0,0,-1},
    {-1,0,0,0,0,0,0,0,0,-1},
    {-1,0,0,0,0,0,0,0,0,-1},
    {-1,0,0,0,0,0,0,0,0,-1},
    {-1,0,0,0,0,0,0,0,0,-1},
    {-1,0,0,0,0,0,0,0,0,-1},
    {-1,0,0,0,0,0,0,0,0,-1},
    {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1}};

  public GameObject[,] fieldGrid = new GameObject[FIELD_Y, FIELD_X];
  public GameObject[,] fieldPiece = new GameObject[FIELD_Y, FIELD_X];
  public GameObject[] fieldPrediction = new GameObject[100];
  public GameObject grid;
  public GameObject piecePrefab;
  public GameObject predictionPrefab;
  static int pieceType = 1;
  public Text scoreWhite;
  public Text scoreBlack;
  public Text gameMode;
  enum State{
    Ready,
    PlayFirst,
    PlaySecond,
    PlayCPU,
    PlayVsPlayer,
    Gameset
  }
  State state;
  CPUController cpuController1;
  CPUController cpuController2;
  Stack pieceHistory;

  void Awake(){
    cpuController1 = new CPUController(1);
    cpuController2 = new CPUController(2);
    pieceHistory = new Stack();
  }

  void Start () {
    ChangeGameStateReady();
    InitializeField();
    WriteFieldHistory();
    putPiece(new Vector2(4,4));
    putPiece(new Vector2(4,5));
    putPiece(new Vector2(5,5));
    putPiece(new Vector2(5,4));
  }
  
  void Update () {
    switch(state){
      case State.PlayFirst:
        if(pieceType == 2){
          cpuController1.thinkCPU(othelloField);
        }
        break;
      case State.PlaySecond:
        if(pieceType == 1){
          cpuController1.thinkCPU(othelloField);
        }
        break;
      case State.PlayCPU:
        if(pieceType == 1){
          cpuController1.thinkCPU(othelloField);
        }
        if(pieceType == 2){
          cpuController2.thinkCPU(othelloField);
        }
        break;
      case State.PlayVsPlayer:
        break;
      default:
        break;
    }
  }

  void InitializeField(){
    for( int ii = 0; ii < FIELD_X; ii++){
      for( int jj = 0; jj < FIELD_Y; jj++){
        if(othelloField[jj,ii] == -1) continue;
        fieldGrid[ii, jj] = (GameObject)Instantiate(
            grid,
            new Vector3( ii, 0, jj),
            Quaternion.identity);
      }
    }
  }

  public void putPiece(Vector2 receiveMessage){
    int searchResult = 0;
    if( receiveMessage.x < 1 || receiveMessage.y < 1 ||
        receiveMessage.x > 8 || receiveMessage.y > 8){
      Debug.Log("範囲異常");
      return;
    }
    if( othelloField[(int)receiveMessage.y, (int)receiveMessage.x] != 0){
      Debug.Log("すでにピースが置かれている");
      return;
    }
    int map_x = (int)receiveMessage.x;
    int map_y = (int)receiveMessage.y;

    Vector3 position = new Vector3(map_x + 0.0f, 0.475f, map_y + 0.0f);
    var rotation = transform.rotation;
    if(pieceType == 1){
      rotation = Quaternion.AngleAxis(0, new Vector3(1, 0, 0));
    }else if(pieceType == 2){
      rotation = Quaternion.AngleAxis(180, new Vector3(1, 0, 0));
    }
    if(state == State.PlayFirst || state == State.PlaySecond ||
       state == State.PlayCPU || state == State.PlayVsPlayer){
      searchResult = SearchDirection(othelloField, map_x, map_y, 0);
    }
    if(searchResult > 0 ||
        state == State.Ready){
      fieldPiece[map_y, map_x] =
        (GameObject)Instantiate(piecePrefab, position, rotation);
      othelloField[map_y, map_x] = pieceType;
      WriteFieldHistory();
      ChangePieceType();
      DestroyPrediction();
      int predictNumber = SearchField(othelloField);
      if(predictNumber == 0 && (state == State.PlayFirst ||
         state == State.PlaySecond || state == State.PlayCPU ||
         state == State.PlayVsPlayer)){
        ChangePieceType();
        predictNumber = SearchField(othelloField);
        if(predictNumber == 0){
          ChangeGameStateGameset();
        }
      }
    }
  }

  public void ChangePieceType(){
    pieceType = 3 - pieceType;
  }

  int SearchField(int[,] searchedField){
    int count = 0;
    for(int ii = 0; ii < FIELD_Y; ii++){
      for(int jj = 0; jj < FIELD_X; jj++){
        if(searchedField[ii, jj] != 0){
          continue;
        }
        //置いた場所が適切か（返す石があるか）を調べる
        int searchResult = SearchDirection(searchedField, jj, ii, 1);
        if(searchResult > 0){
          fieldPrediction[count] = (GameObject)Instantiate(
            predictionPrefab,
            new Vector3(jj, 0.5f, ii),
            Quaternion.identity);
          searchResult = 0;
          count++;
        }
      }
    }
    IsGameover();
    return count;
  }
//changeFlag:0 ゲーム画面、searchedFieldともに反映
//changeFlag:1 返す石の数を返す
//changeFlag:2 searchedFieldを更新する

  public int SearchDirection(int[,] searchedField, int field_x, int field_y, int changeFlag){
    int searchResult = 0;
    for(int ii = -1; ii <= 1; ii++){
      for(int jj = -1; jj <= 1; jj++){
        searchResult = searchResult + SearchLine(searchedField, field_x, field_y, jj, ii, changeFlag);
      }
    }
    return searchResult;
  }

  int SearchLine(int[,] searchedField, int f_x, int f_y, int dh, int dv, bool changeFlag){
    int ii = 0;
    for(ii = 1; ii < FIELD_Y; ii++){
      if(searchedField[f_y + ii * dv, f_x + ii * dh] == 3 - pieceType){
        continue;
      }else{
        break;
      }
    }
    if(searchedField[f_y + ii * dv, f_x + ii * dh] == pieceType){
      if(changeFlag == 0 && ii > 1) {
        searchedField[f_y, f_x] = pieceType;
        ReversePiece(searchedField, f_x + ii * dh, f_y + ii * dv, dh, dv);
      }
      return ii - 1;
    }else {
      return 0;
    }
  }

  void ReversePiece(int[,] searchedField, int f_x, int f_y, int dh, int dv){
    int ii = 0;
    dh = dh * -1; dv = dv * -1;
    for(ii = 1; ii < FIELD_Y; ii++){
      if(searchedField[f_y + ii * dv, f_x + ii * dh] == pieceType){
        break;
      }
      if(changeFlag == 0){
        if(pieceType == 1){
          fieldPiece[f_y + ii * dv, f_x + ii * dh].transform.rotation = 
            Quaternion.AngleAxis(0, new Vector3(1, 0, 0));
        }else if(pieceType == 2){
          fieldPiece[f_y + ii * dv, f_x + ii * dh].transform.rotation = 
            Quaternion.AngleAxis(180, new Vector3(1, 0, 0));
        }
      }
      searchedField[f_y + ii * dv, f_x + ii * dh] = pieceType;
    }
  }

  void DestroyPrediction(){
    for(int ii = 0; ii < FIELD_Y * FIELD_X; ii++){
      if(fieldPrediction[ii] == null){
        continue;
      }
      Destroy(fieldPrediction[ii]);
    }
  }

  void IsGameover(){
    int white = CheckWhitePiece();
    int black = CheckBlackPiece();
    scoreWhite.text = "White : " + white;
    scoreBlack.text = "Black : " + black;
    if(white + black > 63){
      ChangeGameStateGameset();
    }
  }

  public void ChangeGameStateGameset(){
    state = State.Gameset;
    gameMode.text = "mode " + state;
  }

  public void ChangeGameStatePlayFirst(){
    state = State.PlayFirst;
    gameMode.text = "mode " + state;
  }

  public void ChangeGameStatePlaySecond(){
    state = State.PlaySecond;
    gameMode.text = "mode " + state;
  }

  public void ChangeGameStatePlayCPU(){
    state = State.PlayCPU;
    gameMode.text = "mode " + state;
  }

  public void ChangeGameStateReady(){
    state = State.Ready;
    gameMode.text = "mode " + state;
  }

  public void ChangeGameStatePlayerVsPlayer(){
    state = State.PlayVsPlayer;
    gameMode.text = "mode " + state;
  }

  public void InitializeGame(){
    ChangeGameStateReady();
    for(int jj = 1; jj < 9; jj++){
      for(int ii = 1; ii < 9; ii++){
        if(othelloField[jj,ii] != 0){
          othelloField[jj,ii] = 0;
          Destroy(fieldPiece[jj,ii]);
        }
      }
    }
    pieceType = 1;
    FieldHistoryCount = 0;
    putPiece(new Vector2(4,4));
    putPiece(new Vector2(4,5));
    putPiece(new Vector2(5,5));
    putPiece(new Vector2(5,4));
  }

  int CheckWhitePiece(){
    int count = 0;
    for(int jj = 0; jj < FIELD_Y; jj++){
      for(int ii = 0; ii < FIELD_X; ii++){
        if(othelloField[jj,ii] == 2) count++;
      }
    }
    return count;
  }

  int CheckBlackPiece(){
    int count = 0;
    for(int jj = 0; jj < FIELD_Y; jj++){
      for(int ii = 0; ii < FIELD_X; ii++){
        if(othelloField[jj,ii] == 1) count++;
      }
    }
    return count;
  }

  public void GameUndo(){
    if(FieldHistoryCount <= 5) return;
    if(state == State.PlayFirst || state == State.PlaySecond){
      FieldHistoryCount = FieldHistoryCount - 3;
      ChangePieceType();
    }else{
      FieldHistoryCount = FieldHistoryCount - 2;
    }
    for(int jj = 0; jj < FIELD_Y; jj++){
      for(int ii = 0; ii < FIELD_X; ii++){
        if(othelloField[jj,ii] != FieldHistory[FieldHistoryCount,jj,ii]){
          othelloField[jj,ii] = FieldHistory[FieldHistoryCount,jj,ii];
          Destroy(fieldPiece[jj,ii]);
          if(othelloField[jj,ii] != 0){
            var rotation = transform.rotation;
            Vector3 position = new Vector3(ii + 0.0f, 0.475f, jj + 0.0f);
            if(othelloField[jj,ii] == 1){
              rotation = Quaternion.AngleAxis(0, new Vector3(1, 0, 0));
            }else if(othelloField[jj,ii] == 2){
              rotation = Quaternion.AngleAxis(180, new Vector3(1, 0, 0));
            }
            fieldPiece[jj, ii] =
              (GameObject)Instantiate(piecePrefab, position, rotation);
          }
        }
      }
    }
    FieldHistoryCount++;
    ChangePieceType();
    DestroyPrediction();
    SearchField(othelloField);
  }

  public void GameRedo(){
    if(othelloField[0,0] == 0) return;
    if(FieldHistoryCount <= 4) return;
    if(state == State.PlayFirst || state == State.PlaySecond){
      FieldHistoryCount = FieldHistoryCount + 2;
      ChangePieceType();
    }else{
      FieldHistoryCount = FieldHistoryCount + 1;
    }
    for(int jj = 0; jj < FIELD_Y; jj++){
      for(int ii = 0; ii < FIELD_X; ii++){
        if(othelloField[jj,ii] != FieldHistory[FieldHistoryCount,jj,ii]){
          othelloField[jj,ii] = FieldHistory[FieldHistoryCount,jj,ii];
          Destroy(fieldPiece[jj,ii]);
          if(othelloField[jj,ii] != 0){
            var rotation = transform.rotation;
            Vector3 position = new Vector3(ii + 0.0f, 0.475f, jj + 0.0f);
            if(othelloField[jj,ii] == 1){
              rotation = Quaternion.AngleAxis(0, new Vector3(1, 0, 0));
            }else if(othelloField[jj,ii] == 2){
              rotation = Quaternion.AngleAxis(180, new Vector3(1, 0, 0));
            }
            fieldPiece[jj, ii] =
              (GameObject)Instantiate(piecePrefab, position, rotation);
          }
        }
      }
    }
    ChangePieceType();
    DestroyPrediction();
    SearchField(othelloField);
  }

  public void DebugOutputFieldLog(int[,] log){
    for(int jj = FIELD_Y - 1; jj > 0 ; jj--){
      Debug.Log(" " + log[jj,1] +
                " " + log[jj,2] +
                " " + log[jj,3] +
                " " + log[jj,4] +
                " " + log[jj,5] +
                " " + log[jj,6] +
                " " + log[jj,7] +
                " " + log[jj,8] 
          );
    }
  }

  public void WriteFieldHistory(){
    for(int jj = 0; jj < FIELD_Y; jj++){
      for(int ii = 0; ii < FIELD_X; ii++){
        FieldHistory[FieldHistoryCount,jj,ii] = othelloField[jj,ii];
      }
    }
    Debug.Log("FieldHistoryCount " + FieldHistoryCount);
    FieldHistoryCount++;
  }
}
