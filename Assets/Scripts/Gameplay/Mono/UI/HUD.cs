using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT
{
    [Serializable]
    public class HUD
    {
        private class ViewItem
        {
            public int ID;
            public CharacterHudItem View;
        }
        
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _content;
        
        private List<ViewItem> _characterViews = new();
        private HUDConfig _config;


        public void Init(HUDConfig config)
        {
            _config = config;
            EnableEnemyHPBar(false);
        }


        public void AddCharacter(int id, CharacterType type)
        {
            var view = CreateView(id, type);
            _characterViews.Add(view);
            SortItems();
        }

        private void SortItems()
        {
            _characterViews = _characterViews.OrderByDescending(i => i.ID).ToList();
            
            for (int i = 0; i < _characterViews.Count; i++)
            {
                _characterViews[i].View.transform.SetSiblingIndex(i);
            }
        }


        public void Show() => _root.gameObject.SetActive(true);


        public void Hide() => _root.gameObject.SetActive(false);
        

        private ViewItem CreateView(int id, CharacterType type)
        {
            var view = UnityEngine.Object.Instantiate(_config.HudHudItemPrefab, _content);
            var icon = _config.Icons.First(c => c.Type == type).Texture;
            view.ChangeIcon(icon);

            return new ViewItem()
            {
                ID = id,
                View = view
            };
        }
        

        public void ChangePlayerHP(float previous, float current, int id)
        {
            var item = GetView(id);
            if (item == null) return;
            
            item.View.ChangeHpBarWithDelay(previous, current, ConstPrm.UI.CHANGE_HP_BAR_DURATION);
        }
        

        public void ChangeEnemyHP(float previous, float current)
        {
            EnableEnemyHPBar(true);

            var item = GetView(ConstPrm.UI.ENEMY_UI_ID);
            if (item == null) return;
            
            item.View.ChangeHpBarWithDelay(previous, current, ConstPrm.UI.CHANGE_HP_BAR_DURATION, () =>
            {
                EnableEnemyHPBar(false);
            });
        }


        private void EnableEnemyHPBar(bool isActive)
        {
            var item = GetView(ConstPrm.UI.ENEMY_UI_ID);
            if (item == null) return;
            
            item.View.SetActive(isActive);
        }
        
        
        private ViewItem GetView(int id) => _characterViews.FirstOrDefault(x => x.ID == id);
    }
}