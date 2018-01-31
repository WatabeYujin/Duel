using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour 
{
    /************************************************
     * クラス変数
     ***********************************************/

    [SerializeField]
    private float bulletSpeed = 4000;
    [SerializeField]
    private float destroyTime = 4.0f;
    [SerializeField]
    private int atk = 10;
    private Rigidbody brigidbody;

    /************************************************
     * プロパティ
     ***********************************************/

    //持ち主の判別
    public bool IsMine
    {
        get;
        private set;
    }

    public int ATK
    {
        get { return atk; }
    }

    /************************************************
     * クラス関数
     ***********************************************/

    void Awake()
    {
        atk = 10;
        brigidbody = GetComponent<Rigidbody>();
    }
    // Use this for initialization
    void Start () 
    {
        brigidbody.AddForce(transform.forward * bulletSpeed);

        //発射されてから一定時間経過したら破棄する
        Destroy(gameObject, destroyTime);
	}
    //----------------------------------------------

    void OnTriggerEnter(Collider collider)
    {
        if (IsMine)
        {
            //当たった部位によってATKの値変更
            switch (collider.gameObject.tag)
            {
                case "Middle":
                    atk = 10;
                    break;
                case "Right":
                    atk = 20;
                    break;
                case "Left":
                    atk = 40;
                    break;
                default:
                    Debug.Log(collider.gameObject.name);
                    Destroy(gameObject);
                    return;
            }
            Enemy.Instance.DamageDeli(atk, collider.gameObject.tag);
        }
        

        Destroy(gameObject);
    }
    //-----------------------------------------------

    public void Initialize(bool isMine)
    {
        IsMine = isMine;
    }
}
