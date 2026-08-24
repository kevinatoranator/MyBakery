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

namespace MyBakery;


public class ChocoMineGame : Scene
{

    private enum Tool
    {
        None,
        Hammer,
        Pick
    }
    private Dictionary<string, (int, int)> buriedItems;
    private (int, int, int)[] toolPower; 
    private List<List<Tile>> tiles;

    private Sprite layer1, layer2, layer3, layer0;
    private Sprite pick, hammer, stabilityFront, stabilityBack;
    private Vector2 mapOrigin;
    private int depthMined, chocoUncovered;
    private ProgressBar stabilityBar;
    private Boolean started;
    private Tool currentTool;
    private UIButton pickButton, hammerButton;
    private DayScene _mainScene;
    private SpriteFont _font;
    const int MAX_STABILITY = 20;
    //Base bg
    //Layer mineable choco on top of background
    //Grid of tiles, tiles have layers, sprite(could be enum), 

    public ChocoMineGame(DayScene main) : base()
    {
        _mainScene = main;
    }
    public override void Initialize()
    {
        
        started = false;
        currentTool = Tool.None;
        toolPower = new (int, int, int)[1] { (0, 0, 1) };

        mapOrigin = new Vector2(_mainScene.GameBounds.X + 64, _mainScene.GameBounds.Y + 72);
        tiles = new List<List<Tile>>();
        int width = 25;
        int height = 12;
        depthMined = 0;
        
        float[,] test = GenerateWhiteNoise(height, width);
        float[,] perlin = GeneratePerlinNoise(test, 4);
        for (int i = 0; i < height; i++)
        {
            List<Tile> row = new List<Tile>();
            for (int j = 0; j < width; j++)
            {
                row.Add(new Tile(new Vector2(mapOrigin.X + 32 * j, mapOrigin.Y + 32 * i), Math.Clamp((int)Math.Round(perlin[i, j] * 4), 0, 3)));
            }
            tiles.Add(row);
        }

        buriedItems = new Dictionary<string, (int, int)>();
        buriedItems["Circle"] = (2, 1);
        base.Initialize();
    }

    public override void LoadContent()
    {
        pick = _mainScene.Atlas.CreateSprite("Pick");
        hammer = _mainScene.Atlas.CreateSprite("Hammer");
        layer1 = _mainScene.Atlas.CreateSprite("Layer1");
        layer2 = _mainScene.Atlas.CreateSprite("Layer2");
        layer3 = _mainScene.Atlas.CreateSprite("Layer3");
        layer0 = _mainScene.Atlas.CreateSprite("Layer0");

        _font = Content.Load<SpriteFont>("font");

        stabilityFront = _mainScene.Atlas.CreateSprite("ProgressFront");
        stabilityBack = _mainScene.Atlas.CreateSprite("ProgressBack");

        stabilityBar = new ProgressBar(stabilityFront, stabilityBack, MAX_STABILITY, 0, new Vector2(_mainScene.GameBounds.X + 400, _mainScene.GameBounds.Y + 2), false);

        pickButton = new UIButton(new Rectangle((int)(mapOrigin.X + 832), (int)(mapOrigin.Y + 64), (int)pick.Width, (int)pick.Width),
            pick.Region, () => { currentTool = Tool.Pick; toolPower = new (int, int, int)[5]{ (0, -1, 1), (-1, 0, 1), (0, 0, 2), (1, 0, 1), (0, 1, 1)};});
        hammerButton = new UIButton(new Rectangle((int)(mapOrigin.X + 832), (int)(mapOrigin.Y + 128), (int)hammer.Width, (int)hammer.Height),
            hammer.Region, () => { currentTool = Tool.Hammer; toolPower = new (int, int, int)[9] { (-1, -1, 1), (0, -1, 1), (1, -1, 1), (-1, 0, 1), (0, 0, 1), (1, 0, 1), (-1, 1, 1), (0, 1, 1), (1, 1, 1) };});
    }

    public override void Draw(GameTime gameTime)
    {
        for (int i = 0; i < tiles.Count; i++)//Base Tiles
        {
            for (int j = 0; j < tiles[0].Count; j++)
            {
                layer0.Draw(Core.SpriteBatch, new Vector2(mapOrigin.X + j * 32, mapOrigin.Y + i * 32));
            }
        }
        foreach (KeyValuePair<string, (int, int)> item in buriedItems)
        {
            _mainScene.Atlas.CreateSprite(item.Key).Draw(Core.SpriteBatch, new Vector2(mapOrigin.X + item.Value.Item1 * 32, mapOrigin.Y + item.Value.Item2 * 32));
        }


        foreach (List<Tile> row in tiles)//Layers
        {
            foreach (Tile tile in row)
            {
                Sprite sprite;
                if (tile.depth == 3)
                {
                    sprite = layer3;
                }
                else if (tile.depth == 2)
                {
                    sprite = layer2;
                }
                else if (tile.depth == 1)
                {
                    sprite = layer1;
                }
                else
                {
                    continue;
                }
                sprite.Draw(Core.SpriteBatch, tile.location);
            }
        }
        stabilityBar.Draw(Core.SpriteBatch);
        Core.SpriteBatch.DrawString(_font, "Items Found: " + chocoUncovered, new Vector2(_mainScene.GameBounds.X + 10, _mainScene.GameBounds.Y + 50), Color.White);

        pickButton.Draw(gameTime);
        hammerButton.Draw(gameTime);
        if (currentTool == Tool.Hammer)
        {
            hammer.Draw(Core.SpriteBatch, new Vector2(Core.Input.Mouse.MouseLocation().X - 32, Core.Input.Mouse.MouseLocation().Y - 32));
        }
        else if (currentTool == Tool.Pick)
        {
            pick.Draw(Core.SpriteBatch, new Vector2(Core.Input.Mouse.MouseLocation().X - 32, Core.Input.Mouse.MouseLocation().Y - 32));
        }
    }

    public override void Update(GameTime gameTime)
    {
        pickButton.Update(gameTime);
        hammerButton.Update(gameTime);
        if (Core.Input.Mouse.CheckLeftPress() && started)
        {
            if (Core.Input.Mouse.MouseLocation().X > mapOrigin.X && Core.Input.Mouse.MouseLocation().X < mapOrigin.X + tiles[0].Count * 32 &&
            Core.Input.Mouse.MouseLocation().Y > mapOrigin.Y && Core.Input.Mouse.MouseLocation().Y < mapOrigin.Y + tiles.Count * 32)
            {
                int clickedTileX = (int)(Core.Input.Mouse.MouseLocation().X - mapOrigin.X) / 32;
                int clickedTileY = (int)(Core.Input.Mouse.MouseLocation().Y - mapOrigin.Y) / 32;


                for (int i = 0; i < toolPower.Length; i++)
                {
                    int currentX = clickedTileX + toolPower[i].Item1;
                    int currentY = clickedTileY + toolPower[i].Item2;
                    if (currentY < tiles.Count && currentX < tiles[0].Count &&
                    currentY >= 0 && currentX >= 0)
                    {
                        Tile currentTile = tiles[currentY][currentX];
                        if (currentTile.depth > 0)
                        {
                            currentTile.depth -= toolPower[i].Item3;
                            if (currentTile.depth < 0)
                                currentTile.depth = 0;
                            foreach (KeyValuePair<string, (int, int)> item in buriedItems)
                                {
                                    if (currentX >= item.Value.Item1 && currentX <= item.Value.Item1 + 1 && currentY >= item.Value.Item2 && currentY <= item.Value.Item2 + 1)
                                    {
                                        if (tiles[item.Value.Item2][item.Value.Item1].depth == 0 && tiles[item.Value.Item2 + 1][item.Value.Item1].depth == 0
                                        && tiles[item.Value.Item2][item.Value.Item1 + 1].depth == 0 && tiles[item.Value.Item2 + 1][item.Value.Item1 + 1].depth == 0)
                                        {
                                            chocoUncovered++;
                                        }
                                    }
                                }
                        }
                    }
                }
                depthMined++;
                if (depthMined > MAX_STABILITY)
                {
                    if (GameManager.PlayerInfo.inventory.ContainsKey("BoxedChocolate"))
                    {
                        GameManager.PlayerInfo.inventory["BoxedChocolate"] += chocoUncovered;
                    }
                    else
                    {
                        GameManager.PlayerInfo.inventory["BoxedChocolate"] = chocoUncovered;
                    }
                    _mainScene.ChangeLowerTab(new SelectionScene(_mainScene));
                }
            }        
        }
        stabilityBar.Update(depthMined);
        started = true;
    }

    private class Tile
    {
        public Vector2 location;//is this needed or can just use location in array
        public int depth;

        public Tile(Vector2 loc, int depth)
        {
            location = loc;
            this.depth = depth;
        }

    }

    //may move this to external libary

    //perlin noise

    float[,] GenerateWhiteNoise(int height, int width)
    {
        Random random = new Random();
        float[,] noise = new float[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                noise[i, j] = (float)random.NextDouble();
            }
        }
        return noise;
    }

    float[,] GenerateSmoothNoise(float[,] baseNoise, int octave)
    {
        int height = baseNoise.GetLength(0);
        int width = baseNoise.GetLength(1);

        float[,] smoothNoise = new float[height, width];
        int samplePeriod = 1 << octave;
        float sampleFrequency = 1.0f / samplePeriod;

        for (int i = 0; i < height; i++)
        {
            int sample_i0 = (i / samplePeriod) * samplePeriod;
            int sample_i1 = (sample_i0 + samplePeriod) % height;
            float vertical_blend = (i - sample_i0) * sampleFrequency;

            for (int j = 0; j < width; j++)
            {
                int sample_j0 = (j / samplePeriod) * samplePeriod;
                int sample_j1 = (sample_j0 + samplePeriod) % width;
                float horizontal_blend = (j - sample_j0) * sampleFrequency;

                float top = Interpolate(baseNoise[sample_i0, sample_j0], baseNoise[sample_i1, sample_j0], horizontal_blend);

                float bottom = Interpolate(baseNoise[sample_i0, sample_j1], baseNoise[sample_i1, sample_j1], horizontal_blend);

                smoothNoise[i,j] = Interpolate(top, bottom, vertical_blend);
            }
        }

        return smoothNoise;
    }

    float[,] GeneratePerlinNoise(float[,] baseNoise, int octaveCount)
    {
        int height = baseNoise.GetLength(0);
        int width = baseNoise.GetLength(1);

        float[][,] smoothNoise = new float[octaveCount][,];
        float persistance = 0.5f;

        for (int i = 0; i < octaveCount; i++)
        {
            smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
        }

        float[,] perlinNoise = new float[height, width];
        float amplitude = 1.0f;
        float totalAmplitude = 0.0f;

        for (int octave = octaveCount - 1; octave > 0; octave--)
        {
            amplitude *= persistance;
            totalAmplitude += amplitude;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    perlinNoise[i, j] += smoothNoise[octave][i, j] * amplitude;
                }
            }
        }

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                perlinNoise[i, j] /= totalAmplitude;
            }
        }

        return perlinNoise;
    }

    float Interpolate(float x0, float x1, float alpha) {
        return x0 * (1 - alpha) + alpha * x1;
    }
}