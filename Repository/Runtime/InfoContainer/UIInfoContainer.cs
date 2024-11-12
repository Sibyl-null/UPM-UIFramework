using System;
using System.Collections.Generic;
using UIFramework.Runtime.Utility;

namespace UIFramework.Runtime.InfoContainer
{
    public class UIInfoContainer
    {
        private readonly IUILogger _logger;
        private readonly Dictionary<Type, UIInfo> _infos = new(128);

        public UIInfoContainer(IUILogger logger)
        {
            _logger = logger;
        }

        public void AddInfo(UIInfo info)
        {
            if (!_infos.TryAdd(info.PageType, info))
            {
                _logger.Error($"[UI] AddInfo: UIInfo 已添加 - {info.PageType.Name}");
            }
        }

        public bool TryGetInfo(Type pageType, out UIInfo info)
        {
            if (!_infos.TryGetValue(pageType, out info))
            {
                _logger.Error($"[UI] UIInfo 获取失败: {pageType.Name}");
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