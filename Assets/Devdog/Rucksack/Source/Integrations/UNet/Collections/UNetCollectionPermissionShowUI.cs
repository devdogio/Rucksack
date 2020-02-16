using System;
using System.Collections;
using Devdog.General2;
using Devdog.General2.UI;
using Devdog.Rucksack.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.UI
{
    [RequireComponent(typeof(UIWindow))]
    public class UNetCollectionPermissionShowUI : MonoBehaviour
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
                var identity = before.GetComponent<NetworkIdentity>();
                if (identity != null)
                {
                    UNetPermissionsRegistry.collections.RemoveEventListener(identity, OnPermissionChanged);
                }
            }

            if (after != null)
            {
                var identity = after.GetComponent<NetworkIdentity>();
                if (identity != null)
                {
                    UNetPermissionsRegistry.collections.AddEventListener(identity, OnPermissionChanged);
                }
            }
        }

        private void OnPermissionChanged(PermissionChangedResult<IUNetCollection, NetworkIdentity> result)
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