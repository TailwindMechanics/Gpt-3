using Object = UnityEngine.Object;
using System.Collections.Generic;
using System;


namespace Tailwind.EditorTools.External.DataObjects
{
	[Serializable]
	public class AssetDependencyVo
	{
		public Object Asset;
		public List<Object> Dependencies = new();

		public AssetDependencyVo (Object newAsset)
			=> Asset = newAsset;
		public void Add (Object newDependency)
			=> Dependencies.Add(newDependency);
	}
}