/*
 * QuickSharp Copyright (C) 2008-2011 Steve Walker.
 *
 * This file is part of QuickSharp.
 *
 * QuickSharp is free software: you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation, either version 3 of the License, or (at your option)
 * any later version.
 *
 * QuickSharp is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with QuickSharp. If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using QuickSharp.Core;
using ScintillaNet;

namespace QuickSharp.Editor
{
    /// <summary>
    /// Manages keystroke macros.
    /// </summary>
    public class MacroManager
    {
        #region Singleton

        private static MacroManager _singleton;

        /// <summary>
        /// Get a reference to the MacroManager singleton.
        /// </summary>
        /// <returns>A reference to the MacroManager.</returns>
        public static MacroManager GetInstance()
        {
            if (_singleton == null)
                _singleton = new MacroManager();

            return _singleton;
        }

        #endregion

        private bool _isRecording;
        private string _recordedText;
        private List<MacroEvent> _recordedEvents;
        private Scintilla _recordingEditor;
        private EventHandler<CharAddedEventArgs> _textRecorder;

        private MacroManager()
        {
            _isRecording = false;
            _recordedText = String.Empty;
            _recordedEvents = new List<MacroEvent>();
            _textRecorder = new EventHandler<CharAddedEventArgs>(CharAddedHandler);
        }

        /// <summary>
        /// True if keystrokes are being recorded.
        /// </summary>
        public bool IsRecording
        {
            get { return _isRecording; }
        }

        /// <summary>
        /// True if the macro buffer contains recorded keystrokes.
        /// </summary>
        public bool HaveMacro
        {
            get { return _recordedEvents.Count > 0; }
        }

        /// <summary>
        /// Start recording keystrokes.
        /// </summary>
        /// <param name="recordingEditor">The editor from which keystrokes should be recorded.</param>
        public void StartRecording(Scintilla recordingEditor)
        {
            _recordingEditor = recordingEditor;
            if (_recordingEditor == null) return;
            
            _isRecording = true;
            _recordedText = String.Empty;
            _recordedEvents.Clear();

            _recordingEditor.CharAdded += _textRecorder;
            _recordingEditor.NativeInterface.StartRecord();
        }

        /// <summary>
        /// Stop recording keystokes.
        /// </summary>
        public void StopRecording()
        {
            if (_recordingEditor == null) return;

            _recordingEditor.NativeInterface.StopRecord();
            _recordingEditor.CharAdded -= _textRecorder;
            _isRecording = false;
        }

        /// <summary>
        /// capture a recordable keystroke event in the macro buffer.
        /// </summary>
        /// <param name="e"></param>
        public void RecordEvent(MacroRecordEventArgs e)
        {
            MacroEvent macroEvent = new MacroEvent();
            macroEvent.Message = e.Notification.message;
            macroEvent.wParam = e.Notification.wParam;
            macroEvent.lParam = e.Notification.lParam;
            macroEvent.Text = _recordedText;

            _recordedEvents.Add(macroEvent);
            _recordedText = String.Empty;
        }

        /// <summary>
        /// Play back the macro in the buffer.
        /// </summary>
        /// <param name="playbackEditor">The editor to which keystrokes are to be played back.</param>
        public void PlayMacro(Scintilla playbackEditor)
        {
            if (playbackEditor == null ||
                _isRecording ||
                _recordedEvents.Count == 0) return;

            /*
             * Sending each recorded message back to Scintilla seems to work
             * for everything except character inserts. So we intercept these
             * and replace them with a manual text insert of the text collected
             * by the CharAdded event handler. Also, newlines seem to get lost
             * so we also do these manually adding a filter for leftover '\r'
             * characters. These will be added automatically (where needed) by the
             * manual newline operation.
             */

            foreach (MacroEvent e in _recordedEvents)
            {
                if (e.Message == 2025) // SCI_GOTOPOS
                {
                    playbackEditor.NativeInterface.NewLine();
                }
                else if (e.Message == 2170) // SCI_REPLACESEL
                {
                    if (e.Text != "\r")
                        playbackEditor.NativeInterface.AddText(e.Text.Length, e.Text);
                }
                else
                {
                    playbackEditor.NativeInterface.SendMessageDirect(
                        (uint)e.Message, e.wParam, e.lParam);
                }
            }
        }

        /// <summary>
        ///  Clear the contents of the macro buffer.
        /// </summary>
        public void ClearMacro()
        {
            _recordedEvents.Clear();
        }

        /// <summary>
        /// Save the current macro to a file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        public void SaveMacro(string path)
        {
            if (!HaveMacro) return;

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, _recordedEvents);
            }
        }

        /// <summary>
        /// Load the macro buffer from a file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        public void LoadMacro(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                _recordedEvents = (List<MacroEvent>)formatter.Deserialize(fs);
            }
        }

        private void CharAddedHandler(object sender, CharAddedEventArgs e)
        {
            _recordedText += e.Ch.ToString();
        }
    }
}

