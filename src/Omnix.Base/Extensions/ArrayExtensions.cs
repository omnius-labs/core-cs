namespace Omnix.Base.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// 配列をランダムにソートします。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] Shuffle<T>(this T[] array)
        {
            var random = RandomProvider.GetThreadRandom();

            for (int i = 0; i < array.Length; ++i)
            {
                int j = random.Next(i, array.Length);

                var x = array[j];
                array[j] = array[i];
                array[i] = x;
            }

            return array;
        }
    }
}
