using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeneralUtil;

public class Sprite{

    public Texture2D Texture { get; set; }//sprite file
    public Rectangle TextureMapLocation { get; set; }//location on sprite map

    public Sprite(Texture2D texture, Rectangle textureMapLocation){
        Texture = texture;
        TextureMapLocation = textureMapLocation;
    }
}