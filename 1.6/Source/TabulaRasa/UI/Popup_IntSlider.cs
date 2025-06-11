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
        private readonly string _label;
        private readonly int _floor;
        private readonly int _ceiling;
        private readonly Func<int> _current;
        private readonly Action<int> _callback;

        public Popup_IntSlider(string label, int floor, int ceiling, Func<int> current, Action<int> callback)
        {
            _label = label;
            _floor = floor;
            _ceiling = ceiling;
            _current = current;
            _callback = callback;
        }

        public override void SetInitialSizeAndPosition()
        {
            var vector = Verse.UI.MousePositionOnUIInverted;
            if (vector.x + InitialSize.x > Verse.UI.screenWidth)
            {
                vector.x = Verse.UI.screenWidth - InitialSize.x;
            }
            if (vector.y + InitialSize.y - 50 > Verse.UI.screenHeight)
            {
                vector.y = Verse.UI.screenHeight - InitialSize.y - 50;
            }
            windowRect = new Rect(vector.x, vector.y + InitialSize.y - 100, 215, 75);
        }

        public override void DoWindowContents(Rect rect)
        {
            if (!rect.Contains(Event.current.mousePosition))
            {
                var num = GenUI.DistFromRect(rect, Event.current.mousePosition);
                if (num > 75f)
                {
                    Close(false);
                    return;
                }
            }
            _callback((int)Widgets.HorizontalSlider(
                    new Rect(5, 10, 165f, 25f),
                    _current(),
                    _floor,
                    _ceiling,
                    false,
                    "" + _current() + "/" + _ceiling,
                    _label));
        }
    }
}
