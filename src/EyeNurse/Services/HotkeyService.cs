using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;

namespace EyeNurse.Services
{
    public class HotkeyService : IDisposable
    {
        private const int WM_HOTKEY = 0x0312;
        private IntPtr _windowHandle;
        private int _currentId = 9000; // Start with a safe non-zero ID
        private Dictionary<int, Action> _hotkeyActions = new Dictionary<int, Action>();
        private bool _isInitialized = false;
        private HwndSource? _hwndSource;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public void Initialize(IntPtr windowHandle)
        {
            if (_isInitialized) return;

            if (windowHandle == IntPtr.Zero)
            {
                // Create a dedicated message-only window for hotkeys
                // HWND_MESSAGE = -3 (HWND_MESSAGE is supported in Windows XP and later)
                var parameters = new HwndSourceParameters("EyeNurseHotkeySink")
                {
                    ParentWindow = (IntPtr)(-3), 
                    WindowStyle = 0,
                    Width = 0,
                    Height = 0
                };
                
                try 
                {
                    _hwndSource = new HwndSource(parameters);
                    _windowHandle = _hwndSource.Handle;
                    _hwndSource.AddHook(HwndHook);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to create HwndSource: {ex.Message}");
                    // Fallback to thread-level if window creation fails (unlikely)
                    _windowHandle = IntPtr.Zero; 
                    ComponentDispatcher.ThreadFilterMessage += ComponentDispatcher_ThreadFilterMessage;
                }
            }
            else
            {
                _windowHandle = windowHandle;
                // If using an external handle, we can't easily hook it unless we own it via HwndSource
                // But usually we pass IntPtr.Zero in this app structure. 
                // If we do pass a real handle, we assume the window processes messages appropriately or usage of ComponentDispatcher is needed.
                ComponentDispatcher.ThreadFilterMessage += ComponentDispatcher_ThreadFilterMessage;
            }
            
            _isInitialized = true;
        }

        public void Register(string hotkeyStr, Action action)
        {
            if (string.IsNullOrEmpty(hotkeyStr)) return;

            try
            {
                uint modifiers = 0;
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
                        if (p.Length == 1 && char.IsDigit(p[0]))
                        {
                            if (Enum.TryParse("D" + p, true, out Key key))
                            {
                                vk = (uint)KeyInterop.VirtualKeyFromKey(key);
                            }
                        }
                        else if (Enum.TryParse(p, true, out Key key))
                        {
                            vk = (uint)KeyInterop.VirtualKeyFromKey(key);
                        }
                    }
                }

                if (vk != 0)
                {
                    _currentId++;
                    bool result = RegisterHotKey(_windowHandle, _currentId, modifiers, vk);
                    System.Diagnostics.Debug.WriteLine($"RegisterHotKey '{hotkeyStr}' (ID={_currentId}, hWnd={_windowHandle}): {result}");
                    
                    if (result)
                    {
                        _hotkeyActions[_currentId] = action;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error registering hotkey {hotkeyStr}: {ex.Message}");
            }
        }

        public void UnregisterAll()
        {
            foreach (var id in _hotkeyActions.Keys)
            {
                UnregisterHotKey(_windowHandle, id);
            }
            _hotkeyActions.Clear();
            _currentId = 9000;
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = (int)wParam;
                if (_hotkeyActions.TryGetValue(id, out var action))
                {
                    action?.Invoke();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        private void ComponentDispatcher_ThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_HOTKEY)
            {
                int id = (int)msg.wParam;
                if (_hotkeyActions.TryGetValue(id, out var action))
                {
                    action?.Invoke();
                    handled = true;
                }
            }
        }

        public void Dispose()
        {
            UnregisterAll();
            if (_hwndSource != null)
            {
                _hwndSource.RemoveHook(HwndHook);
                _hwndSource.Dispose();
                _hwndSource = null;
            }
        }
    }
}
