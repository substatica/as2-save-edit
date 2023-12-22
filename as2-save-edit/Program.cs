using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace as2_save_edit
{
	class Program
	{
		public class Options
		{
			[Option('s', "save", Required = false, HelpText = "Path to .SAV file")]
			public string SaveFile { get; set; }

			[Option('d', "dev", Required = false, HelpText = "Enable development mode")]
			public bool Development { get; set; }
		}

		public class Item
		{
			public int Id { get; set; }
			public string Name { get; set; }

			public Item(int id, string name)
			{
				this.Id = id;
				this.Name = name;
			}
		}

		static string defaultDirectory = @"c:\Program Files (x86)\Steam\userdata\";
		static string subDirectory = @"remote\";
		static string saveFilename = "Savegame.save";
		static string gameId = "1540210";

		static List<Item> LongWeapons = new List<Item>()
		{
			new Item(130, "M4 Rifle"),
			new Item(9075900, "M4 Rifle (Camo)"),
			new Item(3, "AK Rifle"),
			new Item(7741606, "AK Rifle (Sand)"),
			new Item(7008272, "Pump-Action Rifle"),
			new Item(3543329, "Pump-Action Rifle (Sand)"),
			new Item(106, "Machine Gun, Horizontal Magazine"),
			new Item(10749486, "Machine Gun, Box Magazine"), 
			new Item(10073658, "Double Barrel Shotgun"),
			new Item(108, "Pump-Action Shotgun"),
			new Item(2195687, "Pump-Action Shotgun (Sand)"),
			new Item(179576, "Grenade Launcher"),
			new Item(134, "Thompson SMG, Straight Magazine"),
			new Item(9894073, "Thompson SMG, Drum Magazine (Sand)")
		};

		static List<Item> ShortWeapons = new List<Item>()
		{
			new Item(0, "M9 Pistol"),
			new Item(10405088, "M1911 Pistol"),
			new Item(5724925, "M1911 Pistol, Extended Magazine, Muzzle Break"),
			new Item(3105022, "M1911 Pistol, Extended Magazine, Muzzle Break (Sand)"),
			new Item(997813, "G17 Pistol"),
			new Item(59, "G18 Pistol, Extended Magazine, Muzzle Break"),
			new Item(5473348, "G18 Pistol, Extended Magazine, Muzzle Break (Sand)"),
			new Item(96, "Desert Eagle Pistol"),
			new Item(4166737, "Desert Eagle Pistol, Extended Magazine, Muzzle Break (Sand)"),
			new Item(6092984, ".22 Revolver"),
			new Item(6592128, ".357 Revolver"),
			new Item(1537729, ".45 Revolver"),
			new Item(10073658, "Double-Barrel Sawed Off Shotgun"),
			new Item(9904560, "Uzi SMG"),
			new Item(5454246, "Uzi SMG, Drum Magazine (Sand)"),
			new Item(104, "Mac10 SMG"),
			new Item(11313807, "Mac10 SMG, Drum Magazine (Sand)"),
			new Item(61, "Flame Thrower"),
		};

		static List<Item> Explosives = new List<Item>()
		{
			new Item(57, "Fragmentation Grenade"),
			new Item(58, "Stick Grenade"),
			new Item(30, "Molotov Cocktail"),
			new Item(133, "Land Mine")
		};

		static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
		{
			var helpText = HelpText.AutoBuild(result, h =>
			{
				h.AdditionalNewLineAfterOption = false;
				h.Heading = "  substatica";
				h.Copyright = "  youtube.com/substatica";
				h.AdditionalNewLineAfterOption = true;
				return HelpText.DefaultParsingErrorsHandler(result, h);
			}, e => e);
			Console.WriteLine(helpText);
		}

		static void Main(string[] args)
		{
			Console.WriteLine();
			Console.WriteLine(@"                          ;   :   ;");
			Console.WriteLine(@"                       .   \_,!,_/   ,");
			Console.WriteLine(@"                        `.,'     `.,'");
			Console.WriteLine(@"                         /         \");
			Console.WriteLine(@"                   ~ -- :           : -- ~");
			Console.WriteLine(@"                         \         /");
			Console.WriteLine(@"                        ,'`._   _.'`.");
			Console.WriteLine(@"                       '   / `!` \   `");
			Console.WriteLine(@"                          ;   :   ;");
			Console.WriteLine();
			Console.WriteLine("  ----------------------------------------------------------");
			Console.WriteLine("  Arizona Sunshine 2 Save Edit");
			Console.WriteLine();
			Console.WriteLine("  substatica");
			Console.WriteLine("  https://youtube.com/substatica");
			Console.WriteLine("  ----------------------------------------------------------");

			bool developmentMode = false;
			string filename = null;

			var parser = new CommandLine.Parser(with => with.HelpWriter = null);
			var parserResult = parser.ParseArguments<Options>(args);

			parserResult
				.WithNotParsed(errs => DisplayHelp(parserResult, errs))
				.WithParsed<Options>(o =>
				{
					if (!String.IsNullOrEmpty(o.SaveFile))
					{
						filename = o.SaveFile;
					}

					developmentMode = o.Development;
				});

			if(String.IsNullOrEmpty(filename)) 
			{
				var userDirectories = Directory.GetDirectories(defaultDirectory);

				string as2Directory = null;

				foreach (string userDirectory in userDirectories)
				{
					var gameDirectories = Directory.GetDirectories(userDirectory);
					foreach (string gameDirectory in gameDirectories)
					{
						if (gameDirectory == Path.Combine(userDirectory, gameId))
						{
							as2Directory = gameDirectory;
						}
					}
				}

				if (String.IsNullOrEmpty(as2Directory))
				{
					throw new Exception("Warning: Could not locate save directory please specify Savegame.save path with -s parameter");
				}
				else
				{
					filename = Path.Combine(as2Directory, subDirectory, saveFilename);
				}
			}

			string fileContents = null;

			try
			{
				fileContents = File.ReadAllText(filename);
			}
			catch(Exception ex)
			{
				throw new Exception("Error: Could not read file");
			}

			var jsonContents = fileContents.Trim('\0');
			var save = JsonConvert.DeserializeObject<JsonClasses.SavedGame>(jsonContents);

			Console.WriteLine();
			for (int i = 0; i < save.savedGameStates.Count; i++)
			{
				Console.WriteLine("  " + i + ") " + save.savedGameStates[i].name);
			}

			string slot = null;
			while (String.IsNullOrEmpty(slot)
				|| (Convert.ToInt16(slot) >= ShortWeapons.Count || Convert.ToInt16(slot) < 0))
			{
				Console.WriteLine();
				Console.Write("  Save game to edit: ");
				slot = Console.ReadLine();
			}
			int slotIndex = Convert.ToInt16(slot);

			var PistolAmmo = save.savedGameStates[slotIndex].players[0].resources.Resources.Where(o => Convert.ToInt32(o.resourceId) == 15).First().amount;
			var Shotgun = save.savedGameStates[slotIndex].players[0].resources.Resources.Where(o => Convert.ToInt32(o.resourceId) == 17).First().amount;
			var Rifle = save.savedGameStates[slotIndex].players[0].resources.Resources.Where(o => Convert.ToInt32(o.resourceId) == 19).First().amount;
			var Alternate = save.savedGameStates[slotIndex].players[0].resources.Resources.Where(o => Convert.ToInt32(o.resourceId) == 18).First().amount;

			var AmmoFormat = "  Pistol: {0}/150\r\n  Shotgun: {1}/30\r\n  Rifle: {2}/200\r\n  Fuel/Grenade: {3}/10";

			Console.WriteLine();
			Console.WriteLine("  Ammo");
			Console.WriteLine("  ----------------------------------------------------------");
			Console.WriteLine(String.Format(AmmoFormat, PistolAmmo, Shotgun, Rifle, Alternate));

			Console.WriteLine();
			Console.WriteLine("  Hands/Sleeves");
			Console.WriteLine("  ----------------------------------------------------------");
			foreach (var bodySlot in save.savedGameStates[slotIndex].players[0].hands.BodySlots)
			{
				var itemId = Convert.ToInt32(bodySlot.ItemCellIds[0]);
				if (ShortWeapons.Where(o => o.Id == itemId).Count() > 0)
				{
					Console.WriteLine("  " + ShortWeapons.Where(o => o.Id == itemId).First().Name);
				}
				else if (Explosives.Where(o => o.Id == itemId).Count() > 0)
				{
					Console.WriteLine("  " + Explosives.Where(o => o.Id == itemId).First().Name);
				}
			}

			Console.WriteLine();
			Console.WriteLine("  Player Inventory");
			Console.WriteLine("  ----------------------------------------------------------");
			foreach (var inventorySlot in save.savedGameStates[slotIndex].players[0].inventory.SlotData)
			{
				if (inventorySlot.ItemCellIds.Count > 0)
				{
					var itemId = Convert.ToInt32(inventorySlot.ItemCellIds[0]);
					if (ShortWeapons.Where(o => o.Id == itemId).Count() > 0)
					{
						Console.WriteLine("  " + ShortWeapons.Where(o => o.Id == itemId).First().Name);
					}
					else if (LongWeapons.Where(o => o.Id == itemId).Count() > 0)
					{
						Console.WriteLine("  " + LongWeapons.Where(o => o.Id == itemId).First().Name);
					}
				}
			}

			Console.WriteLine();
			Console.WriteLine("  Companion Inventory");
			Console.WriteLine("  ----------------------------------------------------------");
			foreach (var inventorySlot in save.savedGameStates[slotIndex].companionInventory.SlotData)
			{
				if (inventorySlot.ItemCellIds.Count > 0)
				{
					var itemId = Convert.ToInt32(inventorySlot.ItemCellIds[0]);
					if (ShortWeapons.Where(o => o.Id == itemId).Count() > 0)
					{
						Console.WriteLine("  " + ShortWeapons.Where(o => o.Id == itemId).First().Name);
					}
				}
			}

			save.savedGameStates[slotIndex].players[0].hands.BodySlots.Clear();
			save.savedGameStates[slotIndex].players[0].inventory.SlotData.Clear();
			save.savedGameStates[slotIndex].companionInventory.SlotData.Clear();

			Console.WriteLine();
			for (int i = 0; i < ShortWeapons.Count; i++)
			{
				Console.WriteLine("  " + i + ") " + ShortWeapons[i].Name);
			}

			string rightHip = null;
			while (String.IsNullOrEmpty(rightHip)
				|| (!developmentMode && (Convert.ToInt16(rightHip) >= ShortWeapons.Count || Convert.ToInt16(rightHip) < 0))) 
			{
				Console.WriteLine();
				Console.Write("  New Right Hip Short Weapon: ");
				rightHip = Console.ReadLine();
			}
			int rightHipId = developmentMode ? Convert.ToInt16(rightHip) : ShortWeapons[Convert.ToInt16(rightHip)].Id;

			string leftHip = null;
			while (String.IsNullOrEmpty(leftHip)
				|| (!developmentMode && (Convert.ToInt16(leftHip) >= ShortWeapons.Count || Convert.ToInt16(leftHip) < 0)))
			{
				Console.WriteLine();
				Console.Write("  New Left Hip Short Weapon: ");
				leftHip = Console.ReadLine();
			}
			int leftHipId = developmentMode ? Convert.ToInt16(leftHip) : ShortWeapons[Convert.ToInt16(leftHip)].Id;


			List<Item> CompanionItems = new List<Item>();

			CompanionItems.AddRange(ShortWeapons);
			CompanionItems.AddRange(LongWeapons);
			CompanionItems.AddRange(Explosives);
			CompanionItems.Add(new Item(13194132, "Mini Gun"));

			Console.WriteLine();
			for (int i = 0; i < CompanionItems.Count; i++)
			{
				Console.WriteLine("  " + i + ") " + CompanionItems[i].Name);
			}
			Console.WriteLine("  *Only short weapons can be placed back on companion after being removed");

			string leftCompanion = null;
			while (String.IsNullOrEmpty(leftCompanion)
				|| (!developmentMode && (Convert.ToInt16(leftCompanion) >= CompanionItems.Count || Convert.ToInt16(leftCompanion) < 0)))
			{
				Console.WriteLine();
				Console.Write("  New Companion Left Slot Weapon Short Weapon: ");
				leftCompanion = Console.ReadLine();
			}
			int leftCompanionId = developmentMode ? Convert.ToInt16(leftCompanion) : CompanionItems[Convert.ToInt16(leftCompanion)].Id;

			string rightCompanion = null;
			while (String.IsNullOrEmpty(rightCompanion)
				|| (!developmentMode && (Convert.ToInt16(rightCompanion) >= CompanionItems.Count || Convert.ToInt16(rightCompanion) < 0)))
			{
				Console.WriteLine();
				Console.Write("  New Companion Right Slot Weapon Short Weapon: ");
				rightCompanion = Console.ReadLine();
			}
			int rightCompanionId = developmentMode ? Convert.ToInt16(rightCompanion) : CompanionItems[Convert.ToInt16(rightCompanion)].Id;

			Console.WriteLine();
			for (int i = 0; i < Explosives.Count; i++)
			{
				Console.WriteLine("  " + i + ") " + Explosives[i].Name);
			}

			string rightSleeve = null;
			while (String.IsNullOrEmpty(rightSleeve)
				|| (!developmentMode && (Convert.ToInt16(rightSleeve) >= Explosives.Count || Convert.ToInt16(rightSleeve) < 0)))
			{
				Console.WriteLine();
				Console.Write("  New Right Sleeve Explosive: ");
				rightSleeve = Console.ReadLine();
			}
			int rightSleeveId = developmentMode ? Convert.ToInt16(rightSleeve) : Explosives[Convert.ToInt16(rightSleeve)].Id;

			string leftSleeve = null;
			while (String.IsNullOrEmpty(leftSleeve)
				|| (!developmentMode && (Convert.ToInt16(leftSleeve) >= Explosives.Count || Convert.ToInt16(leftSleeve) < 0)))
			{
				Console.WriteLine();
				Console.Write("  New Left Sleeve Explosive: ");
				leftSleeve = Console.ReadLine();
			}
			int leftSleeveId = developmentMode ? Convert.ToInt16(leftSleeve) : Explosives[Convert.ToInt16(leftSleeve)].Id;

			Console.WriteLine();
			for (int i = 0; i < LongWeapons.Count; i++)
			{
				Console.WriteLine("  " + i + ") " + LongWeapons[i].Name);
			}

			string shoulder = null;
			while (String.IsNullOrEmpty(shoulder)
				|| (!developmentMode && (Convert.ToInt16(shoulder) >= LongWeapons.Count || Convert.ToInt16(shoulder) < 0)))
			{
				Console.WriteLine();
				Console.Write("  New Shoulder Long Weapon: ");
				shoulder = Console.ReadLine();
			}
			int shoulderId = developmentMode ? Convert.ToInt16(shoulder) : LongWeapons[Convert.ToInt16(shoulder)].Id;

			string maxAmmo = null;
			while (String.IsNullOrEmpty(maxAmmo) || (maxAmmo.ToUpper() != "Y" && maxAmmo.ToUpper() != "N"))
			{
				Console.WriteLine();
				Console.Write("  Max Ammo (Y/N): ");
				maxAmmo = Console.ReadLine();
			}
			bool maxAmmoBool = maxAmmo.ToUpper() == "Y";

			save.savedGameStates[slotIndex].players[0].hands.BodySlots.AddRange(new List<JsonClasses.BodySlot>() {
				new JsonClasses.BodySlot()
				{
					SlotType = 2,
					ItemCellIds = new List<int>() { leftSleeveId }
				},
				new JsonClasses.BodySlot()
				{
					SlotType = 3,
					ItemCellIds = new List<int>() { rightSleeveId }
				}
			});

			save.savedGameStates[slotIndex].players[0].inventory.SlotData.AddRange(new List<JsonClasses.SlotDatum>() {
				new JsonClasses.SlotDatum()
				{
					ItemCellIds = new List<int>() { leftHipId }
				},
				new JsonClasses.SlotDatum()
				{
					ItemCellIds = new List<int>() { rightHipId }
				},
				new JsonClasses.SlotDatum()
				{
					ItemCellIds = new List<int>() { shoulderId }
				}
			});

			save.savedGameStates[slotIndex].companionInventory.SlotData.AddRange(new List<JsonClasses.SlotDatum>() {
				new JsonClasses.SlotDatum()
				{
					ItemCellIds = new List<int>()
				},
				new JsonClasses.SlotDatum()
				{
					ItemCellIds = new List<int>() { leftCompanionId }
				},
				new JsonClasses.SlotDatum()
				{
					ItemCellIds = new List<int>() {rightCompanionId }
				}
			});

			if(maxAmmoBool)
			{
				SetResource(15, 150, save.savedGameStates[slotIndex].players[0].resources.Resources);
				SetResource(17, 30, save.savedGameStates[slotIndex].players[0].resources.Resources);
				SetResource(19, 200, save.savedGameStates[slotIndex].players[0].resources.Resources);
				SetResource(18, 10, save.savedGameStates[slotIndex].players[0].resources.Resources);
			}

			var jsonString = JsonConvert.SerializeObject(save, Formatting.None, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			});

			int backupIndex = 0;

			while (File.Exists(filename + ".bak." + backupIndex.ToString("D3")))
			{
				backupIndex++;
				if(backupIndex > 999)
				{
					throw new Exception("Error: Too many backup files, can't generate backup filename");
				}
			}

			File.Copy(filename, filename + ".bak." + backupIndex.ToString("D3"));
			var originalLength = new FileInfo(filename).Length;
			File.Delete(filename);
			
			File.WriteAllText(filename, jsonString);
			var newLength = new FileInfo(filename).Length;
			var paddingLength = originalLength - newLength;

			var paddingBytes = new Byte[paddingLength];
			for (int i = 0; i < paddingBytes.Length; i++) paddingBytes[i] = 0x00;
			AppendAllBytes(filename, paddingBytes);
		}

		public static void SetResource(int resourceId, int newAmount, List<JsonClasses.Resource> list)
		{
			for(int i = 0;i< list.Count();i++)
			{
				if(list[i].resourceId == resourceId)
				{
					list[i].amount = newAmount;
					break;
				}
			}
		}

		public static void AppendAllBytes(string path, byte[] bytes)
		{
			using (var stream = new FileStream(path, FileMode.Append))
			{
				stream.Write(bytes, 0, bytes.Length);
			}
		}
	}
}