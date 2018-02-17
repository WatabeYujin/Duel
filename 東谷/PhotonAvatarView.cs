using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonAvatarView : MonoBehaviour
{

    [SerializeField]
    private Transform gun;
    [SerializeField]
    private int bulletSpeed = 100000;
    [SerializeField]
    private float bulletDestroyTime = 8.0f;
    [SerializeField]
    private ParticleSystem muzzleFlash;
    [SerializeField]
    private Animator gunAnimator;

    //private PhaseCheck phaseCheck;
    //private Standby standby;
    //private Cointoss cointoss;
    //private Battle battle;
    //private ScoreBoad scoreBoard;

    private GameCommonData gameData;

    int wave;

    public AudioSource audioSource;
    OVRHapticsClip hapticsClip = null;

    private PhotonView photonView;
    private OvrAvatar ovrAvatar;
    private OvrAvatarRemoteDriver remoteDriver;
    private WaistGun waistGun;
    private bool isCoinShot = true;

    private List<byte[]> packetData;

    public struct GunData
    {
        public ParticleSystem muzzleFlash;
        public Transform gunT;
        public AudioSource audioSource;
        public OVRHapticsClip hapticsClip;
        public int bulletSpeed;
    }


    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        photonView = GetComponent<PhotonView>();
        //Debug.Log(photonView.isMine + ":Start:" + name);
        if (photonView.isMine)
        {
            ovrAvatar = GetComponent<OvrAvatar>();
            hapticsClip = new OVRHapticsClip(audioSource.clip);

            packetData = new List<byte[]>();
        }
        else
        {
            remoteDriver = GetComponent<OvrAvatarRemoteDriver>();
        }
    }

    private void Update()
    {
        
        if (!photonView.isMine) return;

        if(SceneManager.GetActiveScene().name == "alpha" && gameData == null)
        {
            GameObject batteleController = GameObject.Find("BatteleController");
            gameData = batteleController.GetComponent<GameCommonData>();
            //waistGun = GameObject.FindGameObjectWithTag("SetPos").GetComponent<WaistGun>();
        }

        if (gameData == null) return;

        if (OVRInput.GetDown(OVRInput.RawButton.A) && gameData.GetNowPhase == GameCommonData.Phase.GameSetPhase)
        {
            SceneManager.LoadScene("Title");
        }

        //フライング処理  コイントスフェーズで、スタンバイがFalseで、コインショットがTrue(一回しか通らせないためのフラグ)
        if (gameData.GetNowPhase == GameCommonData.Phase.CointossPhase && !gameData.myData.IsStanby && isCoinShot)
        {
            isCoinShot = false;
            bulletSpeed = 5000;
            gameData.cointoss.BattleStart();
        }

        //OculusTouchの右アナログスティックを下に倒す
        if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickDown) ) 
        {
            //リザルト送り
            if(gameData.GetNowPhase == GameCommonData.Phase.ResultPhase)
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                gunAnimator.SetBool("isSet", true);
                gunAnimator.SetBool("isFire", false);
                audioSource.clip = Resources.Load("Audio/SE/hammerUp") as AudioClip;
                audioSource.Play();
                
                isCoinShot = true;
                bulletSpeed = 100000;
                //gameData.scoreBoard.PassResultCheck(PhotonNetwork.player.ID -1);
                gameData.myData.IsEndResult = true;
                gameData.PassPlayerUpdata();
            }
        }
        //OculusTouchの右人差し指トリガー
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) && !gameData.myData.IsShot)
        {
            //バトル
            if (gameData.GetNowPhase == GameCommonData.Phase.CointossPhase || gameData.GetNowPhase == GameCommonData.Phase.BattlePhase)
            {
                OVRHaptics.RightChannel.Mix(hapticsClip);
                muzzleFlash.Play();
                gunAnimator.SetBool("isFire", true);

                //自分が撃ったフラグ
                gameData.myData.IsShot = true;
                gameData.PassPlayerUpdata();

                Vector3 gunP = gun.position;
                Quaternion gunQ = gun.rotation;
                Vector3 gunForward = gun.forward;
                audioSource.clip = Resources.Load("Audio/SE/fire") as AudioClip;
                audioSource.Play();
                photonView.RPC("Shoot", PhotonTargets.AllViaServer, PhotonNetwork.player.ID, gunP, gunQ, gunForward, bulletSpeed);
            }  
        } 
    }

    /// <summary>
    /// 弾を撃つところ
    /// </summary>
    /// <param name="playerId">所有者</param>
    /// <param name="gunP">初期位置</param>
    /// <param name="gunQ">発射角度</param>
    /// <param name="gunForward">正面の向き</param>
    /// <param name="bulletSpeed">弾のスピード</param>
    [PunRPC]
    public void Shoot(int playerId, Vector3 gunP, Quaternion gunQ, Vector3 gunForward, int bulletSpeed)
    {

        if (playerId == PhotonNetwork.player.ID)
        {
            //発射フラグ
            //gameData.battle.PassShotCheck(PhotonNetwork.player.ID - 1);
            
            //弾を発射
            var obj = Instantiate(Resources.Load("Bullet"), gunP, gunQ) as GameObject;
            obj.GetComponent<Rigidbody>().AddForce(gunForward * bulletSpeed);
            //Bulletに持ち主の判別をさせる
            var bullet = obj.GetComponent<Bullet>();
            bullet.Initialize(playerId == PhotonNetwork.player.ID);

            //発射されてから一定時間経過したら破棄する
            Destroy(obj.gameObject, bulletDestroyTime);
        }
        else
        {
            //弾を発射
            var obj = Instantiate(Resources.Load("Bullet"), gunP, gunQ) as GameObject;
            obj.GetComponent<Rigidbody>().AddForce(gunForward * bulletSpeed);
            //Bulletに持ち主の判別をさせる
            var bullet = obj.GetComponent<Bullet>();
            bullet.Initialize(playerId == PhotonNetwork.player.ID);

            //発射されてから一定時間経過したら破棄する
            Destroy(obj.gameObject, bulletDestroyTime);
        }
    }



    public void OnEnable()
    {
        Invoke("Flags", Time.deltaTime);
        
    }
    void Flags()
    {
        if (photonView.isMine)
        {
            ovrAvatar.RecordPackets = true;
            ovrAvatar.PacketRecorded += OnLocalAvatarPacketRecorded;

        }
    }

    public void OnDisable()
    {
        if (photonView.isMine)
        {
            ovrAvatar.RecordPackets = false;
            ovrAvatar.PacketRecorded -= OnLocalAvatarPacketRecorded;
        }
    }

    

    private int localSequence;

    public void OnLocalAvatarPacketRecorded(object sender, OvrAvatar.PacketEventArgs args)
    {
        using (MemoryStream outputStream = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(outputStream);

            var size = Oculus.Avatar.CAPI.ovrAvatarPacket_GetSize(args.Packet.ovrNativePacket);
            byte[] data = new byte[size];
            Oculus.Avatar.CAPI.ovrAvatarPacket_Write(args.Packet.ovrNativePacket, size, data);

            writer.Write(localSequence++);
            writer.Write(size);
            writer.Write(data);
            //Debug.Log(writer);
            packetData.Add(outputStream.ToArray());
        }
    }

    private void DeserializeAndQueuePacketData(byte[] data)
    {
        using (MemoryStream inputStream = new MemoryStream(data))
        {
            BinaryReader reader = new BinaryReader(inputStream);
            int remoteSequence = reader.ReadInt32();

            int size = reader.ReadInt32();
            byte[] sdkData = reader.ReadBytes(size);

            System.IntPtr packet = Oculus.Avatar.CAPI.ovrAvatarPacket_Read((System.UInt32)data.Length, sdkData);
            remoteDriver.QueuePacket(remoteSequence, new OvrAvatarPacket { ovrNativePacket = packet });
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log("OnPhotonSerializeView");
        if (stream.isWriting)
        {
            //Debug.Log("isWriting");
            if (packetData.Count == 0)
            {
                Debug.Log("packetDataなし");
                return;
            }

            stream.SendNext(packetData.Count);

            foreach (byte[] b in packetData)
            {
                stream.SendNext(b);
            }

            packetData.Clear();
        }

        if (stream.isReading)
        {
            //Debug.Log("isReading");
            int num = (int)stream.ReceiveNext();

            for (int counter = 0; counter < num; ++counter)
            {
                byte[] data = (byte[])stream.ReceiveNext();

                DeserializeAndQueuePacketData(data);
            }
        }
    }

    public GunData GetGunData()
    {
        GunData gd = new GunData { };
        gd.muzzleFlash = muzzleFlash;
        gd.gunT = gun;
        gd.audioSource = audioSource;
        gd.hapticsClip = hapticsClip;
        gd.bulletSpeed = bulletSpeed;
        return gd;
    }

}
