using UnityEngine;
using System.Collections;

public class GetFieldPosition : MonoBehaviour {
  //const float CAMERA_Z = 7.66f;
  const float CAMERA_Z = 10.0f;
  const float OFFSET_X = 0.82f;
  const float OFFSET_Y = 0.66f;
  GameController gameController;

  void Awake(){
    gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
  }

  void Update () {
    GetWorldPosition();
  }

  void GetWorldPosition(){
    if(Input.GetButtonDown("Fire1")){
      Vector3 position = Input.mousePosition;
      position.z = CAMERA_Z;
      position = GetComponent<Camera>().ScreenToWorldPoint(position);
      //Debug.Log(position.x + " " + position.z);
      //position.x = (position.x + OFFSET_X) ;
      //position.z = (position.z + OFFSET_Y) ;
      //Debug.Log(position.x + " " + position.z);
      float msg_x = Mathf.Floor(position.x + OFFSET_X) ;
      float msg_y = Mathf.Floor(position.z + OFFSET_Y) ;
      //Debug.Log(msg_x + " " + msg_y);
      gameController.SendMessage(
          "putPiece", 
          (new Vector2(msg_x, msg_y)),
          SendMessageOptions.DontRequireReceiver);
      //gameController.ChangePieceType();
    }
  }
}
