using UnityEngine;
using System.Collections;
//using GameController;

public class CPUController : MonoBehaviour {
  public const int FIELD_X = 10;
  public const int FIELD_Y = 10;
  int CPUMode = 0;
  GameController gameController;
  int[,] CPU1= new int[FIELD_Y,FIELD_X]{
    {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},
    {-1,9,1,6,5,5,6,1,9,-1},
    {-1,1,1,6,7,7,6,1,1,-1},
    {-1,6,6,7,8,8,7,6,6,-1},
    {-1,5,7,8,9,9,8,7,5,-1},
    {-1,5,7,8,9,9,8,7,5,-1},
    {-1,6,6,7,8,8,7,6,6,-1},
    {-1,1,1,6,7,7,6,1,1,-1},
    {-1,9,1,6,5,5,6,1,9,-1},
    {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1}};
  int[,] CPU2= new int[FIELD_Y,FIELD_X]{
    {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1},
    {-1,9,1,7,5,5,7,1,9,-1},
    {-1,1,1,6,7,7,6,1,1,-1},
    {-1,7,6,7,8,8,7,6,7,-1},
    {-1,5,7,8,9,9,8,7,5,-1},
    {-1,5,7,8,9,9,8,7,5,-1},
    {-1,7,6,7,8,8,7,6,7,-1},
    {-1,1,1,6,7,7,6,1,1,-1},
    {-1,9,1,7,5,5,7,1,9,-1},
    {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1}};
  int[,] ratios = new int[FIELD_Y,FIELD_X];

  public CPUController(int mode){
    CPUMode = mode;
    switch(CPUMode){
      case 1:
        ratios = CPU1;
        break;
      case 2:
        ratios = CPU2;
        break;
      default:
        break;
    }
  }

  public void thinkCPU(int[,] othelloField){
    if(gameController == null){
       gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }
    int ii = 0; int jj = 0;
    Vector2 put = new Vector2(0,0);
    int maxEvaluationvalue = 0;
    for(jj = 0; jj < FIELD_Y; jj++){
      for(ii = 0; ii < FIELD_X; ii++){
        if(othelloField[jj,ii] != 0) continue;
        if (maxEvaluationvalue < CalculationField(othelloField, ratios)){
          if(gameController.SearchDirection(othelloField, ii, jj, 2) > 0){
            maxEvaluationvalue = CalculationField(othelloField, ratios);
            put.x = ii; put.y = jj;
          }
        }else{
          continue;
        }
      }
    }
    Debug.Log("x="+(int)put.x+" y="+(int)put.y);
    gameController.putPiece(put);
  }

}
