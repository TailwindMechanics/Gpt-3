using System.Collections.Generic;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.DepthPerceiver
{
    public class PerspectiveGrid
    {
        public Cell[] CreateGrid(Texture2D screenshot, int subdivisions, Camera cam, List<ObjectData> allObjects)
        {
            var cellSize = Mathf.Min(screenshot.width, screenshot.height) / subdivisions;
            var totalCells = subdivisions * subdivisions;

            var result = new Cell[totalCells];

            var idCounter = 0;
            for (var i = 0; i < subdivisions; i++)
            {
                for (var j = 0; j < subdivisions; j++)
                {
                    var cell = new Cell
                    {
                        ID = idCounter,
                        Bounds = new RectSerializable(j * cellSize, i * cellSize, cellSize, cellSize)
                    };

                    cell.ContainedObjects = DetermineContainedObjects(cell.Bounds.Value(), cam, allObjects);

                    result[idCounter] = cell;
                    idCounter++;
                }
            }

            return result;
        }

        List<ObjectData> DetermineContainedObjects(Rect cellBounds, Camera cam, List<ObjectData> allObjects)
        {
            var objectsInCell = new List<ObjectData>();

            allObjects.ForEach(item =>
            {
                var screenPos = cam.WorldToScreenPoint(item.WorldPosition.Value());
                if (cellBounds.Contains(screenPos))
                {
                    objectsInCell.Add(item);
                }
            });

            return objectsInCell;
        }
    }
}