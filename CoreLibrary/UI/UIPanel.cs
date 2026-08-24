using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Graphics;
using Microsoft.Xna.Framework;

public class UIPanel : UIElement
{
    private bool _visible; //Texture is drawn
    private List<UIElement> _contents; //Bounds for panel contents are done locally not globally so they stay attached to panel
    private Tileset _tileset;
    private List<List<Sprite>> _panel;

    public UIPanel(Rectangle bounds, TextureRegion textureRegion) : base(bounds, textureRegion) 
    {
        _contents = new List<UIElement>();
        _visible = true;
        _tileset = new Tileset(textureRegion, textureRegion.Width/3, textureRegion.Height/3);
        _panel = new List<List<Sprite>>();
        CreatePanel();
    }

    public override void Draw(GameTime gameTime)
    {
        if (_visible)
        {
            for(int i=0; i < _panel.Count; i++)
            {
                for(int j=0; j <_panel[0].Count; j++)
                {
                    _panel[i][j].Draw(Core.SpriteBatch, new Vector2(Location.X + j * _tileset.TileWidth, Location.Y + i * _tileset.TileHeight));
                }
            }
        }
        foreach(UIElement element in _contents)
        {
            element.Draw(gameTime);
        }
    }

    public override void Update(GameTime gameTime)
    {
        foreach(UIElement element in _contents)
        {
            element.Update(gameTime);
        }
    }

    private void CreatePanel()
    {
        int panelWidth = (int)(Bounds.Width /_tileset.TileWidth);
        int panelHeight = (int)(Bounds.Height /_tileset.TileHeight);

        for(int i = 0; i < panelHeight; i++)
        {
            List<Sprite> row = new List<Sprite>();
            for(int j = 0; j < panelWidth; j++)
            {
                if(i == 0 && j == 0)
                    row.Add(new Sprite(_tileset.GetTile(0)));
                else if(i == 0 && j == panelWidth-1)
                    row.Add(new Sprite(_tileset.GetTile(2)));
                else if(i == 0)
                    row.Add(new Sprite(_tileset.GetTile(1)));
                else if(i == panelHeight-1 && j == 0)
                    row.Add(new Sprite(_tileset.GetTile(6)));
                else if(i == panelHeight-1 && j == panelWidth-1)
                    row.Add(new Sprite(_tileset.GetTile(8)));
                else if(i == panelHeight-1)
                    row.Add(new Sprite(_tileset.GetTile(7)));
                else if(j == 0)
                    row.Add(new Sprite(_tileset.GetTile(3)));
                else if(j == panelWidth-1)
                    row.Add(new Sprite(_tileset.GetTile(5)));
                else
                    row.Add(new Sprite(_tileset.GetTile(4)));
            }
            _panel.Add(row);
        }
    }

    public void AddContents(List<UIElement> contents)
    {
        _contents = contents;
        foreach(UIElement element in _contents)
        {
            element.Location = new Vector2(Location.X + element.Location.X, Location.Y + element.Location.Y);
        }
    }
}

//FUTURE TODO
/*
Be able to resize (could be a window class instead)
shrink by using partial sizes of textures

add function that sorts/compresses contents based on size of panel rather than coded presets
*/