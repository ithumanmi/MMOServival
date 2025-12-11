using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Timers;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using static GlobalEnum;
using static UnityEngine.EventSystems.EventTrigger;

namespace CreatorKitCodeInternal {
    public class CharacterControl : MonoBehaviour, 
        AnimationControllerDispatcher.IAttackFrameReceiver,
        AnimationControllerDispatcher.IFootstepFrameReceiver
    {

        public static CharacterControl Instance { get; protected set; }

        public bool isInLobby;
        public float Speed = 10.0f;

        public CharacterData Data => m_CharacterData;
        public CharacterData CurrentTarget => m_CurrentTargetCharacterData;

        public Transform WeaponLocator;

        public List<WeaponRemotePosition> WeaponRemotePositions;
        public List<RemoteWeapon> weaponRemotes = new List<RemoteWeapon>();
        public EnergyManager energyManager;

        [Header("Audio")]
        public AudioClip[] SpurSoundClips;
    
        Vector3 m_LastRaycastResult;
        Animator m_Animator;
        //NavMeshAgent m_Agent;
        CharacterData m_CharacterData;
        private Vector2 moveInput;

        HighlightableObject m_Highlighted;

        RaycastHit[] m_RaycastHitCache = new RaycastHit[16];

        int m_SpeedParamID;
        int m_AttackParamID;
        int m_HitParamID;
        int m_FaintParamID;
        int m_RespawnParamID;

        bool m_IsKO = false;
        float m_KOTimer = 0.0f;

        int m_InteractableLayer;
        int m_LevelLayer;
        Collider m_TargetCollider;
        InteractableObject m_TargetInteractable = null;
        Camera m_MainCamera;

        //NavMeshPath m_CalculatedPath;

        CharacterAudio m_CharacterAudio;

        int m_TargetLayer;
        CharacterData m_CurrentTargetCharacterData = null;
        //this is a flag that tell the controller it need to clear the target once the attack finished.
        //usefull for when clicking elwswhere mid attack animation, allow to finish the attack and then exit.
        bool m_ClearPostAttack = false;

        SpawnPoint m_CurrentSpawn = null;
    
        enum State
        {
            DEFAULT,
            HIT,
            ATTACKING
        }

        State m_CurrentState;

        void Awake()
        {
            Instance = this;
            m_MainCamera = Camera.main;
        }

        // Start is called before the first frame update
        void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            //m_CalculatedPath = new NavMeshPath();

            //m_Agent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponentInChildren<Animator>();

            //m_Agent.speed = Speed;
            //m_Agent.angularSpeed = 360.0f;

            m_LastRaycastResult = transform.position;

            m_SpeedParamID = Animator.StringToHash("Speed");
            m_AttackParamID = Animator.StringToHash("Attack");
            m_HitParamID = Animator.StringToHash("Hit");
            m_FaintParamID = Animator.StringToHash("Faint");
            m_RespawnParamID = Animator.StringToHash("Respawn");

            m_CharacterData = GetComponent<CharacterData>();

            Debug.Log(gameObject.name);
            GameDataManager.instance.EquipmentData.OnEquiped += HandleEquip;
            GameDataManager.instance.EquipmentData.OnUnequip += HandleUnequip;

            m_CharacterData.Init(false);

            m_InteractableLayer = 1 << LayerMask.NameToLayer("Interactable");
            m_LevelLayer = 1 << LayerMask.NameToLayer("Level");
            m_TargetLayer = 1 << LayerMask.NameToLayer("Target");

            m_CurrentState = State.DEFAULT;

            m_CharacterAudio = GetComponent<CharacterAudio>();
        
            m_CharacterData.OnDamage += () =>
            {
                m_Animator.SetTrigger(m_HitParamID);
                m_CharacterAudio.Hit(transform.position);
            };
            moveInput = transform.position;
        }
        private void HandleEquip(EquipmentItem item)
        {
            if (item.Slot == EquipmentSlot.Weapon || item.Slot == (EquipmentSlot)666)
            {
                var obj = Instantiate(item.WorldObjectPrefab, WeaponLocator, false);
                Helpers.RecursiveLayerChange(obj.transform, LayerMask.NameToLayer("PlayerEquipment"));
            }
        }

        private void HandleUnequip(EquipmentItem item)
        {
            if (item.Slot == EquipmentSlot.Weapon || item.Slot == (EquipmentSlot)666)
            {
                Debug.Log(WeaponLocator);
                foreach (Transform t in WeaponLocator)
                {
                    Debug.Log(t);
                    Destroy(t.gameObject);
                }
            }
        }
        private void OnDestroy()
        {
            Instance = null;
        }
        public void SafelyUnsubscribe()
        {
            GameDataManager.instance.EquipmentData.OnEquiped -= HandleEquip;
            GameDataManager.instance.EquipmentData.OnUnequip -= HandleUnequip;
        }

        // Update is called once per frame
        void Update()
        {
            if (isInLobby)
            {
                return;
            }

            Vector3 pos = transform.position;

            if (m_IsKO)
            {
                //m_KOTimer += Time.deltaTime;
                //if (m_KOTimer > 3.0f)
                //{
                //    GoToRespawn();
                //}

                LevelManager.instance.CallLevelFaild();
                return;
            }

            //The update need to run, so we can check the health here.
            //Another method would be to add a callback in the CharacterData that get called
            //when health reach 0, and this class register to the callback in Start
            //(see CharacterData.OnDamage for an example)
            if (m_CharacterData.damageableBehaviour.Stats.IsDeal())
            {
                m_Animator.SetTrigger(m_FaintParamID);

                //m_Agent.isStopped = true;
                //m_Agent.ResetPath();
                m_IsKO = true;
                m_KOTimer = 0.0f;

                Data.Death();

                m_CharacterAudio.Death(pos);

                return;
            }
            if (isInLobby)
            {
                return;
            }
            Ray screenRay = CameraController.Instance.GameplayCamera.ScreenPointToRay(Input.mousePosition);

            if (m_TargetInteractable != null)
            {

                CheckInteractableRange();

            }

            if (m_CurrentTargetCharacterData != null)
            {
                if (m_CurrentTargetCharacterData.damageableBehaviour.Stats.IsDeal())
                    m_CurrentTargetCharacterData = null;
                else
                    CheckAttack();
            }

            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            if (!Mathf.Approximately(mouseWheel, 0.0f))
            {
                Vector3 view = m_MainCamera.ScreenToViewportPoint(Input.mousePosition);
                if (view.x > 0f && view.x < 1f && view.y > 0f && view.y < 1f)
                    CameraController.Instance.Zoom(-mouseWheel * Time.deltaTime * 20.0f);
            }

            if (Input.GetMouseButtonDown(0))
            { //if we click the mouse button, we clear any previously et targets

                if (m_CurrentState != State.ATTACKING)
                {
                    m_CurrentTargetCharacterData = null;
                    m_TargetInteractable = null;
                }
                else
                {
                    m_ClearPostAttack = true;
                }
            }
            if (m_CurrentTargetCharacterData == null)
            {
                m_CurrentState = State.DEFAULT;
            }

            HandleJoystickInput();

            //if (!EventSystem.current.IsPointerOverGameObject() && m_CurrentState != State.ATTACKING)
            //{
            //    //Raycast to find object currently under the mouse cursor
            //    ObjectsRaycasts(screenRay);

            //    if (Input.GetMouseButton(0))
            //    {
            //        if (m_TargetInteractable == null && m_CurrentTargetCharacterData == null)
            //        //if(m_CurrentTargetCharacterData == null)
            //        {
            //            InteractableObject obj = m_Highlighted as InteractableObject;
            //            if (obj)
            //            {
            //                InteractWith(obj);
            //            }
            //            else
            //            {
            //                //CharacterData data = m_Highlighted as CharacterData;
            //                //if (data != null)
            //                //{
            //                //    m_CurrentTargetCharacterData = data;
            //                //}
            //                //else
            //                //{
            //                MoveCheck(screenRay);

            //                //}
            //            }
            //        }
            //    }
            //}

            //m_Animator.SetFloat(m_SpeedParamID, moveInput.velocity.magnitude / m_Agent.speed);
            m_Animator.SetFloat(m_SpeedParamID, moveInput.sqrMagnitude);
            DetectNearbyEnergy();
            //Keyboard shortcuts
            //if (Input.GetKeyUp(KeyCode.I))
            //    UISystem.Instance.ToggleInventory();
        }
        void HandleJoystickInput()
        {
            // Lấy input từ joystick
            moveInput = new Vector2(SelectWeaponManager.Instance.variableJoystick.Horizontal,
                SelectWeaponManager.Instance.variableJoystick.Vertical);


            if (moveInput.sqrMagnitude > 0.01f)
            {
                Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
                transform.position += moveDirection * Speed * Time.deltaTime;

                // Xoay hướng nhân vật
                transform.forward = moveDirection;
            }
        }
        void GoToRespawn()
        {
            m_Animator.ResetTrigger(m_HitParamID);

            //m_Agent.Warp(m_CurrentSpawn.transform.position);
            //m_Agent.isStopped = true;
            //m_Agent.ResetPath();
            m_IsKO = false;

            m_CurrentTargetCharacterData = null;
            m_TargetInteractable = null;

            m_CurrentState = State.DEFAULT;

            m_Animator.SetTrigger(m_RespawnParamID);

            m_CharacterData.damageableBehaviour.Stats.ChangeHealth(m_CharacterData.damageableBehaviour.Stats.stats.health);
        }

        void ObjectsRaycasts(Ray screenRay)
        {
            bool somethingFound = false;

            //first check for interactable Object
            int count = Physics.SphereCastNonAlloc(screenRay, 1.0f, m_RaycastHitCache, 1000.0f, m_InteractableLayer);
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    InteractableObject obj = m_RaycastHitCache[0].collider.GetComponentInParent<InteractableObject>();
                    if (obj != null && obj.IsInteractable)
                    {
                        SwitchHighlightedObject(obj);
                        somethingFound = true;
                        break;
                    }
                }
            }
            else
            {
                count = Physics.SphereCastNonAlloc(screenRay, 1.0f, m_RaycastHitCache, 1000.0f, m_TargetLayer);

                if (count > 0)
                {
                    CharacterData data = m_RaycastHitCache[0].collider.GetComponentInParent<CharacterData>();
                    if (data != null)
                    {
                        SwitchHighlightedObject(data);
                        somethingFound = true;
                    }
                }
            }

            if (!somethingFound && m_Highlighted != null)
            {
                SwitchHighlightedObject(null);
            }
        }

        void SwitchHighlightedObject(HighlightableObject obj)
        {
            if (m_Highlighted != null) m_Highlighted.Dehighlight();

            m_Highlighted = obj;
            if (m_Highlighted != null) m_Highlighted.Highlight();
        }

        void MoveCheck(Ray screenRay)
        {
            //if (m_CalculatedPath.status == NavMeshPathStatus.PathComplete)
            //{
            //    m_Agent.SetPath(m_CalculatedPath);
            //    m_CalculatedPath.ClearCorners();
            //}

            //if (Physics.RaycastNonAlloc(screenRay, m_RaycastHitCache, 1000.0f, m_LevelLayer) > 0)
            //{
            //    Vector3 point = m_RaycastHitCache[0].point;
            //    //avoid recomputing path for close enough click
            //    if (Vector3.SqrMagnitude(point - m_LastRaycastResult) > 1.0f)
            //    {
            //        NavMeshHit hit;
            //        if (NavMesh.SamplePosition(point, out hit, 0.5f, NavMesh.AllAreas))
            //        {//sample just around where we hit, avoid setting destination outside of navmesh (ie. on building)
            //            m_LastRaycastResult = point;
            //            m_Agent.SetDestination(hit.position);

            //            m_Agent.CalculatePath(hit.position, m_CalculatedPath);
            //        }
            //    }
            //}
        }

        void CheckInteractableRange()
        {
            if (m_CurrentState == State.ATTACKING)
                return;

            Vector3 distance = m_TargetCollider.ClosestPointOnBounds(transform.position) - transform.position;

            if (distance.sqrMagnitude < 1.5f * 1.5f)
            {
                StopAgent();
                m_TargetInteractable.InteractWith(m_CharacterData);
                m_TargetInteractable = null;
            }
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            var targetable = other.GetComponent<CharacterData>();
            if (targetable != null)
            {
                m_Targetables.Add(targetable);
                UpdateClosestTarget();
            }
        }
        private List<CharacterData> m_Targetables = new List<CharacterData>(); // Danh sách các đối tượng trong phạm vi

        protected virtual void OnTriggerExit(Collider other)
        {
            var targetable = other.GetComponent<CharacterData>();
            if (targetable != null)
            {
                m_Targetables.Remove(targetable);
                UpdateClosestTarget();
            }
        }
        private void UpdateClosestTarget()
        {
            float closestDistance = float.MaxValue;
            CharacterData closestTarget = null;

            foreach (var target in m_Targetables)
            {
                if (target == null) continue; // Kiểm tra xem đối tượng có tồn tại hay không

                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }

            m_CurrentTargetCharacterData = closestTarget;
        }

        void DetectNearbyEnergy()
        {
            float detectionRadius = 2f;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.GetComponent<Energy>())
                {
                    Energy obj = hitCollider.GetComponent<Energy>();
                    energyManager.AddEnergy(obj.UseEnergy());
                }
            }
        }

        void StopAgent()
        {
            //m_Agent.ResetPath();
            //m_Agent.velocity = Vector3.zero;
        }

        void CheckAttack()
        {
            if (m_CurrentState == State.ATTACKING)
                return;

            if (m_CharacterData.CanAttackReach(m_CurrentTargetCharacterData))
            {
                StopAgent();

                //if the mouse button isn't pressed, we do NOT attack
                //if (Input.GetMouseButton(0))
                //{
                Vector3 forward = (m_CurrentTargetCharacterData.transform.position - transform.position);
                forward.y = 0;
                forward.Normalize();


                transform.forward = forward;
                if (m_CharacterData.CanAttackTarget(m_CurrentTargetCharacterData))
                {
                    m_CurrentState = State.ATTACKING;

                    m_CharacterData.AttackTriggered();
                    m_Animator.SetTrigger(m_AttackParamID);
                }
                //}
            }
            else
            {
                //m_Agent.SetDestination(m_CurrentTargetCharacterData.transform.position);
            }
        }

        public void AttackFrame()
        {
            if (m_CurrentTargetCharacterData == null)
            {
                m_ClearPostAttack = false;
                return;
            }

            //if we can't reach the target anymore when it's time to damage, then that attack miss.
            if (m_CharacterData.CanAttackReach(m_CurrentTargetCharacterData))
            {
                m_CharacterData.Attack(m_CurrentTargetCharacterData);

                var attackPos = m_CurrentTargetCharacterData.transform.position + transform.up * 0.5f;
                VFXManager.PlayVFX(VFXType.Hit, attackPos);
                SFXManager.PlaySound(m_CharacterAudio.UseType, new SFXManager.PlayData() { Clip = m_CharacterData.Equipment.Weapon.GetHitSound(), PitchMin = 0.8f, PitchMax = 1.2f, Position = attackPos });
            }

            if(m_ClearPostAttack)
            {
                m_ClearPostAttack = false;
                m_CurrentTargetCharacterData = null;
                m_TargetInteractable = null;
            }

            m_CurrentState = State.DEFAULT;
        }

        //public void SetNewRespawn(SpawnPoint point)
        //{
        //    if(m_CurrentSpawn != null)
        //        m_CurrentSpawn.Deactivated();

        //    m_CurrentSpawn = point;
        //    m_CurrentSpawn.Activated();
        //}

        public void InteractWith(InteractableObject obj)
        {
            if (obj.IsInteractable)
            {
                m_TargetCollider = obj.GetComponentInChildren<Collider>();
                m_TargetInteractable = obj;
                //m_Agent.SetDestination(obj.transform.position);
            }
        }

        public void FootstepFrame()
        {
            Vector3 pos = transform.position;
        
            m_CharacterAudio.Step(pos);
        
            SFXManager.PlaySound(SFXManager.Use.Player, new SFXManager.PlayData()
            {
                Clip = SpurSoundClips[Random.Range(0, SpurSoundClips.Length)], 
                Position = pos,
                PitchMin = 0.8f,
                PitchMax = 1.2f,
                Volume = 0.3f
            });
        
            VFXManager.PlayVFX(VFXType.StepPuff, pos);
        }
        public bool FullWeaponRemote()
        {
            for (int i = 0; i < WeaponRemotePositions.Count; i++)
            {
                if (!WeaponRemotePositions[i].HaveRemoteWeapon)
                {
                    return false;
                }
            }
            return true;
        }
        public WeaponRemotePosition PosWeaponRemote
        {
            get
            {
                for (int i = 0; i < WeaponRemotePositions.Count; i++)
                {
                    if (!WeaponRemotePositions[i].HaveRemoteWeapon)
                    {
                        return WeaponRemotePositions[i];
                    }
                }
                return null;
            }
        }
    }
}