using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhostSpotMapping : Singleton<GhostSpotMapping>
{ 
    private IDictionary<string, string> __ghostToSpot = new Dictionary<string, string>();
    private string __interactedSpot = null;

    public string GetSpot(string ghostName)
    {
        if (!__ghostToSpot.ContainsKey(ghostName)) return null;
        else return __ghostToSpot[ghostName];
    }

    public string GetInteractedSpot()
    {
        return __interactedSpot;
    }

    public void UpdateSpot(string ghostName, string spotName)
    {
        try { __ghostToSpot.Add(ghostName, spotName); }
        catch (ArgumentException) { __ghostToSpot[ghostName] = spotName; }
    }

    public void UpdateInteracted(string spotName)
    {
        __interactedSpot = spotName;
    }
}
