using System;
using System.Collections.Generic;
using GeneralUtil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyBakery;


public class ChocoMineGame : Minigame
{

    private enum Tool
    {
        None,
        Hammer,
        Pick
    }
    private Dictionary<Sprite, (int, int)> buriedItems;
    private (int, int, int)[] toolPower; 
    private List<List<Tile>> tiles;

    private Sprite layer1, layer2, layer3, layer0;
    private Sprite circle, square, star;
    private Sprite pick, hammer, stabilityFront, stabilityBack;
    private Vector2 mapOrigin;
    private int depthMined, chocoUncovered;
    private ProgressBar stabilityBar;
    private Boolean started;
    private Tool currentTool;
    private Button pickButton, hammerButton;
    //Base bg
    //Layer mineable choco on top of background
    //Grid of tiles, tiles have layers, sprite(could be enum), 

    public override void Start(Texture2D spriteSheet, Texture2D background)
    {
        pick = new Sprite(spriteSheet, new Rectangle(0, 256, 64, 64));
        hammer = new Sprite(spriteSheet, new Rectangle(64, 256, 64, 64));
        layer1 = new Sprite(spriteSheet, new Rectangle(128, 448, 32, 32));
        layer2 = new Sprite(spriteSheet, new Rectangle(160, 448, 32, 32));
        layer3 = new Sprite(spriteSheet, new Rectangle(128, 480, 32, 32));
        layer0 = new Sprite(spriteSheet, new Rectangle(160, 480, 32, 32));
        circle = new Sprite(spriteSheet, new Rectangle(320, 256, 64, 64));
        square = new Sprite(spriteSheet, new Rectangle(384, 256, 64, 64));
        star = new Sprite(spriteSheet, new Rectangle(448, 256, 64, 64));

        stabilityFront = new Sprite(spriteSheet, new Rectangle(512, 256, 128, 64));
        stabilityBack = new Sprite(spriteSheet, new Rectangle(192, 192, 128, 64));
        started = false;
        currentTool = Tool.None;
        toolPower = new (int, int, int)[1] { (0, 0, 1) };

        mapOrigin = new Vector2(gameXOrigin + 64, gameYOrigin + 72);
        tiles = new List<List<Tile>>();
        int width = 25;
        int height = 12;
        depthMined = 0;
        stabilityBar = new ProgressBar(stabilityFront, stabilityBack, 14, 0, new Vector2(gameXOrigin + 400, gameYOrigin + 2), false);

        pickButton = new UIButton("", pick, new Vector2(mapOrigin.X + 832, mapOrigin.Y + 64), () => { currentTool = Tool.Pick; toolPower = new (int, int, int)[5]{ (0, -1, 1), (-1, 0, 1), (0, 0, 2), (1, 0, 1), (0, 1, 1)};});
        hammerButton = new UIButton("", hammer, new Vector2(mapOrigin.X + 832, mapOrigin.Y + 128), () => { currentTool = Tool.Hammer; toolPower = new (int, int, int)[9] { (-1, -1, 1), (0, -1, 1), (1, -1, 1), (-1, 0, 1), (0, 0, 1), (1, 0, 1), (-1, 1, 1), (0, 1, 1), (1, 1, 1) };});

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

        buriedItems = new Dictionary<Sprite, (int, int)>();
        buriedItems[circle] = (2, 1);
    }

    public override void Draw(SpriteFont font, SpriteBatch spriteBatch)
    {
        for (int i = 0; i < tiles.Count; i++)//Base Tiles
        {
            for (int j = 0; j < tiles[0].Count; j++)
            {
                spriteBatch.Draw(layer0.Texture, new Vector2(mapOrigin.X + j * 32, mapOrigin.Y + i * 32), layer0.TextureMapLocation, Color.White);
            }
        }
        foreach (KeyValuePair<Sprite, (int, int)> item in buriedItems)
        {
            spriteBatch.Draw(item.Key.Texture, new Vector2(mapOrigin.X + item.Value.Item1 * 32, mapOrigin.Y + item.Value.Item2 * 32), item.Key.TextureMapLocation, Color.White);
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
                spriteBatch.Draw(sprite.Texture, tile.location, sprite.TextureMapLocation, Color.White);
            }
        }
        stabilityBar.Draw(spriteBatch);
        spriteBatch.DrawString(font, "Items Found: " + chocoUncovered, new Vector2(gameXOrigin + 10, gameYOrigin + 50), Color.White);

        pickButton.Draw(spriteBatch, font);
        hammerButton.Draw(spriteBatch, font);
        if (currentTool == Tool.Hammer)
        {
            spriteBatch.Draw(hammer.Texture, new Vector2(KMouse.MouseLocation().X - 32, KMouse.MouseLocation().Y - 32), hammer.TextureMapLocation, Color.White);
        }
        else if (currentTool == Tool.Pick)
        {
            spriteBatch.Draw(pick.Texture, new Vector2(KMouse.MouseLocation().X - 32, KMouse.MouseLocation().Y - 32), pick.TextureMapLocation, Color.White);
        }
    }

    public override void Update(GameTime gameTime)
    {
        pickButton.Update();
        hammerButton.Update();
        KMouse.CheckMouse();
        if (KMouse.CheckLeftPress() && started)
        {
            if (KMouse.MouseLocation().X > mapOrigin.X && KMouse.MouseLocation().X < mapOrigin.X + tiles[0].Count * 32 &&
            KMouse.MouseLocation().Y > mapOrigin.Y && KMouse.MouseLocation().Y < mapOrigin.Y + tiles.Count * 32)
            {
                int clickedTileX = (int)(KMouse.MouseLocation().X - mapOrigin.X) / 32;
                int clickedTileY = (int)(KMouse.MouseLocation().Y - mapOrigin.Y) / 32;


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
                            foreach (KeyValuePair<Sprite, (int, int)> item in buriedItems)
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
                if (depthMined > 14)
                {
                    if (GameManager.PlayerInfo.inventory.ContainsKey(GameManager.Items.BoxedChocolate))
                    {
                        GameManager.PlayerInfo.inventory[GameManager.Items.BoxedChocolate] += 1;
                    }
                    else
                    {
                        GameManager.PlayerInfo.inventory[GameManager.Items.BoxedChocolate] = 1;
                    }
                    MinigameManager.CurrentMinigameState = MinigameManager.MinigameState.Select;
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