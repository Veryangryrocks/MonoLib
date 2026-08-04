using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoLib;

public static class WindowManager
{
    public static int NativeWidth { get; private set; }
    public static int NativeHeight { get; private set; }

    private static GraphicsDevice _graphicsDevice;
    private static GraphicsDeviceManager _graphicsDeviceManager;

    private static bool _isFullscreened = false;
    private static int _previousWindowWidth;
    private static int _previousWindowHeight;

    public static void Initialize(int nativeWidth, int nativeHeight, GameWindow gameWindow)
    {
        NativeWidth = nativeWidth;
        NativeHeight = nativeHeight;

        gameWindow.AllowUserResizing = true;
        gameWindow.ClientSizeChanged += OnClientSizeChanged;
    }
    public static void Load(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager)
    {
        _graphicsDevice = graphicsDevice;
        _graphicsDeviceManager = graphicsDeviceManager;

        graphicsDeviceManager.PreferredBackBufferWidth = NativeWidth;
        graphicsDeviceManager.PreferredBackBufferHeight = NativeHeight;

        graphicsDeviceManager.ApplyChanges();
    }

    public static void OnClientSizeChanged(object sender, EventArgs e) {}
    public static void Fullscreen()
    {
        _isFullscreened = true;
        _previousWindowWidth = _graphicsDevice.Viewport.Width;
        _previousWindowHeight = _graphicsDevice.Viewport.Height;

        _graphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphicsDeviceManager.IsFullScreen = true;
        _graphicsDeviceManager.ApplyChanges();
    }
    public static void Unfullscreen()
    {
        _isFullscreened = false;

        _graphicsDeviceManager.PreferredBackBufferWidth = _previousWindowWidth;
        _graphicsDeviceManager.PreferredBackBufferHeight = _previousWindowHeight;
        _graphicsDeviceManager.IsFullScreen = false;
        _graphicsDeviceManager.ApplyChanges();
    }
    public static void ToggleFullscreen()
    {
        if (_isFullscreened) Unfullscreen(); else Fullscreen();
    }
    public static Point WindowToScreen(Point windowPosition, Rectangle renderDestination, int nativeWidth, int nativeHeight)
    {
        float x = (windowPosition.X - renderDestination.X) * ((float)nativeWidth / renderDestination.Width);
        float y = (windowPosition.Y - renderDestination.Y) * ((float)nativeHeight / renderDestination.Height);

        return new Vector2(x, y).ToPoint();
    }
}