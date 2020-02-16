using UnityEngine;

using Devdog.General2;

using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.Items;

namespace Devdog.Rucksack.Database
{
	public class UnityDatabaseManager : ManagerBase<UnityDatabaseManager>
	{
		[SerializeField]
		private UnityCurrencyDatabase _currencyDatabase;

		[SerializeField]
		private UnityItemDefinitionDatabase _itemDefinitionDatabase;

		[SerializeField]
		private UnityEquipmentTypeDatabase _equipmentTypeDatabase;

		public static UnityCurrencyDatabase CurrencyDatabase
		{
			get
			{
				return instance._currencyDatabase;
			}
		}

		public static UnityItemDefinitionDatabase itemDefinitionDatabase
		{
			get
			{
				return instance._itemDefinitionDatabase;
			}
		}

		public static UnityEquipmentTypeDatabase equipmentTypeDatabase
		{
			get
			{
				return instance._equipmentTypeDatabase;
			}
		}

		public static Result<UnityCurrency> CurrencyByName(string name)
		{
			return GetByName(CurrencyDatabase, name);
		}

		public static Result<UnityItemDefinition> ItemDefinitionByName(string name)
		{
			return GetByName(itemDefinitionDatabase, name);
		}

		public static Result<UnityEquipmentType> EquipmentTypeByName(string name)
		{
			return GetByName(equipmentTypeDatabase, name);
		}

		private static Result<T> GetByName<T>(IDatabase<T> database, string name) where T: ScriptableObject
		{
			return database.Get(t => t.name == name);
		}

		
	}
}