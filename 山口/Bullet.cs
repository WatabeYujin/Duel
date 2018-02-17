using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.SceneManagement;

public class Bullet : PunBehaviour {

    ///************************************************
    // * クラス変数
    // ***********************************************/
    [SerializeField]
    private int atk = 10;

    private PhotonView bPhotonView = null;
    private GameObject Obj;
    private GameObject gunMarker;

    GameObject gunMarkerClone;

    private Vector3 defaultScale = Vector3.zero;
    private Vector3 oldPosition = Vector3.zero;
    RaycastHit hit;
    private bool isMine;
    private float hitTime;
    private GameCommonData gameData;
    //private Result result;
    /************************************************
    * プロパティ
    ***********************************************/
    public bool IsMine
    {
        get { return isMine; }
        set{ isMine = value; }
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("PlayerBullet:"+PhotonNetwork.player.ID);
        oldPosition = transform.position;
        gunMarker = Resources.Load("GunMarker") as GameObject;
        if (SceneManager.GetActiveScene().name != "alpha") return;
        gameData = GameObject.Find("BatteleController").GetComponent<GameCommonData>();
    }
    void Update()
    {
        float maxDistance = Vector3.Distance(transform.position, oldPosition);
        Debug.DrawRay(oldPosition, transform.forward * maxDistance, Color.red, Time.deltaTime);
        bool isHit = Physics.BoxCast(oldPosition, transform.localScale, transform.forward * maxDistance,out hit,Quaternion.identity, maxDistance);
        //Debug.Log(gunMarker);
        if (isHit && hit.collider.tag != tag && isMine)
        {
            if (gunMarkerClone != null) return;
            gunMarkerClone = Instantiate(gunMarker);
            gunMarkerClone.transform.parent = hit.collider.transform;
            gunMarkerClone.transform.position = hit.point - transform.root.forward * 0.001f;
            gunMarkerClone.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            defaultScale = gunMarkerClone.transform.lossyScale;

            Vector3 lossScale = gunMarkerClone.transform.lossyScale;
            Vector3 localScale = gunMarkerClone.transform.localScale;
            gunMarkerClone.transform.localScale = new Vector3(
                localScale.x / lossScale.x * defaultScale.x,
                localScale.y / lossScale.y * defaultScale.y,
                localScale.z / lossScale.z * defaultScale.z
            );

            //if (SceneManager.GetActiveScene().name != "alpha") return;
            switch (hit.collider.gameObject.tag)
            {
                case "Head":
                    Debug.Log(hit.collider.name);
                    gameData.battle.HitCheck(GameCommonData.HitPosition.Head);
                    DestroyImmediate(gameObject);
                    break;
                case "BodyAndArms":
                    Debug.Log(hit.collider.name);
                    gameData.battle.HitCheck(GameCommonData.HitPosition.Arms_Body);
                    DestroyImmediate(gameObject);
                    break;

                case "Legs":
                    Debug.Log(hit.collider.name);
                    gameData.battle.HitCheck(GameCommonData.HitPosition.Legs);
                    DestroyImmediate(gameObject);
                    break;

                case "Target":
                    Debug.Log(hit.collider.name);
                    hit.collider.gameObject.GetComponent<TargetManager>().HitBullet();
                    DestroyImmediate(gameObject);
                    break;

                case "SceneCube":
                    Debug.Log(hit.collider.name);
                    hit.collider.gameObject.GetComponent<SceneMoveChecker>().HitBullet(IsMine);
                    DestroyImmediate(gameObject);
                    break;

                default:
                    Debug.Log(hit.collider.name);
                    if (SceneManager.GetActiveScene().name != "alpha") break;
                    gameData.battle.HitCheck(GameCommonData.HitPosition.Null);
                    DestroyImmediate(gameObject);
                    break;
            }
        }
        else
        {
            oldPosition = transform.position;
        }
    }
    
    //-----------------------------------------------

    public void Initialize(bool isMine)
    {
        IsMine = isMine;
    }
}
