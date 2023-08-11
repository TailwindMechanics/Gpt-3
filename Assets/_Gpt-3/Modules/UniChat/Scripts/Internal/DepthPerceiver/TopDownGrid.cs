using System.Collections.Generic;
using System.Linq;
using Modules.UniChat.External.DataObjects.Vo;
using UnityEngine;


namespace Modules.UniChat.Internal.DepthPerceiver
{
	public class TopDownGrid
	{
		public Cell[] CreateGrid(List<ObjectData> objects, float cellSize)
		{
			var minX = objects.Min(obj => obj.WorldPosition.X);
			var maxX = objects.Max(obj => obj.WorldPosition.Y);
			var minZ = objects.Min(obj => obj.WorldPosition.Z);
			var maxZ = objects.Max(obj => obj.WorldPosition.Z);

			var rows = Mathf.CeilToInt((maxZ - minZ) / cellSize);
			var columns = Mathf.CeilToInt((maxX - minX) / cellSize);
			var cells = new Cell[rows * columns];

			var idCounter = 0;
			for (var i = 0; i < rows; i++)
			{
				for (var j = 0; j < columns; j++)
				{
					var cellBounds = new RectSerializable(minX + j * cellSize, minZ + i * cellSize, cellSize, cellSize);
					var cell = new Cell
					{
						ID = idCounter,
						Bounds = cellBounds,
						ContainedObjects = objects.Where(obj => cellBounds.Value()
							.Contains(new Vector2(obj.WorldPosition.X, obj.WorldPosition.Y)))
							.ToList()
					};

					cells[idCounter] = cell;
					idCounter++;
				}
			}

			return cells;
		}
	}
}