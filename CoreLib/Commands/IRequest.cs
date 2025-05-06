using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Commands
{
    /// <summary>
    /// メディエーターパターンのリクエストインターフェース
    /// </summary>
    public interface IRequest : MediatR.IRequest
    {
    }

    /// <summary>
    /// メディエーターパターンの結果付きリクエストインターフェース
    /// </summary>
    public interface IRequest<out TResponse> : MediatR.IRequest<TResponse>
    {
    }

    /// <summary>
    /// CQRS パターンのコマンドインターフェース（状態変更操作）
    /// </summary>
    public interface ICommand : IRequest
    {
    }

    /// <summary>
    /// 結果を返すコマンドインターフェース
    /// </summary>
    /// <typeparam name="TResult">コマンド実行結果の型</typeparam>
    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }

    /// <summary>
    /// CQRS パターンのクエリインターフェース（読み取り操作）
    /// </summary>
    /// <typeparam name="TResult">クエリ結果の型</typeparam>
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }

}
