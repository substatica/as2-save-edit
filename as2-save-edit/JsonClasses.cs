using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonClasses
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class BodySlot
    {
        public int SlotType { get; set; }
        public bool IsHidden { get; set; }
        public ReservedItem ReservedItem { get; set; }
        public List<int> ItemCellIds { get; set; }
        public List<int> configurationIds { get; set; }
    }

    public class CompanionInventory
    {
        public List<SlotDatum> SlotData { get; set; }
    }

    public class Hands
    {
        public List<BodySlot> BodySlots { get; set; }
    }

    public class Inventory
    {
        public List<SlotDatum> SlotData { get; set; }
    }

    public class Player
    {
        public Hands hands { get; set; }
        public Inventory inventory { get; set; }
        public ResourceList resources { get; set; }
        public List<object> freeCraftingRecipes { get; set; }
    }

    public class ReservedItem
    {
        public int ReservedItemCellId { get; set; }
        public int ReservedItemIdx { get; set; }
        public bool HasData { get; set; }
    }

    public class Resource
    {
        public int resourceId { get; set; }
        public int amount { get; set; }
    }

    public class ResourceList
    {
        public List<Resource> Resources { get; set; }
    }

    public class SavedGame
    {
        public List<SavedGameState> savedGameStates { get; set; }
    }

    public class SavedGameState
    {
        public string name { get; set; }
        public int version { get; set; }
        public int slot { get; set; }
        public int difficulty { get; set; }
        public int levelCellID { get; set; }
        public string sessionGuid { get; set; }
        public object checkpointGuid { get; set; }
        public List<string> reachedCheckpointsGuids { get; set; }
        public double levelCompletionFraction { get; set; }
        public DateTime lastPlayedDate { get; set; }
        public List<object> triggeredTutorials { get; set; }
        public int playerIndexToLoad { get; set; }
        public List<Player> players { get; set; }
        public List<string> triggeredSequenceGUIDs { get; set; }
        public CompanionInventory companionInventory { get; set; }
    }

    public class SlotDatum
    {
        public List<int> ItemCellIds { get; set; }
        public List<int> configurationIds { get; set; }
        public bool IsHidden { get; set; }
    }


}
