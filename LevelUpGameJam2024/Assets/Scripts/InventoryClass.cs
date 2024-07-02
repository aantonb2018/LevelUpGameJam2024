using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryClass
{
    private int[] equiped = new int[2];
    public int[] Equiped
    {
        get { return equiped; }
        set { equiped = value; }
    }

    private int[] usable = new int[6];
    public int[] Usable
    {
        get { return usable; }
        set { usable = value; }
    }

    private int[] resources = new int[16];
    public int[] Resources
    {
        get { return resources; }
        set { resources = value; }
    }

    public InventoryClass(int[] equiped, int[] usable, int[] resources)
    {
        this.equiped = equiped;
        this.usable = usable;
        this.resources = resources;
    }

    public void EquipWeapon(int idx)
    {
        int temp = equiped[0];
        equiped[0] = resources[idx];
        resources[idx] = equiped[0];
    }
    public void EquipArmour(int idx)
    {
        int temp = equiped[1];
        equiped[1] = resources[idx];
        resources[idx] = equiped[1];
    }

    public void EquipUsable(int idx0, int idx1)
    {
        //Idx 0 será siempre el usable
        int temp = usable[idx0];
        usable[idx0] = resources[idx1];
        resources[idx1] = usable[idx0];
    }

    public void ExchangeUsable(int idx0, int idx1)
    {
        //Idx 0 será siempre el usable
        int temp = usable[idx0];
        usable[idx0] = usable[idx1];
        usable[idx1] = usable[idx0];
    }

    public void ExchangeResources(int idx0, int idx1)
    {
        //Idx 0 será siempre el usable
        int temp = resources[idx0];
        resources[idx0] = resources[idx1];
        resources[idx1] = resources[idx0];
    }

}
