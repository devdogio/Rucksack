#if UNITY_2017 || UNITY_2018

using UnityEngine;
using Photon.Pun;
using Devdog.Rucksack;

namespace Devdog.General2
{
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PUN2TriggerColliderUse : MonoBehaviour, ITriggerColliderUse
    {
        [SerializeField]
        private bool _onlyForPlayers = true;

        private ITrigger _trigger;
        private PhotonView _identity;
        private void Awake()
        {
            _trigger = GetComponentInParent<ITrigger>();
            _identity = GetComponentInParent<PhotonView>();
        }

        private void Start()
        {
            var col = GetComponent<SphereCollider>();
            col.isTrigger = true;

            // Here we try to set the collider's radius to a larger value than the object itself to get a chance to the player object to collide.
            var parentCollider = this.transform.parent?.GetComponentInParent<Collider>();           
            col.radius = parentCollider != null ? Mathf.CeilToInt(Mathf.Max(parentCollider.bounds.extents.x, parentCollider.bounds.extents.y, parentCollider.bounds.extents.z)) : 1;

            var rigid = GetComponent<Rigidbody>();
            rigid.useGravity = false;
            rigid.isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
                return;

            //if (!_identity.IsServer())
            //{
            //    return;
            //}

            if (_onlyForPlayers)
            {
                var player = other.GetComponent<Player>();
                if (player != null)
                {
                    _trigger.Server_Use(player);
                }
            }
            else
            {
                var character = other.GetComponent<Character>();
                if (character != null)
                {
                    _trigger.Server_Use(character);
                }
            }
        }
    }
}

#endif