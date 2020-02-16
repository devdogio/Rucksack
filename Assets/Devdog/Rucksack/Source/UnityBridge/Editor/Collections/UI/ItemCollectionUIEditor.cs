using Devdog.General2.Editors;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Items;
using UnityEditor;

namespace Devdog.Rucksack.UI.Editor
{
    [CustomEditor(typeof(ItemCollectionUI))]
    public class ItemCollectionUIEditor : UnityEditor.Editor
    {
//        private static ModuleList<ICollectionRestriction<IItemInstance>> _restrictions;

        protected void OnEnable()
        {
//            var t = (ItemCollectionUI) target;
//            _restrictions = new ModuleList<ICollectionRestriction<IItemInstance>>(t, this, "Collection restrictions");
//            _restrictions.description = "";
//            _restrictions.allowDuplicateImplementations = true;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
//            _restrictions.DoLayoutList();
        }
    }
}