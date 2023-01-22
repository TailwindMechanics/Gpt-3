using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;


namespace Modules.EditorTools.External.DataObjects
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