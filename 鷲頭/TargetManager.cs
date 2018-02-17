using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TargetManager : MonoBehaviour {

    public enum TargetType
    {
        Target5,
        Target10,
        Target50,
        Target100,
    }

    public TargetType type;

    [SerializeField]
    private GameObject target;
    [SerializeField]
    private GameObject targetSet;
    public static int hitCount = 0;
	
	// Update is called once per frame
	void Update () {
        if(hitCount == 5){
            Invoke("ResetRotate", 0.6f);
        }
    }   

    public void Rotate(){
        target.transform.DORotate(new Vector3(90, 0, 0), 0.5f);
    }

    public void ResetRotate(){
        target.transform.DORotate(new Vector3(0, 0, 0), 0.5f);
        hitCount = 0;
    }

    public void OnTriggerEnter(Collider col){
        switch (type){
            case TargetType.Target5:
                Rotate();
                hitCount++;
                Manager.score += 5;
                break;
            case TargetType.Target10:
                Rotate();
                hitCount++;
                Manager.score += 10;
                break;
            case TargetType.Target50:
                Rotate();
                hitCount++;
                Manager.score += 50;
                break;
            case TargetType.Target100:
                Rotate();
                hitCount++;
                Manager.score += 100;
                break;
            default:
                break;
        }
        Destroy(col.gameObject);
        Manager.hitCheck = true;
    }
    
}
