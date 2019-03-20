using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QPath
{
    /// <summary>
    /// 
    /// tile[] ourPath = QPath.FindPath(ourWorld, theUnit, startTile, endTile);
    /// 
    /// theUnit is an object that is the thing actually trying to path between tiles. 
    /// It might have special logic based on its movement type and the type of tiles 
    /// being moved through. 
    /// 
    /// Our tiles need to be able to return the following information:
    ///     1) List of neihbours 
    ///     2) The cost to enter this tile from another tile
    ///     
    /// </summary>
    public static class QPath
    {
        public static IQPathTile[] FindPath(IQPathWorld world, IQPathUnit unit, IQPathTile startTile, IQPathTile endTile)
        {
            if (world == null || unit == null || startTile == null || endTile == null)
            {
                Debug.LogError("Null values passed to QPath::FindPath");
                return null;
            }

            // Call on our actual path solver

            IQPath_AStar resolver = new IQPath_AStar(world, unit, startTile, endTile);

            return resolver.GetList();
        }
    }
}
