using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class Popup_IntSlider : Window
    {
        public const float width = 215f;
        public const float height = 75f;

        public string label;
        public int floor;
        public int ceiling;
        public Func<int> current;
        public Action<int> callback;

        public override Vector2 InitialSize => new Vector2(width, height);

        public Popup_IntSlider(string label, int floor, int ceiling, Func<int> current, Action<int> callback)
        {
            this.label = label;
            this.floor = floor;
            this.ceiling = ceiling;
            this.current = current;
            this.callback = callback;
            closeOnClickedOutside = true;
        }

        public override void SetInitialSizeAndPosition()
        {
            Vector2 v = UI.MousePositionOnUIInverted;
            v.x = Mathf.Clamp(v.x, 0f, UI.screenWidth - InitialSize.x);
            v.y = Mathf.Clamp(v.y - InitialSize.y, 0f, UI.screenHeight - InitialSize.y);
            windowRect = new Rect(v.x, v.y, InitialSize.x, InitialSize.y);
        }

        public override void DoWindowContents(Rect rect)
        {
            if (!rect.Contains(Event.current.mousePosition))
            {
                float distance = GenUI.DistFromRect(rect, Event.current.mousePosition);
                if (distance > 75f)
                {
                    Close(false);
                    return;
                }
            }
            callback((int)Widgets.HorizontalSlider(
                    new Rect(5, 10, 165f, 25f),
                    current(),
                    floor,
                    ceiling,
                    false,
                    "" + current() + "/" + ceiling,
                    label));
        }
    }
}
