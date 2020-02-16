using Devdog.Rucksack.Items;
using UnityEngine;

namespace Devdog.Rucksack.UI
{
    public class LayoutItemCollectionUI : CollectionUIBase<IItemInstance>
    {

        [SerializeField]
        private int _columnCount;
        public int columnCount { get { return _columnCount; } }


        
#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();
    
            if(_columnCount <= 0)
            {
                _columnCount = 1;
                logger.Error($"columnCount should be at least 1", this);
            }
    
            if(uiContainer != null && uiContainer.GetComponent<DynamicLayoutGroup>() == null)
            {
                logger.Warning($"No {nameof(DynamicLayoutGroup)} component found on container", this);
            }
        }

#endif


        protected override void OnEnable()
        {
            base.OnEnable();

            var group = uiContainer.GetComponent<DynamicLayoutGroup>();
            group.columnCount = columnCount;
        }
    }
}