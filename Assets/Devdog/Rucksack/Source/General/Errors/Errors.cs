
namespace Devdog.Rucksack
{
    public static partial class Errors
    {
        ///////////////////////////////////////////////////////////////////
        ///////////////////////////// Character ///////////////////////////
        ///////////////////////////////////////////////////////////////////
        [ErrorParameters("{Character}")]
        public static Error CharacterInvalid = new Error(1, "Character is invalid"); // , new LocalizedString("CollectionFullError") { message = "Collection is full, can't add item."}
 
        ///////////////////////////////////////////////////////////////////
        /////////////////////////// Collections ///////////////////////////
        ///////////////////////////////////////////////////////////////////
        [ErrorParameters("{CollectionName}", "{Item}", "{Amount}")]
        public static Error CollectionFull = new Error(101, "Collection is full"); // , new LocalizedString("CollectionFullError") { message = "Collection is full, can't add item."}

        [ErrorParameters("{CollectionNameFrom}", "{Item}")]
        public static Error CollectionCanNotMoveItem = new Error(102, "Collection can not move item");

        [ErrorParameters("{CollectionName}", "{Item}")]
        public static Error CollectionAlreadyContainsSpecificInstance = new Error(103, "Collection already contains this specific instance");
        
        [ErrorParameters("{CollectionName}", "{Item}")]
        public static Error CollectionDoesNotContainItem = new Error(104, "Collection does not contain item");
        
        [ErrorParameters("{CollectionName}")]
        public static Error CollectionIsReadOnly = new Error(105, "Collection is read only");
        
        [ErrorParameters("{CollectionName}", "{Item}")]
        public static Error CollectionInvalidItemType = new Error(106, "Item type is invalid");
        
        
        // Layout collections
        [ErrorParameters("{CollectionName}", "{Item}")]
        public static Error LayoutCollectionItemBlocked = new Error(131, "Layout collection item is blocked");

        
        
        // Currency collections
        public static Error CurrencyCollectionDoesNotContainCurrency = new Error(141, "Currency collection does not contain currency");

        
        
        // Character collections
        public static Error CharacterCollectionEquipmentTypeInvalid = new Error(151, "Character equipment type invalid");
        
        public static Error CharacterCollectionInvalid = new Error(152, "Character equipment collection invalid");
        
        public static Error CharacterCollectionItemNotEquippedToCollection = new Error(153, "Character collection item not equipped to collection");
        
        
                
        ///////////////////////////////////////////////////////////////////
        ///////////////////////////// Filters /////////////////////////////
        ///////////////////////////////////////////////////////////////////
        [ErrorParameters("{CollectionName}", "{Item}", "{Amount}")]
        public static Error CollectionRestrictionPreventedAction = new Error(191, "Collection restriction prevented action");
        
        
        
        ///////////////////////////////////////////////////////////////////
        ////////////////////////////// Items //////////////////////////////
        ///////////////////////////////////////////////////////////////////
        [ErrorParameters("{Item}", "{MaxStackSize}", "{Amount}")]
        public static Error ItemIsExceedingMaxStackSize = new Error(201, "Item is exceeding max stack size");
        
        [ErrorParameters("{ItemA}", "{ItemB}")]
        public static Error ItemsAreNotEqual = new Error(202, "Items are not equal");

        [ErrorParameters("{Item}")]
        public static Error ItemCanNotBeDropped = new Error(203, "Item can not be dropped");
        
        [ErrorParameters("{Item}")]
        public static Error ItemCanNotBeUsed = new Error(204, "Item can not be used");
        
        
        
        
        ///////////////////////////////////////////////////////////////////
        ///////////////////////////// Database ////////////////////////////
        ///////////////////////////////////////////////////////////////////
        [ErrorParameters]
        public static Error DatabaseItemNotFound = new Error(301, "Item not found");
        
        [ErrorParameters]
        public static Error DatabaseSetFailed = new Error(302, "Set failed");

        [ErrorParameters]
        public static Error DatabaseUnexpectedError = new Error(309, "Unexpected error");


        ///////////////////////////////////////////////////////////////////
        ////////////////////////////// Vendor /////////////////////////////
        ///////////////////////////////////////////////////////////////////
        [ErrorParameters("{Vendor}", "{Item}")]
        public static Error VendorDoesNotContainItem = new Error(401, "Vendor does not contain item");
        
        [ErrorParameters("{Vendor}", "{Item}")]
        public static Error VendorProductHasNoPrice = new Error(402, "Vendor product has no price");
       
        
        
        ///////////////////////////////////////////////////////////////////
        //////////////////////////// Currencies ///////////////////////////
        ///////////////////////////////////////////////////////////////////
        [ErrorParameters("{Currency}", "{Target}")]
        public static Error CurrencyCanNotConvertToTarget = new Error(501, "Currency can not be converted to target currency");

        

        ///////////////////////////////////////////////////////////////////
        /////////////////////////////// UI ////////////////////////////////
        ///////////////////////////////////////////////////////////////////
        [ErrorParameters]
        public static Error UIDragFailed = new Error(601, "Drag operation failed");

        [ErrorParameters]
        public static Error UIDragFailedNoReceiver = new Error(602, "Drag operation failed - No slot found");
        
        [ErrorParameters]
        public static Error UIDragFailedIncompatibleDragObject = new Error(603, "Drag operation failed - Incompatible drag object");
        
        ///////////////////////////////////////////////////////////////////
        ///////////////////////////// Network /////////////////////////////
        ///////////////////////////////////////////////////////////////////
        [ErrorParameters]
        public static Error NetworkNoAuthority = new Error(901, "No authority to do operation.");
        
        [ErrorParameters]
        public static Error NetworkValidationFailed = new Error(902, "Network input validation failed.");
    }
}