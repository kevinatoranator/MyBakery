using System;
using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Graphics;
using CoreLibrary.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyBakery;
using MyBakery.Scenes;

public class OptionScene : Scene
{

    private SpriteFont _font;
    private List<UIElement> _UIElements;
    private Game1 _game;

    public OptionScene(Game1 game) : base()
    {
        _game = game;
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void LoadContent()
    {
        _font = Core.Content.Load<SpriteFont>("Font");

        TextureAtlas Atlas = TextureAtlas.FromFile(Core.Content, "atlas-definition.xml"); //Replace with Menu SpriteSheet
        Sprite button = Atlas.CreateSprite("Button");
        _UIElements = new List<UIElement>();
        Rectangle bounds = Core.GraphicsDevice.PresentationParameters.Bounds;
        List<UIButton> resolutions =
        [
            new UIButton(new Rectangle((int)(bounds.Width/2 - button.Width/2), bounds.Height/2, (int)button.Width, (int)button.Height),
             button.Region, "1280x720", _font, () => {Core.Graphics.PreferredBackBufferWidth = 1280; Core.Graphics.PreferredBackBufferHeight = 720; Core.Graphics.ApplyChanges(); Console.WriteLine("Small");}),
            new UIButton(new Rectangle((int)(bounds.Width/2 - button.Width/2), (int)(bounds.Height/2 + button.Height), (int)button.Width, (int)button.Height),
             button.Region, "1920x1080", _font, () => {Core.Graphics.PreferredBackBufferWidth = 1920; Core.Graphics.PreferredBackBufferHeight = 1080; Core.Graphics.ApplyChanges(); Console.WriteLine("Big");}),
        ];
        _UIElements.Add(new UIDropdown(new Rectangle((int)(bounds.Width/2 - button.Width/2), bounds.Height/2, (int)button.Width, (int)button.Height), button.Region, resolutions));
        _UIElements.Add(new UIButton(new Rectangle(0, 0, (int)button.Width, (int)button.Height), button.Region, "Return", _font, () => {Core.ChangeScene(new TitleScene(_game));}));
    }

    public override void Update(GameTime gameTime)
    {
        foreach(UIElement element in _UIElements)
        {
            element.Update(gameTime);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.Black);
        Core.SpriteBatch.Begin(samplerState : SamplerState.PointClamp);
        
        foreach(UIElement element in _UIElements)
        {
            element.Draw(gameTime);
        }

        Core.SpriteBatch.End();
    }
}
