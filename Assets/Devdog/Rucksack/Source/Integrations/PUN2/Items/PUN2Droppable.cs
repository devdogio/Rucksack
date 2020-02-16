using System;
using UnityEngine;
using Photon.Pun;
using Devdog.General2;

namespace Devdog.Rucksack.Items
{
    [RequireComponent(typeof(PhotonView))]
    public sealed class PUN2Droppable : MonoBehaviourPun, IPunInstantiateMagicCallback
    {
        private static readonly ILogger _logger;
        static PUN2Droppable()
        {
            _logger = new UnityLogger("[PUN2][Items] ");
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (info.photonView.InstantiationData == null || info.photonView.InstantiationData.Length == 0)
            {
                // No data
                return;
            }

            float pickupDistance = (float)info.photonView.InstantiationData[0];

            _logger.Log("[Client] Dropping item " + this.gameObject.name, this);

            this.GetOrAddComponent<PUN2Trigger>();
            
            var rangeHandler = new GameObject("_RangeHandler");
            rangeHandler.transform.SetParent(this.transform);
            rangeHandler.transform.localPosition = Vector3.zero;
            rangeHandler.transform.localRotation = Quaternion.identity;
            var handler = rangeHandler.AddComponent<TriggerRangeHandler>();
            handler.useRange = pickupDistance;

            var autoUse = new GameObject("autoUse");
            autoUse.transform.SetParent(this.transform);
            autoUse.transform.localPosition = Vector3.zero;
            autoUse.transform.localRotation = Quaternion.identity;
            /*var triggerColliderUse = */autoUse.AddComponent<PUN2TriggerColliderUse>();

            this.GetOrAddComponent<TriggerInputHandler>();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // TODO: check weather the root prefab is in a \Resources folder!
        }
#endif
    }
}
