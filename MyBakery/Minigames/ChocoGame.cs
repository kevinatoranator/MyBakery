using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary.Graphics;
using CoreLibrary.Scenes;
using MyBakery.Scenes;
using CoreLibrary;

namespace MyBakery;

public class ChocoGame : Scene
{
    const int spriteSize = 64;

    //Chocogame
    private Sprite chocoSprite, playerSprite, emberSprite, progressFront, progressBack, bgSprite;
    private Texture2D bg;
    private List<FallingObject> fallingObjects;
    private Player player;
    private int collectedChocolate, gameTimeLeft, quota;
    private double timePassed;
    private DayScene _mainScene;
    private SpriteFont _font;

    private ProgressBar timerBar, quotaBar;

    public ChocoGame(DayScene main) : base()
    {
        _mainScene = main;
    }

    public override void LoadContent()
    {
        _font = Content.Load<SpriteFont>("font");
        bg = Content.Load<Texture2D>("chocofall_bg");
        bgSprite = new Sprite(new TextureRegion(bg, 0, 0, bg.Width, bg.Height));
        bgSprite.Scale *= 2;
        playerSprite = _mainScene.Atlas.CreateSprite("ToastDog");
        chocoSprite = _mainScene.Atlas.CreateSprite("ChocoChip");
        emberSprite = _mainScene.Atlas.CreateSprite("Ember");
        progressFront = _mainScene.Atlas.CreateSprite("ProgressFront");
        progressBack = _mainScene.Atlas.CreateSprite("ProgressBack");

        timerBar = new ProgressBar(progressFront, progressBack, 60, 60, new Vector2(_mainScene.GameBounds.Left + 10, _mainScene.GameBounds.Top + 30), false);
        quotaBar = new ProgressBar(progressFront, progressBack, quota, 0, new Vector2(_mainScene.GameBounds.Left + 10, _mainScene.GameBounds.Top + 130), false);
        
    }

    public override void Initialize()
    {
        
        

        fallingObjects = new List<FallingObject>();

        player = new Player() { location = new Vector2(_mainScene.GameBounds.Left * 2, _mainScene.GameBounds.Bottom - 100) };
        collectedChocolate = 0;
        timePassed = 0;
        quota = 20; //Make dynamic based on... average?

        base.Initialize();

    }

    public override void Draw(GameTime gameTime)
    {
                //Chocogame

        bgSprite.Draw(Core.SpriteBatch, new Vector2(_mainScene.GameBounds.X, _mainScene.GameBounds.Y));
        timerBar.Draw(Core.SpriteBatch);
        Core.SpriteBatch.DrawString(_font, "Chocolate Quota: ", new Vector2(_mainScene.GameBounds.X + 10, _mainScene.GameBounds.Y + 100), Color.Black);
        quotaBar.Draw(Core.SpriteBatch);

        foreach(FallingObject o in fallingObjects){
            if(o.type == "chocolate")
                chocoSprite.Draw(Core.SpriteBatch, o.location);
            else if(o.type == "ember")
                emberSprite.Draw(Core.SpriteBatch, o.location);
        }

        playerSprite.Draw(Core.SpriteBatch, player.location);

    }

    public override void Update(GameTime gameTime)
    {
        //Chocogame

        if(Keyboard.GetState().IsKeyDown(Keys.Left) && player.location.X > _mainScene.GameBounds.X)
            player.location = new Vector2(player.location.X - 7, player.location.Y);
        if(Keyboard.GetState().IsKeyDown(Keys.Right) && player.location.X < _mainScene.GameBounds.Right-spriteSize)
            player.location = new Vector2(player.location.X + 7, player.location.Y);

        timePassed += gameTime.ElapsedGameTime.TotalSeconds;
        gameTimeLeft = (int)(60 - timePassed);
        timerBar.Update(gameTimeLeft);
        quotaBar.Update(collectedChocolate);
        if(gameTimeLeft < 0){
            fallingObjects.Clear();
            gameTimeLeft = 0;
            if(collectedChocolate > quota)
                collectedChocolate += collectedChocolate/2;
            if (GameManager.PlayerInfo.inventory.ContainsKey("ChocoChip"))
            {
                GameManager.PlayerInfo.inventory["ChocoChip"] += collectedChocolate;
            }
            else
            {
                GameManager.PlayerInfo.inventory["ChocoChip"] = collectedChocolate;
            }
            
            _mainScene.ChangeLowerTab(new SelectionScene(_mainScene));
        }

        if(gameTimeLeft > 0){
            Random rand = new();
            int spawnChance = rand.Next(100)+1;
            int xlocation = rand.Next(_mainScene.GameBounds.Width-spriteSize) + _mainScene.GameBounds.X;
            int fallSpeed = rand.Next(5)+2;
            if(spawnChance > 98){
                fallingObjects.Add(new FallingObject(){location = new Vector2(xlocation, _mainScene.GameBounds.Y), fallSpeed = fallSpeed, type="chocolate"});
            }else if(spawnChance == 1){
                fallingObjects.Add(new FallingObject(){location = new Vector2(xlocation, _mainScene.GameBounds.Y), fallSpeed = fallSpeed, type="ember"});
            }
        }

        List<FallingObject> qclist = fallingObjects.ToList();
        foreach(FallingObject o in qclist){
            if(o.location.Y >= player.location.Y + 5)
                o.fallSpeed = 0;
                //fallingObjects.Remove(o);
            else if(Collide(new Vector2(o.location.X, o.location.Y+spriteSize*3/4), player.location, spriteSize, spriteSize/4, spriteSize, 5)){
                fallingObjects.Remove(o);
                if(o.type == "chocolate")
                    collectedChocolate++;
                else if(o.type == "ember")
                    collectedChocolate = 0;
            }else
                o.location = new Vector2(o.location.X, o.location.Y+o.fallSpeed);
        }
        if(fallingObjects.Count>15)
            fallingObjects.Remove(fallingObjects.First());
    }

    public Boolean Collide(Vector2 vec1, Vector2 vec2, int xsize1, int ysize1, int xsize2, int ysize2){
        Rectangle obj1 = new Rectangle((int)vec1.X, (int)vec1.Y, xsize1, ysize1);
        Rectangle obj2 = new Rectangle((int)vec2.X, (int)vec2.Y, xsize2, ysize2);

        return obj1.Intersects(obj2);
    }

    //Chocogame
    private class Player{
        Vector2 _location;

        public Vector2 location{
            get => _location;
            set => _location = value;
        }
    }

    private class FallingObject
    {
        Vector2 _location;
        int _fallSpeed;
        String _type;

        public Vector2 location
        {
            get => _location;
            set => _location = value;
        }
        public int fallSpeed
        {
            get => _fallSpeed;
            set => _fallSpeed = value;
        }
        public String type
        {
            get => _type;
            set => _type = value;
        }

        public Rectangle hitBox
        {
            get => new Rectangle((int)location.X, (int)location.Y, 64, 64);
        }
    }
}