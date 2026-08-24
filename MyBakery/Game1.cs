using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CoreLibrary;
using CoreLibrary.Graphics;
using MyBakery.Scenes;
using System;

namespace MyBakery;

public class Game1 : Core
{
    public Version Version = new Version(0, 1, 0);

    public Game1() : base("My Bakery", 1920, 1080, false){}

    protected override void Initialize()
    {
        base.Initialize();
        ChangeScene(new TitleScene(this));
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


