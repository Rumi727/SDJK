using System;
using System.IO;
using UnityEngine;

namespace SCKRM.Resource
{
    public static class ImageLoader
    {
        // This was made by aaro4130 on the Unity forums.  Thanks boss!
        // It's been optimized and slimmed down for the purpose of loading Quake 3 TGA textures from memory streams.
        public static Texture2D LoadTGA(string path, bool mipmapUse = false)
        {
            using (BinaryReader r = new BinaryReader(File.OpenRead(path)))
            {
                // Skip some header info we don't care about.
                // Even if we did care, we have to move the stream seek point to the beginning,
                // as the previous method in the workflow left it at the end.
                r.BaseStream.Seek(2, SeekOrigin.Begin);

                bool runLength = r.ReadByte() == 10;

                r.BaseStream.Seek(9, SeekOrigin.Current);

                short width = r.ReadInt16();
                short height = r.ReadInt16();
                int bitDepth = r.ReadByte();

                // Skip a byte of header information we don't care about.
                r.BaseStream.Seek(1, SeekOrigin.Current);

                Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, mipmapUse);
                Color32[] pulledColors = new Color32[width * height];
                
                if (!runLength)
                {
                    if (bitDepth == 32)
                    {
                        for (int i = 0; i < width * height; i++)
                        {
                            byte blue = r.ReadByte();
                            byte green = r.ReadByte();
                            byte red = r.ReadByte();
                            byte alpha = r.ReadByte();

                            pulledColors[i] = new Color32(red, green, blue, alpha);
                        }
                    }
                    else if (bitDepth == 24)
                    {
                        for (int i = 0; i < width * height; i++)
                        {
                            byte blue = r.ReadByte();
                            byte green = r.ReadByte();
                            byte red = r.ReadByte();

                            pulledColors[i] = new Color32(red, green, blue, 255);
                        }
                    }
                    else
                        throw new Exception("TGA texture had non 32/24 bit depth.");
                }
                else // Code that helped load the compressed TGA (actually it's just a copy paste) https://github.com/EricHaung/TGALoaderForUnity/blob/main/TGALoader.cs
                {
                    if (bitDepth == 32)
                    {
                        int index = 0;
                        while (index < width * height)
                        {
                            byte pixelCount = r.ReadByte();

                            if (pixelCount >= 128)
                            {
                                pixelCount -= 127;

                                byte blue = r.ReadByte();
                                byte green = r.ReadByte();
                                byte red = r.ReadByte();
                                byte alpha = r.ReadByte();

                                for (int i = 0; i < pixelCount && i < pulledColors.Length; i++)
                                    pulledColors[index + i] = new Color32(red, green, blue, alpha);
                            }
                            else
                            {
                                pixelCount++;

                                for (int i = 0; i < pixelCount && i < pulledColors.Length; i++)
                                {
                                    byte blue = r.ReadByte();
                                    byte green = r.ReadByte();
                                    byte red = r.ReadByte();
                                    byte alpha = r.ReadByte();

                                    pulledColors[index + i] = new Color32(red, green, blue, alpha);
                                }
                            }

                            index += pixelCount;
                        }
                    }
                    else if (bitDepth == 24)
                    {
                        int index = 0;
                        while (index < width * height)
                        {
                            byte pixelCount = r.ReadByte();

                            if (pixelCount >= 128)
                            {
                                pixelCount -= 127;

                                byte blue = r.ReadByte();
                                byte green = r.ReadByte();
                                byte red = r.ReadByte();

                                for (int i = 0; i < pixelCount && i < pulledColors.Length; i++)
                                    pulledColors[index + i] = new Color32(red, green, blue, 255);
                            }
                            else
                            {
                                pixelCount++;

                                for (int i = 0; i < pixelCount && i < pulledColors.Length; i++)
                                {
                                    byte blue = r.ReadByte();
                                    byte green = r.ReadByte();
                                    byte red = r.ReadByte();

                                    pulledColors[index + i] = new Color32(red, green, blue, 255);
                                }
                            }

                            index += pixelCount;
                        }
                    }
                    else
                        throw new Exception("TGA texture had non 32/24 bit depth.");
                }

                tex.SetPixels32(pulledColors);
                tex.Apply();
                return tex;
            }
        }

        public static Texture2D LoadDDS(string path)
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(path));
            long length = new FileInfo(path).Length;
            byte[] header = reader.ReadBytes(128);
            int height = header[13] * 256 + header[12];
            int width = header[17] * 256 + header[16];
            bool mipmaps = header[28] > 0;
            TextureFormat textureFormat = header[87] == 49 ? TextureFormat.DXT1 : TextureFormat.DXT5;
            byte[] source = reader.ReadBytes(Convert.ToInt32(length) - 128);
            reader.Close();
            Texture2D texture = new Texture2D(width, height, textureFormat, mipmaps);
            texture.LoadRawTextureData(source);
            texture.name = Path.GetFileName(path);
            texture.Apply(false);
            return texture;
        }
    }
}