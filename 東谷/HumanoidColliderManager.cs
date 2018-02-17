using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace BodyColliderSystem
{
    /// <summary>
    /// Transform型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BodyPropertyObj<T> where T : Object
    {
        public T head;
        public T spine;
        public T hip;
        public T leftArm;
        public T leftForeArm;
        public T rightArm;
        public T rightForeArm;
        public T leftUpLeg;
        public T leftLeg;
        public T rightUpLeg;
        public T rightLeg;
        // 11
    }

    public class HumanoidColliderManager : ScriptableWizard
    {

        //------------------------------------------------------------------------------
        // 変数
        //------------------------------------------------------------------------------
        #region 体のTransformをクラスから作成
        [System.Serializable]
        class BodyTransform : BodyPropertyObj<Transform> { };

        [SerializeField]
        private BodyTransform bodyTransform;    // 実行データ
        private static BodyTransform saveBodyTransform; // 値保存用
        #endregion

        private List<Transform> listBodyTransform;

        enum BodyEnum : int
        {
            head,
            spine,
            hip,
            lefArm,
            lefForeArm,
            righArm,
            righForeArm,
            lefUpLeg,
            lefLeg,
            righUpLeg,
            righLeg,
        }

        public float armRadius;
        public float legRadius;

        //------------------------------------------------------------------------------
        // 関数群
        //------------------------------------------------------------------------------

        void Awake()
        {
            bodyTransform = saveBodyTransform;
            Debug.Log("Awake");
            
        }

        [MenuItem("MyMenu/AddBodyCollider")]
        static void CreateWizard()
        {
            //　ウィザードを表示
            ScriptableWizard.DisplayWizard<HumanoidColliderManager>("AddBodyCollider", "Add", "DestroyCollider");
        }

        //　ウィザードで更新があった時に実行
        void OnWizardUpdate()
        {
            saveBodyTransform = bodyTransform;
        }

        //　ウィザードの決定ボタンを押した時に実行
        void OnWizardCreate()
        {
            listBodyTransform = new List<Transform> {
                bodyTransform.head,
                bodyTransform.spine,
                bodyTransform.hip,
                bodyTransform.leftArm,
                bodyTransform.leftForeArm,
                bodyTransform.rightArm,
                bodyTransform.rightForeArm,
                bodyTransform.leftUpLeg,
                bodyTransform.leftLeg,
                bodyTransform.rightUpLeg,
                bodyTransform.rightLeg,
            };

            for (int i = 0; i <= (int)BodyEnum.righLeg; i++)
            {
                Transform targetTra = listBodyTransform[i];
                if (AddColliderCheck(targetTra.gameObject)) continue;
                switch (i)
                {
                    case (int)BodyEnum.head:    //頭はsphere
                        SphereCollider targetSc = SetColliderComponent<SphereCollider>(targetTra.gameObject);
                        SetHead(targetSc);
                        break;
                    case (int)BodyEnum.spine:   //体はBox
                        BoxCollider targetBcs = SetColliderComponent<BoxCollider>(targetTra.gameObject);
                        SetSpine(targetBcs);
                        break;
                    case (int)BodyEnum.hip:
                        BoxCollider targetBch = SetColliderComponent<BoxCollider>(targetTra.gameObject);
                        SetHip(targetBch);
                        break;
                    case (int)BodyEnum.lefArm:  //四肢はカプセル
                    case (int)BodyEnum.lefForeArm:
                    case (int)BodyEnum.lefLeg:
                    case (int)BodyEnum.lefUpLeg:
                    case (int)BodyEnum.righArm:
                    case (int)BodyEnum.righForeArm:
                    case (int)BodyEnum.righLeg:
                    case (int)BodyEnum.righUpLeg:
                        CapsuleCollider targetCap = SetColliderComponent<CapsuleCollider>(targetTra.gameObject);
                        SetCaps(targetCap, i);
                        break;
                    default:
                        Debug.LogError("なんかおかしい");
                        break;
                }
            }
        }

        //胴体全体を覆うコライダーを腰から上(spine)と尻(hip)に分ける
        BoxCollider SetSpine(BoxCollider spine)
        {
            float height = bodyTransform.leftArm.transform.position.y - bodyTransform.hip.transform.position.y;
            float width = Mathf.Abs(bodyTransform.leftArm.transform.position.x - bodyTransform.rightArm.transform.position.x);
            float depth = width / 2;

            float hHeight = bodyTransform.hip.transform.position.y - bodyTransform.leftUpLeg.transform.position.y;

            spine.size = new Vector3(width, height, depth);
            spine.isTrigger = true;
            spine.center = new Vector3(0, 0.2f, 0); // 直す

            return spine;
        }
        BoxCollider SetHip(BoxCollider hip)
        {
            float width = Mathf.Abs(bodyTransform.leftArm.transform.position.x - bodyTransform.rightArm.transform.position.x);
            float depth = width / 2;
            float height = (bodyTransform.hip.transform.position.y - bodyTransform.leftUpLeg.transform.position.y)*2;

            hip.size = new Vector3(width, height, depth);
            hip.isTrigger = true;
            hip.center = new Vector3(0, 0, 0);

            return hip;
        }
        SphereCollider SetHead(SphereCollider head)
        {
            float radius = (bodyTransform.head.position.y - bodyTransform.leftArm.position.y)/2;
            head.isTrigger = true;
            head.radius = radius;
            return head;
        }
        CapsuleCollider SetCaps(CapsuleCollider capsuleCol, int bodyEnum)
        {
            capsuleCol.isTrigger = true;
            float height;
            Vector3 center;

            if (bodyEnum <= 6) // 腕
            {
                capsuleCol.radius = armRadius;
                capsuleCol.direction = 0;
                height = Mathf.Abs(bodyTransform.leftArm.position.x - bodyTransform.leftForeArm.position.x);
                center = Vector3.zero;

                switch (bodyEnum)
                {
                    case (int)BodyEnum.lefArm: // 肩からひじ
                        center = new Vector3(-(height / 2), 0, 0);
                        break;
                    case (int)BodyEnum.lefForeArm: // ひじから手
                        center = new Vector3(-(height / 2) - armRadius, 0, 0);
                        height += armRadius;
                        break;
                    case (int)BodyEnum.righArm:
                        center = new Vector3(height / 2, 0, 0);
                        break;
                    case (int)BodyEnum.righForeArm:
                        center = new Vector3(height / 2 + armRadius, 0, 0);
                        height += armRadius;
                        break;
                }
            }
            else // 足
            {
                capsuleCol.radius = legRadius;
                capsuleCol.direction = 1;
                height = Mathf.Abs(bodyTransform.leftLeg.position.y - bodyTransform.leftUpLeg.position.y);
                center = new Vector3(0, -(height / 2), 0);
                switch (bodyEnum)
                {
                    case (int)BodyEnum.lefLeg:
                    case (int)BodyEnum.righLeg:
                        height += legRadius*2;
                        center.y -= legRadius;
                        break;
                }
                capsuleCol.center = center;
                capsuleCol.height = height;
            }

            capsuleCol.center = center;
            capsuleCol.height = height;
            return capsuleCol;
        }

        /// <summary>
        /// コライダーをくっつける　返り値はコライダー型
        /// </summary>
        /// <param name="bp">体の部位のトランスフォーム</param>
        /// <param name="colliderType">コライダーの種類　0=sphere　1=capsule　2=Box</param>
        T SetColliderComponent<T>(GameObject bp) where T : Object
        {
            T colliderType;
            colliderType = bp.AddComponent(typeof(T)) as T;
            return colliderType;
        }

        /// <summary>
        /// コライダーがあるとTrue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="colliderType">調査対象</param>
        /// <returns></returns>
        bool AddColliderCheck(GameObject obj)
        {
            if (obj.GetComponent<BoxCollider>() != null ||
                obj.GetComponent<SphereCollider>() != null ||
                obj.GetComponent<CapsuleCollider>() != null
                )
            {
                Debug.Log(obj.name + " には、すでに何らかのコライダーがあります。");
                return true;
            }
            return false;
        }

        void OnWizardOtherButton()
        {
            listBodyTransform = new List<Transform> {
                bodyTransform.head,
                bodyTransform.spine,
                bodyTransform.hip,
                bodyTransform.leftArm,
                bodyTransform.leftForeArm,
                bodyTransform.rightArm,
                bodyTransform.rightForeArm,
                bodyTransform.leftUpLeg,
                bodyTransform.leftLeg,
                bodyTransform.rightUpLeg,
                bodyTransform.rightLeg,
            };

            for (int i = 0; i < listBodyTransform.Count; i++)
            {
                if(listBodyTransform[i].GetComponent<BoxCollider>() != null)
                {
                    DestroyImmediate(listBodyTransform[i].GetComponent<BoxCollider>());
                }
                else if (listBodyTransform[i].GetComponent<SphereCollider>() != null)
                {
                    DestroyImmediate(listBodyTransform[i].GetComponent<SphereCollider>());
                }
                else if (listBodyTransform[i].GetComponent<CapsuleCollider>() != null)
                {
                    DestroyImmediate(listBodyTransform[i].GetComponent<CapsuleCollider>());
                }
                else
                {
                    Debug.LogWarning(listBodyTransform[i].name + ": コライダーが見当たりません。");
                }
            }
            Debug.Log("Done!");
        }
    }
}
#endif