


using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyBakery;

public class Shop{

    public List<ShopObject> placedShopObjects; //placed shopObjects
    UIButton register;//Start button
    Sprite shopSprite, displaySprite;
    SpriteFont font;
    //or
    //List<Sprite> shoptTiles;
    private static Dropdown displayMenu;
    public static List<ShopObject> placeableShopObjects; //List of placedable
    private BakeryDisplay openedDisplay;
    public enum ShopObjectTypes
    {
        Counter,
        Door,
        Display
    }

    public Shop(Sprite sprite, UIButton register, Sprite display, SpriteFont font)
    {
        this.register = register;
        shopSprite = sprite;
        displaySprite = display;
        this.font = font;
        placeableShopObjects = new List<ShopObject>() { new BakeryDisplay(displaySprite, Vector2.Zero, font, new Rectangle(0, 64, 64, 64)) { Quantity = 1, Name = "new display" } };
    }

    public void Update(GameTime gameTime){
        if(!BakeryManager.IsOpen)
            register.Update(gameTime);

        if(GameManager.MouseClicked){
            Boolean occupied = false;
            Vector2 mouseLocation = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Y);
            openedDisplay = null;
            Boolean mouseOnDisplay =false;
            foreach(ShopObject shopObject in placedShopObjects){
                if (shopObject.Type == ShopObjectTypes.Display)
                {
                    BakeryDisplay display = shopObject as BakeryDisplay;
                    if (mouseLocation.X > shopObject.Hitbox.Left && mouseLocation.X < shopObject.Hitbox.Right && mouseLocation.Y > shopObject.Hitbox.Top && mouseLocation.Y < shopObject.Hitbox.Bottom)
                        mouseOnDisplay = true;
                    if(display.menu != null)
                        openedDisplay = display;
                }
                
            }
            if(displayMenu == null && mouseLocation.X > GameManager.gameWidth/3 && mouseLocation.Y < GameManager.gameHeight/2 && openedDisplay == null && !mouseOnDisplay){
                displayMenu = new Dropdown(placeableShopObjects, font, mouseLocation);
            }
            else if(displayMenu != null){
                displayMenu.Update(gameTime);
                if(displayMenu.selectedDisplay != null){
                    foreach(ShopObject shopObject in placedShopObjects.ToArray()){//should probably check when clicking before making menu but this works if displays are different sizes
                        if (shopObject.Type == ShopObjectTypes.Display)
                        {
                            BakeryDisplay display = shopObject as BakeryDisplay;
                            if (display.Hitbox.Intersects(new Rectangle(Mouse.GetState().Position.X, Mouse.GetState().Y, display.Sprite.TextureMapLocation.Width, display.Sprite.TextureMapLocation.Height)))
                            {
                                occupied = true;
                            }
                       }
                    }
                    if(!occupied && placeableShopObjects[0].Quantity > 0){
                        PlaceShopObject(displayMenu.Location, placeableShopObjects[0].Type);
                        placeableShopObjects[0].Quantity -= 1;//hard coded for now
                    }
                    
                }
                if(displayMenu.Clicked){
                    displayMenu = null;
                }
            }
        }
        foreach(BakeryDisplay display in placedShopObjects){
            display.Update(gameTime);           
        }
    }
    public void Draw(SpriteBatch spriteBatch, SpriteFont font){
        
        spriteBatch.Draw(shopSprite.Texture, new Rectangle(GameManager.gameWidth/3, 0, GameManager.gameWidth*2/3, GameManager.gameHeight/2), Color.White);
        if(!BakeryManager.IsOpen)
            register.Draw(spriteBatch, font);
        foreach(BakeryDisplay display in placedShopObjects){
            display.Draw(spriteBatch);
        }
        if(displayMenu != null){
            displayMenu.Draw(spriteBatch);
        }
    }

    public void PlaceShopObject(Vector2 location, ShopObjectTypes type){
        if (type == ShopObjectTypes.Display)
        {
            placedShopObjects.Add(new BakeryDisplay(displaySprite, location, font, new Rectangle((int)location.X, (int)(location.Y + 64), 64, 64)));
        }
    }

}