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
	public class Win_HologramColorPicker : Window
	{
		private Win_HologramColorPicker.Controls _activeControl = Win_HologramColorPicker.Controls.none;
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

		public Comp_HologramProjection holoComp;
		public int colorLayer;

		public Win_HologramColorPicker(Color color, Comp_HologramProjection comp, int layer, Action<Color> callback = null, Vector2? position = null)
		{
			this._callback = callback;
			this._initialPosition = position;
			this.curColour = color;
			this.holoComp = comp;
			this.colorLayer = layer;
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
			this._hexOut = (this._hexIn = Win_HologramColorPicker.RGBtoHex(this.tempColour));
		}

		public void NotifyRGBUpdated()
		{
			HSV.ToHSV(tempColour, out _H, out _S, out _V);
			_A = tempColour.a;
			CreateColourPickerBG();
			CreateHuePickerBG();
			CreateAlphaPickerBG();
			_huePosition = (1f - _H) / UnitsPerPixel;
			_position.x = _S / UnitsPerPixel;
			_position.y = (1f - _V) / UnitsPerPixel;
			_alphaPosition = (1f - _A) / UnitsPerPixel;
			CreatePreviewBG(ref _tempPreviewBG, tempColour);
			_hexOut = (_hexIn = RGBtoHex(tempColour));
		}

		public void SetColor()
		{
			curColour = tempColour;
			holoComp.hologramColors[colorLayer] = curColour;
			CreatePreviewBG(ref _previewBG, tempColour);
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
			if (Win_HologramColorPicker.first)
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
				this._activeControl = Win_HologramColorPicker.Controls.none;
			}
			if (Mouse.IsOver(rect))
			{
				if (Input.GetMouseButtonDown(0))
				{
					this._activeControl = Win_HologramColorPicker.Controls.colourPicker;
				}
				if (this._activeControl == Win_HologramColorPicker.Controls.colourPicker)
				{
					Vector2 pos = Event.current.mousePosition - new Vector2(rect.xMin, rect.yMin);
					this.PickerAction(pos);
				}
			}
			if (Mouse.IsOver(rect2))
			{
				if (Input.GetMouseButtonDown(0))
				{
					this._activeControl = Win_HologramColorPicker.Controls.huePicker;
				}
				if (Event.current.type == EventType.ScrollWheel)
				{
					this.H -= Event.current.delta.y * this.UnitsPerPixel;
					this._huePosition = Mathf.Clamp(this._huePosition + Event.current.delta.y, 0f, (float)this._pickerSize);
					Event.current.Use();
				}
				if (this._activeControl == Win_HologramColorPicker.Controls.huePicker)
				{
					float pos2 = Event.current.mousePosition.y - rect2.yMin;
					this.HueAction(pos2);
				}
			}
			if (Mouse.IsOver(rect3))
			{
				if (Input.GetMouseButtonDown(0))
				{
					this._activeControl = Win_HologramColorPicker.Controls.alphaPicker;
				}
				if (Event.current.type == EventType.ScrollWheel)
				{
					this.A -= Event.current.delta.y * this.UnitsPerPixel;
					this._alphaPosition = Mathf.Clamp(this._alphaPosition + Event.current.delta.y, 0f, (float)this._pickerSize);
					Event.current.Use();
				}
				if (this._activeControl == Win_HologramColorPicker.Controls.alphaPicker)
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
				if (Win_HologramColorPicker.TryGetColorFromHex(this._hexIn, out color))
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
}
