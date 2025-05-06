using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Core.Interfaces
{
    /// <summary>
    /// オブジェクトの状態をスナップショットとして保存・復元するインターフェース
    /// </summary>
    public interface ISnapshot<T>
    {
        /// <summary>
        /// 現在の状態のスナップショットを作成する
        /// </summary>
        /// <returns>状態を表すスナップショットオブジェクト</returns>
        object CreateSnapshot();

        /// <summary>
        /// スナップショットから状態を復元する
        /// </summary>
        /// <param name="snapshot">復元するスナップショット</param>
        void RestoreSnapshot(object snapshot);
    }
}
