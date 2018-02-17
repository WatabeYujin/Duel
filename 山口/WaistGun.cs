using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// スタンバイフェイズ時の腰の判定のスクリプト
/// </summary>
public class WaistGun : MonoBehaviour {
    /***********************************
     * クラス変数
     **********************************/
    //private Standby standby;
    //private PhaseCheck phaseCheck;
    private GameCommonData gameData;
	
    /***********************************
     * クラス関数
     **********************************/
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "alpha" && gameData == null)
        {
            GameObject battleController = GameObject.Find("BatteleController");
            gameData = battleController.GetComponent<GameCommonData>();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Gun") // 銃が当たったら
        {
            if (SceneManager.GetActiveScene().name != "alpha" || gameData.GetNowPhase != GameCommonData.Phase.StandbyPhase) return; // alphaScene以外とスタンバイフェイズ以外は
            gameData.myData.IsStanby = true;
            gameData.PassPlayerUpdata();
            //gameData.standby.PassReadyCheck(PhotonNetwork.player.ID - 1, true);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        
        if (collider.gameObject.tag == "Gun")
        {
            if (SceneManager.GetActiveScene().name != "alpha") return;
            gameData.myData.IsStanby = false;
            //gameData.standby.PassReadyCheck(PhotonNetwork.player.ID - 1, false);
        }
    }
}
