using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoLib;
using MonoLib.Graphics;
using MonoLib.IO;

namespace MonoLib;

public static class Manager
{
    public static Action OnInitialize;
    public static Action OnLoad;
    public static Action OnUpdate;
    public static Action OnDraw;
    public static void Initialize(int nativeWidth, int nativeHeight, GameWindow gameWindow)
    {
        WindowManager.Initialize(nativeWidth, nativeHeight, gameWindow);
        OnInitialize.Invoke();
    }
    public static void Load(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager contentManager, string mgcbPath, string jsonCacheRootDirectory, string renderGraphRelativePath)
    {
        WindowManager.Load(graphicsDevice, graphicsDeviceManager);
        ContentCache.Load(contentManager, mgcbPath);
        JsonCache.Load(jsonCacheRootDirectory);
        RenderTargetCache.Load(graphicsDevice);
        OnLoad.Invoke();
        GraphicsManager.Load(graphicsDevice, JsonCache.Get<List<Pass>>(renderGraphRelativePath));
    }
    public static void Update()
    {
        InputManager.Update();
        OnUpdate.Invoke();
    }
    public static void Draw()
    {
        OnDraw.Invoke();
        GraphicsManager.Render();
    }
}