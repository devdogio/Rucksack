using System;
using System.Collections;
using Devdog.General2;
using Devdog.General2.UI;
using Devdog.Rucksack.Collections;
using UnityEngine;

using Photon.Pun;

namespace Devdog.Rucksack.UI
{
    [RequireComponent(typeof(UIWindow))]
    public class PUN2CollectionPermissionShowUI : MonoBehaviourPun
    {
        [Required]
        [SerializeField]
        private string _collectionName;

        [Required]
        [SerializeField]
        private UIWindow _window;


        private void Awake()
        {
            PlayerManager.OnPlayerChanged += OnPlayerChanged;
        }

        private void OnDestroy()
        {
            PlayerManager.OnPlayerChanged -= OnPlayerChanged;
        }

        private void OnPlayerChanged(Player before, Player after)
        {
            if (before != null)
            {
                var identity = before.GetComponent<PhotonView>();
                if (identity != null)
                {
                    PUN2PermissionsRegistry.collections.RemoveEventListener(identity, OnPermissionChanged);
                }
            }

            if (after != null)
            {
                var identity = after.GetComponent<PhotonView>();
                if (identity != null)
                {
                    PUN2PermissionsRegistry.collections.AddEventListener(identity, OnPermissionChanged);
                }
            }
        }

        private void OnPermissionChanged(PermissionChangedResult<IPUN2Collection, PhotonView> result)
        {
            if (result.obj.collectionName == _collectionName)
            {
                if (result.permission.CanRead())
                {
                    _window.Show();
                }
                else
                {
                    _window.Hide();
                }
            }
        }
    }
}