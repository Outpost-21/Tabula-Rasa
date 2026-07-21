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
        private const float Width = 215f;
        private const float Height = 75f;
        private const float CursorGap = 8f;

        private readonly string _label;
        private readonly int _floor;
        private readonly int _ceiling;
        private readonly Func<int> _current;
        private readonly Action<int> _callback;

        public override Vector2 InitialSize => new Vector2(Width, Height);

        public Popup_IntSlider(string label, int floor, int ceiling, Func<int> current, Action<int> callback)
        {
            _label = label;
            _floor = floor;
            _ceiling = ceiling;
            _current = current;
            _callback = callback;

            closeOnClickedOutside = true;
        }

        public override void SetInitialSizeAndPosition()
        {
            var size = InitialSize;
            var position = Verse.UI.MousePositionOnUIInverted;
            position.x = Mathf.Clamp(position.x, 0f, Verse.UI.screenWidth - size.x);
            position.y = Mathf.Clamp(position.y - size.y - CursorGap, 0f, Verse.UI.screenHeight - size.y);
            windowRect = new Rect(position.x, position.y, size.x, size.y);
        }

        public override void DoWindowContents(Rect rect)
        {
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
