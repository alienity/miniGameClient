using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class ChangeHierarchyOnCode : MonoBehaviour, IPointerClickHandler
{
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
        // transform.SetAsFirstSibling();
        // transform.SetSiblingIndex(2);
    }

}
