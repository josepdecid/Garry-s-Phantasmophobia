using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhostSpotMapping : Singleton<GhostSpotMapping>
{ 
    private IDictionary<string, string> __ghostToSpot = new Dictionary<string, string>();
    private IDictionary<string, string> __spotToGhost = new Dictionary<string, string>();

    private string __interactedSpot = null;

    public string GetSpot(string ghostName)
    {
        if (!__ghostToSpot.ContainsKey(ghostName)) return null;
        else return __ghostToSpot[ghostName];
    }

    public string GetGhost(string spotName)
    {
        if (!__spotToGhost.ContainsKey(spotName)) return null;
        else return __spotToGhost[spotName];
    }

    public void UpdateSpot(string ghostName, string spotName)
    {
        try
        {
            __ghostToSpot.Add(ghostName, spotName);
            __spotToGhost.Add(spotName, ghostName);
        }
        catch (ArgumentException)
        {
            __ghostToSpot[ghostName] = spotName;
            __spotToGhost[spotName] = ghostName;
        }
    }

    public void RemoveSpot(string ghostName)
    {
        string spotName = __ghostToSpot[ghostName];

        __ghostToSpot[ghostName] = null;
        __spotToGhost[spotName] = null;
    }

    public string GetInteractedSpot()
    {
        return __interactedSpot;
    }

    public void UpdateInteracted(string spotName)
    {
        __interactedSpot = spotName;
    }
}
