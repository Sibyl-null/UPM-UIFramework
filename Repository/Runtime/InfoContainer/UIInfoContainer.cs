using System;
using System.Collections.Generic;
using UI.Runtime.Utility;

namespace UI.Runtime.InfoContainer
{
    public class UIInfoContainer
    {
        private readonly Dictionary<int, UIInfo> _infos = new Dictionary<int, UIInfo>(128);
        private readonly Dictionary<Type, List<UIInfo>> _typeInfoMap = new Dictionary<Type, List<UIInfo>>(128);
        
        public void AddInfo(UIInfo info)
        {
            if (!_infos.TryAdd(info.UIType, info))
            {
                UILogger.Error($"[UI] AddInfo: UIInfo 已添加 - {UIUtility.LogUIType(info.UIType)}");
                return;
            }

            if (_typeInfoMap.TryGetValue(info.PageType, out var list))
            {
                list.Add(info);
            }
            else
            {
                _typeInfoMap.Add(info.PageType, new List<UIInfo> {info});
            }
        }

        public bool TryGetInfo(int uiType, out UIInfo info)
        {
            if (!_infos.TryGetValue(uiType, out info))
            {
                UILogger.Error($"[UI] UIInfo 获取失败: {UIUtility.LogUIType(uiType)}");
                return false;
            }

            return true;
        }

        public bool TryGetInfo(Type pageType, int? uiType, out UIInfo info)
        {
            if (!_typeInfoMap.TryGetValue(pageType, out var list))
            {
                UILogger.Error($"[UI] UIInfo 获取失败: {pageType.Name}");
                info = null;
                return false;
            }

            if (list == null || list.Count == 0)
            {
                UILogger.Error($"[UI] UIInfo 获取失败: {pageType.Name}");
                info = null;
                return false;
            }

            if (uiType == null)
            {
                if (list.Count > 1)
                {
                    UILogger.Error($"[UI] UIInfo 获取失败: {pageType.Name} - 该 Page 对应多个 UIType, 请明确指定");
                    info = null;
                    return false;
                }
                
                info = list[0];
                return true;
            }
            
            foreach (UIInfo uiInfo in list)
            {
                if (uiInfo.UIType == uiType)
                {
                    info = uiInfo;
                    return true;
                }
            }

            info = null;
            return false;
        }

        public void Release()
        {
            _infos.Clear();
        }
    }
}