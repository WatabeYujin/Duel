using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
public class Bullet : PunBehaviour {

    ///************************************************
    // * クラス変数
    // ***********************************************/
    [SerializeField]
    private int atk = 10;

    private PhotonView bPhotonView = null;
    private GameObject Obj;
    private Transform gunMarkerTra;

    private Vector3 oldPosition = Vector3.zero;
    RaycastHit hit;
    private bool isMine;
    private float hitTime;
    private Result result;
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
        gunMarkerTra = GameObject.Find("GunMarker").transform;
        result = GameObject.Find("BatteleController").GetComponent<Result>();
    }
    void Update()
    {
        float maxDistance = Vector3.Distance(transform.position, oldPosition);
        Debug.DrawRay(oldPosition, transform.forward * maxDistance, Color.red, Time.deltaTime);
        bool isHit = Physics.BoxCast(oldPosition, transform.localScale, transform.forward * maxDistance,out hit,Quaternion.identity, maxDistance);
        if (isHit && hit.collider.tag != "sphere" && isMine)
        {
            gunMarkerTra.parent = hit.collider.transform;
            gunMarkerTra.position = hit.point - transform.root.forward * 0.001f;
            gunMarkerTra.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);

            Vector3 lossScale = gunMarkerTra.lossyScale;
            Vector3 localScale = gunMarkerTra.localScale;
            gunMarkerTra.localScale = new Vector3(
                localScale.x / lossScale.x * Vector3.one.x,
                localScale.y / lossScale.y * Vector3.one.y,
                localScale.z / lossScale.z * Vector3.one.z
            );

            switch (hit.collider.gameObject.tag)
            {
                case "Head":
                    Debug.Log(hit.collider.name);
                    result.PassHitCheck(PhotonNetwork.player.ID-1, Result.HitPosition.Head);
                    DestroyImmediate(gameObject);
                    break;
                case "BodyAndArms":
                    Debug.Log(hit.collider.name);
                    result.PassHitCheck(PhotonNetwork.player.ID-1, Result.HitPosition.Arms_Body);
                    DestroyImmediate(gameObject);

                    break;

                case "Legs":
                    Debug.Log(hit.collider.name);
                    result.PassHitCheck(PhotonNetwork.player.ID-1, Result.HitPosition.Legs);
                    DestroyImmediate(gameObject);
                    break;

                default:
                    Debug.Log(hit.collider.name);
                    result.PassHitCheck(PhotonNetwork.player.ID-1, Result.HitPosition.Null);
                    DestroyImmediate(gameObject);
                    return;
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
