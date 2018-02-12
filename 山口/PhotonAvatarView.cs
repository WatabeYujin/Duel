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

    private PhaseCheck phaseCheck;
    private Standby standby;
    private Cointoss cointoss;
    private Battle battle;
    private ScoreBoad scoreBoard;

    int wave;

    public AudioSource audioClip;
    OVRHapticsClip hapticsClip;

    private PhotonView photonView;
    private OvrAvatar ovrAvatar;
    private OvrAvatarRemoteDriver remoteDriver;
    private WaistGun waistGun;
    private bool isCoinShot = true;

    private List<byte[]> packetData;

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
            hapticsClip = new OVRHapticsClip(audioClip.clip);

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

        if(SceneManager.GetActiveScene().name == "alpha" && phaseCheck == null)
        {
            GameObject batteleController = GameObject.Find("BatteleController");
            phaseCheck = batteleController.GetComponent<PhaseCheck>();
            standby = batteleController.GetComponent<Standby>();
            cointoss = batteleController.GetComponent<Cointoss>();
            battle = batteleController.GetComponent<Battle>();
            scoreBoard = batteleController.GetComponent<ScoreBoad>();
            //waistGun = GameObject.FindGameObjectWithTag("SetPos").GetComponent<WaistGun>();
        }

        if (phaseCheck == null) return;

        //フライング処理
        if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.CointossPhase && !standby.PlayerReady && isCoinShot)
        {
            isCoinShot = false;
            bulletSpeed = 5000;
            cointoss.PassBattleStart();
        }

        //OculusTouchの右アナログスティックを下に倒す
        if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickDown) ) 
        {
            //リザルト送り
            if(phaseCheck.NowPhaseGet == PhaseCheck.Phase.ResultPhase)
            {
                isCoinShot = true;
                bulletSpeed = 100000;
                scoreBoard.PassResultCheck(PhotonNetwork.player.ID -1);
            }
        }
        //OculusTouchの右人差し指トリガー
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) && !battle.PlayerShot)
        {
            //バトル
            if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.CointossPhase || phaseCheck.NowPhaseGet == PhaseCheck.Phase.BattlePhase)
            {
                OVRHaptics.RightChannel.Mix(hapticsClip);
                muzzleFlash.Play();

                Vector3 gunP = gun.position;
                Quaternion gunQ = gun.rotation;
                Vector3 gunForward = gun.forward;
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
        
        if(playerId == PhotonNetwork.player.ID)
        {
            audioClip.Play();
            //発射フラグ
            battle.PassShotCheck(PhotonNetwork.player.ID - 1);
            //弾を発射
            var obj = Instantiate(Resources.Load("Bullet2(1)"), gunP, gunQ) as GameObject;
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
            var obj = Instantiate(Resources.Load("Bullet2(1)"), gunP, gunQ) as GameObject;
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
            Debug.Log(writer);
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
        Debug.Log("OnPhotonSerializeView");
        if (stream.isWriting)
        {
            Debug.Log("isWriting");
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
            Debug.Log("isReading");
            int num = (int)stream.ReceiveNext();

            for (int counter = 0; counter < num; ++counter)
            {
                byte[] data = (byte[])stream.ReceiveNext();

                DeserializeAndQueuePacketData(data);
            }
        }
    }
}
