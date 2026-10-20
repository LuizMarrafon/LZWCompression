using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AlgoritmoLZW
{
    class Filtros
    {

        //sem acesso direito a memoria
        public static void negativo(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int r, g, b;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //obtendo a cor do pixel
                    Color cor = imageBitmapSrc.GetPixel(x, y);

                    r = cor.R;
                    g = cor.G;
                    b = cor.B;

                    //nova cor
                    Color newcolor = Color.FromArgb(255 - r, 255 - g, 255 - b);

                    imageBitmapDest.SetPixel(x, y, newcolor);
                }
            }
        }

        //com acesso direto a memória
        public static void convert_to_grayDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3;
            Int32 gs;

            //lock dados bitmap origem
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //lock dados bitmap destino
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();

                int r, g, b;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        b = *(src++); //está armazenado dessa forma: b g r 
                        g = *(src++);
                        r = *(src++);
                        gs = (Int32)(r * 0.2990 + g * 0.5870 + b * 0.1140);
                        *(dst++) = (byte)gs;
                        *(dst++) = (byte)gs;
                        *(dst++) = (byte)gs;
                    }
                    src += padding;
                    dst += padding;
                }
            }
            //unlock imagem origem
            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            //unlock imagem destino
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }

        public static void dilatacao(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;

            //Replicando a imagem de origem para a imagem de destino
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color cor = imageBitmapSrc.GetPixel(x, y);
                    imageBitmapDest.SetPixel(x, y, cor);
                }
            }
            //Elemento estruturante, vou usar o ponto X no centro da mascara, mas poderia ser outro ponto
            int[,] mascara = {
                {0, 1, 0},
                {1, 1, 1},
                {0, 1, 0}
            };

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    if(x > 0 && x < width - 1 && y > 0 && y < height - 1)
                    {
                        Color cor = imageBitmapSrc.GetPixel(x, y);
                        //isso aqui é como se o pixel preto fosse o X da matriz q la ta como 1 mas o preto é 0
                        if(cor.R == 0)
                        {
                            for(int j = 0; j <= 2; j++)
                            {
                                for(int i = 0; i <= 2; i++)
                                {
                                    if (mascara[i,j] == 1)
                                    {
                                        imageBitmapDest.SetPixel(x + j - 1, y + i - 1, Color.FromArgb(0, 0, 0));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void dilatacaoDMA(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            int pixelSize = 3;

            //lock dados bitmap origem
            BitmapData bitmapDataSrc = imageBitmapSrc.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //lock dados bitmap destino
            BitmapData bitmapDataDst = imageBitmapDest.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int padding = bitmapDataSrc.Stride - (width * pixelSize);

            unsafe
            {
                byte* src = (byte*)bitmapDataSrc.Scan0.ToPointer();
                byte* dst = (byte*)bitmapDataDst.Scan0.ToPointer();
                byte* baseSrc = src;
                byte* baseDst = dst;

                int r, g, b;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        b = *(src++);
                        g = *(src++);
                        r = *(src++);
                        *(dst++) = (byte)b;
                        *(dst++) = (byte)g;
                        *(dst++) = (byte)r;
                    }
                    src += padding;
                    dst += padding;
                }


                dst = baseDst;
                src = baseSrc;
                //Elemento estruturante
                int[,] mascara = {
                    {0, 1, 0},
                    {1, 1, 1},
                    {0, 1, 0}
                };

                for(int y = 0; y < height; y++)
                {
                    for(int x = 0; x < width; x++)
                    {
                        if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
                        {
                            src = (baseSrc + (y * bitmapDataSrc.Stride) + (x * pixelSize));
                            b = *src;
                            if(b == 0)
                            {
                                for(int j = 0; j <= 2; j++)
                                {
                                    for(int i = 0; i <= 2; i++)
                                    {
                                        if (mascara[i,j] == 1)
                                        {
                                            dst = (baseDst + ((y + i - 1) * bitmapDataDst.Stride) + ((x + j - 1) * pixelSize));
                                            *(dst++) = 0; //b
                                            *(dst++) = 0; //g
                                            *(dst++) = 0; //r
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            //unlock imagem origem
            imageBitmapSrc.UnlockBits(bitmapDataSrc);
            //unlock imagem destino
            imageBitmapDest.UnlockBits(bitmapDataDst);
        }

        private static bool mascaraErosao(int[,] mascara, Bitmap imageBitmapSrc, int x, int y)
        {
            bool flag = true;
            for(int j = 0; j <= 2; j++)
            {
                for (int i = 0; i <= 2; i++)
                {
                    Color cor = imageBitmapSrc.GetPixel(x + j -1, y + i - 1);
                    if (mascara[i,j] == 1 && cor.R != 0)
                    {
                        flag = false;
                    }
                }
            }
            return flag;
        }

        public static void erosao(Bitmap imageBitmapSrc, Bitmap imageBitmapDest)
        {
            int width = imageBitmapSrc.Width;
            int height = imageBitmapSrc.Height;
            //Replicando a imagem de origem para a imagem de destino
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color cor = imageBitmapSrc.GetPixel(x, y);
                    imageBitmapDest.SetPixel(x, y, cor);
                }
            }
            //Elemento estruturante
            int[,] mascara = {
                {0, 1, 0},
                {1, 1, 1},
                {0, 1, 0}
            };
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
                    {
                        Color cor = imageBitmapSrc.GetPixel(x, y);
                        if(cor.R == 0)
                        {
                            if (mascaraErosao(mascara, imageBitmapSrc, x, y))
                            {
                                imageBitmapDest.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                            }
                            else
                            {
                                imageBitmapDest.SetPixel(x,y, Color.FromArgb(255, 255, 255));
                            }
                        }
                    }
                }
            }
        }
    }
}
