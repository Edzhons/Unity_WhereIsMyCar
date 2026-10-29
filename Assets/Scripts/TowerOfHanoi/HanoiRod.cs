using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Attach to each rod's UI drop target (Image with Raycast Target enabled).</summary>
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

    public void LayoutDisks(float diskHeight, float bottomOffset)
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

            diskRect.SetParent(rodRect.parent, true);
            diskRect.SetAsLastSibling();
            diskRect.anchoredPosition = new Vector2(
                rodRect.anchoredPosition.x,
                rodRect.anchoredPosition.y + bottomOffset + (i + 0.5f) * diskHeight
            );
            diskRect.localRotation = Quaternion.identity;
        }
    }
}
