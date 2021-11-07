using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Scripting;

public class PrefabSlot : VisualElement
{
    public Image icon;
    public string prefabGuid = "";

    public PrefabSlot()
    {
        //Create a new Image element and add it to the root
        icon = new Image();
        Add(icon);
        //Add USS style properties to the elements
        icon.AddToClassList("slotIcon");
        AddToClassList("slotContainer");
        RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        //Not the left mouse button or this is an empty slotIn
        if (evt.button != 0 || prefabGuid.Equals(""))
        {
            return;
        }

        //Clear the image
        icon.image = null;

        //Start the drag
        UIHandler.StartDrag(evt.position, this);
    }

    public void HoldItem(PrefabDetails prefab)
    {
        // icon.image = prefab.Icon.texture;
        prefabGuid = prefab.GUID;
    }

    public void DropItem()
    {
        prefabGuid = "";
        icon.image = null;
    }

    #region UXML
    [Preserve]
    public new class UxmlFactory : UxmlFactory<PrefabSlot, UxmlTraits> { }
    [Preserve]
    public new class UxmlTraits : VisualElement.UxmlTraits { }
    #endregion
}
