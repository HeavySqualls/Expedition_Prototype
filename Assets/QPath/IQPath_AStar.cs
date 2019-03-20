using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QPath
{
    public class IQPath_AStar
    {
        public IQPath_AStar (IQPathWorld world, IQPathUnit unit, IQPathTile startTile, IQPathTile endTile)
        {
            // Do setup
            this.world = world;
            this.unit = unit;
            this.startTile = startTile;
            this.endTile = endTile;

            // Do we need to explicitly create a graph??
        }

        IQPathWorld world;
        IQPathUnit unit;
        IQPathTile startTile;
        IQPathTile endTile;

        Queue<IQPathTile> path;

        public void DoWork()
        {
            path = new Queue<IQPathTile>();

            HashSet<IQPathTile> closedSet = new HashSet<IQPathTile>();

            HashSet<IQPathTile> openSet = new HashSet<IQPathTile>();








        }

        public IQPathTile[] GetList()
        {
            return path.ToArray();
        }

    }
}
