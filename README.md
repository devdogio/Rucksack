<p align="center">
    <img src="https://i.imgur.com/Fp7Z9uy.png" alt="Rucksack">
</p>
<h3 align="center" style="text-align:center;">
	The Inventory System for Unity - now free and open-source!
</h3>

<hr>

<h3 align="center" style="text-align:center;">
	Other Devdog Publishing Tools:
</h3>
<p align="center">	
	<a href="https://odininspector.com" target="_blank">
		<img src="https://i.imgur.com/mIPtgxG.png" alt="Odin Inspector">
	</a>
</p>
<hr>

## Important Links
<p align="center">
	<b>Discuss Rucksack with the rest of the community on Discord</b></p>
<p align="center">
	<a href="https://discord.gg/AgDmStu">
		<img src="https://discordapp.com/api/guilds/355444042009673728/embed.png" alt="Discord server"></a></p>

<p align="center">
	<b>Read the Rucksack Documentation</b></p>
<p align="center">
	<a href="https://rucksack-docs.readthedocs.io/en/latest/">
		<img src="https://i.imgur.com/0uTxaXy.png" alt="Documentation"></a></p>

## Why is Rucksack being open-sourced?

After years on the Asset Store, we're open-sourcing Rucksack because we're moving on to new projects and tools. You can read more in our blog post.

100% in the hands of the community, we hope to see the tool flourish, development continue, and any upcoming bugs fixed (the tool should be bug-free as-is).

## Getting started

Clone the repository + submodules, or download the zip file + [the general library](https://github.com/devdogio/general) and place it in Assets/Devdog.

Integrations stored in the Integrations folder may have to be removed, if you do not have these plugins in your project.

## What's included with Rucksack?

Rucksack is the most flexible and extensible inventory solution for singleplayer and multiplayer games. Easily setup player inventories, NPC inventories, banks, premium shops and much more in offline and online games!

Rucksack requires .Net 4.6.

⚔️ Website: Learn more about Rucksack
⚔️ Video guides: Get started quickly
⚔️ Support: Submit any questions

⚔️ HIGHLIGHTS ⚔️
- Source code
- Great editors for easy and quick iterations for you and your team
- Multiplayer built-in with UNet integration
- Modular event based UI system
- Easily create custom item behaviors
- Collection restrictions (inventories)
- Insanely extensible
- Visual equipment / NPCs with equipment
- Buy / Sell items from vendors
- Network permission model with server-based authorization
- Vendors can sell any ‘product’ for any ‘currency’
- Shared and non-shared (instanced) banks
- Modular input system
- Item inheritance
- Multiple shared inventories
- Different prices at vendors for different users

All Rucksack licenses include the full source code, so you can customize anything.

⚔️ Integrations ⚔️
- Playmaker
- Easy Build System
- Text Mesh Pro
- RFPSP
- UMA
- More coming!

• Multiplayer-compatible with UNet integration:
With a built-in integration for UNet that allows you to integrate into any server/networking setup, Rucksack is perfect for both online multiplayer and singleplayer games. Rucksack is secure and uses a server-authoritative permission model; So, no cheaters!

• Modular event based UI system:
You’re in full control of your UI with our modular and event-based UI system. The system is very fast and extensible, yet simple to use, which means you’ll be setting up your inventory / collections UI in no time! It doesn’t matter if you’re making a fantasy RPG or a casual mobile game with IAP - Rucksack’s UI system can handle it all.

• Items with behaviours:
In Rucksack, items can have behaviors, and you can very easily make your own item types and add custom behavior. We won’t limit your options!

• Restrict collections:
Need a quest items-only inventory? Or a consumables-only inventory? Not a problem for Rucksack; Use any metric to restrict the objects that can be stored in a collection.

• Insanely extensible:
Rucksack has been built from the ground-up to be extensible in every possible way, ensuring that you’ll never be limited in what you can achieve with the system if you want to extend it.

• Visual item equipment / NPC’s with equipment:
Any character in your game, such as an NPC, can have their own inventories and equipment, which means you can make them equip different weapons and gear by simply binding it onto the NPC (static, skinned, and dynamic cloth meshes, or write your own equipment handler).

• Permission Model with server-based authorization:
Our server-based authorization model makes your game cheat-proof, while remaining flexible and extensible to fit every kind of server / network setup. Example use with UNet: UNetPermissionRegistry.collections.SetPermission(collection, player, Permission.Read).

• Vendors can sell anything for anything:
Need to be able to sell houses for cats, or skill for skillpoints? With Rucksack, any item can be sold, and anything can be a currency. Go nuts!

• Shared and non-shared (instanced) banks and chests:
With Rucksack, you can easily differentiate between and setup both shared and non-shared (instanced) banks and chests in-game. The system is so flexible that the shared banks and chests could theoretically be shared between every player in a game, everyone in your party - and everything in-between.

• Modular input and UI system:
Rucksack’s modular input system is extensible and changeable, and allows you to create input modules to handle your input. The beauty is that you can very easily replace or swap out certain input modules for maximum control (example)

• Item inheritance:
Want to reduce the data transfers in your online game? With item inheritance, you can create variations of items (e.g. a Sword that can also be a Fire Sword). With Rucksack’s inheritance system, the Fire Sword can inherit its base stats from the Sword, while only overriding or adding those properties that have changed or have been added. In a large game, this saves a lot of duplicate data of unused props from being transferred.

• Different prices at vendors for different users:
You can easily achieve and setup different prices at vendors / shops based on whatever properties you want, such as different in-app purchase prices based on country (requires custom IVendor<'T> implementation).

⚔️ PERFECT FOR ⚔️
Rucksack is perfect for any game genre including:
- RPGs
- MMOs
- Online and Offline games
- Adventure
- Survival
- Mobile games
- Games with skin shops
- Strategy
- Simulation games

⚔️ USEFUL LINKS ⚔️
- [Documentation](http://rucksack-docs.readthedocs.io/en/latest/)

⚔️COMMUNITY ⚔️
Join in on the discussion on Discord for support and answers to the most common questions.

Rucksack is in constant development, and the Unity forum and Discord is used to inform us of which new features we should add next. So share your suggestions, or chime in agreement with other community members’ suggestions. 