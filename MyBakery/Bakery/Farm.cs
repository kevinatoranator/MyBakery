


using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CoreLibrary.Graphics;
using CoreLibrary.Input;
using CoreLibrary;
using CoreLibrary.Scenes;
using MyBakery.Scenes;
using System.Linq;

namespace MyBakery;

public class Farm : Scene
{
    private List<List<Plot>> plots;
    private SpriteFont _font;
    private Sprite _dirt, _button;
    private Vector2 farmOrigin;
    private DayScene _dayScene;
    public bool isActive;
    private List<UIElement> _UIElements;
    private bool _menuOpen;
    private TextureAtlas _farmAtlas;
    private List<Crop> _availableCrops;

    public Farm(DayScene dayScene) : base()
    {
        _dayScene = dayScene;
        
    }
    public override void Initialize()
    {
        farmOrigin = new Vector2(_dayScene.BakeryBounds.Left + 64, 64);
        isActive = false;
        _UIElements = new List<UIElement>();
        _menuOpen = false;

        plots = new List<List<Plot>>() { new List<Plot>() { new Plot(farmOrigin), new Plot(new Vector2(_dayScene.BakeryBounds.Left + 128, 64)), new Plot(new Vector2(_dayScene.BakeryBounds.Left + 192, 64))},
        new List<Plot>() { new Plot(new Vector2(_dayScene.BakeryBounds.Left + 64, 128)), new Plot(new Vector2(_dayScene.BakeryBounds.Left + 128, 128)), new Plot(new Vector2(_dayScene.BakeryBounds.Left + 192, 128))},
        new List<Plot>() { new Plot(new Vector2(_dayScene.BakeryBounds.Left + 64, 192)), new Plot(new Vector2(_dayScene.BakeryBounds.Left + 128, 192)), new Plot(new Vector2(_dayScene.BakeryBounds.Left + 192, 192))} };
        base.Initialize();
    }

    public override void LoadContent()
    {
        _farmAtlas = TextureAtlas.FromFile(Core.Content, "atlas-definition-farm.xml");
        _dirt = _farmAtlas.CreateSprite("Dirt");
        _button = _dayScene.Atlas.CreateSprite("Button");
        _font = Content.Load<SpriteFont>("font");

        _availableCrops = new List<Crop>()
        {
            new Crop(1, 4, [10, 10, 10, 10, 10], "Wheat", new AnimatedSprite(_farmAtlas.GetAnimation("Wheat"))),
            new Crop(1, 3, [10, 10, 10, 10], "SugarCane", new AnimatedSprite(_farmAtlas.GetAnimation("SugarCane"))),
            new Crop(1, 5, [10, 10, 10, 10, 10, 10], "Oat", new AnimatedSprite(_farmAtlas.GetAnimation("Oat")))
        }; //FINISH THIS
    }


    public override void Update(GameTime gameTime)
    {
        foreach(UIElement element in _UIElements.ToList())
        {
            element.Update(gameTime);
        }
        if (Core.Input.Mouse.CheckLeftPress() && isActive)
        {
            Vector2 mouseLocation = new Vector2(Core.Input.Mouse.MouseLocation().X, Core.Input.Mouse.MouseLocation().Y);
            if (_menuOpen)
            {
                _UIElements.RemoveAt(_UIElements.Count-1);//temp test since the last element in theory could be anything, find away to find the actually dropdown
                _menuOpen = false;
            }
            else
            {
                if (mouseLocation.X > farmOrigin.X && mouseLocation.X < farmOrigin.X + plots[0].Count * 64 &&
                mouseLocation.Y > farmOrigin.Y && mouseLocation.Y < farmOrigin.Y + plots.Count * 64)
                {
                    int clickedTileX = (int)(mouseLocation.X - farmOrigin.X) / 64;
                    int clickedTileY = (int)(mouseLocation.Y - farmOrigin.Y) / 64;
                    Plot selectedPlot = plots[clickedTileY][clickedTileX];

                    if(selectedPlot.crop != null)//planted
                    {
                        if(selectedPlot.crop.CurrentStage < selectedPlot.crop.stages)
                        {
                            List<UIElement> contents = new List<UIElement>();
                            string cropInfo = "Crop: " + selectedPlot.crop.Type + "\nStage: " + selectedPlot.crop.CurrentStage + "/" + selectedPlot.crop.stages;
                            Vector2 infoSize = new Vector2(_font.MeasureString(cropInfo).X, _font.MeasureString(cropInfo).Y);
                            _UIElements.Add(new UIPanel(new Rectangle((int)mouseLocation.X, (int)mouseLocation.Y, (int)(infoSize.X + infoSize.X%64), (int)(infoSize.Y + infoSize.Y%64)), _dayScene.Atlas.CreateSprite("Panel").Region));
                            contents.Add(new UILabel(new Rectangle(0,0, _UIElements[_UIElements.Count-1].Bounds.Width, _UIElements[_UIElements.Count-1].Bounds.Height),
                            _UIElements[_UIElements.Count-1].TextureRegion, cropInfo, _font, false));
                            _menuOpen = true;
                            ((UIPanel)_UIElements[_UIElements.Count-1]).AddContents(contents);
                        }
                        else//Fully Grown
                        {
                            if (GameManager.PlayerInfo.inventory.ContainsKey(selectedPlot.crop.Type))
                            {
                                GameManager.PlayerInfo.inventory[selectedPlot.crop.Type] += 4;
                            }
                            else
                            {
                                GameManager.PlayerInfo.inventory[selectedPlot.crop.Type] = 4;
                            }
                            selectedPlot.crop = null;
                        }
                    }
                    else //empty
                    {
                        List<UIButton> cropList = new List<UIButton>();
                        int buttonCount = 0;
                        foreach (Crop crop in _availableCrops)
                        {
                            cropList.Add(new UIButton(new Rectangle((int)mouseLocation.X, (int)(mouseLocation.Y + buttonCount * _button.Height + 1), (int)_button.Width, (int)_button.Height),
                            _button.Region, crop.Type, _font,
                            () => {
                                selectedPlot.crop = new Crop(crop.cost, crop.stages, crop.stageTimes, crop.Type, new AnimatedSprite(_farmAtlas.GetAnimation(crop.Type)));
                                }));
                            buttonCount++;
                        }
                        _UIElements.Add(new UIDropdown(new Rectangle((int)mouseLocation.X, (int)mouseLocation.Y, cropList[0].TextureRegion.Width, cropList[0].TextureRegion.Height), _button.Region, cropList){Opened = true});
                        _menuOpen = true;
                    }   
                }
            }
            
        }
        foreach (List<Plot> row in plots)
        {
            foreach (Plot plot in row)
            {
                if (plot.crop != null && _dayScene.DayStarted)
                {
                    plot.crop.Update(gameTime);
                }
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        foreach (List<Plot> row in plots)
        {
            foreach (Plot plot in row)
            {
                _dirt.Draw(Core.SpriteBatch, plot.location);
                if (plot.crop != null)
                {
                    plot.crop.Draw(gameTime, plot.location);
                }
            }
        }

        foreach(UIElement element in _UIElements.ToList())
        {
            element.Draw(gameTime);
        }
    }

    private class Plot
    {
        public Crop crop { get; set; }
        public Vector2 location;
        //Maybe on later interations there are bonuses, fertility, etc.

        public Plot(Vector2 location)
        {
            this.location = location;
        }
    }

    private class Crop
    {
        public int stages;
        public int cost;
        public int[] stageTimes;
        private AnimatedSprite _animatedSprite; // should have a value that correlates to inventory items
        public double GrowthTime { get; set; }
        public int CurrentStage { get; set; }
        public string Type;

        public Crop(int cost, int stages, int[] stageTimes, string type, AnimatedSprite animatedSprite)
        {
            this.cost = cost;
            this.stages = stages;
            this.stageTimes = stageTimes;
            _animatedSprite = animatedSprite;
            GrowthTime = 0;
            CurrentStage = 0;
            Type = type;
        }

        public void Update(GameTime gameTime)
        {
            GrowthTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (GrowthTime > stageTimes[CurrentStage])
            {
                CurrentStage += 1;
                GrowthTime = 0;
            }
            _animatedSprite.Update(gameTime);
        }
        public void Draw(GameTime gameTime, Vector2 location)
        {
            _animatedSprite.Draw(Core.SpriteBatch, location);
        }
    }
}