//using System;
using UnityEngine;

public class TexGen
{
	private TexGen ()
	{
	}
	
	static public void SetSpriteFromTexture(Renderer renderer, float size, Texture2D texture)
	{
		var sprite = Sprite.Create(
			texture,
			new Rect(0, 0, texture.width, texture.height),
			new Vector2(0.5f, 0.5f),
			texture.width / size);

		((SpriteRenderer)renderer).sprite = sprite;
	}

	// Generate a texture by calling a function and passing (x,y) coordinates
	static public Texture2D MakeTexture(int size, System.Func<int, int, Color> colorFunc)
	{
		var texture = new Texture2D(size, size);
		texture.filterMode = FilterMode.Point;
		
		int y = 0;
		while (y < texture.height) {
			int x = 0;
			while (x < texture.width) {
				texture.SetPixel(x, y, colorFunc(x, y));
				++x;
			}
			++y;
		}
		texture.Apply();
		return texture;
	}

	// Generate a texture by calling a function and passing normalised (x,y) coordinates on the unit square
	static public Texture2D MakeTextureUnit(int size, System.Func<float, float, Color> colorFunc)
	{
		var texture = new Texture2D(size, size);
		texture.filterMode = FilterMode.Point;
		
		int y = 0;
		while (y < texture.height) {
			int x = 0;
			while (x < texture.width) {
				texture.SetPixel(x, y, colorFunc((float)x / texture.width, (float)y / texture.height));
				++x;
			}
			++y;
		}
		texture.Apply();
		return texture;
	}

	// sierpenski triangle
	static public Texture2D MakeTexture_Sierpinski(int size)
	{
		return MakeTexture(size, (x, y) => ((x & y) != 0 ? Color.red : Color.blue));
	}

	static public Texture2D MakeTexture_Random(int size)
	{
		return MakeTexture(size, (x, y) => new Color(Random.value, Random.value, Random.value));
	}

	static public Texture2D MakeTexture_RandomMirroredSprite(int size, float opaquePixelChance)
	{
		var texture = new Texture2D(size, size);
		texture.filterMode = FilterMode.Point;

		var baseColor = new Color(Random.value, Random.value, Random.value);

		Color color;
		for(int y = 0; y < texture.height; y++)
		{
			for(int x = 0; x < texture.width / 2; x++)
			{
				// TODO: make opaque pixels more likely towards centre of sprite, to get a better-connected shape

				if(Random.value < opaquePixelChance)
					color = new Color(baseColor.r, baseColor.g, baseColor.b, 1);
				else
					color = Color.clear;
				texture.SetPixel(x, y, color);
				texture.SetPixel(texture.width - 1 - x, y, color);
			}
		}
		texture.Apply();
		return texture;
	}

	static public Texture2D MakeTexture_RandomMirroredSprite(int size, float opaquePixelChance, Color baseColor, int seed)
	{
		var texture = new Texture2D(size, size);
		texture.filterMode = FilterMode.Point;

		Color color;
		int prevSeed = Random.seed;
		Random.seed = seed;
		for(int y = 0; y < texture.height; y++)
		{
			for(int x = 0; x < texture.width / 2; x++)
			{
				// TODO: make opaque pixels more likely towards centre of sprite, to get a better-connected shape
				
				if(Random.value < opaquePixelChance)
					color = new Color(baseColor.r, baseColor.g, baseColor.b, 1);
				else
					color = Color.clear;
				texture.SetPixel(x, y, color);
				texture.SetPixel(texture.width - 1 - x, y, color);
			}
		}
		Random.seed = prevSeed;

		texture.Apply();
		return texture;
	}
}