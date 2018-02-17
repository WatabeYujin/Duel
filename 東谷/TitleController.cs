using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour {

    OVRManager ovrM;
    OVRCameraRig ovrCR;

    GameObject ovrCameraRig;
    GameObject localAvatar;
    GameObject gunman;

    [SerializeField]
    private Transform[] tutPos, hidePos;
    private Vector3 defPos;
    private Quaternion defRot;

    bool[] loadStanby = new bool[2];

    PhotonView pv;

    //---------------------------------
    // 弾発射関係
    //---------------------------------
    private GameObject bullet;
    PhotonAvatarView.GunData gunData;

    //-----------------------------------------------------------
        
    void Start ()
    {
        localAvatar = GameObject.Find("LocalAvatar(Clone)");
        ovrCameraRig = GameObject.Find("OVRCameraRig");
        gunman = GameObject.Find("Gunman");
        bullet = Resources.Load("Bullet") as GameObject;
        ovrM = ovrCameraRig.GetComponent<OVRManager>();
        pv = GetComponent<PhotonView>();

        gunman.SetActive(false);
        defPos = localAvatar.transform.position;
        defRot = localAvatar.transform.rotation;
    }

	// Update is called once per frame
	void Update ()
    {
        if (GameObject.Find("LocalAvatar(Clone)") && gunData.gunT == null)
        {
            gunData = GameObject.Find("LocalAvatar(Clone)").GetComponent<PhotonAvatarView>().GetGunData();
            gunData.audioSource.clip = Resources.Load("Audio/SE/fire") as AudioClip;
        }

        // ヘッドセットつけているかフラグ
        if (!ovrM.isUserPresent)
        {
            // VRのカメラ
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = false;
            localAvatar.transform.position = hidePos[PhotonNetwork.player.ID - 1].position;
            localAvatar.transform.rotation = hidePos[PhotonNetwork.player.ID - 1].rotation;
            ovrCameraRig.transform.position = hidePos[PhotonNetwork.player.ID - 1].position;
            ovrCameraRig.transform.rotation = hidePos[PhotonNetwork.player.ID - 1].rotation;
        }
        else
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = true;
            localAvatar.transform.position = tutPos[PhotonNetwork.player.ID - 1].position;
            localAvatar.transform.rotation = tutPos[PhotonNetwork.player.ID - 1].rotation;
            ovrCameraRig.transform.position = tutPos[PhotonNetwork.player.ID - 1].position;
            ovrCameraRig.transform.rotation = tutPos[PhotonNetwork.player.ID - 1].rotation;
        }

        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            OVRHaptics.RightChannel.Mix(gunData.hapticsClip);
            gunData.muzzleFlash.Play();
            gunData.audioSource.Play();
            //弾を発射
            var obj = Instantiate(Resources.Load("Bullet"), gunData.gunT.position, gunData.gunT.rotation) as GameObject;
            obj.GetComponent<Rigidbody>().AddForce(gunData.gunT.forward * gunData.bulletSpeed);
            //Bulletに持ち主の判別をさせる
            var bullet = obj.GetComponent<Bullet>();
            bullet.Initialize(true);
            //発射されてから一定時間経過したら破棄する
            Destroy(obj.gameObject, 5f);
        }

        if (loadStanby[0] && loadStanby[1])
        {
            SceneManager.LoadScene("alpha");
            ResetPlayer();
        }
    }

    /// <summary>
    /// チュートリアル用に位置を調整したのをリセットする
    /// </summary>
    public void ResetPlayer()
    {
        localAvatar.transform.position = defPos;
        ovrCameraRig.transform.position = defPos;
        ovrCameraRig.transform.rotation = defRot;
        gunman.SetActive(true);
    }

    public void PassLoadNextScene(bool flag)
    {
        pv.RPC("LoadNextScene", PhotonTargets.AllViaServer, PhotonNetwork.player.ID - 1);
    }

    [PunRPC]
    public void LoadNextScene(int id)
    {
        loadStanby[id] = true;
        Debug.Log(loadStanby[0] + " " + loadStanby[1]);
    }
}
