using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonSponer : MonoBehaviour {

    [SerializeField]
    private Transform[] playerTransform = new Transform[3];

    public readonly byte InstantiateVrAvatarEventCode = 123;

    private int playerConect = 0;

    [SerializeField]
    private Transform[] vrSet = new Transform[3];

    //void Awake()
    //{
    //    PhotonNetwork.OnEventCall += OnRaiseEvent;
    //}

    public void OnJoinedRoom()
    {
        Debug.Log("入室しました");
        int viewId = PhotonNetwork.AllocateViewID();

        Debug.Log(viewId);
        PhotonNetwork.OnEventCall += OnEvent;
        PhotonNetwork.RaiseEvent((byte)InstantiateVrAvatarEventCode, viewId, true, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All });
    }

    private void OnEvent(byte eventcode, object content, int senderid)
    {
        Debug.Log("Event!");
        if (eventcode == InstantiateVrAvatarEventCode)
        {
            GameObject go = null;

            if (PhotonNetwork.player.ID == senderid)
            {
                if(PhotonNetwork.player.ID == 1)
                {
                    //プレイヤーIDで生成位置を決める
                    go = Instantiate(Resources.Load("LocalAvatar"),playerTransform[0].position,playerTransform[0].rotation) as GameObject;
                    playerTransform[2].position = playerTransform[0].position;
                }
                else
                {
                    go = Instantiate(Resources.Load("LocalAvatar"), playerTransform[1].position, playerTransform[1].rotation) as GameObject;
                    playerTransform[2].position = playerTransform[1].position;
                    playerTransform[2].rotation = playerTransform[1].rotation;
                }
                Debug.Log("LocalAvatar召還！");
                GameObject.Find("1PconectText").GetComponent<Text>().enabled = true;
                playerConect++;
            }
            else
            {
                if (PhotonNetwork.player.ID == 1)
                {
                    go = Instantiate(Resources.Load("RemoteAvatar"), playerTransform[1].position, playerTransform[1].rotation) as GameObject;
                }
                else
                {
                    go = Instantiate(Resources.Load("RemoteAvatar"), playerTransform[0].position, playerTransform[0].rotation) as GameObject;
                }
                Invoke("SetIK", 1.0f);

                GameObject.Find("2PconectText").GetComponent<Text>().enabled = true;
                playerConect++;
            }

            if (go != null)
            {
                PhotonView pView = go.GetComponent<PhotonView>();

                if (pView != null)
                {
                    pView.viewID = (int)content;
                }
            }
            if(playerConect == 2)
            {
                SceneManager.LoadScene("Title");
            }
        }
    }
    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update () {
		
	}

    public void SetIK()
    {
        vrSet[0].transform.parent = GameObject.Find("RemoteAvatar(Clone)/body/body_renderPart_0/root_JNT/body_JNT/chest_JNT/neckBase_JNT/neck_JNT/head_JNT").transform;
        vrSet[1].transform.parent = GameObject.Find("RemoteAvatar(Clone)/hand_left").transform;
        vrSet[2].transform.parent = GameObject.Find("RemoteAvatar(Clone)/hand_right").transform;
        for(int i = 0; i < vrSet.Length; i++)
        {
            vrSet[i].transform.localPosition = Vector3.zero;
            vrSet[i].transform.localRotation = Quaternion.identity;
            vrSet[i].transform.localScale = Vector3.one;

        }
        vrSet[1].transform.localRotation = Quaternion.Euler(new Vector3(-90.0f, 90.0f, 0.0f));
        vrSet[2].transform.localRotation = Quaternion.Euler(new Vector3(-90.0f, -90.0f, 0.0f));
    }
}
