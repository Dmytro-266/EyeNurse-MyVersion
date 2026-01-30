using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace EyeNurse.Services
{
    public class HotkeyService
    {
        private const int WM_HOTKEY = 0x0312;
        private IntPtr _windowHandle;
        private int _currentId;
        private Dictionary<int, Action> _hotkeyActions = new Dictionary<int, Action>();

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public void Initialize(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
            ComponentDispatcher.ThreadFilterMessage += ComponentDispatcher_ThreadFilterMessage;
        }

        public void Register(string hotkeyStr, Action action)
        {
            if (string.IsNullOrEmpty(hotkeyStr)) return;

            try
            {
                uint modifiers = 0;
                // Simple parsing
                var parts = hotkeyStr.Split('+');
                uint vk = 0;

                foreach (var part in parts)
                {
                    var p = part.Trim();
                    if (string.Equals(p, "Ctrl", StringComparison.OrdinalIgnoreCase) || string.Equals(p, "Control", StringComparison.OrdinalIgnoreCase)) modifiers |= 0x0002;
                    else if (string.Equals(p, "Alt", StringComparison.OrdinalIgnoreCase)) modifiers |= 0x0001;
                    else if (string.Equals(p, "Shift", StringComparison.OrdinalIgnoreCase)) modifiers |= 0x0004;
                    else if (string.Equals(p, "Win", StringComparison.OrdinalIgnoreCase)) modifiers |= 0x0008;
                    else
                    {
                        // Parse key
                        if (Enum.TryParse(p, true, out Key key))
                        {
                            vk = (uint)KeyInterop.VirtualKeyFromKey(key);
                        }
                    }
                }

                if (vk != 0)
                {
                    _currentId++;
                    if (RegisterHotKey(_windowHandle, _currentId, modifiers, vk))
                    {
                        _hotkeyActions[_currentId] = action;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle parsing errors silently or log
                System.Diagnostics.Debug.WriteLine($"Failed to register hotkey {hotkeyStr}: {ex.Message}");
            }
        }

        public void UnregisterAll()
        {
            foreach (var id in _hotkeyActions.Keys)
            {
                UnregisterHotKey(_windowHandle, id);
            }
            _hotkeyActions.Clear();
        }

        private void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_HOTKEY)
            {
                int id = (int)msg.wParam;
                if (_hotkeyActions.ContainsKey(id))
                {
                    _hotkeyActions[id]?.Invoke();
                    handled = true;
                }
            }
        }
    }
}
