using System;
using System.Drawing;

namespace StegoModel
{
    namespace ImageFilter
    {
        /// <summary>
        /// Фильтр для изображений.
        /// </summary>
        public interface IFilterImage
        {
            /// <summary>
            /// Применить фильтр к изображению.
            /// </summary>
            /// <param name="input">Исходное изображение.</param>
            /// <param name="size">Размер матрицы свертки.</param>
            /// <returns>Возвращает изображение,
            ///     к которому был применен фильтр.</returns>
            Bitmap Apply(Bitmap input, Int32 size);
        }

        /// <summary>
        /// Фильтр размытия изображения.
        /// </summary>
        public class Blur : IFilterImage
        {
            /// <summary>
            /// Размыть изображение по свертке фильтра размытия.
            /// </summary>
            /// <param name="input">Исходное изображение для размытия.</param>
            /// <param name="filter">Матрица свертки для размытия.</param>
            /// <returns>Размытое изображение.</returns>
            private Bitmap Convolve(Bitmap input, float[,] filter)
            {
                //Find center of filter
                var xMiddle = (int)Math.Floor(filter.GetLength(0) / 2.0);
                var yMiddle = (int)Math.Floor(filter.GetLength(1) / 2.0);

                //Конечное изображение
                var output = new Bitmap(input.Width, input.Height);

                var reader = new FastBitmap(input);
                var writer = new FastBitmap(output);
                reader.LockImage();
                writer.LockImage();
                
                for (int x = 0; x < input.Width; x++)
                {
                    for (int y = 0; y < input.Height; y++)
                    {
                        var r = 0.0f;
                        var g = 0.0f;
                        var b = 0.0f;

                        //Применение фильтра сглаживание пикселей изображения
                        for (int xFilter = 0; xFilter < filter.GetLength(0); xFilter++)
                        {
                            for (int yFilter = 0; yFilter < filter.GetLength(1); yFilter++)
                            {
                                int x0 = x - xMiddle + xFilter;
                                int y0 = y - yMiddle + yFilter;

                                //Если x0, y0 находиться внутри изображения
                                if (x0 >= 0 && x0 < input.Width &&
                                    y0 >= 0 && y0 < input.Height)
                                {
                                    Color clr = reader.GetPixel(x0, y0);

                                    r += clr.R * filter[xFilter, yFilter];
                                    g += clr.G * filter[xFilter, yFilter];
                                    b += clr.B * filter[xFilter, yFilter];
                                }
                            }
                        }

                        //Нормализация (основных цветов RGB)
                        if (r > 255)
                            r = 255;
                        if (g > 255)
                            g = 255;
                        if (b > 255)
                            b = 255;

                        if (r < 0)
                            r = 0;
                        if (g < 0)
                            g = 0;
                        if (b < 0)
                            b = 0;

                        var newColor = Color.FromArgb(
                            (int)r, (int)g, (int)b);
                        //Установка новых цветов для конечного изображения
                        writer.SetPixel(x, y, newColor);
                    }
                }

                reader.UnlockImage();
                writer.UnlockImage();

                return output;
            }

            /// <summary>
            /// Получить свертку фильтра по горизонтали изображения.
            /// </summary>
            /// <param name="size">Размер изображения по горизонтали.</param>
            /// <returns>Возвращает вектор-столбец фильтра
            ///     в формате {1,..,n}.</returns>
            private float[,] GetHorizontalFilter(int size)
            {
                var smallFilter = new float[size, 1];
                float constant = size;

                for (int i = 0; i < size; i++)
                {
                    smallFilter[i, 0] = 1.0f / constant;
                }

                return smallFilter;
            }

            /// <summary>
            /// Получить свертку фильтра по вертикали изображения.
            /// </summary>
            /// <param name="size">Размер изображения по вертикали.</param>
            /// <returns>Возвращает вектор-строку фильтра
            ///     в формате {1},...,{n}</returns>
            private float[,] GetVerticalFilter(int size)
            {
                var smallFilter = new float[1, size];
                float constant = size;

                for (int i = 0; i < size; i++)
                {
                    smallFilter[0, i] = 1.0f / constant;
                }

                return smallFilter;
            }

            /// <summary>
            /// Получить свертку фильтра для изображения.
            /// </summary>
            /// <param name="size">Размерность квадратной матрицы.</param>
            /// <returns>Возвращает квадратную матрицу размером NxN
            ///     в формате {1,...,n},...,{1,...,n}</returns>
            private float[,] GetBoxFilter(int size)
            {
                var filter = new float[size, size];
                float constant = size * size;

                for (int i = 0; i < filter.GetLength(0); i++)
                {
                    for (int j = 0; j < filter.GetLength(1); j++)
                    {
                        filter[i, j] = 1.0f / constant;
                    }
                }

                return filter;
            }

            /// <summary>
            /// Размытие изображения на основе матрицы свертки.
            /// </summary>
            /// <param name="img">Исходное изображение.</param>
            /// <param name="size">Степерь размытия пикселей.</param>
            /// <returns>Возвращает размытое изображение.</returns>
            private Bitmap BoxBlur(Image img, int size)
            {
                //Применить матрицу свертки для изображения(медленнее)
                return Convolve(new Bitmap(img), GetBoxFilter(size));
            }

            /// <summary>
            /// Размытие изображения на основе
            ///     раздельных векторов свертки.
            /// </summary>
            /// <param name="img">Исходное изображение.</param>
            /// <param name="size">Степерь размытия пикселей.</param>
            /// <returns>Возвращает размытое изображение.</returns>
            private Bitmap FastBoxBlur(Image img, int size)
            {
                //Применить отдельно векторы-свертки
                //  для изображения(быстрее)
                return Convolve(Convolve(new Bitmap(img),
                    GetHorizontalFilter(size)), GetVerticalFilter(size));
            }

            /// <summary>
            /// Применить фильтр к изображению.
            /// </summary>
            /// <param name="input">Исходное изображение.</param>
            /// <returns>Возвращает изображение,
            ///     к котором был применен фильтр.</returns>
            public Bitmap Apply(Bitmap input, Int32 size)
            {
                return FastBoxBlur(input, size);
            }
        }
    }
}
