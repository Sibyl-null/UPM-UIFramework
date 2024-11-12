using System.Collections.Generic;
using UIFramework.Runtime.Page;
using UnityEngine;

namespace UIFramework.Runtime.LayerController
{
    public interface ILayerController
    {
        void Release();

        void AddPageInOrder(IPage target);
        void RemovePageInOrder(IPage target);

        Transform GetOrAddLayer(int layer);
        List<IPage> GetPagesInLayer(int layer);
    }
}