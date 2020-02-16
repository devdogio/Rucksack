using System.Linq;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;
using Devdog.Rucksack.Collections;

namespace Devdog.Rucksack.UI
{
    public class EquipmentCollectionUI : CollectionUIBase<IEquippableItemInstance>
    {


        protected override void OnCollectionChanged(ICollection<IEquippableItemInstance> old, ICollection<IEquippableItemInstance> newCollection)
        {
            if (old != null)
            {
                old.OnSlotsChanged -= OnCollectionSlotsChanged;
                old.OnSizeChanged -= OnCollectionSizeChanged;
            }

            if (newCollection != null)
            {
                if (newCollection.slotCount > 0)
                {
                    repaintableElements = uiContainer.GetComponentsInChildren<CollectionSlotUIBase>(true);
                    for (var i = 0; i < repaintableElements.Length; i++)
                    {
                        var s = (CollectionSlotUIBase<IEquippableItemInstance>)repaintableElements[i];
                        s.collectionUI = this;
                        s.collectionIndex = i;
                        
                        Repaint(i);
                    }
                }
                else
                {                    
                    IndexSlotsAndMountPointsFromComponents();
                }

                newCollection.OnSlotsChanged += OnCollectionSlotsChanged;
                newCollection.OnSizeChanged += OnCollectionSizeChanged;
            }
        }

        /// <summary>
        /// Index all slots that are defined in the uiContainer. 
        /// <remarks>This will clear all existing data slots in the collection and generate new slots. Therefore, this will clear the itemGuid data</remarks>
        /// </summary>
        public virtual void IndexSlotsAndMountPointsFromComponents()
        {
            var col = collection as EquipmentCollection<IEquippableItemInstance>;
            if (col == null)
            {
                logger.Warning($"Character equipment collection is not of type {typeof(EquipmentCollection<IEquippableItemInstance>).Name}. Can not repaint existing slots...", this);
                return;
            }

            if (col.characterOwner == null)
            {
                logger.Warning($"UnityEquipmentCollection does not have an assigned character. Can not repaint slots", this);
                return;
            }
            
            var uiSlots = uiContainer.GetComponentsInChildren<CollectionSlotUIBase<IEquippableItemInstance>>(true);
            col.slots = new EquipmentCollectionSlot<IEquippableItemInstance>[uiSlots.Length];
            
            int i = 0;
            foreach (var slot in uiSlots)
            {
                slot.collectionUI = this;
                slot.collectionIndex = i;

                var equipmentTypes = new IEquipmentType[0];
                var equipmentTypesComp = slot.GetComponent<UnityEquipmentTypes>();
                if (equipmentTypesComp != null)
                {
                    equipmentTypes = equipmentTypesComp.equipmentTypes.Cast<IEquipmentType>().ToArray();
                }
                
                col.slots[i] = new EquipmentCollectionSlot<IEquippableItemInstance>()
                {
                    collection = collection,
                    index = i,
                    equipmentTypes = equipmentTypes,
                };
                
                slot.Repaint(null, 0);
                i++;
            }

            repaintableElements = uiSlots.Cast<CollectionSlotUIBase>().ToArray();
            logger.LogVerbose($"Indexed {uiSlots.Length} slots into character equipment collection for {col.characterOwner}", this);
        }
    }
}