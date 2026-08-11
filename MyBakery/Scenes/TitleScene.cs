using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary;
using CoreLibrary.Scenes;

namespace MyBakery.Scenes;

public class TitleScene : Scene
{
    private const string TITLE_TEXT = "My Bakery";
    private const string START_PROMPT = "Press Enter to Start";

    private SpriteFont _font;
    private Vector2 _titleLocation;
    private Vector2 _titleOrigin;

    private Vector2 _startPromptLocation;
    private Vector2 _startPromptOrigin;

    public override void Initialize()
    {
        base.Initialize();

        Vector2 size = _font.MeasureString(TITLE_TEXT);
        _titleLocation = new Vector2(640, 100);
        _titleOrigin = size * 0.5f;

        size = _font.MeasureString(START_PROMPT);
        _startPromptLocation = new Vector2(640, 620);
        _startPromptOrigin = size * 0.5f;
    }

    public override void LoadContent()
    {
        _font = Core.Content.Load<SpriteFont>("Font");
    }

    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Keyboard.CheckKeyPress(Keys.Enter))
        {
            Core.ChangeScene(new DayScene());
        } 
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.Black);

        Core.SpriteBatch.Begin(samplerState : SamplerState.PointClamp);
        Color dropShadowColor = Color.Black *0.5f;

        Core.SpriteBatch.DrawString(_font, TITLE_TEXT, _titleLocation + new Vector2(10, 10), dropShadowColor, 0.0f, _titleOrigin, 1.0f, SpriteEffects.None, 1.0f);
        Core.SpriteBatch.DrawString(_font, TITLE_TEXT, _titleLocation, Color.White , 0.0f, _titleOrigin, 1.0f, SpriteEffects.None, 1.0f);

        Core.SpriteBatch.DrawString(_font, START_PROMPT, _startPromptLocation, Color.White , 0.0f, _startPromptOrigin, 1.0f, SpriteEffects.None, 0.0f);

        Core.SpriteBatch.End();

    }

}