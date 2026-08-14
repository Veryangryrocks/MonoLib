using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoLib.Graphics;
using MonoLib.Graphics.Passes;

namespace MonoLib.IO;

public static class InputManager
{
    public enum MouseButtons { LEFT, MIDDLE, RIGHT }
    private static KeyboardState _keyboardState;
    private static MouseState _mouseState;
    private static Dictionary<string, InputWrapper> _inputWrapperDict = new();
    public static Point MousePosition { get; private set; }

    public static void Add(string key, InputWrapper inputWrapper)
    {
        if (inputWrapper == null)
            throw new ArgumentNullException(nameof(key));
        if (_inputWrapperDict.ContainsKey(key))
            throw new ArgumentException(nameof(key));

        _inputWrapperDict.Add(key, inputWrapper);
    }
    public static bool Get(string key)
    {
        if (!_inputWrapperDict.ContainsKey(key))
            throw new KeyNotFoundException(nameof(key));
        
        return _inputWrapperDict[key].IsActive();
    }

    public static void Update()
    {
        _keyboardState = Keyboard.GetState();
        _mouseState = Mouse.GetState();

        MousePosition = _mouseState.Position;

        foreach (KeyValuePair<string, InputWrapper> kvp in _inputWrapperDict)
            kvp.Value.Update();
    }

    private static Point WindowToScreen(Point windowPosition, string rasterPassKey)
    {
        RasterPass rasterPass = GraphicsManager.RasterPassDict[rasterPassKey];
        Rectangle renderDestination = GraphicsManager.GetRenderDestination(rasterPass.Width, rasterPass.Height);
        return WindowManager.WindowToScreen(windowPosition, renderDestination, rasterPass.Width, rasterPass.Height);
    }
    public static Point GetScreenMousePosition(string rasterPassKey)
    {
        return WindowToScreen(MousePosition, rasterPassKey);
    }
    public static Point GetRPMousePosition(string rasterPassKey)
    {
        RasterPass rasterPass = GraphicsManager.RasterPassDict[rasterPassKey];
        Point screenPosition = GetScreenMousePosition(rasterPassKey);
        return rasterPass.ScreenToRP(screenPosition);
    }

    public static Hold FromSingleKeyHold(Keys key, bool isInversed = false) => new Hold(new OrInputBind([new Key(key, isInversed)]));
    public static Hold FromSingleMouseButtonHold(MouseButtons mouseButton, bool isInversed = false) => new Hold(new OrInputBind([new MouseButton(mouseButton, isInversed)]));
    public static Tap FromSingleKeyTap(Keys key, bool isInversed = false) => new Tap(new OrInputBind([new Key(key, isInversed)]));
    public static Tap FromSingleMouseButtonTap(MouseButtons mouseButton, bool isInversed = false) => new Tap(new OrInputBind([new MouseButton(mouseButton, isInversed)])); 

    public abstract class Input
    {
        protected bool _isInversed;
        public Input(bool isInversed = false)
        {
            _isInversed = isInversed;
        }
        public abstract bool IsActive();
    }
    public sealed class Key : Input
    {
        private Keys _key;
        public Key(Keys key, bool isInversed = false) : base(isInversed)
        {
            _key = key;
        }
        public override bool IsActive() => _keyboardState.IsKeyDown(_key) ^ _isInversed;
    }
    public sealed class MouseButton : Input
    {
        private MouseButtons _mouseButton;
        public MouseButton(MouseButtons mouseButton, bool isInversed = false) : base(isInversed)
        {
            _mouseButton = mouseButton;
        }
        public override bool IsActive()
        {
            switch (_mouseButton)
            {
                case MouseButtons.LEFT:
                    return _mouseState.LeftButton == ButtonState.Pressed ^ _isInversed;
                case MouseButtons.MIDDLE:
                    return _mouseState.MiddleButton == ButtonState.Pressed ^ _isInversed;
                case MouseButtons.RIGHT:
                    return _mouseState.RightButton == ButtonState.Pressed ^ _isInversed;
                default:
                    return false;
            }
        }
    }
    
    public abstract class InputBind
    {
        protected Input[] _inputsArray;
        public int Length => _inputsArray.Length;
        public InputBind(Input[] inputsArray)
        {
            _inputsArray = inputsArray;
        }
        public abstract bool IsActive();
    }
    public sealed class AndInputBind : InputBind
    {
        public AndInputBind(Input[] inputsArray) : base(inputsArray) {}
        public override bool IsActive()
        {
            foreach (Input input in _inputsArray)
            {
                if (!input.IsActive())
                {
                    return false;
                }
            }
            return true;
        }
    }
    public sealed class OrInputBind : InputBind
    {
        public OrInputBind(Input[] inputsArray) : base(inputsArray) {}
        public override bool IsActive()
        {
            foreach (Input input in _inputsArray)
            {
                if (input.IsActive())
                {
                    return true;
                }
            }
            return false;
        }
    }

    public abstract class InputWrapper
    {
        protected InputBind _inputBind;
        public InputWrapper(InputBind inputBind)
        {
            _inputBind = inputBind;
        }
        public abstract void Update();
        public abstract bool IsActive();
    }
    public sealed class Hold : InputWrapper
    {
        private int _framesHeld;
        public Hold(InputBind inputBind) : base(inputBind)
        {
            _framesHeld = 0;
        }
        public override void Update()
        {
            if (_inputBind.IsActive())
            {
                _framesHeld++;
            }
            else
            {
                _framesHeld = 0;
            }
        }
        public override bool IsActive() => _framesHeld >= 1;
    }
    public sealed class Tap : InputWrapper
    {
        private int _framesHeld;
        public Tap(InputBind inputBind) : base(inputBind)
        {
            _framesHeld = 0;
        }
        public override void Update()
        {
            if (_inputBind.IsActive())
            {
                _framesHeld++;
            }
            else
            {
                _framesHeld = 0;
            }
        }
        public override bool IsActive() => _framesHeld == 1;
    }
}