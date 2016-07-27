using System;
using UnityEngine;

public class EvaluateField {
  int map_x;
  int map_y;
  int evaluationValueMax;
  int evaluationValueMin;
  int evaluationValue[];
  int[,] evaluatedField;
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

  public EvaluateField(int[,] field){
    evaluatedField = field;
    gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
  }

  public void EvaluateMaxValue(){
    int evaluate_value_max = 0;
    for(jj = 0; jj < FIELD_Y; jj++){
      for(ii = 0; ii < FIELD_X; ii++){
        if(evaluatedField[jj,ii] != 0) continue;
        if(gameController.SearchDirection(evaluatedField, ii, jj, 1) > 0){
          int[,] tmp_field = evaluatedField;
          gameController.SearchDirection(evaluatedField, ii, jj, 2);
          if(evaluate_value_max < CalculationField(evaluatedField, CPU1)){
            evaluate_value_max = CalculationField(evaluatedField, CPU1);
          }
          evaluatedField = tmp_field;
        }
      }
    }
  }

  int CalculationField(int[,] field, int[,] weight){
    int Value = 0;
    for(int jj = 0; jj < FIELD_Y; jj++){
      for(int ii = 0; ii < FIELD_X; ii++){
        if(field[jj,ii] == -1) continue;
        Value += field[jj,ii] * weight[jj,ii];
      }
    }
    return Value;
  }
}
