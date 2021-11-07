using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[Serializable]
public class PrefabDetails
{
    public string Name;
    public string GUID;
    public Sprite Icon;
    public bool CanDrop;
}

public enum SlotChangeType
{
    Pickup,
    Drop
}
public delegate void OnInventoryChangedDelegate(string[] itemGuid, SlotChangeType change);

public class SceneController : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> IconSprites;
    private static Dictionary<string, PrefabDetails> m_ItemDatabase = new Dictionary<string, PrefabDetails>();
    private List<PrefabDetails> m_PlayerInventory = new List<PrefabDetails>();
    public static event OnInventoryChangedDelegate OnInventoryChanged = delegate { };


    private void Awake()
    {
        PopulateDatabase();
    }

    private void Start()
    {
        m_PlayerInventory.AddRange(m_ItemDatabase.Values);
        OnInventoryChanged.Invoke(m_PlayerInventory.Select(x => x.GUID).ToArray(), SlotChangeType.Pickup);
    }

    public void PopulateDatabase()
    {
        m_ItemDatabase.Add("8B0EF21A-F2D9-4E6F-8B79-031CA9E202BA", new PrefabDetails()
        {
            Name = "History of the Syndicate: 1501 to 1825 ",
            GUID = "8B0EF21A-F2D9-4E6F-8B79-031CA9E202BA",
            // Icon = IconSprites.FirstOrDefault(x => x.name.Equals("syndicate")),
            CanDrop = false
        });

        m_ItemDatabase.Add("992D3386-B743-4CD3-9BB7-0234A057C265", new PrefabDetails()
        {
            Name = "Health Potion",
            GUID = "992D3386-B743-4CD3-9BB7-0234A057C265",
            // Icon = IconSprites.FirstOrDefault(x => x.name.Equals("potion")),
            CanDrop = true
        });

        m_ItemDatabase.Add("1B9C6CAA-754E-412D-91BF-37F22C9A0E7B", new PrefabDetails()
        {
            Name = "Bottle of Poison",
            GUID = "1B9C6CAA-754E-412D-91BF-37F22C9A0E7B",
            // Icon = IconSprites.FirstOrDefault(x => x.name.Equals("poison")),
            CanDrop = true
        });

    }

    public static PrefabDetails GetItemByGuid(string guid)
    {
        if (m_ItemDatabase.ContainsKey(guid))
        {
            return m_ItemDatabase[guid];
        }

        return null;
    }

}
