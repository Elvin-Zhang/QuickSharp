using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class AutoComplete : ScintillaHelperBase
	{
		internal AutoComplete(Scintilla scintilla) : base(scintilla)
		{
			
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeAutoHide() ||
				ShouldSerializeCancelAtStart() ||
				ShouldSerializeDropRestOfWord() ||
				ShouldSerializeFillUpCharacters() ||
				ShouldSerializeImageSeparator() ||
				ShouldSerializeIsCaseSensitive() ||
				ShouldSerializeListSeparator() ||
				ShouldSerializeMaxHeight() ||
				ShouldSerializeMaxWidth() ||
				ShouldSerializeSingleLineAccept() ||
				ShouldSerializeStopCharacters();
		}

		private List<string> _list = new List<string>();
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<string> List
		{
			get
			{
				return _list;
			}
			set
            {
				if (value == null)
					value = new List<string>();

            	_list = value;
            }
		}

		public string ListString
		{
			get
			{
				return getListString(_list);
			}
			set
            {
				_list = new List<string>(value.Split(ListSeparator));
            }
		}



		#region Show
		
		public void Show(int lengthEntered, string list)
		{
			if (list == string.Empty)
				_list = new List<string>();
			else
				_list = new List<string>(list.Split(ListSeparator));
			Show(lengthEntered, list, true);
		}

		internal void Show(int lengthEntered, string list, bool dontSplit)
		{
			//	We may have the auto-detect of lengthEntered. In which case
			//	look for the last word character as the start
			int le = lengthEntered;
			if (le < 0)
				le = getLengthEntered();

			NativeScintilla.AutoCShow(le, list);

			//	Now it may have been that the auto-detect lengthEntered
			//	caused to AutoCShow call to fail becuase no words matched
			//	the letters we autodetected. In this case just show the
			//	list with a 0 lengthEntered to make sure it will show
			if (!IsActive && lengthEntered < 0)
				NativeScintilla.AutoCShow(0, list);
		}

		public void Show()
		{
			Show(-1, getListString(_list), false);
		}

		public void Show(string list)
		{
			Show(-1, list);
		}

		private int getLengthEntered()
		{
			if (!_automaticLengthEntered)
				return 0;

			int pos = NativeScintilla.GetCurrentPos();
			return pos - NativeScintilla.WordStartPosition(pos, true);
		}

		public void Show(int lengthEntered, IEnumerable<string> list)
		{
			_list = new List<string>(list);
			Show(-1);
		}

		public void Show(IEnumerable<string> list)
		{
			_list = new List<string>(list);
			Show(-1);
		}

		public void Show(int lengthEntered)
		{
			Show(lengthEntered, getListString(_list), false);
        }


		#endregion

		#region ShowUserList
		public void ShowUserList(int listType, string list)
		{
			NativeScintilla.UserListShow(listType, list);
		}

		public void ShowUserList(int listType, IEnumerable<string> list)
		{
			Show(listType, getListString(list), true);
		}
		#endregion

		public void Cancel()
		{
			NativeScintilla.AutoCCancel();
		}

		[Browsable(false)]
		public bool IsActive
		{
			get
			{
				return NativeScintilla.AutoCActive();
			}
		}

		[Browsable(false)]
		public int LastStartPosition
		{
			get
			{
				return NativeScintilla.AutoCPosStart();
			}
		}

		public void Accept()
		{
			NativeScintilla.AutoCComplete();
		}

		#region StopCharacters
		private string _stopCharacters = string.Empty;
		public string StopCharacters
		{
			get
			{
				return _stopCharacters;
			}
			set
			{
				_stopCharacters = value;
				NativeScintilla.AutoCStops(value);
			}
		}

		private bool ShouldSerializeStopCharacters()
		{
			return _stopCharacters != string.Empty;
		}

		private void ResetStopCharacters()
		{
			_stopCharacters = string.Empty;
		} 
		#endregion

		#region ListSeparator
		[TypeConverter(typeof(ScintillaNet.WhiteSpaceStringConverter))]
		public char ListSeparator
		{
			get
			{
				return NativeScintilla.AutoCGetSeparator();
			}
			set
			{
				NativeScintilla.AutoCSetSeparator(value);
			}
		}

		private bool ShouldSerializeListSeparator()
		{
			return ListSeparator != ' ';
		}

		private void ResetListSeparator()
		{
			ListSeparator = ' ';
		} 
		#endregion

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string SelectedText
		{
			get
			{
				return _list[NativeScintilla.AutoCGetCurrent()];
			}
			set
            {
            	NativeScintilla.AutoCSelect(value);
            }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex
		{
			get
			{
				return NativeScintilla.AutoCGetCurrent();
			}
			set
			{
				SelectedText = _list[value];
			}
		}

		#region CancelAtStart
		public bool CancelAtStart
		{
			get
			{
				return NativeScintilla.AutoCGetCancelAtStart();
			}
			set
			{
				NativeScintilla.AutoCSetCancelAtStart(value);
			}
		}

		private bool ShouldSerializeCancelAtStart()
		{
			return !CancelAtStart;
		}

		private void ResetCancelAtStart()
		{
			CancelAtStart = true;
		} 
		#endregion

		#region FillUpCharacters
		private string _fillUpCharacters = string.Empty;
		public string FillUpCharacters
		{
			get
			{
				return _fillUpCharacters;
			}
			set
			{
				_fillUpCharacters = value;
				NativeScintilla.AutoCSetFillUps(value);
			}
		}

		private bool ShouldSerializeFillUpCharacters()
		{
			return _fillUpCharacters != string.Empty;
		}

		private void ResetFillUpCharacters()
		{
			_fillUpCharacters = string.Empty;
		}

		#endregion

		#region SingleLineAccept
		public bool SingleLineAccept
		{
			get
			{
				return NativeScintilla.AutoCGetChooseSingle();
			}
			set
			{
				NativeScintilla.AutoCSetChooseSingle(value);
			}
		}

		private bool ShouldSerializeSingleLineAccept()
		{
			return SingleLineAccept;
		}

		private void ResetSingleLineAccept()
		{
			SingleLineAccept = false;
		} 
		#endregion

		#region IsCaseSensitive
		public bool IsCaseSensitive
		{
			get
			{
				return !NativeScintilla.AutoCGetIgnoreCase();
			}
			set
			{
				NativeScintilla.AutoCSetIgnoreCase(!value);
			}
		}

		private bool ShouldSerializeIsCaseSensitive()
		{
			return !IsCaseSensitive;
		}

		private void ResetIsCaseSensitive()
		{
			IsCaseSensitive = true;
		} 
		#endregion

		#region AutoHide
		public bool AutoHide
		{
			get
			{
				return NativeScintilla.AutoCGetAutoHide();
			}
			set
			{
				NativeScintilla.AutoCSetAutoHide(value);
			}
		}

		private bool ShouldSerializeAutoHide()
		{
			return !AutoHide;
		}

		private void ResetAutoHide()
		{
			AutoHide = true;
		} 
		#endregion

		#region DropRestOfWord
		public bool DropRestOfWord
		{
			get
			{
				return NativeScintilla.AutoCGetDropRestOfWord();
			}
			set
			{
				NativeScintilla.AutoCSetDropRestOfWord(value);
			}
		}

		private bool ShouldSerializeDropRestOfWord()
		{
			return DropRestOfWord;
		}

		private void ResetDropRestOfWord()
		{
			DropRestOfWord = false;
		} 
		#endregion

		#region RegisterImage
		public void RegisterImage(int type, string xpmImage)
		{
			NativeScintilla.RegisterImage(type, xpmImage);
		}

		private void RegisterImage(int type, Bitmap image, Color transparentColor)
		{
			NativeScintilla.RegisterImage(type, XpmConverter.ConvertToXPM(image, Utilities.ColorToHtml(transparentColor)));
		}

		private void RegisterImage(int type, Bitmap image)
		{
			NativeScintilla.RegisterImage(type, XpmConverter.ConvertToXPM(image));
		}

		#endregion

		#region RegisterImages
		public void RegisterImages(IList<string> xpmImages)
		{
			for (int i = 0; i < xpmImages.Count; i++)
				NativeScintilla.RegisterImage(i, xpmImages[i]);
		}

		public void RegisterImages(IList<Bitmap> xpmImages)
		{
			for (int i = 0; i < xpmImages.Count; i++)
				RegisterImage(i, xpmImages[i]);
		}

		public void RegisterImages(IList<Bitmap> xpmImages, Color transparentColor)
		{
			for (int i = 0; i < xpmImages.Count; i++)
				RegisterImage(i, xpmImages[i], transparentColor);
		}

		public void RegisterImages(ImageList xpmImages)
		{
			RegisterImages(XpmConverter.ConvertToXPM(xpmImages));
		}

		public void RegisterImages(ImageList xpmImages, Color transparentColor)
		{
			RegisterImages(XpmConverter.ConvertToXPM(xpmImages, Utilities.ColorToHtml(transparentColor)));
		} 
		#endregion


		public void ClearRegisteredImages()
		{
			NativeScintilla.ClearRegisteredImages();
		}

		#region ImageSeparator
		public char ImageSeparator
		{
			get
			{
				return NativeScintilla.AutoCGetTypeSeparator();
			}
			set
			{
				NativeScintilla.AutoCSetTypeSeparator(value);
			}
		}

		private bool ShouldSerializeImageSeparator()
		{
			return ImageSeparator != '?';
		}

		private void ResetImageSeparator()
		{
			ImageSeparator = '?';
		} 
		#endregion

		#region MaxHeight
		public int MaxHeight
		{
			get
			{
				return NativeScintilla.AutoCGetMaxHeight();
			}
			set
			{
				NativeScintilla.AutoCSetMaxHeight(value);
			}
		}

		private bool ShouldSerializeMaxHeight()
		{
			return MaxHeight != 5;
		}

		private void ResetMaxHeight()
		{
			MaxHeight = 5;
		} 
		#endregion

		#region MaxWidth
		public int MaxWidth
		{
			get
			{
				return NativeScintilla.AutoCGetMaxWidth();
			}
			set
			{
				NativeScintilla.AutoCSetMaxWidth(value);
			}
		}

		private bool ShouldSerializeMaxWidth()
		{
			return MaxWidth != 0;
		}

		private void ResetMaxWidth()
		{
			MaxWidth = 0;
		} 
		#endregion

		#region AutomaticLengthEntered
		private bool _automaticLengthEntered = true;
		public bool AutomaticLengthEntered
		{
			get
			{
				return _automaticLengthEntered;
			}
			set
			{
				_automaticLengthEntered = value;
			}
		}

		private bool ShouldSerializeAutomaticLengthEntered()
		{
			return !AutomaticLengthEntered;
		}

		private void ResetAutomaticLengthEntered()
		{
			AutomaticLengthEntered = true;
		} 
		#endregion

		private string getListString(IEnumerable<string> list)
		{
			StringBuilder listString = new StringBuilder();
			foreach (string s in list)
			{
				listString.Append(s).Append(" ");
			}
			if (listString.Length > 1)
				listString.Remove(listString.Length - 1, 1);

			return listString.ToString().Trim();
		}
	}
}


