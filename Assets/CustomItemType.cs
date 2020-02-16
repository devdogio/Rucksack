using System;
using Devdog.Rucksack.Items;
using UnityEngine;

public class CustomItemType : UnityItemDefinition
{
	[SerializeField]
	private bool _canSell;
	public bool canSell
	{
		get { return this.GetValue(o => o._canSell); }
		set { _canSell = value; }
	}

	[SerializeField]
	private UnityItemDefinition _someObject;
	public UnityItemDefinition someObject
	{
		get { return this.GetValue(o => o._someObject); }
		set { _someObject = value; }
	}
	
	
	public CustomItemType()
		: this(Guid.Empty, null)
	{ }

	public CustomItemType(Guid ID)
		: this(ID, null)
	{ }
        
	public CustomItemType(Guid ID, CustomItemType parent)
	{
		this.ID = ID;
		this.parent = parent;
	}
	
	
}
