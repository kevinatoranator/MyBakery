using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary;
using CoreLibrary.Scenes;
using System.Collections.Generic;
using CoreLibrary.Graphics;

namespace MyBakery.Scenes;

public class TitleScene : Scene
{
    private const string TITLE_TEXT = "My Bakery";
    private SpriteFont _font;
    private Vector2 _titleLocation;
    private Vector2 _titleOrigin;
    private List<UIElement> _UIElements;
    private Game1 _game;

    public TitleScene(Game1 game) : base()
    {
        _game = game;
    }

    public override void Initialize()
    {
        base.Initialize();

        Vector2 size = _font.MeasureString(TITLE_TEXT);
        _titleLocation = new Vector2(640, 100);
        _titleOrigin = size * 0.5f;
    }

    public override void LoadContent()
    {
        _font = Core.Content.Load<SpriteFont>("Font");

        TextureAtlas Atlas = TextureAtlas.FromFile(Core.Content, "atlas-definition.xml"); //Replace with Menu SpriteSheet
        Sprite button = Atlas.CreateSprite("Button");
        Rectangle bounds = Core.GraphicsDevice.PresentationParameters.Bounds;
        _UIElements =
        [
            new UILabel(new Rectangle(bounds.Width/3, bounds.Height/3, bounds.Width/3, (int)button.Height), button.Region, "My Bakery", _font){TextColor = Color.White},
            new UIButton(new Rectangle((int)(bounds.Width/2 - button.Width/2), bounds.Height/2, (int)button.Width, (int)button.Height), button.Region, "Start Game", _font, () => {Core.ChangeScene(new DayScene());}),
            new UIButton(new Rectangle((int)(bounds.Width/2 - button.Width/2), (int)(bounds.Height/2 + button.Height), (int)button.Width, (int)button.Height), button.Region, "Options", _font, () => {Core.ChangeScene(new OptionScene(_game));}),
            new UIButton(new Rectangle((int)(bounds.Width/2 - button.Width/2), (int)(bounds.Height/2 + button.Height * 2),(int)button.Width, (int)button.Height),
             button.Region, "Quit", _font, () => { _game.Exit();}),
        ];

         //TextureRegion testpanel = Atlas.CreateSprite("Panel").Region;
         //_UIElements.Add(new UIPanel(new Rectangle(64, 64, 128, 640), testpanel));
    }

    public override void Update(GameTime gameTime)
    {
        //if (Core.Input.Keyboard.CheckKeyPress(Keys.Enter))
        //{
        //    Core.ChangeScene(new DayScene());
        //} 

        foreach(UIElement element in _UIElements)
        {
            element.Update(gameTime);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.Black);

        Core.SpriteBatch.Begin(samplerState : SamplerState.PointClamp);
        Color dropShadowColor = Color.Black *0.5f;

        //Core.SpriteBatch.DrawString(_font, TITLE_TEXT, _titleLocation + new Vector2(10, 10), dropShadowColor, 0.0f, _titleOrigin, 1.0f, SpriteEffects.None, 1.0f);
        //Core.SpriteBatch.DrawString(_font, TITLE_TEXT, _titleLocation, Color.White , 0.0f, _titleOrigin, 1.0f, SpriteEffects.None, 1.0f);
        Core.SpriteBatch.DrawString(_font, "V " + _game.Version.ToString(), new Vector2(Core.Graphics.PreferredBackBufferWidth-64, Core.Graphics.PreferredBackBufferHeight - 48), Color.White , 0.0f, _titleOrigin, 1.0f, SpriteEffects.None, 1.0f);

        foreach(UIElement element in _UIElements)
        {
            element.Draw(gameTime);
        }

        Core.SpriteBatch.End();

    }

}