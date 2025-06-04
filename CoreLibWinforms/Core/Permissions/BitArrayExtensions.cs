using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibWinforms.Core.Permissions
{
    /// <summary>
    /// BitArrayクラスの拡張メソッドを提供します。
    /// </summary>
    public static class BitArrayExtensions
    {

        /// <summary>
        /// BitArray内に少なくとも1つ以上のtrueビットがあるかを確認します。
        /// </summary>
        /// <param name="bitArray">チェック対象のBitArray</param>
        /// <returns>1つでもtrueビットがあればtrue、そうでなければfalse</returns>
        public static bool Any(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// BitArray内のすべてのビットがtrueかを確認します。
        /// </summary>
        /// <param name="bitArray">チェック対象のBitArray</param>
        /// <returns>すべてのビットがtrueならtrue、そうでなければfalse</returns>
        public static bool All(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (!bitArray[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 指定した位置のビットを反転させます。
        /// </summary>
        /// <param name="bitArray">操作対象のBitArray</param>
        /// <param name="index">反転するビットのインデックス</param>
        /// <returns>操作後のBitArray（元のインスタンスと同じ）</returns>
        public static BitArray Flip(this BitArray bitArray, int index)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            if (index < 0 || index >= bitArray.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            bitArray[index] = !bitArray[index];
            return bitArray;
        }

        /// <summary>
        /// BitArray内のすべてのビットがfalseかどうかを判定します。
        /// </summary>
        /// <param name="bitArray">チェック対象のBitArray</param>
        /// <returns>すべてのビットがfalseならtrue</returns>
        public static bool IsEmpty(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 2つのBitArrayが等しいかどうかを比較します。
        /// </summary>
        /// <param name="bitArray">比較元のBitArray</param>
        /// <param name="other">比較対象のBitArray</param>
        /// <returns>等しい場合はtrue、そうでなければfalse</returns>
        public static bool SequenceEqual(this BitArray bitArray, BitArray other)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (bitArray.Length != other.Length)
                return false;

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i] != other[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// BitArray内でtrueに設定されているビットのインデックスのリストを取得します。
        /// </summary>
        /// <param name="bitArray">チェック対象のBitArray</param>
        /// <returns>trueビットのインデックスを含むリスト</returns>
        public static List<int> GetTrueIndices(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            var indices = new List<int>();
            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                    indices.Add(i);
            }
            return indices;
        }
        
        
    }

}
