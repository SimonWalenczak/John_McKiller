using System.Collections.Generic;
using UnityEngine;

public class DockChecker : MonoBehaviour
{
    public List<Docker> Dockers;

    private void Update()
    {
        for (int i = 0; i < Dockers.Count; i++)
        {
            if (Dockers[i].HasBoat)
            {
                foreach (var dock in Dockers)
                {
                    dock.DocksOccupied = true;
                }
                break;
            }
        }
    }
}
