using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PhotonManager : PunBehaviour
{
    /************************************************
     * クラス変数
     ***********************************************/

    [SerializeField]
    private string ObjectName; //生成するプレイヤーオブジェクトの名前
    public Transform ground;

    /************************************************
     * クラス関数
     ***********************************************/

    void Start()
    {
        // Photonネットワークの設定を行う
        PhotonNetwork.ConnectUsingSettings("0.1");
        PhotonNetwork.sendRate = 30;
    }
    //-----------------------------------------------

    // 「ロビー」に接続した際に呼ばれるコールバック
    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    // いずれかの「ルーム」への接続に失敗した際のコールバック
    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed");

        // ルームを作成（今回の実装では、失敗＝マスタークライアントなし、として「ルーム」を作成）
        PhotonNetwork.CreateRoom(null);
    }
    // Photonサーバに接続した際のコールバック
    public override void OnConnectedToPhoton()
    {
        Debug.Log("OnConnectedToPhoton");
    }

    // マスタークライアントに接続した際のコールバック
    public override void OnConnectedToMaster()
    {
       
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinRandomRoom();
    }

    // いずれかの「ルーム」に接続した際のコールバック
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        float x = Random.Range(-25.0f, 25.0f);
        float z = Random.Range(-25.0f, 25.0f);

        Vector3 pos = new Vector3(x, 2, z) + ground.position;
        
        // 「ルーム」に接続したらPlayerを生成する）
        GameObject player = PhotonNetwork.Instantiate(ObjectName, pos, Quaternion.identity, 0); 
                              
    }

    // 現在の接続状況を表示（デバッグ目的）
    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    //------------------------------------------------------
    
    
}
