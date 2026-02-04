using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class TextureArray : Texture2D
{
    public int ArraySize { get; private set; }

    public TextureArray(GraphicsDevice graphicsDevice, int width, int height, int arraySize)
        : base(
            graphicsDevice,
            width,
            height,
            false,
            SurfaceFormat.Color,
            SurfaceType.Texture,
            false,
            arraySize
        )
    {
        ArraySize = arraySize;
    }

    public void Add(int index, Texture2D texture)
    {
        if (index < 0 || index >= ArraySize)
            throw new ArgumentOutOfRangeException(
                nameof(index),
                index,
                "Texture array index out of range."
            );
        if (texture is null)
            throw new ArgumentNullException(nameof(texture));
        if (texture.Width != Width || texture.Height != Height)
            throw new InvalidOperationException(
                $"Texture size {texture.Width}x{texture.Height} does not match array size {Width}x{Height}."
            );
        if (texture.Format != SurfaceFormat.Color || Format != SurfaceFormat.Color)
            throw new InvalidOperationException(
                $"Texture format mismatch: texture={texture.Format}, array={Format}. Expected SurfaceFormat.Color."
            );

        if (texture.LevelCount != LevelCount)
        {
            Debug.WriteLine(
                $"[TextureArray] LevelCount mismatch: texture={texture.LevelCount}, array={LevelCount}. Using min."
            );
        }

        int levels = Math.Min(texture.LevelCount, LevelCount);
        for (int i = 0; i < levels; i++)
        {
            float divisor = 1.0f / (1 << i);
            var mipWidth = (int)(texture.Width * divisor);
            var mipHeight = (int)(texture.Height * divisor);
            var pixelData = new Color[mipWidth * mipHeight];

            texture.GetData<Color>(
                i,
                0,
                new Rectangle(0, 0, mipWidth, mipHeight),
                pixelData,
                0,
                pixelData.Length
            );

            this.SetData<Color>(
                i,
                index,
                new Rectangle(0, 0, mipWidth, mipHeight),
                pixelData,
                0,
                pixelData.Length
            );
        }
    }

    // public static TextureArray LoadFromContentFolder(GraphicsDevice graphicsDevice, int widthPerTex, int heightPerTex, string path)
    // {
    //     var paths = Directory.GetFiles(Environment.CurrentDirectory + @"\Content\" + path);

    //     TextureArray pTexArray = new TextureArray(graphicsDevice, widthPerTex, heightPerTex, paths.Length);

    //     int index = 0;

    //     foreach (var file in paths)
    //         pTexArray.Add(index++, Content.Load<Texture2D>(path + @"\" + Path.GetFileNameWithoutExtension(file)));

    //     return pTexArray;
    // }
}
