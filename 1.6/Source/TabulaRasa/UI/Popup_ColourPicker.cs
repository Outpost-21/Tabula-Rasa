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
    public class Popup_ColourPicker : Window
    {
        private Comp_Shield shield;
        private Color color;

        private float colorHue;
        private float colorSaturation;
        private float colorValue;

        private Color oldColor;

        private String bufferColorCode;

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(500f, 380f);
            }
        }

        public Popup_ColourPicker(Comp_Shield shield)
        {
            this.shield = shield;
            this.color = shield.currentColor;
            Color.RGBToHSV(this.color, out colorHue, out colorSaturation, out colorValue);
            UpdateBufferColorCode();

            this.optionalTitle = "ShieldGenColorTitle".Translate();
            this.forcePause = true;
            this.absorbInputAroundWindow = true;
            this.closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            this.oldColor = this.color;

            Text.Font = GameFont.Medium;

            //RGB
            this.color.r = Widgets.HorizontalSlider(new Rect(160f, 0f, 200f, 30f), this.color.r, 0f, 1f, false, "R");
            this.color.g = Widgets.HorizontalSlider(new Rect(160f, 30f, 200f, 30f), this.color.g, 0f, 1f, false, "G");
            this.color.b = Widgets.HorizontalSlider(new Rect(160f, 60f, 200f, 30f), this.color.b, 0f, 1f, false, "B");
            if (this.color != this.oldColor)
            {
                Color.RGBToHSV(this.color, out colorHue, out colorSaturation, out colorValue);
            }

            //HSV
            this.colorHue = Widgets.HorizontalSlider(new Rect(160f, 110f, 200f, 30f), this.colorHue, 0f, 1f, false, "H");
            this.colorSaturation = Widgets.HorizontalSlider(new Rect(160f, 140f, 200f, 30f), this.colorSaturation, 0f, 1f, false, "S");
            this.colorValue = Widgets.HorizontalSlider(new Rect(160f, 170f, 200f, 30f), this.colorValue, 0f, 1f, false, "V");
            this.color = Color.HSVToRGB(colorHue, colorSaturation, colorValue);

            //Hex color
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(160f, 220f, 120f, 25f), "ShieldGenHexLabel".Translate());
            HexColorCodeField(new Rect(280f, 218f, 100f, 25f));

            //Preview
            DrawColourSquare(new Rect(13f, 36f, 128f, 128f));

            if (Widgets.ButtonText(new Rect(inRect.width / 2f - 50f, inRect.height - 40f, 100f, 40f), "OK", true, false, true) || (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return))
            {
                shield.currentColor = this.color;
                Find.WindowStack.TryRemove(this, true);
            }
            Text.Font = GameFont.Medium;
        }

        private void DrawColourSquare(Rect rect)
        {
            GUI.color = this.color;
            Texture2D tex = ContentFinder<Texture2D>.Get("UI/Shield/ColorPicker");
            GUI.DrawTexture(rect, tex);
            GUI.color = Color.white;
        }

        private void HexColorCodeField(Rect rect)
        {
            if (oldColor != color)
            {
                UpdateBufferColorCode();
            }

            bufferColorCode = Widgets.TextField(rect, bufferColorCode);
            Color outColor = Color.white;
            bool isColorFormat = ColorUtility.TryParseHtmlString(bufferColorCode, out outColor);
            Color textColor = isColorFormat ? Widgets.NormalOptionColor : new Color(0.5f, 0.5f, 0.5f);
            if (Widgets.ButtonText(new Rect(rect.xMax + 15, rect.y, 70f, rect.height), "OK", false, false, textColor, true))
            {
                if (isColorFormat)
                {
                    this.color = outColor;
                    Color.RGBToHSV(this.color, out colorHue, out colorSaturation, out colorValue);
                }
                else
                {
                    Messages.Message("ShieldGenHexColorCodeIsIllFormed".Translate(), MessageTypeDefOf.CautionInput);
                }
            }

        }

        private void UpdateBufferColorCode()
        {
            int r = (int)(color.r * 255);
            int g = (int)(color.g * 255);
            int b = (int)(color.b * 255);
            int code = r * 65536 + g * 256 + b;
            bufferColorCode = "#" + code.ToString("X6");
        }
    }
}
