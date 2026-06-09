using CoreLibrary.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;
    
    
public abstract class Component{

    public abstract void Draw(SpriteBatch spriteBatch, TextureAtlas spriteSheet);
    public abstract void Update(GameTime gameTime);
}
