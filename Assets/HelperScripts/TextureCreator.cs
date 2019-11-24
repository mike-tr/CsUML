using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class TextureCreator {

    public static Texture2D ColorToTexture(Color color, int size)
    {
        int s2 = size * size;
        Color[] colors = new Color[s2];
        for (int i = 0; i < s2; i++)
        {
            colors[i] = color;
        }
        return TextureFromColorMap(colors, FilterMode.Point, size, size);
    }

    public static Texture2D TextureFromColorMap(Color[] colorMap, FilterMode mode, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = mode;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static void SaveTexture(Texture2D texture, string location, string name)
    {
        byte[] bytes = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);

        File.WriteAllBytes(Application.dataPath + location + "/" + name + ".png", bytes);
    }

    public static Sprite SpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    public static Texture2D textureFromSprite(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }
}
