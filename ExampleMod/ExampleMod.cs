using Terraria.GameContent.UI;
using Terraria.ModLoader;
using Terraria.UI;

namespace ExampleMod
{
	// This is a partial class, meaning some of its parts were split into other files. See ExampleMod.*.cs for other portions.
	public partial class ExampleMod : Mod
	{
		public const string AssetPath = $"{nameof(ExampleMod)}/Assets/";

		public static int ExampleCustomCurrencyId;

		public override void Load() {
			// Registers a new custom currency
			ExampleCustomCurrencyId = CustomCurrencyManager.RegisterCurrency(new Content.Currencies.ExampleCustomCurrency(ModContent.ItemType<Content.Items.ExampleItem>(), 999L, "Mods.ExampleMod.Currencies.ExampleCustomCurrency"));
		}

		public override void Unload() {
			// The Unload() methods can be used for unloading/disposing/clearing special objects, unsubscribing from events, or for undoing some of your mod's actions.
			// Be sure to always write unloading code when there is a chance of some of your mod's objects being kept present inside the vanilla assembly.
			// The most common reason for that to happen comes from using events, NOT counting On.* and IL.* code-injection namespaces.
			// If you subscribe to an event - be sure to eventually unsubscribe from it.

			// NOTE: When writing unload code - be sure use 'defensive programming'. Or, in other words, you should always assume that everything in the mod you're unloading might've not even been initialized yet.
			// NOTE: There is rarely a need to null-out values of static fields, since TML aims to completely dispose mod assemblies in-between mod reloads.
		}

		public override void PostSetupContent() {
			// Showcases mod support with Boss Checklist without referencing the mod
			Mod bossChecklist = ModLoader.GetMod("BossChecklist");
			if (bossChecklist != null) {
				bossChecklist.Call(
					"AddBoss",
					10.5f,
					new List<int> { ModContent.NPCType<NPCs.Abomination.Abomination>(), ModContent.NPCType<NPCs.Abomination.CaptiveElement2>() },
					this, // Mod
					"$Mods.ExampleMod.NPCName.Abomination",
					(Func<bool>)(() => ExampleWorld.downedAbomination),
					ModContent.ItemType<Items.Abomination.FoulOrb>(),
					new List<int> { ModContent.ItemType<Items.Armor.AbominationMask>(), ModContent.ItemType<Items.Placeable.AbominationTrophy>() },
					new List<int> { ModContent.ItemType<Items.Abomination.SixColorShield>(), ModContent.ItemType<Items.Abomination.MoltenDrill>() },
					"$Mods.ExampleMod.BossSpawnInfo.Abomination"
				);
				bossChecklist.Call(
					"AddBoss",
					15.5f,
					ModContent.NPCType<PuritySpirit>(),
					this,
					"Purity Spirit",
					(Func<bool>)(() => ExampleWorld.downedPuritySpirit),
					ItemID.Bunny,
					new List<int> { ModContent.ItemType<Items.Armor.PuritySpiritMask>(), ModContent.ItemType<Items.Armor.BunnyMask>(), ModContent.ItemType<Items.Placeable.PuritySpiritTrophy>(), ModContent.ItemType<Items.Placeable.BunnyTrophy>(), ModContent.ItemType<Items.Placeable.TreeTrophy>() },
					new List<int> { ModContent.ItemType<Items.PurityShield>(), ItemID.Bunny },
					$"Kill a [i:{ItemID.Bunny}] in front of [i:{ModContent.ItemType<Items.Placeable.ElementalPurge>()}]"
				);
			}
		}

		public override void AddRecipeGroups() {
			// Creates a new recipe group
			RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Lang.GetItemNameValue(ItemType("ExampleItem")), new[]
			{
				ItemType("ExampleItem"),
				ItemType("EquipMaterial"),
				ItemType("BossItem")
			});
			// Registers the new recipe group with the specified name
			RecipeGroup.RegisterGroup("ExampleMod:ExampleItem", group);

			// Modifying a vanilla recipe group. Now we can use Lava Snail to craft Snail Statue
			RecipeGroup snailGroup = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Snails"]];
			snailGroup.ValidItems.Add(ModContent.ItemType<NPCs.ExampleCritterItem>());

			// We also add ExampleSand to the Sand group, which is used in the Magic Sand Dropper recipe
			RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Sand"]].ValidItems.Add(ModContent.ItemType<Items.Placeable.ExampleSand>());
		}

		// Learn how to do Recipes: https://github.com/tModLoader/tModLoader/wiki/Basic-Recipes 
		public override void AddRecipes() {
			// Here is an example of a recipe.
			ModRecipe recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemType("ExampleItem"));
			recipe.SetResult(ItemID.Wood, 999);
			recipe.AddRecipe();

			// To make ExampleMod more organized, the rest of the recipes are added elsewhere, see the method calls below.
			// See RecipeHelper.cs
			RecipeHelper.AddExampleRecipes(this);
			RecipeHelper.ExampleRecipeEditing(this);
		}

		public override void UpdateMusic(ref int music, ref MusicPriority priority) {
			if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active) {
				return;
			}
			// Make sure your logic here goes from lowest priority to highest so your intended priority is maintained.
			if (Main.LocalPlayer.GetModPlayer<ExamplePlayer>().ZoneExample) {
				music = GetSoundSlot(SoundType.Music, "Sounds/Music/MarbleGallery");
				priority = MusicPriority.BiomeLow;
			}

			if (Main.LocalPlayer.HasBuff(BuffType("CarMount"))) {
				music = GetSoundSlot(SoundType.Music, "Sounds/Music/DriveMusic");
				priority = MusicPriority.Environment;
			}

		}

		public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor) {
			if (ExampleWorld.exampleTiles <= 0) {
				return;
			}

			float exampleStrength = ExampleWorld.exampleTiles / 200f;
			exampleStrength = Math.Min(exampleStrength, 1f);

			int sunR = backgroundColor.R;
			int sunG = backgroundColor.G;
			int sunB = backgroundColor.B;
			// Remove some green and more red.
			sunR -= (int)(180f * exampleStrength * (backgroundColor.R / 255f));
			sunG -= (int)(90f * exampleStrength * (backgroundColor.G / 255f));
			sunR = Utils.Clamp(sunR, 15, 255);
			sunG = Utils.Clamp(sunG, 15, 255);
			sunB = Utils.Clamp(sunB, 15, 255);
			backgroundColor.R = (byte)sunR;
			backgroundColor.G = (byte)sunG;
			backgroundColor.B = (byte)sunB;
		}

		//const int ShakeLength = 5;
		//readonly int ShakeCount = 0;
		//readonly float previousRotation = 0;
		//readonly float targetRotation = 0;
		//readonly float previousOffsetX = 0;
		//readonly float previousOffsetY = 0;
		//readonly float targetOffsetX = 0;
		//readonly float targetOffsetY = 0;

		// Volcano Tremor
		/* TODO To be fixed later.
		public override Matrix ModifyTransformMatrix(Matrix Transform)
		{
			if (!Main.gameMenu)
			{
				ExampleWorld world = GetModWorld<ExampleWorld>();
				if (world.VolcanoTremorTime > 0)
				{
					if (world.VolcanoTremorTime % ShakeLength == 0)
					{
						ShakeCount = 0;
						previousRotation = targetRotation;
						previousOffsetX = targetOffsetX;
						previousOffsetY = targetOffsetY;
						targetRotation = (Main.rand.NextFloat() - .5f) * MathHelper.ToRadians(15);
						targetOffsetX = Main.rand.Next(60) - 30;
						targetOffsetY = Main.rand.Next(40) - 20;
						if (world.VolcanoTremorTime == ShakeLength)
						{
							targetRotation = 0;
							targetOffsetX = 0;
							targetOffsetY = 0;
						}
					}
					float transX = Main.screenWidth / 2;
					float transY = Main.screenHeight / 2;

					float lerp = (float)(ShakeCount) / ShakeLength;
					float rotation = MathHelper.Lerp(previousRotation, targetRotation, lerp);
					float offsetX = MathHelper.Lerp(previousOffsetX, targetOffsetX, lerp);
					float offsetY = MathHelper.Lerp(previousOffsetY, targetOffsetY, lerp);

					world.VolcanoTremorTime--;
					ShakeCount++;


					return Transform
						* Matrix.CreateTranslation(-transX, -transY, 0f)
						* Matrix.CreateRotationZ(rotation)
						* Matrix.CreateTranslation(transX, transY, 0f)
						* Matrix.CreateTranslation(offsetX, offsetY, 0f);
					//Matrix.CreateFromAxisAngle(new Vector3(Main.screenWidth / 2, Main.screenHeight / 2, 0f), .2f);
					//Matrix.CreateRotationZ(MathHelper.ToRadians(30));
				}
			}
			return Transform;
		}
		*/

		public override void UpdateUI(GameTime gameTime) {
			if (ExampleUI.Visible) {
				_exampleUserInterface?.Update(gameTime);
			}
			_exampleResourceBarUserInterface?.Update(gameTime);
			ExamplePersonUserInterface?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1) {
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"ExampleMod: Coins Per Minute",
					delegate {
						if (ExampleUI.Visible) {
							_exampleUserInterface.Draw(Main.spriteBatch, new GameTime());
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}

			int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (inventoryIndex != -1) {
				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
					"ExampleMod: Example Person UI",
					delegate {
						// If the current UIState of the UserInterface is null, nothing will draw. We don't need to track a separate .visible value.
						ExamplePersonUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}

			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"ExampleMod: Example Resource Bar",
					delegate {
						_exampleResourceBarUserInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		public static bool NoInvasion(NPCSpawnInfo spawnInfo) => !spawnInfo.invasion && (!Main.pumpkinMoon && !Main.snowMoon || spawnInfo.spawnTileY > Main.worldSurface || Main.dayTime) && (!Main.eclipse || spawnInfo.spawnTileY > Main.worldSurface || !Main.dayTime);

		public static bool NoBiome(NPCSpawnInfo spawnInfo) {
			Player player = spawnInfo.player;
			return !player.ZoneJungle && !player.ZoneDungeon && !player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHoly && !player.ZoneSnow && !player.ZoneUndergroundDesert;
		}

		public static bool NoZoneAllowWater(NPCSpawnInfo spawnInfo) => !spawnInfo.sky && !spawnInfo.player.ZoneMeteor && !spawnInfo.spiderCave;

		public static bool NoZone(NPCSpawnInfo spawnInfo) => NoZoneAllowWater(spawnInfo) && !spawnInfo.water;

		public static bool NormalSpawn(NPCSpawnInfo spawnInfo) => !spawnInfo.playerInTown && NoInvasion(spawnInfo);

		public static bool NoZoneNormalSpawn(NPCSpawnInfo spawnInfo) => NormalSpawn(spawnInfo) && NoZone(spawnInfo);

		public static bool NoZoneNormalSpawnAllowWater(NPCSpawnInfo spawnInfo) => NormalSpawn(spawnInfo) && NoZoneAllowWater(spawnInfo);

		public static bool NoBiomeNormalSpawn(NPCSpawnInfo spawnInfo) => NormalSpawn(spawnInfo) && NoBiome(spawnInfo) && NoZone(spawnInfo);

		public override void HandlePacket(BinaryReader reader, int whoAmI) {
			ExampleModMessageType msgType = (ExampleModMessageType)reader.ReadByte();
			switch (msgType) {
				// This message sent by the server to initialize the Volcano Tremor on clients
				case ExampleModMessageType.SetTremorTime:
					int tremorTime = reader.ReadInt32();
					ExampleWorld world = ModContent.GetInstance<ExampleWorld>();
					world.VolcanoTremorTime = tremorTime;
					break;
				// This message sent by the server to initialize the Volcano Rubble.
				case ExampleModMessageType.VolcanicRubbleMultiplayerFix:
					int numberProjectiles = reader.ReadInt32();
					for (int i = 0; i < numberProjectiles; i++) {
						int identity = reader.ReadInt32();
						bool found = false;
						for (int j = 0; j < 1000; j++) {
							if (Main.projectile[j].owner == 255 && Main.projectile[j].identity == identity && Main.projectile[j].active) {
								Main.projectile[j].hostile = true;
								//Main.projectile[j].name = "Volcanic Rubble";
								found = true;
								break;
							}
						}
						if (!found) {
							Logger.Error("Error: Projectile not found");
						}
					}
					break;
				case ExampleModMessageType.PuritySpirit:
					if (Main.npc[reader.ReadInt32()].modNPC is PuritySpirit spirit && spirit.npc.active) {
						spirit.HandlePacket(reader);
					}
					break;
				case ExampleModMessageType.HeroLives:
					Player player = Main.player[reader.ReadInt32()];
					int lives = reader.ReadInt32();
					player.GetModPlayer<ExamplePlayer>().heroLives = lives;
					if (lives > 0) {
						NetworkText text;
						if (lives == 1) {
							text = NetworkText.FromKey("Mods.ExampleMod.LifeLeft", player.name);
						}
						else {
							text = NetworkText.FromKey("Mods.ExampleMod.LivesLeft", player.name, lives);
						}
						NetMessage.BroadcastChatMessage(text, new Color(255, 25, 25));
					}
					break;
				// This message syncs ExamplePlayer.exampleLifeFruits
				case ExampleModMessageType.ExamplePlayerSyncPlayer:
					byte playernumber = reader.ReadByte();
					ExamplePlayer examplePlayer = Main.player[playernumber].GetModPlayer<ExamplePlayer>();
					int exampleLifeFruits = reader.ReadInt32();
					examplePlayer.exampleLifeFruits = exampleLifeFruits;
					examplePlayer.nonStopParty = reader.ReadBoolean();
					// SyncPlayer will be called automatically, so there is no need to forward this data to other clients.
					break;
				case ExampleModMessageType.NonStopPartyChanged:
					playernumber = reader.ReadByte();
					examplePlayer = Main.player[playernumber].GetModPlayer<ExamplePlayer>();
					examplePlayer.nonStopParty = reader.ReadBoolean();
					// Unlike SyncPlayer, here we have to relay/forward these changes to all other connected clients
					if (Main.netMode == NetmodeID.Server) {
						var packet = GetPacket();
						packet.Write((byte)ExampleModMessageType.NonStopPartyChanged);
						packet.Write(playernumber);
						packet.Write(examplePlayer.nonStopParty);
						packet.Send(-1, playernumber);
					}
					break;
				case ExampleModMessageType.ExampleTeleportToStatue:
					if (Main.npc[reader.ReadByte()].modNPC is NPCs.ExamplePerson person && person.npc.active) {
						person.StatueTeleport();
					}
					break;
				default:
					Logger.WarnFormat("ExampleMod: Unknown Message type: {0}", msgType);
					break;
			}
		}
	}
}
