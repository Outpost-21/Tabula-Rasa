using RimWorld;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
	public static class SettingsUtil
	{
		public static void CheckboxEnhanced(this Listing_Standard listing, string name, string explanation, ref bool value, string tooltip = null)
		{
			float curHeight = listing.CurHeight;
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			listing.CheckboxLabeled(name, ref value, null);
			Text.Font = GameFont.Tiny;
			listing.ColumnWidth -= 34f;
			GUI.color = Color.gray;
			listing.Label(explanation, -1f, null);
			listing.ColumnWidth += 34f;
			Text.Font = GameFont.Small;
			Rect rect = listing.GetRect(0f);
			rect.height = listing.CurHeight - curHeight;
			rect.y -= rect.height;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				if (!tooltip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, tooltip);
				}
			}
			GUI.color = Color.white;
			listing.Gap(6f);
		}

		public static void Note(this Listing_Standard listing, string name, GameFont font = GameFont.Small)
		{
			Text.Font = font;
			listing.ColumnWidth -= 34f;
			GUI.color = Color.white;
			listing.Label(name, -1f, null);
			listing.ColumnWidth += 34f;
			Text.Font = GameFont.Small;
		}

		public static void ValueLabeled<T>(this Listing_Standard listing, string name, string explanation, ref T value, string tooltip = null)
		{
			float curHeight = listing.CurHeight;
			Rect rect = listing.GetRect(Text.LineHeight + listing.verticalSpacing);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect, (name));
			Text.Anchor = TextAnchor.MiddleRight;
			if (typeof(T).IsEnum)
			{
				Widgets.Label(rect, (value.ToString().Replace("_", " ")));
			}
			else
			{
				Widgets.Label(rect, value.ToString());
			}
			Text.Anchor = anchor;

			Text.Font = GameFont.Tiny;
			listing.ColumnWidth -= 34f;
			GUI.color = Color.gray;
			listing.Label(explanation, -1f, null);
			listing.ColumnWidth += 34f;
			Text.Font = GameFont.Small;

			rect = listing.GetRect(0f);
			rect.height = listing.CurHeight - curHeight;
			rect.y -= rect.height;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				if (!tooltip.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, tooltip);
				}
				if (Event.current.isMouse && Event.current.button == 0 && Event.current.type == EventType.MouseDown)
				{
					T[] array = Enum.GetValues(typeof(T)).Cast<T>().ToArray<T>();
					for (int i = 0; i < array.Length; i++)
					{
						T t = array[(i + 1) % array.Length];
						if (array[i].ToString() == value.ToString())
						{
							value = t;
							break;
						}
					}
					Event.current.Use();
				}
			}
			GUI.color = Color.white;
			listing.Gap(6f);
		}
		public static void SettingsDropdown(this Listing_Standard listing, string name, string explanation, ref TabulaRasaSettingsPage value, float width)
		{
			float curHeight = listing.CurHeight;
			Rect rect = listing.GetRect(Text.LineHeight + listing.verticalSpacing);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect, name);
			Text.Anchor = TextAnchor.MiddleRight;

			Rect rect2 = new Rect(width - 150f, 0f, 150f, 29f);
			if (Widgets.ButtonText(rect2, value.ToString().Replace("_", " "), true, true, true))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				List<TabulaRasaSettingsPage> enumValues = Enum.GetValues(typeof(TabulaRasaSettingsPage)).Cast<TabulaRasaSettingsPage>().ToList();
				foreach (TabulaRasaSettingsPage enumValue in enumValues)
				{
					list.Add(new FloatMenuOption(enumValue.ToString().Replace("_", " "), delegate ()
					{
						TabulaRasaMod.mod.currentPage = enumValue;
					}));
				}

				Find.WindowStack.Add(new FloatMenu(list));
			}

			Text.Anchor = anchor;

			Text.Font = GameFont.Tiny;
			listing.ColumnWidth -= 34f;
			GUI.color = Color.gray;
			listing.Label(explanation);
			listing.ColumnWidth += 34f;
			Text.Font = GameFont.Small;

			rect = listing.GetRect(0f);
			rect.height = listing.CurHeight - curHeight;
			rect.y -= rect.height;
			GUI.color = Color.white;
			listing.Gap(6f);
		}
	}

	public static class EnhancedListingStandard
	{
		private static float gap = 12f;

		private static float lineGap = 3f;
		public static float Gap
		{
			get
			{
				return EnhancedListingStandard.gap;
			}
			set
			{
				EnhancedListingStandard.gap = value;
			}
		}

		public static float LineGap
		{
			get
			{
				return EnhancedListingStandard.lineGap;
			}
			set
			{
				EnhancedListingStandard.lineGap = value;
			}
		}

		public static Listing_Standard BeginListingStandard(this Rect rect, int columns = 1)
		{
			Listing_Standard listing_Standard = new Listing_Standard
			{
				ColumnWidth = rect.width / (float)columns - (float)columns * 5f
			};
			listing_Standard.Begin(rect);
			return listing_Standard;
		}

		public static void AddHorizontalLine(this Listing_Standard listing_Standard, float? gap = null)
		{
			listing_Standard.Gap(gap ?? EnhancedListingStandard.lineGap);
			listing_Standard.GapLine(gap ?? EnhancedListingStandard.lineGap);
		}

		public static void AddLabelLine(this Listing_Standard listing_Standard, string label, float? height = null)
		{
			listing_Standard.Gap(EnhancedListingStandard.Gap);
			Widgets.Label(listing_Standard.GetRect(height), label);
		}

		public static Rect GetRect(this Listing_Standard listing_Standard, float? height = null)
		{
			return listing_Standard.GetRect(height ?? Text.LineHeight);
		}

		public static Rect LineRectSpilter(this Listing_Standard listing_Standard, out Rect leftHalf, float leftPartPct = 0.5f, float? height = null)
		{
			Rect rect = listing_Standard.GetRect(height);
			leftHalf = GenUI.Rounded(GenUI.LeftPart(rect, leftPartPct));
			return rect;
		}

		public static Rect LineRectSpilter(this Listing_Standard listing_Standard, out Rect leftHalf, out Rect rightHalf, float leftPartPct = 0.5f, float? height = null)
		{
			Rect rect = listing_Standard.LineRectSpilter(out leftHalf, leftPartPct, height);
			rightHalf = GenUI.Rounded(GenUI.RightPart(rect, 1f - leftPartPct));
			return rect;
		}

		public static void AddLabeledRadioList(this Listing_Standard listing_Standard, string header, string[] labels, ref string val, float? headerHeight = null)
		{
			listing_Standard.Gap(EnhancedListingStandard.Gap);
			listing_Standard.AddLabelLine(header, headerHeight);
			listing_Standard.AddRadioList(EnhancedListingStandard.GenerateLabeledRadioValues(labels), ref val, null);
		}

		public static void AddLabeledRadioList<T>(this Listing_Standard listing_Standard, string header, Dictionary<string, T> dict, ref T val, float? headerHeight = null)
		{
			listing_Standard.Gap(EnhancedListingStandard.Gap);
			listing_Standard.AddLabelLine(header, headerHeight);
			listing_Standard.AddRadioList(EnhancedListingStandard.GenerateLabeledRadioValues<T>(dict), ref val, null);
		}

		private static void AddRadioList<T>(this Listing_Standard listing_Standard, List<EnhancedListingStandard.LabeledRadioValue<T>> items, ref T val, float? height = null)
		{
			foreach (EnhancedListingStandard.LabeledRadioValue<T> labeledRadioValue in items)
			{
				listing_Standard.Gap(EnhancedListingStandard.Gap);
				if (Widgets.RadioButtonLabeled(listing_Standard.GetRect(height), labeledRadioValue.Label, EqualityComparer<T>.Default.Equals(labeledRadioValue.Value, val)))
				{
					val = labeledRadioValue.Value;
				}
			}
		}

		private static List<EnhancedListingStandard.LabeledRadioValue<string>> GenerateLabeledRadioValues(string[] labels)
		{
			List<EnhancedListingStandard.LabeledRadioValue<string>> list = new List<EnhancedListingStandard.LabeledRadioValue<string>>();
			foreach (string text in labels)
			{
				list.Add(new EnhancedListingStandard.LabeledRadioValue<string>(text, text));
			}
			return list;
		}

		private static List<EnhancedListingStandard.LabeledRadioValue<T>> GenerateLabeledRadioValues<T>(Dictionary<string, T> dict)
		{
			List<EnhancedListingStandard.LabeledRadioValue<T>> list = new List<EnhancedListingStandard.LabeledRadioValue<T>>();
			foreach (KeyValuePair<string, T> keyValuePair in dict)
			{
				list.Add(new EnhancedListingStandard.LabeledRadioValue<T>(keyValuePair.Key, keyValuePair.Value));
			}
			return list;
		}

		public static void AddLabeledTextField(this Listing_Standard listing_Standard, string label, ref string settingsValue, float leftPartPct = 0.5f)
		{
			listing_Standard.Gap(EnhancedListingStandard.Gap);
			listing_Standard.LineRectSpilter(out Rect rect, out Rect rect2, leftPartPct, null);
			Widgets.Label(rect, label);
			string text = settingsValue.ToString();
			settingsValue = Widgets.TextField(rect2, text);
		}

		public static void AddLabeledNumericalTextField<T>(this Listing_Standard listing_Standard, string label, ref T settingsValue, float leftPartPct = 0.5f, float minValue = 1f, float maxValue = 100000f) where T : struct
		{
			listing_Standard.Gap(EnhancedListingStandard.Gap);
			listing_Standard.LineRectSpilter(out Rect rect, out Rect rect2, leftPartPct, null);
			Widgets.Label(rect, label);
			string text = settingsValue.ToString();
			Widgets.TextFieldNumeric<T>(rect2, ref settingsValue, ref text, minValue, maxValue);
		}

		public static void AddLabeledCheckbox(this Listing_Standard listing_Standard, string label, ref bool settingsValue)
		{
			listing_Standard.Gap(EnhancedListingStandard.Gap);
			listing_Standard.CheckboxLabeled(label, ref settingsValue, null);
		}

		public static void AddLabeledSlider(this Listing_Standard listing_Standard, string label, ref float value, float leftValue, float rightValue, string leftAlignedLabel = null, string rightAlignedLabel = null, float roundTo = -1f, bool middleAlignment = false)
		{
			listing_Standard.Gap(EnhancedListingStandard.Gap);
			listing_Standard.LineRectSpilter(out Rect rect, out Rect rect2, 0.5f, null);
			Widgets.Label(rect, label);
			float num = value;
			value = Widgets.HorizontalSlider(GenUI.BottomPart(rect2, 0.7f), num, leftValue, rightValue, middleAlignment, null, leftAlignedLabel, rightAlignedLabel, roundTo);
		}

		public static void AddColorPickerButton(this Listing_Standard listing_Standard, string label, Color color, Action<Color> callback, string buttonText = "Change")
		{
			listing_Standard.Gap(EnhancedListingStandard.Gap);
			Rect rect = listing_Standard.GetRect(null);
			float num = Text.CalcSize(buttonText).x + 10f;
			float num2 = num + 5f + rect.height;
			Rect rect2 = GenUI.RightPartPixels(rect, num + 5f + rect.height);
			if (Widgets.ButtonText(GenUI.LeftPartPixels(rect2, num), buttonText, true, false, true))
			{
				Find.WindowStack.Add(new Dialog_ColourPicker(color, callback, null));
			}
			GUI.color = color;
			GUI.DrawTexture(GenUI.RightPartPixels(rect2, rect2.height), BaseContent.WhiteTex);
			GUI.color = Color.white;
			Widgets.Label(GenUI.LeftPartPixels(rect, rect.width - num2), label);
		}
		public static void AddHoloColorPickerButton(this Listing_Standard listing_Standard, string label, Color color, Comp_HologramProjection comp, int layer, string buttonText = "Change")
		{
			listing_Standard.Gap(EnhancedListingStandard.Gap);
			Rect rect = listing_Standard.GetRect(null);
			float num = Text.CalcSize(buttonText).x + 10f;
			float num2 = num + 5f + rect.height;
			Rect rect2 = GenUI.RightPartPixels(rect, num + 5f + rect.height);
			if (Widgets.ButtonText(GenUI.LeftPartPixels(rect2, num), buttonText, true, false, true))
			{
				Find.WindowStack.Add(new Win_HologramColorPicker(color, comp, layer, null));
			}
			GUI.color = color;
			GUI.DrawTexture(GenUI.RightPartPixels(rect2, rect2.height), BaseContent.WhiteTex);
			GUI.color = Color.white;
			Widgets.Label(GenUI.LeftPartPixels(rect, rect.width - num2), label);
		}

		public static void AddColorPickerButton(this Listing_Standard listing_Standard, string label, Color color, string fieldName, object colorContainer, string buttonText = "Change")
		{
			listing_Standard.AddColorPickerButton(label, color, delegate (Color c)
			{
				colorContainer.GetType().GetField(fieldName).SetValue(colorContainer, color);
			}, buttonText);
		}

		public static float Slider(this Listing_Standard listing_Standard, float val, float min, float max, string label = null, string leftAlignedLabel = null, string rightAlignedLabel = null, float roundTo = -1f, bool middleAlignment = false)
		{
			float result = Widgets.HorizontalSlider(listing_Standard.GetRect(22f), val, min, max, middleAlignment, label, leftAlignedLabel, rightAlignedLabel, roundTo);
			listing_Standard.Gap(listing_Standard.verticalSpacing);
			return result;
		}

		public static void AddLabeledSlider<T>(this Listing_Standard listing_Standard, string label, ref T value) where T : Enum
		{
			object value2 = value;
			listing_Standard.Gap(10f);
			listing_Standard.LineRectSpilter(out Rect rect, out Rect rect2, 0.5f, null);
			Widgets.Label(rect, label);
			float num = (float)Convert.ToInt32(value2);
			float num2 = Widgets.HorizontalSlider(GenUI.BottomPart(rect2, 0.7f), num, 0f, (float)(Enum.GetValues(typeof(T)).Length - 1), true, Enum.GetName(typeof(T), value), null, null, 1f);
			value = (T)((object)Enum.ToObject(typeof(T), (int)num2));
		}

		public class LabeledRadioValue<T>
		{
			private T val;

			private string label;

			public T Value
			{
				get
				{
					return val;
				}
				set
				{
					val = value;
				}
			}

			public string Label
			{
				get
				{
					return label;
				}
				set
				{
					label = value;
				}
			}

			public LabeledRadioValue(string label, T val)
			{
				Label = label;
				Value = val;
			}
		}
	}

	public class ColourWrapper
	{
		public Color Color { get; set; }

		public ColourWrapper(Color color)
		{
			this.Color = color;
		}
	}

	public class Dialog_ColourPicker : Window
	{
		private Dialog_ColourPicker.Controls _activeControl = Dialog_ColourPicker.Controls.none;
		private Texture2D _colourPickerBG;
		private Texture2D _huePickerBG;
		private Texture2D _alphaPickerBG;
		private Texture2D _tempPreviewBG;
		private Texture2D _previewBG;
		private Texture2D _pickerAlphaBG;
		private Texture2D _sliderAlphaBG;
		private Texture2D _previewAlphaBG;
		private Color _alphaBGColorA = Color.white;
		private Color _alphaBGColorB = new Color(0.85f, 0.85f, 0.85f);
		private int _pickerSize = 300;
		private int _sliderWidth = 15;
		private int _alphaBGBlockSize = 10;
		private int _previewSize = 90;
		private int _handleSize = 10;
		private float _margin = 6f;
		private float _fieldHeight = 30f;
		private float _huePosition;
		private float _alphaPosition;
		private float _unitsPerPixel;
		private float _H;
		private float _S = 1f;
		private float _V = 1f;
		private float _A = 1f;
		private Vector2 _position = Vector2.zero;
		private string _hexOut;
		private string _hexIn;
		private Action<Color> _callback;
		public Color curColour = Color.blue;
		public Color tempColour = Color.white;
		private Vector2? _initialPosition;
		public static bool first;

		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2((float)this._pickerSize + 3f * this._margin + (float)(2 * this._sliderWidth) + (float)(2 * this._previewSize) + 36f, (float)this._pickerSize + 36f);
			}
		}

		public Vector2 InitialPosition
		{
			get
			{
				Vector2? initialPosition = this._initialPosition;
				if (initialPosition == null)
				{
					return new Vector2((float)UI.screenWidth - this.InitialSize.x, (float)UI.screenHeight - this.InitialSize.y) / 2f;
				}
				return initialPosition.GetValueOrDefault();
			}
		}

		public Dialog_ColourPicker(Color color, Action<Color> callback = null, Vector2? position = null)
		{
			this._callback = callback;
			this._initialPosition = position;
			this.curColour = color;
			this.NotifyRGBUpdated();
		}

		public float UnitsPerPixel
		{
			get
			{
				if (this._unitsPerPixel == 0f)
				{
					this._unitsPerPixel = 1f / (float)this._pickerSize;
				}
				return this._unitsPerPixel;
			}
		}

		public float H
		{
			get
			{
				return this._H;
			}
			set
			{
				this._H = Mathf.Clamp(value, 0f, 1f);
				this.NotifyHSVUpdated();
				this.CreateColourPickerBG();
				this.CreateAlphaPickerBG();
			}
		}

		public float S
		{
			get
			{
				return this._S;
			}
			set
			{
				this._S = Mathf.Clamp(value, 0f, 1f);
				this.NotifyHSVUpdated();
				this.CreateAlphaPickerBG();
			}
		}

		public float V
		{
			get
			{
				return this._V;
			}
			set
			{
				this._V = Mathf.Clamp(value, 0f, 1f);
				this.NotifyHSVUpdated();
				this.CreateAlphaPickerBG();
			}
		}

		public float A
		{
			get
			{
				return this._A;
			}
			set
			{
				this._A = Mathf.Clamp(value, 0f, 1f);
				this.NotifyHSVUpdated();
				this.CreateColourPickerBG();
			}
		}

		public void NotifyHSVUpdated()
		{
			this.tempColour = HSV.ToRGBA(this.H, this.S, this.V, 1f);
			this.tempColour.a = this.A;
			this.CreatePreviewBG(ref this._tempPreviewBG, this.tempColour);
			this._hexOut = (this._hexIn = Dialog_ColourPicker.RGBtoHex(this.tempColour));
		}

		public void NotifyRGBUpdated()
		{
			HSV.ToHSV(this.tempColour, out this._H, out this._S, out this._V);
			this._A = this.tempColour.a;
			this.CreateColourPickerBG();
			this.CreateHuePickerBG();
			this.CreateAlphaPickerBG();
			this._huePosition = (1f - this._H) / this.UnitsPerPixel;
			this._position.x = this._S / this.UnitsPerPixel;
			this._position.y = (1f - this._V) / this.UnitsPerPixel;
			this._alphaPosition = (1f - this._A) / this.UnitsPerPixel;
			this.CreatePreviewBG(ref this._tempPreviewBG, this.tempColour);
			this._hexOut = (this._hexIn = Dialog_ColourPicker.RGBtoHex(this.tempColour));
		}

		public void SetColor()
		{
			this.curColour = this.tempColour;
			this._callback?.Invoke(this.curColour);
			this.CreatePreviewBG(ref this._previewBG, this.tempColour);
		}

		public Texture2D ColourPickerBG
		{
			get
			{
				if (this._colourPickerBG == null)
				{
					this.CreateColourPickerBG();
				}
				return this._colourPickerBG;
			}
		}

		public Texture2D HuePickerBG
		{
			get
			{
				if (this._huePickerBG == null)
				{
					this.CreateHuePickerBG();
				}
				return this._huePickerBG;
			}
		}

		public Texture2D AlphaPickerBG
		{
			get
			{
				if (this._alphaPickerBG == null)
				{
					this.CreateAlphaPickerBG();
				}
				return this._alphaPickerBG;
			}
		}

		public Texture2D TempPreviewBG
		{
			get
			{
				if (this._tempPreviewBG == null)
				{
					this.CreatePreviewBG(ref this._tempPreviewBG, this.tempColour);
				}
				return this._tempPreviewBG;
			}
		}

		public Texture2D PreviewBG
		{
			get
			{
				if (this._previewBG == null)
				{
					this.CreatePreviewBG(ref this._previewBG, this.curColour);
				}
				return this._previewBG;
			}
		}

		public Texture2D PickerAlphaBG
		{
			get
			{
				if (this._pickerAlphaBG == null)
				{
					this.CreateAlphaBG(ref this._pickerAlphaBG, this._pickerSize, this._pickerSize);
				}
				return this._pickerAlphaBG;
			}
		}

		public Texture2D SliderAlphaBG
		{
			get
			{
				if (this._sliderAlphaBG == null)
				{
					this.CreateAlphaBG(ref this._sliderAlphaBG, this._sliderWidth, this._pickerSize);
				}
				return this._sliderAlphaBG;
			}
		}

		public Texture2D PreviewAlphaBG
		{
			get
			{
				if (this._previewAlphaBG == null)
				{
					this.CreateAlphaBG(ref this._previewAlphaBG, this._previewSize, this._previewSize);
				}
				return this._previewAlphaBG;
			}
		}

		private void SwapTexture(ref Texture2D tex, Texture2D newTex)
		{
			UnityEngine.Object.Destroy(tex);
			tex = newTex;
		}

		private void CreateColourPickerBG()
		{
			int pickerSize = this._pickerSize;
			int pickerSize2 = this._pickerSize;
			float unitsPerPixel = this.UnitsPerPixel;
			float unitsPerPixel2 = this.UnitsPerPixel;
			Texture2D texture2D = new Texture2D(pickerSize, pickerSize2);
			for (int i = 0; i < pickerSize; i++)
			{
				for (int j = 0; j < pickerSize2; j++)
				{
					float s = (float)i * unitsPerPixel;
					float v = (float)j * unitsPerPixel2;
					texture2D.SetPixel(i, j, HSV.ToRGBA(this.H, s, v, this.A));
				}
			}
			texture2D.Apply();
			this.SwapTexture(ref this._colourPickerBG, texture2D);
		}

		private void CreateHuePickerBG()
		{
			Texture2D texture2D = new Texture2D(1, this._pickerSize);
			int pickerSize = this._pickerSize;
			float num = 1f / (float)pickerSize;
			for (int i = 0; i < pickerSize; i++)
			{
				texture2D.SetPixel(0, i, HSV.ToRGBA(num * (float)i, 1f, 1f, 1f));
			}
			texture2D.Apply();
			this.SwapTexture(ref this._huePickerBG, texture2D);
		}

		private void CreateAlphaPickerBG()
		{
			Texture2D texture2D = new Texture2D(1, this._pickerSize);
			int pickerSize = this._pickerSize;
			float num = 1f / (float)pickerSize;
			for (int i = 0; i < pickerSize; i++)
			{
				texture2D.SetPixel(0, i, new Color(this.tempColour.r, this.tempColour.g, this.tempColour.b, (float)i * num));
			}
			texture2D.Apply();
			this.SwapTexture(ref this._alphaPickerBG, texture2D);
		}

		private void CreateAlphaBG(ref Texture2D bg, int width, int height)
		{
			Texture2D texture2D = new Texture2D(width, height);
			Color[] array = new Color[this._alphaBGBlockSize * this._alphaBGBlockSize];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this._alphaBGColorA;
			}
			Color[] array2 = new Color[this._alphaBGBlockSize * this._alphaBGBlockSize];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = this._alphaBGColorB;
			}
			int num = 0;
			for (int k = 0; k < width; k += this._alphaBGBlockSize)
			{
				int num2 = num;
				for (int l = 0; l < height; l += this._alphaBGBlockSize)
				{
					texture2D.SetPixels(k, l, this._alphaBGBlockSize, this._alphaBGBlockSize, (num2 % 2 == 0) ? array : array2);
					num2++;
				}
				num++;
			}
			texture2D.Apply();
			this.SwapTexture(ref bg, texture2D);
		}

		public void CreatePreviewBG(ref Texture2D bg, Color col)
		{
			this.SwapTexture(ref bg, SolidColorMaterials.NewSolidColorTexture(col));
		}

		public void PickerAction(Vector2 pos)
		{
			this._S = this.UnitsPerPixel * pos.x;
			this._V = 1f - this.UnitsPerPixel * pos.y;
			this.CreateAlphaPickerBG();
			this.NotifyHSVUpdated();
			this._position = pos;
		}

		public void HueAction(float pos)
		{
			this.H = 1f - this.UnitsPerPixel * pos;
			this._huePosition = pos;
		}

		public void AlphaAction(float pos)
		{
			this.A = 1f - this.UnitsPerPixel * pos;
			this._alphaPosition = pos;
		}

		public override void SetInitialSizeAndPosition()
		{
			Vector2 vector = new Vector2(Mathf.Min(this.InitialSize.x, (float)UI.screenWidth), Mathf.Min(this.InitialSize.y, (float)UI.screenHeight - 35f));
			Vector2 vector2 = new Vector2(Mathf.Max(0f, Mathf.Min(this.InitialPosition.x, (float)UI.screenWidth - vector.x)), Mathf.Max(0f, Mathf.Min(this.InitialPosition.y, (float)UI.screenHeight - vector.y)));
			this.windowRect = new Rect(vector2.x, vector2.y, vector.x, vector.y);
		}

		public override void PreOpen()
		{
			base.PreOpen();
			this.NotifyHSVUpdated();
			this._alphaPosition = this.curColour.a / this.UnitsPerPixel;
		}

		public static string RGBtoHex(Color col)
		{
			int num = (int)Mathf.Clamp(col.r * 256f, 0f, 255f);
			int num2 = (int)Mathf.Clamp(col.g * 256f, 0f, 255f);
			int num3 = (int)Mathf.Clamp(col.b * 256f, 0f, 255f);
			int num4 = (int)Mathf.Clamp(col.a * 256f, 0f, 255f);
			return string.Concat(new string[]
			{
				"#",
				num.ToString("X2"),
				num2.ToString("X2"),
				num3.ToString("X2"),
				num4.ToString("X2")
			});
		}

		public static bool TryGetColorFromHex(string hex, out Color col)
		{
			Color color = new Color(0f, 0f, 0f);
			if (hex != null && hex.Length == 9)
			{
				try
				{
					string text = hex.Substring(1, hex.Length - 1);
					color.r = (float)int.Parse(text.Substring(0, 2), NumberStyles.AllowHexSpecifier) / 255f;
					color.g = (float)int.Parse(text.Substring(2, 2), NumberStyles.AllowHexSpecifier) / 255f;
					color.b = (float)int.Parse(text.Substring(4, 2), NumberStyles.AllowHexSpecifier) / 255f;
					if (text.Length == 8)
					{
						color.a = (float)int.Parse(text.Substring(6, 2), NumberStyles.AllowHexSpecifier) / 255f;
					}
					else
					{
						color.a = 1f;
					}
				}
				catch (Exception)
				{
					col = Color.white;
					return false;
				}
				col = color;
				return true;
			}
			col = Color.white;
			return false;
		}

		public override void DoWindowContents(Rect inRect)
		{
			if (Dialog_ColourPicker.first)
			{
				//LogUtil.LogMessage(this.InitialSize.ToString());
				//LogUtil.LogMessage(this.windowRect.ToString());
			}
			Rect rect = new Rect(inRect.xMin, inRect.yMin, (float)this._pickerSize, (float)this._pickerSize);
			Rect rect2 = new Rect(rect.xMax + this._margin, inRect.yMin, (float)this._sliderWidth, (float)this._pickerSize);
			Rect rect3 = new Rect(rect2.xMax + this._margin, inRect.yMin, (float)this._sliderWidth, (float)this._pickerSize);
			Rect rect4 = new Rect(rect3.xMax + this._margin, inRect.yMin, (float)this._previewSize, (float)this._previewSize);
			Rect rect5 = new Rect(rect4.xMax, inRect.yMin, (float)this._previewSize, (float)this._previewSize);
			Rect rect6 = new Rect(rect3.xMax + this._margin, inRect.yMax - this._fieldHeight, (float)(this._previewSize * 2), this._fieldHeight);
			Rect rect7 = new Rect(rect3.xMax + this._margin, inRect.yMax - 2f * this._fieldHeight - this._margin, (float)this._previewSize - this._margin / 2f, this._fieldHeight);
			Rect rect8 = new Rect(rect7.xMax + this._margin, rect7.yMin, (float)this._previewSize - this._margin / 2f, this._fieldHeight);
			Rect rect9 = new Rect(rect3.xMax + this._margin, inRect.yMax - 3f * this._fieldHeight - 2f * this._margin, (float)(this._previewSize * 2), this._fieldHeight);
			GUI.DrawTexture(rect, this.PickerAlphaBG);
			GUI.DrawTexture(rect3, this.SliderAlphaBG);
			GUI.DrawTexture(rect4, this.PreviewAlphaBG);
			GUI.DrawTexture(rect5, this.PreviewAlphaBG);
			GUI.DrawTexture(rect, this.ColourPickerBG);
			GUI.DrawTexture(rect2, this.HuePickerBG);
			GUI.DrawTexture(rect3, this.AlphaPickerBG);
			GUI.DrawTexture(rect4, this.TempPreviewBG);
			GUI.DrawTexture(rect5, this.PreviewBG);
			Rect rect10 = new Rect(rect2.xMin - 3f, rect2.yMin + this._huePosition - (float)(this._handleSize / 2), (float)this._sliderWidth + 6f, (float)this._handleSize);
			Rect rect11 = new Rect(rect3.xMin - 3f, rect3.yMin + this._alphaPosition - (float)(this._handleSize / 2), (float)this._sliderWidth + 6f, (float)this._handleSize);
			Rect rect12 = new Rect(rect.xMin + this._position.x - (float)(this._handleSize / 2), rect.yMin + this._position.y - (float)(this._handleSize / 2), (float)this._handleSize, (float)this._handleSize);
			GUI.DrawTexture(rect10, this.TempPreviewBG);
			GUI.DrawTexture(rect11, this.TempPreviewBG);
			GUI.DrawTexture(rect12, this.TempPreviewBG);
			GUI.color = Color.gray;
			Widgets.DrawBox(rect10, 1);
			Widgets.DrawBox(rect11, 1);
			Widgets.DrawBox(rect12, 1);
			GUI.color = Color.white;
			if (Input.GetMouseButtonUp(0))
			{
				this._activeControl = Dialog_ColourPicker.Controls.none;
			}
			if (Mouse.IsOver(rect))
			{
				if (Input.GetMouseButtonDown(0))
				{
					this._activeControl = Dialog_ColourPicker.Controls.colourPicker;
				}
				if (this._activeControl == Dialog_ColourPicker.Controls.colourPicker)
				{
					Vector2 pos = Event.current.mousePosition - new Vector2(rect.xMin, rect.yMin);
					this.PickerAction(pos);
				}
			}
			if (Mouse.IsOver(rect2))
			{
				if (Input.GetMouseButtonDown(0))
				{
					this._activeControl = Dialog_ColourPicker.Controls.huePicker;
				}
				if (Event.current.type == EventType.ScrollWheel)
				{
					this.H -= Event.current.delta.y * this.UnitsPerPixel;
					this._huePosition = Mathf.Clamp(this._huePosition + Event.current.delta.y, 0f, (float)this._pickerSize);
					Event.current.Use();
				}
				if (this._activeControl == Dialog_ColourPicker.Controls.huePicker)
				{
					float pos2 = Event.current.mousePosition.y - rect2.yMin;
					this.HueAction(pos2);
				}
			}
			if (Mouse.IsOver(rect3))
			{
				if (Input.GetMouseButtonDown(0))
				{
					this._activeControl = Dialog_ColourPicker.Controls.alphaPicker;
				}
				if (Event.current.type == EventType.ScrollWheel)
				{
					this.A -= Event.current.delta.y * this.UnitsPerPixel;
					this._alphaPosition = Mathf.Clamp(this._alphaPosition + Event.current.delta.y, 0f, (float)this._pickerSize);
					Event.current.Use();
				}
				if (this._activeControl == Dialog_ColourPicker.Controls.alphaPicker)
				{
					float pos3 = Event.current.mousePosition.y - rect3.yMin;
					this.AlphaAction(pos3);
				}
			}
			Text.Font = GameFont.Small;
			if (Widgets.ButtonText(rect6, "OK", true, false, true))
			{
				this.SetColor();
				this.Close(true);
			}
			if (Widgets.ButtonText(rect7, "Apply", true, false, true))
			{
				this.SetColor();
			}
			if (Widgets.ButtonText(rect8, "Cancel", true, false, true))
			{
				this.Close(true);
			}
			if (this._hexIn != this._hexOut)
			{
				Color color = this.tempColour;
				if (Dialog_ColourPicker.TryGetColorFromHex(this._hexIn, out color))
				{
					this.tempColour = color;
					this.NotifyRGBUpdated();
				}
				else
				{
					GUI.color = Color.red;
				}
			}
			this._hexIn = Widgets.TextField(rect9, this._hexIn);
			GUI.color = Color.white;
		}

		private enum Controls
		{
			colourPicker,
			huePicker,
			alphaPicker,
			none
		}
	}

	internal class HSV
	{
		public static Color ToRGBA(float H, float S, float V, float A = 1f)
		{
			if (S == 0f)
			{
				return new Color(V, V, V, A);
			}
			if (V == 0f)
			{
				return new Color(0f, 0f, 0f, A);
			}
			Color black = Color.black;
			float num = H * 6f;
			int num2 = Mathf.FloorToInt(num);
			float num3 = num - (float)num2;
			float num4 = V * (1f - S);
			float num5 = V * (1f - S * num3);
			float num6 = V * (1f - S * (1f - num3));
			switch (num2 + 1)
			{
				case 0:
					black.r = V;
					black.g = num4;
					black.b = num5;
					break;
				case 1:
					black.r = V;
					black.g = num6;
					black.b = num4;
					break;
				case 2:
					black.r = num5;
					black.g = V;
					black.b = num4;
					break;
				case 3:
					black.r = num4;
					black.g = V;
					black.b = num6;
					break;
				case 4:
					black.r = num4;
					black.g = num5;
					black.b = V;
					break;
				case 5:
					black.r = num6;
					black.g = num4;
					black.b = V;
					break;
				case 6:
					black.r = V;
					black.g = num4;
					black.b = num5;
					break;
				case 7:
					black.r = V;
					black.g = num6;
					black.b = num4;
					break;
			}
			black.r = Mathf.Clamp(black.r, 0f, 1f);
			black.g = Mathf.Clamp(black.g, 0f, 1f);
			black.b = Mathf.Clamp(black.b, 0f, 1f);
			black.a = Mathf.Clamp(A, 0f, 1f);
			return black;
		}

		public static void ToHSV(Color rgbColor, out float H, out float S, out float V)
		{
			if (rgbColor.b > rgbColor.g && rgbColor.b > rgbColor.r)
			{
				HSV.RGBToHSVHelper(4f, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
				return;
			}
			if (rgbColor.g > rgbColor.r)
			{
				HSV.RGBToHSVHelper(2f, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
				return;
			}
			HSV.RGBToHSVHelper(0f, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
		}

		private static void RGBToHSVHelper(float offset, float dominantcolor, float colorone, float colortwo, out float H, out float S, out float V)
		{
			V = dominantcolor;
			if (V != 0f)
			{
				float num;
				if (colorone > colortwo)
				{
					num = colortwo;
				}
				else
				{
					num = colorone;
				}
				float num2 = V - num;
				if (num2 != 0f)
				{
					S = num2 / V;
					H = offset + (colorone - colortwo) / num2;
				}
				else
				{
					S = 0f;
					H = offset + (colorone - colortwo);
				}
				H /= 6f;
				if (H < 0f)
				{
					H += 1f;
					return;
				}
			}
			else
			{
				S = 0f;
				H = 0f;
			}
		}
	}
}
