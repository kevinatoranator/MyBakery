


using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;

public class Shop{

    public List<BakeryDisplay> displays;
    UIButton register;//Start button
    Sprite shopSprite, displaySprite;
    SpriteFont font;
    //or
    //List<Sprite> shoptTiles;
    private static Dropdown displayMenu;
    public static List<BakeryDisplay> ownedDisplays;
    private BakeryDisplay openedDisplay;

    public Shop(Sprite sprite, UIButton register, Sprite display, SpriteFont font){
        this.register = register;
        shopSprite = sprite;
        displaySprite = display;
        this.font = font;
        ownedDisplays = new List<BakeryDisplay>(){new BakeryDisplay(displaySprite, Vector2.Zero, font){Quantity = 1, Name = "new display"}};
    }

    public void Update(GameTime gameTime){
        if(!BakeryManager.IsOpen)
            register.Update(gameTime);

        if(GameManager.MouseClicked){
            Boolean occupied = false;
            Vector2 mouseLocation = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Y);
            openedDisplay = null;
            Boolean mouseOnDisplay =false;
            foreach(BakeryDisplay display in displays){
                if(mouseLocation.X > display.HitBox.Left && mouseLocation.X < display.HitBox.Right && mouseLocation.Y > display.HitBox.Top && mouseLocation.Y < display.HitBox.Bottom)
                    mouseOnDisplay = true;
                if(display.menu != null)
                    openedDisplay = display;
            }
            if(displayMenu == null && mouseLocation.X > GameManager.gameWidth/3 && mouseLocation.Y < GameManager.gameHeight/2 && openedDisplay == null && !mouseOnDisplay){
                displayMenu = new Dropdown(ownedDisplays, font, mouseLocation);
            }
            else if(displayMenu != null){
                displayMenu.Update(gameTime);
                if(displayMenu.selectedDisplay != null){
                    foreach(BakeryDisplay display in displays.ToArray()){//should probably check when clicking before making menu but this works if displays are different sizes
                        if(display.HitBox.Intersects(new Rectangle(Mouse.GetState().Position.X, Mouse.GetState().Y, display.Sprite.TextureMapLocation.Width, display.Sprite.TextureMapLocation.Height))){
                            occupied = true;
                        }
                    }
                    if(!occupied && ownedDisplays[0].Quantity > 0){
                        PlaceDisplay(displayMenu.Location);
                        ownedDisplays[0].Quantity -= 1;//hard coded for now
                    }
                    
                }
                if(displayMenu.Clicked){
                    displayMenu = null;
                }
            }
        }
        foreach(BakeryDisplay display in displays){
            display.Update(gameTime);           
        }
    }
    public void Draw(SpriteBatch spriteBatch, SpriteFont font){
        
        spriteBatch.Draw(shopSprite.Texture, new Rectangle(GameManager.gameWidth/3, 0, GameManager.gameWidth*2/3, GameManager.gameHeight/2), Color.White);
        if(!BakeryManager.IsOpen)
            register.Draw(spriteBatch, font);
        foreach(BakeryDisplay display in displays){
            display.Draw(spriteBatch);
        }
        if(displayMenu != null){
            displayMenu.Draw(spriteBatch);
        }
    }

    public void PlaceDisplay(Vector2 location){
        displays.Add(new BakeryDisplay(displaySprite, location, font));
    }

}