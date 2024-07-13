using System;
using System.Collections.Generic;

namespace UIFramework.Runtime.InfoContainer
{
    public class UIInfoContainer
    {
        private readonly Dictionary<Type, UIInfo> _infos = new(128);
        
        public void AddInfo(UIInfo info)
        {
            if (!_infos.TryAdd(info.PageType, info))
            {
                UILogger.Error($"[UI] AddInfo: UIInfo 已添加 - {info.PageType.Name}");
            }
        }

        public bool TryGetInfo(Type pageType, out UIInfo info)
        {
            if (!_infos.TryGetValue(pageType, out info))
            {
                UILogger.Error($"[UI] UIInfo 获取失败: {pageType.Name}");
                return false;
            }

            return true;
        }

        public void Release()
        {
            _infos.Clear();
        }
    }
}