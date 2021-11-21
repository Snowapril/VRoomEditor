using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using RTG;

public class UIHandler : MonoBehaviour
{
    // Start is called before the first frame
    public List<GameObject> gltfPrefabs;
    public List<GameObject> primitivePrefabs;
    private VisualElement prefabs;

    public List<PrefabSlot> InventoryItems = new List<PrefabSlot>();
    private VisualElement m_Root;
    private VisualElement m_SlotContainer;
    private static VisualElement m_GhostIcon;

    private static bool m_IsDragging;
    private static PrefabSlot m_OriginalSlot;

    private void Awake()
    {
        //Search the root for the SlotContainer Visual Element
        m_SlotContainer = m_Root.Q<VisualElement>("prefabs");
        //Create PrefabSlots and add them as children to the SlotContainer
        for (int i = 0; i < 20; i++)
        {
            PrefabSlot item = new PrefabSlot();
            InventoryItems.Add(item);
            m_SlotContainer.Add(item);
        }
    
        SceneController.OnInventoryChanged += SceneController_OnInventoryChanged;
        // m_GhostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        // m_GhostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }
    
    void Start()
    {
        // prefabs = root.Q<VisualElement>("prefabs");
    }
    
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 6; ++i)
        {
            if (Input.GetKeyDown(KeyCode.F1 + i)) //If we click left mouse button
            {
                Vector3 instantiatePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject newGameObject = Instantiate(gltfPrefabs[i], instantiatePosition, Quaternion.identity); //Creating a reference to our prefab and instantiating at the mouse position
                newGameObject.name = "MyObject"; //This will name the object instead of it instantiating as Clone
                ObjectTransformGizmo universalGizmo = RTGizmosEngine.Get.CreateObjectUniversalGizmo();
                universalGizmo.SetTargetObject(newGameObject);
            }
        }
    
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // var exporter = new GLTFSceneExporter(new[] { transform }, new ExportOptions());
            var appPath = Application.dataPath;
            var wwwPath = appPath.Substring(0, appPath.LastIndexOf("Assets")) + "www";
            // exporter.SaveGLTFandBin(Path.Combine(wwwPath, "TestScene"), "TestScene");
        }
    }

    public static void StartDrag(Vector2 position, PrefabSlot originalSlot)
    {   
        //Set tracking variables
        m_IsDragging = true;
        m_OriginalSlot = originalSlot;

        //Set the new position
        m_GhostIcon.style.top = position.y - m_GhostIcon.layout.height / 2;
        m_GhostIcon.style.left = position.x - m_GhostIcon.layout.width / 2;

        //Set the image
        m_GhostIcon.style.backgroundImage = SceneController.GetItemByGuid(originalSlot.prefabGuid).Icon.texture;

        //Flip the visibility on
        m_GhostIcon.style.visibility = Visibility.Visible;

    }

    /// <summary>
    /// Perform the drag
    /// </summary>
    private void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!m_IsDragging)
        {
            return;
        }

        //Set the new position
        m_GhostIcon.style.top = evt.position.y - m_GhostIcon.layout.height / 2;
        m_GhostIcon.style.left = evt.position.x - m_GhostIcon.layout.width / 2;

    }

    /// <summary>
    /// Finish the drag and compute whether the item should be moved to a new slot
    /// </summary>
    private void OnPointerUp(PointerUpEvent evt)
    {

        if (!m_IsDragging)
        {
            return;
        }

        //Check to see if they are dropping the ghost icon over any inventory slots.
        IEnumerable<PrefabSlot> slots = InventoryItems.Where(x => x.worldBound.Overlaps(m_GhostIcon.worldBound));

        //Found at least one
        if (slots.Count() != 0)
        {
            PrefabSlot closestSlot = slots.OrderBy(x => Vector2.Distance(x.worldBound.position, m_GhostIcon.worldBound.position)).First();

            //Set the new inventory slot with the data
            closestSlot.HoldItem(SceneController.GetItemByGuid(m_OriginalSlot.prefabGuid));

            //Clear the original slot
            m_OriginalSlot.DropItem();
        }
        //Didn't find any (dragged off the window)
        else
        {
            m_OriginalSlot.icon.image = SceneController.GetItemByGuid(m_OriginalSlot.prefabGuid).Icon.texture;
        }


        //Clear dragging related visuals and data
        m_IsDragging = false;
        m_OriginalSlot = null;
        m_GhostIcon.style.visibility = Visibility.Hidden;
    }

    /// <summary>
    /// Listen for changes to the players inventory and act
    /// </summary>
    /// <param name="prefabGuid">Reference ID for the Item Database</param>
    /// <param name="change">Type of change that occurred. This could be extended to handle drop logic.</param>
    private void SceneController_OnInventoryChanged(string[] prefabGuid, SlotChangeType change)
    {
        //Loop through each item and if it has been picked up, add it to the next empty slot
        foreach (string item in prefabGuid)
        {
            if (change == SlotChangeType.Pickup)
            {
                var emptySlot = InventoryItems.FirstOrDefault(x => x.prefabGuid.Equals(""));

                if (emptySlot != null)
                {
                    emptySlot.HoldItem(SceneController.GetItemByGuid(item));
                }
            }
        }
    }
}