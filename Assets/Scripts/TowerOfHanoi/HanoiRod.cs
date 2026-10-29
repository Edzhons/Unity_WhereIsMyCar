using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to each rod's UI drop target (Image with Raycast Target enabled).
/// The target's pivot is where the bottom disk rests (top of the rod's base).
/// </summary>
public class HanoiRod : MonoBehaviour, IDropHandler
{
    private readonly List<HanoiDisk> disks = new List<HanoiDisk>();
    private HanoiGameManager manager;
    public int DiskCount => disks.Count;
    public HanoiDisk TopDisk => disks.Count == 0 ? null : disks[disks.Count - 1];

    public void Initialize(HanoiGameManager gameManager)
    {
        manager = gameManager;
    }

    public void AddDisk(HanoiDisk disk)
    {
        if (disk == null || disks.Contains(disk))
            return;

        disks.Add(disk);
        disk.SetCurrentRod(this);
    }

    public void RemoveDisk(HanoiDisk disk)
    {
        if (disk == null)
            return;

        disks.Remove(disk);
    }

    public void ClearDisks()
    {
        disks.Clear();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (manager == null)
            manager = FindFirstObjectByType<HanoiGameManager>();

        if (manager == null || eventData.pointerDrag == null)
            return;

        HanoiDisk disk = eventData.pointerDrag.GetComponent<HanoiDisk>();
        if (disk != null)
            manager.TryMove(disk, this);
    }

    public void LayoutDisks(float diskHeight, float bottomOffset, bool animate)
    {
        RectTransform rodRect = transform as RectTransform;
        if (rodRect == null)
            return;

        for (int i = 0; i < disks.Count; i++)
        {
            HanoiDisk disk = disks[i];
            if (disk == null)
                continue;

            RectTransform diskRect = disk.GetComponent<RectTransform>();
            if (diskRect == null)
                continue;

            // Lower disks first, so each disk is drawn over the one below it.
            diskRect.SetParent(rodRect.parent, true);
            diskRect.SetAsLastSibling();
            diskRect.localRotation = Quaternion.identity;
            disk.MoveTo(new Vector2(
                rodRect.anchoredPosition.x,
                rodRect.anchoredPosition.y + bottomOffset + i * diskHeight
            ), animate);
        }
    }
}
