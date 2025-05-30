using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Extensions
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
        /// BitArrayをバイト配列に変換します。
        /// </summary>
        /// <param name="bitArray">変換対象のBitArray</param>
        /// <returns>変換されたバイト配列</returns>
        public static byte[] ToByteArray(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            int byteLength = (bitArray.Length + 7) / 8;
            byte[] bytes = new byte[byteLength];
            bitArray.CopyTo(bytes, 0);
            return bytes;
        }

        /// <summary>
        /// BitArrayを16進数文字列に変換します。
        /// </summary>
        /// <param name="bitArray">変換対象のBitArray</param>
        /// <returns>16進数文字列</returns>
        public static string ToHexString(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            byte[] bytes = ToByteArray(bitArray);
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// BitArrayをバイナリ文字列に変換します。
        /// </summary>
        /// <param name="bitArray">変換対象のBitArray</param>
        /// <param name="separator">ビット間の区切り文字（オプション）</param>
        /// <returns>バイナリ文字列</returns>
        public static string ToBinaryString(this BitArray bitArray, string separator = "")
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            StringBuilder sb = new StringBuilder(bitArray.Length + (bitArray.Length / 8));
            for (int i = 0; i < bitArray.Length; i++)
            {
                sb.Append(bitArray[i] ? '1' : '0');
                if (separator.Length > 0 && i < bitArray.Length - 1 && (i + 1) % 8 == 0)
                    sb.Append(separator);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 指定した位置から始まるビットのパターンを取得します。
        /// </summary>
        /// <param name="bitArray">対象のBitArray</param>
        /// <param name="startIndex">開始インデックス</param>
        /// <param name="length">取得する長さ</param>
        /// <returns>指定範囲のビットを含む新しいBitArray</returns>
        public static BitArray Slice(this BitArray bitArray, int startIndex, int length)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            if (startIndex < 0 || startIndex >= bitArray.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (length < 0 || startIndex + length > bitArray.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            BitArray result = new BitArray(length);
            for (int i = 0; i < length; i++)
            {
                result[i] = bitArray[startIndex + i];
            }
            return result;
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
        /// BitArrayの値を整数に変換します（最大32ビットまで）。
        /// </summary>
        /// <param name="bitArray">変換対象のBitArray</param>
        /// <returns>整数値</returns>
        public static int ToInt32(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            if (bitArray.Length > 32)
                throw new ArgumentException("BitArray length exceeds 32 bits", nameof(bitArray));

            int result = 0;
            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                    result |= (1 << i);
            }
            return result;
        }

        /// <summary>
        /// BitArrayの値を64ビット整数に変換します。
        /// </summary>
        /// <param name="bitArray">変換対象のBitArray</param>
        /// <returns>64ビット整数値</returns>
        public static long ToInt64(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            if (bitArray.Length > 64)
                throw new ArgumentException("BitArray length exceeds 64 bits", nameof(bitArray));

            long result = 0;
            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                    result |= (1L << i);
            }
            return result;
        }

        /// <summary>
        /// BitArrayをビットごとに反転します（新しいインスタンスを返します）。
        /// </summary>
        /// <param name="bitArray">反転対象のBitArray</param>
        /// <returns>反転されたBitArray</returns>
        public static BitArray Invert(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            BitArray result = new BitArray(bitArray);
            result.Not();
            return result;
        }

        /// <summary>
        /// BitArray内のtrueビットの数を返します。
        /// </summary>
        /// <param name="bitArray">チェック対象のBitArray</param>
        /// <returns>trueビットの数</returns>
        public static int CountBits(this BitArray bitArray)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            int count = 0;
            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                    count++;
            }
            return count;
        }

        /// <summary>
        /// BitArrayの右シフト操作を行います。
        /// </summary>
        /// <param name="bitArray">シフト対象のBitArray</param>
        /// <param name="shiftCount">シフトするビット数</param>
        /// <returns>シフトされた新しいBitArray</returns>
        public static BitArray RightShift(this BitArray bitArray, int shiftCount)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            if (shiftCount < 0)
                throw new ArgumentOutOfRangeException(nameof(shiftCount), "Shift count must be non-negative");

            if (shiftCount == 0 || bitArray.Length == 0)
                return new BitArray(bitArray);

            if (shiftCount >= bitArray.Length)
                return new BitArray(bitArray.Length); // すべてfalse

            BitArray result = new BitArray(bitArray.Length);
            for (int i = 0; i < bitArray.Length - shiftCount; i++)
            {
                result[i] = bitArray[i + shiftCount];
            }
            return result;
        }

        /// <summary>
        /// BitArrayの左シフト操作を行います。
        /// </summary>
        /// <param name="bitArray">シフト対象のBitArray</param>
        /// <param name="shiftCount">シフトするビット数</param>
        /// <returns>シフトされた新しいBitArray</returns>
        public static BitArray LeftShift(this BitArray bitArray, int shiftCount)
        {
            if (bitArray == null)
                throw new ArgumentNullException(nameof(bitArray));

            if (shiftCount < 0)
                throw new ArgumentOutOfRangeException(nameof(shiftCount), "Shift count must be non-negative");

            if (shiftCount == 0 || bitArray.Length == 0)
                return new BitArray(bitArray);

            if (shiftCount >= bitArray.Length)
                return new BitArray(bitArray.Length); // すべてfalse

            BitArray result = new BitArray(bitArray.Length);
            for (int i = shiftCount; i < bitArray.Length; i++)
            {
                result[i] = bitArray[i - shiftCount];
            }
            return result;
        }

    }
}
