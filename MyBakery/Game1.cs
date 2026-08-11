using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary;
using CoreLibrary.Graphics;
using MyBakery.Scenes;

namespace MyBakery;

public class Game1 : Core
{
    public TextureAtlas atlas;

    public Game1() : base("My Bakery", 1920, 1080, false){}

    protected override void Initialize()
    {
        base.Initialize();
        ChangeScene(new TitleScene());
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)){
            Exit();
        }   
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}


