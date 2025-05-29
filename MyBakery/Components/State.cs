

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;

public abstract class State{

    protected ContentManager _content;
    protected GraphicsDevice _graphicsDevice;
    protected Game1 _game;

    protected int _stateX, _stateY;

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    public abstract void Update(GameTime gameTime);

    public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
    {
        _game = game;
        _graphicsDevice = graphicsDevice;
        _content = content;
    }
}